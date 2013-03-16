namespace RentIt

open System
open System.Data
open System.Security

    module Db =

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
            for i in "ZAQ12wsx" do
                secPass.AppendChar(i)

            secPass.MakeReadOnly()

            let connectionString = "server=" + server + ";Database=" + database// + ";Uid=" + userName + ";Pwd=" + pass
            //let connectionString = "Data Source=" + server + ";Initial Catalog=" + database + ";Integrated Security=True;User ID=" + userName + ";Password=" + pass
            
            let cred = SqlClient.SqlCredential(userName, secPass)
            
            let connDB = new SqlClient.SqlConnection(connectionString, cred)

            let performSql sql =
                let cmd = new SqlClient.SqlCommand(sql, connDB)
                connDB.Open()
                cmd.ExecuteReader()

            ////// Helper functions
            let splitDbPass (dbPass:string) :AccountTypes.PasswordTypes.Password =
                let parts = dbPass.Split [|':'|]
                { salt = parts.[0]; hash = parts.[1] }

            let getNextLoggableID =
                let sql = "SELECT MAX(Id) FROM loggable"
                use reader = performSql sql
                if reader.HasRows then 
                    let result = unbox (reader.["Id"])
                    connDB.Close()
                    result
                else
                    connDB.Close()
                    0
        ///////////////////////////////////////////////////////////////////////////////////

        // Exceptions
        exception NoUserWithSuchName
        exception UsernameAlreadyInUse
        exception NewerVersionExist
        exception IllegalAccountVersion

        ////// API functions

        /// Retrieves an account from persistence based on its associated username
        /// Raises NoUserWithSuchName
        let getUserByName userName :AccountTypes.Account =
            let internalFun =
                if (Map.containsKey<string, AccountTypes.Account> userName Internal.cache.CachedUsers) then 
                    Map.find<string, AccountTypes.Account> userName Internal.cache.CachedUsers
                else
                    let sql = "SELECT * FROM [table] where [username] =" + userName
                    use reader = Internal.performSql sql

                    if reader.HasRows = false then raise NoUserWithSuchName
                    
                    let result :AccountTypes.Account =  {
                                    user = unbox (reader.["Username"]);
                                    email = unbox (reader.["Email"]);
                                    password = Internal.splitDbPass (unbox (reader.["Password"]));
                                    created = unbox (reader.["Created_date"]);
                                    banned = false;
                                    info = AccountTypes.TypeInfo.Admin ();
                                    version = uint32(0) }
                    Internal.cache.addUser(result.user, result)
                    result
            lock Internal.cache (fun() -> internalFun)

        /// Retrieves all accounts of a specific type
        let getAllUsersByType (accType:AccountTypes.AccountType) :AccountTypes.Account list =
            let internalFun =
                let sql = "SELECT * FROM [TABLE] where [?] = " + accType.ToString() // Query is not right!
                use reader = Internal.performSql sql
                let result =    [ 
                        while reader.Read() do
                        let tmp :AccountTypes.Account = {
                            user = "dude";
                            email = "where.is@my.car";
                            password = Internal.splitDbPass "123:abc";
                            created = System.DateTime.Now;
                            banned = false;
                            info = AccountTypes.TypeInfo.Admin ();
                            version = uint32(0) }
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
                
                let sql = "UPDATE"
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
                let sql = "insert into"
                let a' = Internal.performSql sql
                Internal.cache.addUser(acc.user, acc)
            lock Internal.cache (fun() -> internalFun)