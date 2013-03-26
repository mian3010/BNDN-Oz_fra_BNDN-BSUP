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
        exception NoSuchAccountType

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
                { salt = parts.[1]; hash = parts.[0] }

            let getNextLoggableID() :int =
                let fieldQ = Persistence.ReadField.createReadFieldProc [] "loggable" "Id" Persistence.ReadField.Max
                let reader = Persistence.Api.read fieldQ "loggable" [] []

                if reader.IsEmpty then
                    0
                else
                    int(reader.Item(0).Item("loggable_Id_Max"))

        ///////////////////////////////////////////////////////////////////////////////////

        ////// API functions

        /// Retrieves an account from persistence based on its associated username
        /// Raises NoUserWithSuchName
        let getUserByName userName :AccountTypes.Account =
            //let internalFun =
            (*
                // If the user already exist in the cache, get him/her from there!
                if (Map.containsKey<string, AccountTypes.Account> userName Internal.cache) then
                    Map.find<string, AccountTypes.Account> userName Internal.cache
                else
                *)
                    let fieldsQ         = Persistence.ReadField.createReadFieldProc [] "User" "Username" Persistence.ReadField.All
                    
                    let baseObjectNameQ = "User"
                    let joinsQ          = []
                    let filtersQ        = Persistence.Filter.createFilter [] "User" "Username" "=" userName
                    let read = Persistence.Api.read fieldsQ baseObjectNameQ joinsQ filtersQ
                    (*let fieldsQ = Persistence.ReadField.createReadField [] "User" "Username" //Persistence.ReadField.All    
                    let filterQ = Persistence.Filter.createFilter [] "User" "Username" "=" userName
                    let read    = Persistence.Api.read fieldsQ "User" [] filterQ*)
    
                    // Oooops, no user with that username. Hmm, u stupid or some?
                    if read.IsEmpty then
                        raise NoUserWithSuchName

                    // Get the first, and (hopefully) only result
                    let result = read.Item(0)

                    // Create the account, using the information from "result"
                    let acc :AccountTypes.Account = {
                        user = result.Item "Username"
                        email = result.Item"Email"
                        password = Internal.splitDbPass (result.Item "Password")
                        created = DateTime.Parse (result.Item "Created_date")
                        banned = if result.Item "Banned" = "0" then false else true
                        info = {
                                name = None;
                                address = {
                                            address = result.TryFind "Address"
                                            postal = Some 2400
                                               (* let zipString = result.TryFind "Zipcode"
                                                if zipString = None then 
                                                    None else Some (int (string zipString))*) // CANT CONVERT!!! FIX IT! NOW! WITH CAPS! no
                                            country = Some (result.Item "Country_Name") }
                                birth = Some (DateTime.Parse "10/10/2013 12:00:00 AM")
                                    (*let bodString = result.TryFind "Date_of_birth"
                                    if bodString = None then 
                                        None else Some (DateTime.Parse (string bodString))*) // CANT CONVERT!!!
                                        
                                about = result.TryFind "About_me"
                                credits = Some 666
                                    (*let cString = result.TryFind "Balance"
                                    if cString = None then
                                        None else Some (int (string cString)) *)
                                }
                        accType = result.Item "Type_name"
                        version = uint32 0 }
                    // Add the new account to the cache
                    // Internal.cache <- Internal.cache.Add (acc.user, acc)
                    acc // Return him (or her)!
              
                        
            //lock Internal.cache (fun() -> internalFun)

        /// Retrieves all accounts of a specific type
        let getAllUsersByType accType :AccountTypes.Account list =
            let internalFun =
                let fieldsQ = Persistence.ReadField.createReadFieldProc [] "user" "Username" Persistence.ReadField.All
                let filterQ = Persistence.Filter.createFilter [] "user" "Type_name" "=" accType
                let reader  = Persistence.Api.read fieldsQ "user" [] filterQ
                let result = [
                    for map in reader do
                    let tmp :AccountTypes.Account = {
                        user = map.Item "Username"
                        email = map.Item"Email"
                        password = Internal.splitDbPass (map.Item "Password")
                        created = DateTime.Parse (map.Item "Created_date")
                        banned = if map.Item "Banned" = "0" then false else true
                        info = {
                                name = None;
                                address = {
                                            address = map.TryFind "Address"
                                            postal = 
                                                let zipString = map.TryFind "Zipcode"
                                                if zipString = None then 
                                                    None else Some (int (string zipString))
                                            country = Some (map.Item "Country_Name") }
                                birth =
                                    let bodString = map.TryFind "Date_of_birth"
                                    if bodString = None then 
                                        None else Some (DateTime.Parse (string bodString))
                                        
                                about = map.TryFind "About_me"
                                credits =
                                    let cString = map.TryFind "Balance"
                                    if cString = None then
                                        None else Some (int (string cString)) }
                        accType = map.Item "Type_name"
                        version = uint32 0 }
                    yield tmp
                    if not (Map.containsKey<string, AccountTypes.Account> tmp.user Internal.cache) then
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
                
                let talbeName = "user"
                let dataQ = Persistence.DataIn.createDataIn [] talbeName "Email" acc.email
                let dataQ = Persistence.DataIn.createDataIn dataQ talbeName "Address" (string acc.info.address.address)
                let dataQ = Persistence.DataIn.createDataIn dataQ talbeName "Date_of_birth" (string acc.info.birth)
                let dataQ = Persistence.DataIn.createDataIn dataQ talbeName "Password" (string acc.password.hash + ":" + acc.password.salt)
                let dataQ = Persistence.DataIn.createDataIn dataQ talbeName "Created_date" (string acc.created)
                let dataQ = Persistence.DataIn.createDataIn dataQ talbeName "Banned" (if acc.banned then "1" else "0")
                let dataQ = Persistence.DataIn.createDataIn dataQ talbeName "About_me" (string acc.info.about)
                let dataQ = Persistence.DataIn.createDataIn dataQ talbeName "Type_name" acc.accType
                let dataQ = Persistence.DataIn.createDataIn dataQ talbeName "Balance" (string acc.info.credits)
                let dataQ = Persistence.DataIn.createDataIn dataQ talbeName "Zipcode" (string acc.info.address.postal)
                let dataQ = Persistence.DataIn.createDataIn dataQ talbeName "Country_Name" (string acc.info.address.country)
                let filtersQ   = Persistence.Filter.createFilter [] talbeName "Username" "=" acc.user
                let updateR = Persistence.Api.update talbeName filtersQ dataQ


                let newAcc :AccountTypes.Account = {
                                        user = acc.user
                                        email = acc.email
                                        password = acc.password
                                        created = acc.created
                                        banned = acc.banned
                                        info = {
                                                name = None
                                                address = {
                                                            address = None
                                                            postal = None
                                                            country = None }
                                                birth = None
                                                about = None
                                                credits = None }
                                        accType = acc.accType
                                        version = acc.version + uint32 1 }
                Internal.cache <- Internal.cache.Add(acc.user, acc)
                newAcc
                    
            lock Internal.cache (fun() -> internalFun)

        /// Raises UsernameAlreadyInUse
        let createUser (acc:AccountTypes.Account) =
            let internalFun =
                let nextId = Internal.getNextLoggableID() + 1
                let transactionQ = Persistence.Transaction.createTransaction


                let talbeName = "loggable"
                let fieldProcessor = Persistence.Field.Default
                let dataQ = Persistence.DataIn.createDataIn []    talbeName "Id" (string nextId)
                let createIdQ = Persistence.Create.createCreate talbeName dataQ
                let transactionQ = Persistence.Transaction.addCreate transactionQ createIdQ

                let tableName = "user"
                let fieldProcessor = Persistence.Field.Default
                let dataQ = Persistence.DataIn.createDataIn []    talbeName "Id" (string nextId)
                let dataQ = Persistence.DataIn.createDataIn dataQ talbeName "Username" acc.user
                let dataQ = Persistence.DataIn.createDataIn dataQ talbeName "Email" acc.email
                let dataQ = Persistence.DataIn.createDataIn dataQ talbeName "Address" (string acc.info.address.address)
                let dataQ = Persistence.DataIn.createDataIn dataQ talbeName "Date_of_birth" (string acc.info.birth)
                let dataQ = Persistence.DataIn.createDataIn dataQ talbeName "Password" (string acc.password.hash + ":" + acc.password.salt)
                let dataQ = Persistence.DataIn.createDataIn dataQ talbeName "Created_date" (string acc.created)
                let dataQ = Persistence.DataIn.createDataIn dataQ talbeName "Banned" (if acc.banned then "1" else "0")
                let dataQ = Persistence.DataIn.createDataIn dataQ talbeName "About_me" (string acc.info.about)
                let dataQ = Persistence.DataIn.createDataIn dataQ talbeName "Type_name" acc.accType
                let dataQ = Persistence.DataIn.createDataIn dataQ talbeName "Balance" (string acc.info.credits)
                let dataQ = Persistence.DataIn.createDataIn dataQ talbeName "Zipcode" (string acc.info.address.postal)
                let dataQ = Persistence.DataIn.createDataIn dataQ talbeName "Country_Name" (string acc.info.address.country)
                let createUserQ = Persistence.Create.createCreate talbeName dataQ
                let transactionQ = Persistence.Transaction.addCreate transactionQ createUserQ

                let transactionR = Persistence.Api.transactionQ transactionQ
                
                Internal.cache <- Internal.cache.Add(acc.user, acc)
            lock Internal.cache (fun() -> internalFun)