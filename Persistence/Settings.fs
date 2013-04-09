namespace RentIt.Persistence
open System
open System.Data
open System.Security

    module Settings = 
        // Database connection
        (*
        // Server settings
        
        let server = "localhost"
        let database = "RENTIT27"
        let userName = "RentIt27db"
        let pass = "ZAQ12wsx"

        let secPass = new SecureString()
        for i in pass do
            secPass.AppendChar(i)

        secPass.MakeReadOnly()

        let connectionString = "server=" + server + ";Database=" + database
            
        let cred = SqlClient.SqlCredential(userName, secPass)
            
        let connDb = new SqlClient.SqlConnection(connectionString, cred)
        *)
        
        // Local settings
        let internal server = "(localdb)\Projects"
        let internal database = "RENTIT27"
        let internal connDb = new SqlClient.SqlConnection("server=" + server + ";Integrated Security=True;Database=" + database)
        
        //General
        connDb.Open();