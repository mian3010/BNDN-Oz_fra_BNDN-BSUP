namespace RentIt

open System
open System.Data

    module Persistence =

        let internal server = "(localdb)\Projects"
        let internal database = "RENTIT27"
        let internal connDb = new SqlClient.SqlConnection("server=" + server + ";Integrated Security=True;Database=" + database)

        type Filter = {
         field : string;
         operator : string;
         value : string;
        }
        let internal joinFilter (filter:Filter) = filter.field + " " + filter.operator + " '" + filter.value + "'"
        let rec internal joinFilters (filters:Filter List) = 
         match filters with 
           | [] -> ""
           | x::[] -> joinFilter x
           | x::xs -> joinFilter x+" AND "+joinFilters xs

        let internal performSql sql =
         let cmd = new SqlClient.SqlCommand(sql, connDb)
         connDb.Open()
         cmd.ExecuteReader()

        let internal extractRow (reader:SqlClient.SqlDataReader) data =
         let rec looper = function
          | 0, d -> d
          | i, (d:Map<string,string>) -> looper ((i-1), (d.Add(reader.GetName(i-1), (reader.GetValue(i-1).ToString()))))
         looper ((reader.FieldCount), data)

        let internal extractData (reader:SqlClient.SqlDataReader) = 
         [ while reader.Read() do
                      let start:Map<string,string> = Map.empty
                      yield (extractRow reader start)
         ]
         
        let Get objectName (filters:Filter List) =
         use reader = performSql ("SELECT * FROM [" + objectName + "] WHERE " + joinFilters filters)

         let result = extractData reader
         result