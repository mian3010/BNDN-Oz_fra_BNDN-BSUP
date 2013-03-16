namespace RentIt

open System
open System.Data

    // The Persistence module is used for reading and wrting data.
    // This module works with a SQL database
    module Persistence =

        let internal server = "(localdb)\Projects"
        let internal database = "RENTIT27"
        let internal connDb = new SqlClient.SqlConnection("server=" + server + ";Integrated Security=True;Database=" + database)

        // Filter holds query information
        type Filter = {
         field : string;
         operator : string;
         value : string;
        }

        // joinFilter returns Filter as a string
        let internal joinFilter (filter:Filter) = filter.field + " " + filter.operator + " '" + filter.value + "'"

        // joinFilters takes a List of Filter and returns as a string
        let rec internal joinFilters (filters:Filter List) = 
         match filters with 
           | [] -> ""
           | x::[] -> joinFilter x
           | x::xs -> joinFilter x+" AND "+joinFilters xs

        // Takes a query and performs it on the database
        let internal performSql sql =
         let cmd = new SqlClient.SqlCommand(sql, connDb)
         connDb.Open()
         cmd.ExecuteReader()

        // Read a row
        let internal extractRow (reader:SqlClient.SqlDataReader) data =
         let rec looper = function
          | 0, d -> d
          | i, (d:Map<string,string>) -> looper ((i-1), (d.Add(reader.GetName(i-1), (reader.GetValue(i-1).ToString()))))
         looper ((reader.FieldCount), data)

        // Read rows
        let internal extractData (reader:SqlClient.SqlDataReader) = 
         [ while reader.Read() do
                      let start:Map<string,string> = Map.empty
                      yield (extractRow reader start)
         ]
        
        // Get stuff from database. Takes a List of Filters
        let Get objectName (filters:Filter List) =
         use reader = performSql ("SELECT * FROM [" + objectName + "] WHERE " + joinFilters filters)

         let result = extractData reader
         result