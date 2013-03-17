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

            ////// Persistence
            type Cache() = class
                let mutable cachedUsers = Map.empty : Map<string, AccountTypes.Account>

                member x.CachedUsers = Map.empty : Map<string, AccountTypes.Account>

                member x.addUser(key, value) = cachedUsers <- cachedUsers.Add(key, value)
            end

            // Cache elements
            let cache = new Cache()

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
            let splitDbPass (dbPass:string) :AccountTypes.PasswordTypes.Password =
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


            let getTypeInfoFromString (info:string) (reader:SqlClient.SqlDataReader) :AccountTypes.TypeInfo =
                match info with
                | "Admin" -> AccountTypes.TypeInfo.Admin()
                | "Content Provider" -> AccountTypes.TypeInfo.ContentProvider
                                                                {
                                                                    name = Some "John Doe";
                                                                    address = 
                                                                        {
                                                                            address = Some (reader.GetString 3);
                                                                            postal = Some (reader.GetInt32 11);
                                                                            country = Some (reader.GetString 12)
                                                                        }
                                                                }
                | "Customer" -> AccountTypes.TypeInfo.Customer
                                                    {
                                                        name = Some "John Doe";
                                                        address = 
                                                            {
                                                                address = Some (reader.GetString 3);
                                                                postal = Some (reader.GetInt32 11);
                                                                country = Some (reader.GetString 12)
                                                            };
                                                        birth = Some (reader.GetDateTime 4);
                                                        about = Some (reader.GetString 8);
                                                        credits = reader.GetInt32 10
                                                    }
                | _ -> raise NuSuchAccountType

            let accTypeToString = function
                | AccountTypes.AccountType.Admin -> "Admin"
                | AccountTypes.AccountType.ContentProvider -> "Content Provider"
                | AccountTypes.AccountType.Customer -> "Customer"

        ///////////////////////////////////////////////////////////////////////////////////

        ////// API functions

        /// Retrieves an account from persistence based on its associated username
        /// Raises NoUserWithSuchName
        let getUserByName userName :AccountTypes.Account =
            let internalFun =
                if (Map.containsKey<string, AccountTypes.Account> userName Internal.cache.CachedUsers) then 
                    Map.find<string, AccountTypes.Account> userName Internal.cache.CachedUsers
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
                            info = Internal.getTypeInfoFromString (reader.GetString 9) reader
                            version = uint32 0 }
                        Internal.cache.addUser(result.user, result)
                        result
                    else
                        raise NoUserWithSuchName
            lock Internal.cache (fun() -> internalFun)

        /// Retrieves all accounts of a specific type
        let getAllUsersByType (accType:AccountTypes.AccountType) :AccountTypes.Account list =
            let internalFun =
                let sql = "SELECT * FROM [user] where (Type_name = '" + Internal.accTypeToString accType + "')"
                use reader = Internal.performSql sql
                let result =    [ 
                        while reader.Read() do
                        let tmp :AccountTypes.Account = {
                            user = reader.GetString 1
                            email = reader.GetString 2
                            password = Internal.splitDbPass (reader.GetString 5)
                            created = reader.GetDateTime 6
                            banned = if reader.GetByte 7 = byte 0 then false else true
                            info = Internal.getTypeInfoFromString (reader.GetString 9) reader
                            version = uint32 0
                        }
                        yield tmp
                        if not (Map.containsKey<string, AccountTypes.Account> tmp.user Internal.cache.CachedUsers) then
                            Internal.cache.addUser(tmp.user, tmp) ]
                result
            lock Internal.cache (fun() -> internalFun)

        /// Retrieves the date and time which the user {user} last authenticated
        /// 'None' means that the user never has authenticated
        /// Raises NoUserWithSuchName
        let getUsersLastAuthTime (user:string) :System.DateTime option =
            raise (new System.NotImplementedException())

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
                let sql = sql + 
                            let accType = AccountTypes.typeInfoToString acc.info
                            if accType = "Content Provider" || accType = "Customer" then
                                "Address = '" + "Dummy" + "'" +
                                if accType = "Customer" then
                                    "Date_of_birth = " + "11-11-2008 13:23:44" + ", 
                                    About_me = '" + "Dummy" + "', 
                                    Balance = " + "0" + ", 
                                    Zipcode = " + "2400" + ", 
                                    Country_Name = '" + "Dummy" + "'"
                                else
                                    ""
                            else
                                ""
                let sql = sql + "WHERE (Username = '" + acc.user + "')"
                let a' = Internal.performSql sql
                let newAcc :AccountTypes.Account = {
                                        user = acc.user;
                                        email = acc.email;
                                        password = acc.password;
                                        created = acc.created;
                                        banned = acc.banned;
                                        info = acc.info;
                                        version = acc.version + uint32(1) }
                Internal.cache.addUser(acc.user, acc)
                newAcc
                    
            lock Internal.cache (fun() -> internalFun)

        /// Raises UsernameAlreadyInUse
        let createUser (acc:AccountTypes.Account) =
            let internalFun =
                let nextId = Internal.getNextLoggableID() + 1
                let sql = "INSERT INTO [loggable] (Id) VALUES (" + string nextId + ");"
                //let sql = sql + "INSERT INTO [user] (Id, Username) VALUES (" + string nextId + ", 'kruger2')"
                let sql = sql + "INSERT INTO [user]
                            (Id, Username, Email, Password, Banned, Created_date, Type_name, Country_Name" +
                            let accType = AccountTypes.typeInfoToString acc.info
                            if accType = "Content Provider" || accType = "Customer" then
                                ",Address, Zipcode" +
                                    if accType = "Customer" then
                                        ", Date_of_birth,  About_me, Balance)"
                                    else
                                        ")"
                            else
                                ")"
                let sql = sql + " VALUES (" + string nextId + "," + 
                                            "'" + acc.user + "'," + 
                                            "'" + acc.email + "'," + 
                                            "'" + acc.password.salt + ":" + acc.password.hash + "'," +
                                            if acc.banned then "1" else "0"
                                            + "," + 
                                            "'" + string DateTime.Now + "'," + 
                                            "'" + AccountTypes.typeInfoToString acc.info + "'" +
                                            ",'DK'" +
                                            let accType = AccountTypes.typeInfoToString acc.info
                                            if accType = "Content Provider" || accType = "Customer" then
                                                ",'Ferskenvej 3', 2400" +
                                                if accType = "Customer" then
                                                    ",11-11-2008 13:23:44,  'About_me', '999')"
                                                else
                                                    ")"
                                            else ")"
                let a' = Internal.performSql sql
                Internal.cache.addUser(acc.user, acc)
                printf "%s" sql
            lock Internal.cache (fun() -> internalFun)