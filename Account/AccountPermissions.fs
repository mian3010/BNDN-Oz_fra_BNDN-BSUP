namespace RentIt

module AccountPermissions =  

    // The different kinds of users which may wish to attempt actions
    type Invoker =    Auth of Account.Account   // authenticated invokers
                    | Unauth                    // anonymous invokers

    exception AccountBanned    // Raised when a banned invoker attempts to perform an action
    exception PermissionDenied // Raised when an invoker attempts to perform an inaccessible action
    
    open AccountTypes
    open Account
    open Ops

    ///////////////////////////////////////////////////////////////////////

    module internal Internal =

        type CheckTarget =   Other of Permissions.Target
                           | Type of string

        let own = CheckTarget.Other Permissions.Target.Own
        let any = CheckTarget.Other Permissions.Target.Any

        let invokerToId = function
        | Invoker.Auth acc -> Permissions.Auth acc.user
        | Invoker.Unauth   -> Permissions.Unauth

        let check (invoker:Invoker) (permission:string) (target:CheckTarget) =
            match invoker with
            | Invoker.Auth acc when acc.banned  ->  false
            | _                                 ->  let check = Permissions.checkUserPermission (invokerToId invoker) permission
                                                    match target with
                                                    | Other x -> check x
                                                    | Type t  -> check (Permissions.Target.Type t)

        let hasPermission (accType:string) (permission:string) = true

        /// Returns a copy of A with all fields having default values set to their default values
        let defaultFrom (A:Account) :Account =
            let info =  {
                            name =      A.info.name;
                            address =   A.info.address;
                            birth =     A.info.birth;
                            about =     A.info.about;
                            credits =   if hasPermission A.accType "HAS_CREDITS" then Some 0 else None; // By default accounts start with 0 credits
                        }
            {
                user = A.user;
                email = A.email;
                password = A.password;
                created = A.created;
                banned = false;         // Default: Not banned
                accType = A.accType;
                info = info;            // Default info
                version = uint32(0);    // Default: 0
            }
        
        /// Whether {invoker} may change the ban status of one account to something else
        let bannedOk invoker (targetAcc:Account) (banned:bool) =
            not (targetAcc.banned = banned) &&
            check invoker "BAN_UNBAN" (CheckTarget.Type targetAcc.accType)

        /// Whether {invoker} may change the email address of one account to something else
        let emailOk invoker (targetAcc:Account) (email:string) =
            if not (targetAcc.email = email) then
                let check = check invoker "CHANGE_EMAIL"
                match invoker with
                | Invoker.Auth a when a.user = targetAcc.user   -> check own
                | _                                             -> check (CheckTarget.Type targetAcc.accType)
            else not (targetAcc.email = null)

        /// Whether {invoker} may change the password of one account to something else
        let passwordOk invoker (targetAcc:Account) (password:Password) =
            if not (targetAcc.password = password) then
                let check = check invoker "CHANGE_PASSWORD"
                match invoker with
                | Invoker.Auth a when a.user = targetAcc.user   -> check own
                | _                                             -> check (CheckTarget.Type targetAcc.accType)
            else true

        /// Whether {invoker} may change the credits of one account to something else
        let creditsOk invoker (targetAcc:Account) (updated:int option) =
            let current = targetAcc.info.credits

            if current = updated then true
            else
                match current with
                | None          ->  false // Account has no credits, but updated to some amount of credits
                | Some current  ->  match updated with
                                    | None          ->  false // Account had credits, but updated to no credits
                                    | Some updated  ->  if updated > current then
                                                            let check = check invoker "GIVE_CREDITS"
                                                            match invoker with
                                                            | Invoker.Auth a when a.user = targetAcc.user   -> check own
                                                            | _                                             -> check any
                                                        else
                                                            let check = check invoker "TAKE_CREDITS"
                                                            match invoker with
                                                            | Invoker.Auth a when a.user = targetAcc.user   -> check own
                                                            | _                                             -> check any
    

    ///////////////////////////////////////////////////////////////////////

    // Different helper functions to test access rights for various kinds of actions on accounts

    ///////////////////////////////////////////////////////////////////////
    
    open Internal

    /// Whether some invoker may change the info of some account to something else
    let mayPerformAccountUpdate invoker (targetAcc:Account) (updatedAcc:Account) =
        let A = targetAcc
        let B = updatedAcc
        // All immtable fields are not allowed to change!
        if     not (A.user   == B.user)     then false
        elif   not (A.created = B.created)  then false
        elif   not (A.accType = B.accType)  then false
        else // Test the fields, which may change, against permissions
            (bannedOk   invoker A B.banned) &&
            (emailOk    invoker A B.email) &&
            (passwordOk invoker A B.password) &&
            (creditsOk  invoker A B.info.credits)

    /// Whether some invoker may create the account {newAcc} incl. all stated info
    let mayCreateAccount invoker (newAcc:Account) =
        let N = newAcc
        // Verify required and/or immutable fields
        if   N.user    = null then                  false
        elif N.email   = null then                  false
        elif N.accType = null then                  false
        elif N.created > (System.DateTime.Now) then false
        else
            // Creator must have permissions to create accounts with non-default values (credits, banned, ...)
            let nonDefaultsOk = mayPerformAccountUpdate invoker (defaultFrom N) N

            nonDefaultsOk && (check invoker "CREATE" (CheckTarget.Type N.accType))

    /// Whether some invoker may delete some account
    let mayDeleteAccount invoker targetAcc = false // Not supported

    /// Whether some invoker may read the account info of the account with username {accUser}
    let mayRetrieveAccount invoker accUser =
        let check = check invoker "READ"
        try
            let accType = (Account.getByUsername accUser).accType
            match invoker with
            | Invoker.Auth a when a.user = accUser  -> check own
            | _                                     -> check (CheckTarget.Type accType)
        with
        | NoSuchUser -> false

    /// Whether some invoker may retrieve all accounts of a specific type
    let mayRetrieveAccounts invoker (targetType:string) =
        check invoker "READ" (CheckTarget.Type targetType)

    /// Whether some invoker may read the authentication log info about the account with username {accUser}
    let mayReadAuthTime invoker accUser =
        let check = check invoker "READ_AUTH_INFO"
        match invoker with
        | Invoker.Auth a when a.user = accUser  -> check own
        | _                                     -> check any                   

    /// Whether some invoker may reset the password of some account with username {accUser}
    let mayResetPassword invoker accUser =
        let check = check invoker "RESET_PASSWORD"
        try
            let accType = (Account.getByUsername accUser).accType
            match invoker with
            | Invoker.Auth a when a.user = accUser  -> check own
            | _                                     -> check (CheckTarget.Type accType)
        with
        | NoSuchUser -> false
