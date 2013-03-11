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
            Id:int
            data1:int
            data2:float
            name:string }

        let testRead = seq {
            let sql  = "SELECT * FROM Table1" // Dummy SQL
            let cmd = new SqlClient.SqlCommand(sql, Internal.connDB)
            //let cmd = Internal.connDB?GetProducts
            Internal.connDB.Open()
            let reader = cmd.ExecuteReader()

            while reader.Read() do
                yield { 
                    Id = unbox (reader.["Id"])
                    data1 = unbox (reader.["TestData1"])
                    data2 = unbox (reader.["TestData2"])
                    name = unbox (reader.["Name"]) }
                //let values = Array.init (reader.FieldCount) (fun i -> reader.[i])
                //values |> printfn "%A"
                
            Internal.connDB.Close() }