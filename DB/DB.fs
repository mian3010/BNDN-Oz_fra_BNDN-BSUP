namespace RentIt

open System
open System.Data
open System.Security

    module Db =

        // Exceptions
        exception NoUserWithSuchName
        exception UsernameAlreadyInUse
        exception NewerVersionExist
        exception IllegalAccountVersion
        exception NuSuchAccountType

        module internal Internal =

            // Cache elements
            let mutable cache = Map.empty : Map<string, AccountTypes.Account>

            // Database connection
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
            
            let connDB = new SqlClient.SqlConnection(connectionString, cred)

            let performSql sql =
                connDB.Close()
                let cmd = new SqlClient.SqlCommand(sql, connDB)
                connDB.Open()
                cmd.ExecuteReader()

            ////// Helper functions
            let splitDbPass (dbPass:string) :AccountTypes.Password =
                let parts = dbPass.Split [|':'|]
                { salt = parts.[0]; hash = parts.[1] }

            let getNextLoggableID() :int =
                let sql = "SELECT MAX(Id) FROM loggable"
                use reader = performSql sql
                if reader.Read() then
                    let result = reader.GetInt32(0)
                    result
                else
                    0

        ///////////////////////////////////////////////////////////////////////////////////

        ////// API functions

        /// Retrieves an account from persistence based on its associated username
        /// Raises NoUserWithSuchName
        let getUserByName userName :AccountTypes.Account =
            let internalFun =
                if (Map.containsKey<string, AccountTypes.Account> userName Internal.cache) then 
                    Map.find<string, AccountTypes.Account> userName Internal.cache
                else
                    let sql = "SELECT * FROM [user] WHERE (Username = '" + userName + "')"
                    use reader = Internal.performSql sql

                    if reader.Read() then
                        let result :AccountTypes.Account =  {
                            user = reader.GetString 1 //unbox (reader.["Username"]);
                            email = reader.GetString 2 //unbox (reader.["Email"]);
                            password = Internal.splitDbPass (reader.GetString 5) //unbox (reader.["Password"]))
                            created = reader.GetDateTime 6 //unbox (reader.["Created_date"]);
                            banned = if reader.GetByte 7 = byte 0 then false else true
                            info = {
                                    name = None ;
                                    address = {
                                                address = None;
                                                postal  = None;
                                                country = None };
                                    birth   = None;
                                    about   = None;
                                    credits = None }
                            accType = "Admin"
                            version = uint32 0 }
                        Internal.cache <- Internal.cache.Add (result.user, result)
                        result
                    else
                        raise NoUserWithSuchName
            lock Internal.cache (fun() -> internalFun)

        /// Retrieves all accounts of a specific type
        let getAllUsersByType accType :AccountTypes.Account list =
            let internalFun =
                let sql = "SELECT * FROM [user] where (Type_name = '" + accType + "')"
                use reader = Internal.performSql sql
                let result =    [ 
                        while reader.Read() do
                        let tmp :AccountTypes.Account = {
                            user = reader.GetString 1
                            email = reader.GetString 2
                            password = Internal.splitDbPass (reader.GetString 5)
                            created = reader.GetDateTime 6
                            banned = if reader.GetByte 7 = byte 0 then false else true
                            info = {
                                    name = None ;
                                    address = {
                                                address = None;
                                                postal  = None;
                                                country = None };
                                    birth   = None;
                                    about   = None;
                                    credits = None }
                            accType = "Admin"
                            version = uint32 0
                        }
                        yield tmp
                        if not (Map.containsKey<string, AccountTypes.Account> tmp.user Internal.cache.CachedUsers) then
                            Internal.cache <- Internal.cache.Add (tmp.user, tmp) ]
                result
            lock Internal.cache (fun() -> internalFun)

        /// Retrieves the date and time which the user {user} last authenticated
        /// 'None' means that the user never has authenticated
        /// Raises NoUserWithSuchName
        let getUsersLastAuthTime (user:string) :System.DateTime option =
            None

        /// Updates the persisted account record to the passed {acc} account record
        /// The account which is updated is the one with the identical username
        /// Raises NoUserWithSuchName
        /// Raises NewerVersionExist
        /// Raises IllegalAccountVersion
        let update (acc:AccountTypes.Account) :AccountTypes.Account =
            let internalFun :AccountTypes.Account =
                let cu = getUserByName acc.user

                if (cu.version > acc.version) then
                    raise NewerVersionExist
                elif (cu.version < acc.version) then
                    raise IllegalAccountVersion
                
                let sql =   "UPDATE [user] SET 
                            Email = '" + acc.email + "',
                            Password = '" + acc.password.salt + ":" + acc.password.hash + "',
                            Banned = " + if acc.banned then "1" else "0"
                            + "Address = '" + "Dummy" + "'" +
                            "Date_of_birth = " + "11-11-2008 13:23:44" + ", 
                            About_me = '" + "Dummy" + "', 
                            Balance = " + "0" + ", 
                            Zipcode = " + "2400" + ", 
                            Country_Name = '" + "Dummy" + "'"
                                
                let sql = sql + "WHERE (Username = '" + acc.user + "')"
                let a' = Internal.performSql sql
                let newAcc :AccountTypes.Account = {
                                        user = acc.user;
                                        email = acc.email;
                                        password = acc.password;
                                        created = acc.created;
                                        banned = acc.banned;
                                        info = {
                                                name = None ;
                                                address = {
                                                            address = None;
                                                            postal  = None;
                                                            country = None };
                                                birth   = None;
                                                about   = None;
                                                credits = None }
                                        accType = "Admin"
                                        version = uint32 0 }
                Internal.cache <- Internal.cache.Add(acc.user, acc)
                newAcc
                    
            lock Internal.cache (fun() -> internalFun)

        /// Raises UsernameAlreadyInUse
        let createUser (acc:AccountTypes.Account) =
            let internalFun =
                let nextId = Internal.getNextLoggableID() + 1
                let sql = "INSERT INTO [loggable] (Id) VALUES (" + string nextId + ");"
                //let sql = sql + "INSERT INTO [user] (Id, Username) VALUES (" + string nextId + ", 'kruger2')"
                let sql = sql + "INSERT INTO [user]
                            (Id, Username, Email, Password, Banned, Created_date, Type_name, Country_Name ,Address, Zipcode, Date_of_birth,  About_me, Balance)"
                            
                let sql = sql + " VALUES (" + string nextId + "," + 
                                            "'" + acc.user + "'," + 
                                            "'" + acc.email + "'," + 
                                            "'" + acc.password.salt + ":" + acc.password.hash + "'," +
                                            if acc.banned then "1" else "0"
                                            + "," + 
                                            "'" + string DateTime.Now + "'," + 
                                            "'" + acc.accType + "'" +
                                            ",'DK'" +
                                            ",'Ferskenvej 3', 2400" +
                                            ",11-11-2008 13:23:44,  'About_me', '999')"
                let a' = Internal.performSql sql
                Internal.cache <- Internal.cache.Add(acc.user, acc)
                printf "%s" sql
            lock Internal.cache (fun() -> internalFun)