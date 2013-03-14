namespace RentIt

open System
open System.Data

    module DB =

        module internal Internal =

            let server = "(localdb)\Projects"
            let database = "test"

            let connDB = new SqlClient.SqlConnection("server=" + server + ";Integrated Security=True;Database=" + database)

            let performSql sql =
             let cmd = new SqlClient.SqlCommand(sql, connDB)
             connDB.Open()
             cmd.ExecuteReader()

        ///////////////////////////////////////////////////////////////////////////////////

        // Exceptions
        exception NoUserWithSuchName

        // Persistence
        type Cache() = class
            let mutable cachedUsers = Map.empty : Map<string, Account.Account>

            member x.CachedUsers = Map.empty : Map<string, Account.Account>

            member x.addUser(key, value) = cachedUsers <- cachedUsers.Add(key, value)
        end

        let cache = new Cache()

        // API functions

        let getUserByName userName :Account.Account =
            if (Map.containsKey userName cache.CachedUsers) 
            then Map.find userName cache.CachedUsers
            else
                let sql = "SELECT * FROM [table] where [username] =" ^ userName
                use reader = Internal.performSql sql
                if reader.HasRows = false then raise NoUserWithSuchName
                let result :Account.Account =  {
                                user = "dude";
                                email = "where.is@my.car";
                                password = {salt = "123"; hash = "456" };
                                created = System.DateTime.Now;
                                banned = false;
                                info = Account.TypeInfo.Admin ();
                                version = uint32(0) }
                Internal.connDB.Close()
                let a = cache.addUser(result.user, result)
                
                result

        let getAllUsersByType (accType:Account.AccountType) :Account.Account list =
            raise (new System.NotImplementedException())

        let getUsersLastAuthTime (user:string) :System.DateTime option =
            raise (new System.NotImplementedException())

        let update (acc:Account.Account) =
            raise (new System.NotImplementedException())

        let createUser(acc:Account.Account) =
            raise (new System.NotImplementedException())