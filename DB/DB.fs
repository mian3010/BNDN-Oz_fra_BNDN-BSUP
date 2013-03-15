namespace RentIt

open System
open System.Data

    module Db =

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
                let mutable cachedUsers = Map.empty : Map<string, AccountTypes.Account>

                member x.CachedUsers = Map.empty : Map<string, AccountTypes.Account>

                member x.addUser(key, value) = cachedUsers <- cachedUsers.Add(key, value)
            end

            ////// Helper functions
            let splitDbPass (dbPass:string) :AccountTypes.PasswordTypes.Password =
                let parts = dbPass.Split [|':'|]
                { salt = parts.[0]; hash = parts.[1] }

        ///////////////////////////////////////////////////////////////////////////////////

        // Exceptions
        exception NoUserWithSuchName
        exception UsernameAlreadyInUse
        exception NewerVersionExist
        exception IllegalAccountVersion

        // Cache elements
        let internal cache = new Internal.Cache()

        ////// API functions

        /// Retrieves an account from persistence based on its associated username
        /// Raises NoUserWithSuchName
        let getUserByName userName :AccountTypes.Account =
            let internalFun =
                if (Map.containsKey<string, AccountTypes.Account> userName cache.CachedUsers) then 
                    Map.find<string, AccountTypes.Account> userName cache.CachedUsers
                else
                    let sql = "SELECT * FROM [table] where [username] =" + userName
                    use reader = Internal.performSql sql

                    if reader.HasRows = false then raise NoUserWithSuchName
                    
                    let result :AccountTypes.Account =  {
                                    user = "dude";
                                    email = "where.is@my.car";
                                    password = Internal.splitDbPass "123:abc";
                                    created = System.DateTime.Now;
                                    banned = false;
                                    info = AccountTypes.TypeInfo.Admin ();
                                    version = uint32(0) }
                    cache.addUser(result.user, result)
                    result
            lock cache (fun() -> internalFun)

        /// Retrieves all accounts of a specific type
        let getAllUsersByType (accType:AccountTypes.AccountType) :AccountTypes.Account list =
            let internalFun =
                let sql = "SELECT * FROM [TABLE] where [?] = " + accType.ToString() // Query is not right!
                use reader = Internal.performSql sql
                let result =    [ while reader.Read() do
                                    let tmp :AccountTypes.Account = {
                                        user = "dude";
                                        email = "where.is@my.car";
                                        password = Internal.splitDbPass "123:abc";
                                        created = System.DateTime.Now;
                                        banned = false;
                                        info = AccountTypes.TypeInfo.Admin ();
                                        version = uint32(0) }
                                    yield tmp
                                    if not (Map.containsKey<string, AccountTypes.Account> tmp.user cache.CachedUsers) then
                                        cache.addUser(tmp.user, tmp)
                                ]
                result
            lock cache (fun() -> internalFun)

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
                cache.addUser(acc.user, acc)
                newAcc
                    
            lock cache (fun() -> internalFun)

        /// Raises UsernameAlreadyInUse
        let createUser (acc:AccountTypes.Account) =
            let internalFun =
                let sql = "insert into"
                let a' = Internal.performSql sql
                cache.addUser(acc.user, acc)
            lock cache (fun() -> internalFun)
            