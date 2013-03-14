namespace RentIt

open System
open System.Data

    module DB =

        module internal Internal =

            let server = "(localdb)\Projects"
            let database = "test"

            let connDB = new SqlClient.SqlConnection("server=" + server + ";Integrated Security=True;Database=" + database)
        ///////////////////////////////////////////////////////////////////////////////////

        type test = { 
            Id:int}

        let testRead = seq {
            let sql  = "SELECT * FROM Table1" // Dummy SQL
            let cmd = new SqlClient.SqlCommand(sql, Internal.connDB)
            //let cmd = Internal.connDB?GetProducts
            Internal.connDB.Open()
            let reader = cmd.ExecuteReader()

            while reader.Read() do
                yield { 
                    Id = unbox (reader.["Id"])}
                
            Internal.connDB.Close() }