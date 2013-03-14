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

            ////// Persistence
            type Cache() = class
                let mutable cachedUsers = Map.empty : Map<string, Account.Account>

                member x.CachedUsers = Map.empty : Map<string, Account.Account>

                member x.addUser(key, value) = cachedUsers <- cachedUsers.Add(key, value)
            end

        

        ///////////////////////////////////////////////////////////////////////////////////

        // Exceptions
        exception NoUserWithSuchName
        exception UsernameAlreadyInUse
        exception NewerVersionExist
        exception NoSuchAccountTypeExits

        let internal cache = new Internal.Cache()

        ////// API functions

        /// Retrieves an account from persistence based on its associated username
        /// Raises NoUserWithSuchName
        let getUserByName userName :Account.Account =
            let sql = "SELECT * FROM [table] where [username] =" + userName
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
            result


        /// Retrieves all accounts of a specific type
        /// Raises NoSuchAccountTypeExits
        let getAllUsersByType (accType:Account.AccountType) :Account.Account list =
            let sql = "SELECT * FROM [TABLE] where = " + accType.ToString() // Query is not right!
            use reader = Internal.performSql sql
            if reader.HasRows = false then raise NoSuchAccountTypeExits
            let result =    [ while reader.Read() do
                                let tmp :Account.Account = {
                                    user = "dude";
                                    email = "where.is@my.car";
                                    password = {salt = "123"; hash = "456" };
                                    created = System.DateTime.Now;
                                    banned = false;
                                    info = Account.TypeInfo.Admin ();
                                    version = uint32(0) }
                                yield tmp
                            ]
            result

        /// Retrieves the date and time which the user {user} last authenticated
        /// 'None' means that the user never has authenticated
        /// Raises NoUserWithSuchName
        let getUsersLastAuthTime (user:string) :System.DateTime option =
            raise (new System.NotImplementedException())

        /// Updates the persisted account record to the passed {acc} account record
        /// The account which is updated is the one with the identical username
        /// Raises NoUserWithSuchName
        /// Raises NewerVersionExist
        let update (acc:Account.Account) =
            raise (new System.NotImplementedException())

        /// Raises UsernameAlreadyInUse
        let createUser(acc:Account.Account) =
            raise (new System.NotImplementedException())