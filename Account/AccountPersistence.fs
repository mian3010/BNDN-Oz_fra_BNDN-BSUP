namespace RentIt

open System
open System.Data
open System.Security
open AccountPersistenceExceptions
open AccountExceptions

    module AccountPersistence =

        module internal Internal =

            let getCountry (name:string) =
              let fieldsQ = Persistence.ReadField.createReadFieldProc [] "" "" Persistence.ReadField.All
              let filtersQ = Persistence.FilterGroup.createSingleFilterGroup [] "Country" "Name" name
              let res = Persistence.Api.read fieldsQ "Country" [] filtersQ
              if res.Length < 1 then raise NoSuchCountry
              res.Head.["Name"]

            let getUserType (name:string) =
              let fieldsQ = Persistence.ReadField.createReadFieldProc [] "" "" Persistence.ReadField.All
              let filtersQ = Persistence.FilterGroup.createSingleFilterGroup [] "UserType" "Name" name
              let res = Persistence.Api.read fieldsQ "UserType" [] filtersQ
              if res.Length < 1 then raise UnknownAccType
              res.Head.["Name"]

            // Cache elements
            let mutable cache = Map.empty : Map<string, AccountTypes.Account>

            ////// Helper functions
            let splitDbPass (dbPass:string) :AccountTypes.Password =
                let parts = dbPass.Split [|':'|]
                { salt = parts.[1]; hash = parts.[0] }

            let getNextLoggableID() :int =
                let reader = Persistence.Api.create "Loggable" []
                
                (int ((reader.Item 0).Item "Id"))

            let tryFind (map:Map<string, string>) target=
                let value = map.TryFind target
                if (value.Equals (Some "")) then None else value

        ///////////////////////////////////////////////////////////////////////////////////

        ////// API functions

        /// Retrieves an account from persistence based on its associated username
        /// Raises NoUserWithSuchName
        let getUserByName userName :AccountTypes.Account =
            let internalFun =
            
                // If the user already exist in the cache, get him/her from there!
                if (Map.containsKey<string, AccountTypes.Account> userName Internal.cache) then
                    Map.find<string, AccountTypes.Account> userName Internal.cache
                else
                
                    let fieldsQ         = Persistence.ReadField.createReadFieldProc [] "User" "Username" Persistence.ReadField.All
                    let baseObjectNameQ = "User"
                    let filtersQ        = Persistence.FilterGroup.createSingleFilterGroup [] "User" "Username" userName
                    let read = Persistence.Api.read fieldsQ baseObjectNameQ [] filtersQ
    
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
                        banned = if (result.Item "Banned").Equals "False" then false else true

                        info = {
                                name = Internal.tryFind result "Name";
                                address = {
                                            address = Internal.tryFind result "Address"
                                            postal = 
                                                let zipString = Internal.tryFind result  "Zipcode"
                                                if zipString = None then 
                                                    None else Some (int zipString.Value)
                                            country = Some (result.Item "Country_Name") }
                                birth =
                                    let bodString = Internal.tryFind result "Date_of_birth"
                                    if bodString = None then 
                                        None else Some (DateTime.Parse bodString.Value)
                                        
                                about = Internal.tryFind result "About_me"
                                credits =
                                    let cString = Internal.tryFind result "Balance"
                                    if cString = None then
                                        None else Some (int cString.Value)
                                }
                        accType = result.Item "Type_name"
                        version = uint32 0 }
                    // Add the new account to the cache
                    Internal.cache <- Internal.cache.Add (acc.user, acc)
                    acc // Return him (or her)!
              
            lock Internal.cache (fun() -> internalFun)

        /// Retrieves all accounts of a specific type
        let getAllUsersByType accType :AccountTypes.Account list =
            let internalFun =
                let fieldsQ = Persistence.ReadField.createReadFieldProc [] "user" "Username" Persistence.ReadField.All
                let filterQ = Persistence.FilterGroup.createSingleFilterGroup [] "user" "Type_name" accType
                let reader  = Persistence.Api.read fieldsQ "user" [] filterQ
                let result = [
                    for map in reader do
                    let tmp :AccountTypes.Account = {
                        user = map.Item "Username"
                        email = map.Item"Email"
                        password = Internal.splitDbPass (map.Item "Password")
                        created = DateTime.Parse (map.Item "Created_date")
                        banned = if (map.Item "Banned").Equals "False" then false else true
                        info = {
                                name = Internal.tryFind map "Name";
                                address = {
                                            address = Internal.tryFind map "Address"
                                            postal = 
                                                let zipString = Internal.tryFind map "Zipcode"
                                                if zipString = None then 
                                                    None else Some (int zipString.Value)
                                            country = Some (map.Item "Country_Name") }
                                birth =
                                    let bodString = Internal.tryFind map "Date_of_birth"
                                    if bodString = None then 
                                        None else Some (DateTime.Parse bodString.Value)
                                        
                                about = Internal.tryFind map "About_me"
                                credits =
                                    let cString = Internal.tryFind map "Balance"
                                    if cString = None then
                                        None else Some (int cString.Value)
                                }
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
                
                let tableName = "User"
                let fieldProcessor = Persistence.Field.Default

                let dataQ = ref (Persistence.DataIn.createDataIn [] tableName "Username" acc.user)
                dataQ := Persistence.DataIn.createDataIn !dataQ tableName "Email" acc.email
                if (acc.info.address.address.IsSome) then dataQ := Persistence.DataIn.createDataIn !dataQ tableName "Address" (string acc.info.address.address.Value)
                if (acc.info.birth.IsSome) then dataQ := Persistence.DataIn.createDataIn !dataQ tableName "Date_of_birth" (string acc.info.birth.Value)
                dataQ := Persistence.DataIn.createDataIn !dataQ tableName "Password" (string acc.password.hash + ":" + acc.password.salt)
                dataQ := Persistence.DataIn.createDataIn !dataQ tableName "Created_date" (string acc.created)
                dataQ := Persistence.DataIn.createDataIn !dataQ tableName "Banned" (if acc.banned then "1" else "0")
                if (acc.info.about.IsSome) then dataQ := Persistence.DataIn.createDataIn !dataQ tableName "About_me" (string acc.info.about.Value)
                dataQ := Persistence.DataIn.createDataIn !dataQ tableName "Type_name" acc.accType
                if (acc.info.credits.IsSome) then dataQ := Persistence.DataIn.createDataIn !dataQ tableName "Balance" (string acc.info.credits.Value)
                if (acc.info.address.postal.IsSome) then dataQ := Persistence.DataIn.createDataIn !dataQ tableName "Zipcode" (string acc.info.address.postal.Value)
                if (acc.info.address.country.IsSome) then dataQ := Persistence.DataIn.createDataIn !dataQ tableName "Country_Name" (string acc.info.address.country.Value)
                let filtersQ   = Persistence.FilterGroup.createSingleFilterGroup [] tableName "Username" acc.user
                let updateR = Persistence.Api.update tableName filtersQ !dataQ


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
                let nextId = Internal.getNextLoggableID()
                let transactionQ = Persistence.Transaction.createTransaction

                let tableName = "User"
                let fieldProcessor = Persistence.Field.Default

                let dataQ = ref (Persistence.DataIn.createDataIn []    tableName "Id" (string nextId))
                dataQ := Persistence.DataIn.createDataIn !dataQ tableName "Username" acc.user
                dataQ := Persistence.DataIn.createDataIn !dataQ tableName "Email" acc.email
                if (acc.info.address.address.IsSome) then dataQ := Persistence.DataIn.createDataIn !dataQ tableName "Address" (string acc.info.address.address.Value)
                if (acc.info.birth.IsSome) then dataQ := Persistence.DataIn.createDataIn !dataQ tableName "Date_of_birth" (string acc.info.birth.Value)
                dataQ := Persistence.DataIn.createDataIn !dataQ tableName "Password" (string acc.password.hash + ":" + acc.password.salt)
                dataQ := Persistence.DataIn.createDataIn !dataQ tableName "Created_date" (string acc.created)
                dataQ := Persistence.DataIn.createDataIn !dataQ tableName "Banned" (if acc.banned then "1" else "0")
                if (acc.info.about.IsSome) then dataQ := Persistence.DataIn.createDataIn !dataQ tableName "About_me" (string acc.info.about.Value)
                dataQ := Persistence.DataIn.createDataIn !dataQ tableName "Type_name" acc.accType
                if (acc.info.credits.IsSome) then dataQ := Persistence.DataIn.createDataIn !dataQ tableName "Balance" (string acc.info.credits.Value)
                if (acc.info.address.postal.IsSome) then dataQ := Persistence.DataIn.createDataIn !dataQ tableName "Zipcode" (string acc.info.address.postal.Value)
                if (acc.info.address.country.IsSome) then dataQ := Persistence.DataIn.createDataIn !dataQ tableName "Country_Name" (string acc.info.address.country.Value)
                try 
                  Persistence.Api.create tableName !dataQ |> ignore
                with
                  | PersistenceExceptions.AlreadyExists -> raise NoUserWithSuchName
                  | PersistenceExceptions.ReferenceDoesNotExist ->
                      try
                      // Check each param to dertimin exception
                      if acc.info.address.country.IsSome then
                        Internal.getCountry acc.info.address.country.Value |> ignore
                      Internal.getUserType acc.accType |> ignore
                      with
                        | _ as e -> raise e
                                      
                Internal.cache <- Internal.cache.Add(acc.user, acc)
            lock Internal.cache (fun() -> internalFun)

        /// <summary>
        /// Get a list of countries 
        /// </summary>
        /// <returns> String list of countries </returns>
        let getListOfCountries () =
          let fieldsQ = Persistence.ReadField.createReadFieldProc [] "" "" Persistence.ReadField.All
          let v = Persistence.Api.read fieldsQ "Country" [] []
          let mutable l:string list = []
          for c in v do
            l <- l@[c.["Name"]]
          List.toArray l