namespace RentIt

open System
open System.Data

    module DB =

        module internal Internal =

            let server = "(localdb)\Projects"
            let database = "test"

            let connDB = new SqlClient.SqlConnection("server=" + server + ";Integrated Security=True;Database=" + database)
        ///////////////////////////////////////////////////////////////////////////////////

        let testRead = 
            let sql  = "SELECT * FROM Table1" // Dummy SQL
            let cmd = new SqlClient.SqlCommand(sql, Internal.connDB)
            Internal.connDB.Open()
            let reader = cmd.ExecuteReader()

            while reader.Read() do
                let values = Array.init (reader.FieldCount) (fun i -> reader.[i])
                values |> printfn "%A"
                
            

            Internal.connDB.Close()