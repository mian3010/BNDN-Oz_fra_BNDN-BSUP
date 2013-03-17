namespace RentIt
open System
open System.Data

    module Settings = 

        let internal server = "(localdb)\Projects"
        let internal database = "RENTIT27"
        let internal connDb = new SqlClient.SqlConnection("server=" + server + ";Integrated Security=True;Database=" + database)
        connDb.Open();