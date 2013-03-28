namespace RentIt.Persistence
open System
open System.Data
    module Helper = 
        /// Takes a query and performs it on the database
        let performSql sql = 
            let cmd = new SqlClient.SqlCommand(sql, Settings.connDb)
            cmd.ExecuteReader()

        // Read a row
        let internal extractRow (reader:SqlClient.SqlDataReader) data =
            let rec readData = function
                  | 0, d -> d
                  | i, (d:Map<string,string>) -> readData ((i-1), (d.Add(reader.GetName(i-1), (reader.GetValue(i-1).ToString()))))
            readData ((reader.FieldCount), data)

        // Read rows
        let internal extractData (reader:SqlClient.SqlDataReader) = 
            [ while reader.Read() do
                let start:Map<string,string> = Map.empty
                yield (extractRow reader start)
            ]