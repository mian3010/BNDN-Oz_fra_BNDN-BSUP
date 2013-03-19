namespace RentIt.Persistence
open System
open System.Data
    module Helper = 
        /// Takes a query and performs it on the database
        let internal performSql sql = 
            let cmd = new SqlClient.SqlCommand(sql, Settings.connDb)
            cmd.ExecuteReader()
