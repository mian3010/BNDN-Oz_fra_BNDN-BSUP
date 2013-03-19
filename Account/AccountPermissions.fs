namespace RentIt

    (*type Permission =    CREATE_CUSTOMER
                       | CREATE_CONTENT_PROVIDER
                       | CREATE_ADMIN
                      // Full read permissions to own account or to accounts of a specific type. Includes "overview" operation.
                       | READ_OWN
                       | READ_CUSTOMER
                       | READ_CONTENT_PROVIDER
                       | READ_ADMIN
                      // Read the last time of authentication of own account or for account of a specific type.
                       | READ_OWN_AUTH_INFO
                       | READ_AUTH_INFO
                      // Ban/unban accounts of a specific type
                       | BAN_UNBAN_CUSTOMER
                       | BAN_UNBAN_CONTENT_PROVIDER
                       | BAN_UNBAN_ADMIN
                      // Give/take credits from a customer account
                       | GIVE_OWN_CREDITS // Without this permission a customer cannot buy more credits
                       | TAKE_OWN_CREDITS // Without this permission a customer cannot do any product purchase
                       | GIVE_CREDITS
                       | TAKE_CREDITS
                      // Change various specific fields (email / password) of own account or for account of a specific type.
                       | CHANGE_OWN_PASSWORD
                       | CHANGE_CUSTOMER_PASSWORD
                       | CHANGE_CONTENT_PROVIDER_PASSWORD
                       | CHANGE_ADMIN_PASSWORD
                       | RESET_OWN_PASSWORD
                       | RESET_CUSTOMER_PASSWORD
                       | RESET_CONTENT_PROVIDER_PASSWORD
                       | RESET_ADMIN_PASSWORD
                       | CHANGE_OWN_EMAIL
                       | CHANGE_CUSTOMER_EMAIL
                       | CHANGE_CONTENT_PROVIDER_EMAIL
                       | CHANGE_ADMIN_EMAIL
                      // Change all non-specific fields (except immutable data, such as Created) of own account or for account of a specific type.
                       | EDIT_OWN
                       | EDIT_CUSTOMER
                       | EDIT_CONTENT_PROVIDER
                       | EDIT_ADMIN*)

module AccountPermissions =  

    // The different kinds of users which may wish to attempt actions
    type Invoker =    Auth of Account.Account   // authenticated invokers
                    | Unauth                    // anonymous invokers

    exception AccountBanned    // Raised when a banned invoker attempts to perform an action
    exception PermissionDenied // Raised when an invoker attempts to perform an inaccessible action
    
    open Permissions
    open AccountTypes
    open Account
    open Ops

    ///////////////////////////////////////////////////////////////////////

    module internal Internal =

        let own = Permissions.Target.Own
        let any = Permissions.Target.Any

        let invokerToId = function
        | Invoker.Auth acc -> Permissions.Auth acc.user
        | Invoker.Unauth   -> Permissions.Unauth

        let check (invoker:Invoker) (permission:string) target =
            match invoker with
            | Invoker.Auth acc when acc.banned  ->  false
            | _                                 ->  let check = Permissions.checkPermissions (invokerToId invoker) permission
                                                    match target with
                                                    | string as t   -> (Permissions.Target.Type t)
                                                    | x             -> check x

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
        
        /// Checks whether the banned status of account A and B is the same, and if not, whether the change is allowed
        let bannedOk invoker (targetAcc:Account) (updatedAcc:Account) =
            not (targetAcc.banned = updatedAcc.banned) &&
            check invoker "BAN_UNBAN" targetAcc.accType

        /// Checks whether the email field of account A and B is the same, and if not, whether the change is allowed
        /// {invoker} is the user trying to carry out this change
        let emailOk invoker (targetAcc:Account) (updatedAcc:Account) =
            if not (targetAcc.email = updatedAcc.email) then
                let check = check invoker "CHANGE_EMAIL"
                match invoker with
                | Invoker.Auth a when a.user = targetAcc.user   -> check own
                | _                                             -> check targetAcc.accType
            else not (targetAcc.email = null)

        /// Checks whether the password of account A and B is the same, and if not, whether the change is allowed
        /// {invoker} is the user trying to carry out this change
        let passwordOk invoker (targetAcc:Account) (updatedAcc:Account) =
            if not (targetAcc.password = updatedAcc.password) then
                let check = check invoker "CHANGE_PASSWORD"
                match invoker with
                | Invoker.Auth a when a.user = targetAcc.user   -> check own
                | _                                             -> check targetAcc.accType
            else true

        /// Checks whether the credits of the customer account described by A may change to the credits described by B, if they are different
        /// {invoker} is the user trying to carry out this change
        /// {user} is the username associated with the account partially described by A
        let creditsOk invoker (targetAcc:Account) (updatedAcc:Account) =
            let current = targetAcc.info.credits
            let updated = updatedAcc.info.credits

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
            (bannedOk   invoker A B) &&
            (emailOk    invoker A B) &&
            (passwordOk invoker A B) &&
            (creditsOk  invoker A B)

    /// Whether some invoker may create the account {newAcc} incl. all stated info
    let mayCreateAccount invoker (newAcc:Account) =
        let N = newAcc
        // Verify required and/or immutable fields
        if   N.user    = null then                  false
        elif N.email   = null then                  false
        elif N.accType = null then                  false
        elif N.created > (System.DateTime.Now) then false
        else
            let nonDefaultsOk = mayPerformAccountUpdate invoker (defaultFrom N) N // Creator must have permissions to create accounts with non-default values (credits, banned, ...)
            nonDefaultsOk && (check invoker "CREATE" newAcc.accType)

    /// Whether some invoker may delete some account
    let mayDeleteAccount invoker targetAcc = false // Not supported

    /// Whether some invoker may read the account info of the account with username {accUser}
    let mayRetrieveAccount invoker accUser =
        let check = check invoker "READ"
        try
            let accType = (Account.getByUsername accUser).accType
            match invoker with
            | Invoker.Auth a when a.user = accUser  -> check own
            | _                                     -> check accType
        with
        | NoSuchUser -> false

    /// Whether some invoker may retrieve all accounts of a specific type
    let mayRetrieveAccounts invoker (targetType:string) =
        check invoker "READ" targetType

    /// Whether some invoker may read the authentication log info about the account with username {accUser}
    let mayReadAuthTime invoker accUser =
        let check = check invoker "READ_AUTH_INFO"
        try
            let accType = (Account.getByUsername accUser).accType
            match invoker with
            | Invoker.Auth a when a.user = accUser  -> check own
            | _                                     -> check any
        with
        | NoSuchUser -> false                             

    /// Whether some invoker may reset the password of some account with username {accUser}
    let mayResetPassword invoker accUser =
        let check = check invoker "RESET_PASSWORD"
        try
            let accType = (Account.getByUsername accUser).accType
            match invoker with
            | Invoker.Auth a when a.user = accUser  -> check own
            | _                                     -> check accType
        with
        | NoSuchUser -> false
