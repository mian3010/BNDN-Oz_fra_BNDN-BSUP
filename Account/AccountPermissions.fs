namespace RentIt
open AccountExceptions

module AccountPermissions =  

    // The different kinds of users which may wish to attempt actions
    type Invoker =    Auth of AccountTypes.Account   // authenticated invokers
                    | Unauth                    // anonymous invokers

    type Access =     Accepted
                    | Denied of string          // Reason why it was denied

    exception AccountBanned              // Raised when a banned invoker attempts to perform an action
    exception PermissionDenied of string // Raised when an invoker attempts to perform an inaccessible action
    
    open AccountTypes
    open Account
    open Ops

    ///////////////////////////////////////////////////////////////////////

    module internal Internal =

        type CheckTarget =   Other of   Permissions.Target
                           | Type of    string

        let own = CheckTarget.Other Permissions.Target.Own
        let any = CheckTarget.Other Permissions.Target.Any

        let invokerToId = function
        | Invoker.Auth acc -> Permissions.Auth acc.user
        | Invoker.Unauth   -> Permissions.Unauth

        let check (invoker:Invoker) (permission:string) (target:CheckTarget) =
            match invoker with
            | Invoker.Auth acc when acc.banned  ->  Access.Denied "Invokers account is banned"
            | _                                 ->  let check = Permissions.checkUserPermission (invokerToId invoker) permission
                                                    let hasPermission = match target with
                                                                        | Other x -> check x
                                                                        | Type t  -> check (Permissions.Target.Type t)
                                                    if hasPermission then Access.Accepted
                                                    else Access.Denied ("The system has not granted invoker's account the permission "+permission+", or invoker is not allowed to perform its action on the given target")

        // TODO: Someone oughta fix this:
        let hasPermission (accType:string) (permission:string) = 
          Permissions.checkUserTypePermission accType permission

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
            if targetAcc.banned = banned then Access.Accepted
            else check invoker "BAN_UNBAN" (CheckTarget.Type targetAcc.accType)

        /// Whether {invoker} may change the email address of one account to something else
        let emailOk invoker (targetAcc:Account) (email:string) =
            if not (targetAcc.email = email) then
                let check = check invoker "CHANGE_EMAIL"
                match invoker with
                | Invoker.Auth a when a.user == targetAcc.user  -> check own
                | _                                             -> check (CheckTarget.Type targetAcc.accType)
            else if not (targetAcc.email = null) then Access.Accepted
            else Access.Denied "You may not change the email address to null"

        /// Whether {invoker} may change the password of one account to something else
        let passwordOk invoker (targetAcc:Account) (password:Password) =
            if not (targetAcc.password = password) then
                let check = check invoker "CHANGE_PASSWORD"
                match invoker with
                | Invoker.Auth a when a.user == targetAcc.user  -> check own
                | _                                             -> check (CheckTarget.Type targetAcc.accType)
            else Access.Accepted

        /// Whether {invoker} may edit the addition information of one account to something else
        let editOk invoker (targetAcc:Account) (info:ExtraAccInfo) =
            let I = targetAcc.info
            let unchanged = 
                I.about = info.about &&
                I.address = info.address &&
                I.birth = info.birth &&
                I.name = info.name
            if not unchanged then
                let check = check invoker "EDIT"
                match invoker with
                | Invoker.Auth a when a.user == targetAcc.user  -> check own
                | _                                             -> check (CheckTarget.Type targetAcc.accType)
            else Access.Accepted

        /// Whether {invoker} may change the credits of one account to something else
        let creditsOk invoker (targetAcc:Account) (updated:int option) =
            let current = targetAcc.info.credits

            if current = updated then Access.Accepted
            else
                match current with
                | None          ->  Access.Denied "Target account is not capable of having credits" // Account has no credits, but updated to some amount of credits
                | Some current  ->  match updated with
                                    | None          ->  Access.Denied "Target account have credits. Invoker set credits to None" // Account had credits, but updated to no credits
                                    | Some updated  ->  if updated > current then
                                                            let check = check invoker "GIVE_CREDITS"
                                                            match invoker with
                                                            | Invoker.Auth a when a.user == targetAcc.user  -> check own
                                                            | _                                             -> check any
                                                        else
                                                            let check = check invoker "TAKE_CREDITS"
                                                            match invoker with
                                                            | Invoker.Auth a when a.user == targetAcc.user  -> check own
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
        if     not (A.user   == B.user)     then Access.Denied "No invoker may change the username of an account!"
        elif   not (A.created = B.created)  then Access.Denied "No invoker may change the creation time of an account!"
        elif   not (A.accType = B.accType)  then Access.Denied "No invoker may change the account type of an account!"
        else // Test the fields, which may change, against permissions

            let bannedAccess =                  (bannedOk   invoker A B.banned)
            if bannedAccess = Access.Accepted then

                let emailAccess =               (emailOk    invoker A B.email)
                if emailAccess = Access.Accepted then

                    let passwordAccess =        (passwordOk invoker A B.password)
                    if passwordAccess = Access.Accepted then

                        let creditsAccess =     (creditsOk  invoker A B.info.credits)
                        if creditsAccess = Access.Accepted then

                            editOk invoker A B.info

                        else creditsAccess
                    else passwordAccess
                else emailAccess
            else bannedAccess

    /// Whether some invoker may create the account {newAcc} incl. all stated info
    let mayCreateAccount invoker (newAcc:Account) =
        let N = newAcc
        // Verify required and/or immutable fields
        if   N.user    = null then                  Access.Denied "No invoker may create accounts without a username!"
        elif N.email   = null then                  Access.Denied "No invoker may create accounts without an email!"
        elif N.accType = null then                  Access.Denied "No invoker may create accounts without an account type!"
        elif N.created > (System.DateTime.Now) then Access.Denied "No invoker may create accounts with a creation time of the future!"
        else
            // Creator must have permissions to create accounts with non-default values (credits, banned, ...)
            let nonDefaults = mayPerformAccountUpdate invoker (defaultFrom N) N

            if nonDefaults = Access.Accepted then check invoker "CREATE" (CheckTarget.Type N.accType)
            else nonDefaults

    /// Whether some invoker may delete some account
    let mayDeleteAccount invoker targetAcc = Access.Denied "Not supported by the system" // Not supported

    /// Whether some invoker may read the account info of the account with username {accUser}
    let mayRetrieveAccount invoker accUser =
        let check = check invoker "READ"
        try
            let accType = (Account.getByUsername accUser).accType
            match invoker with
            | Invoker.Auth a when a.user == accUser -> check own
            | _                                     -> check (CheckTarget.Type accType)
        with
        | NoSuchUser -> Access.Denied "No account with the target username exists"

    /// Whether some invoker may retrieve all accounts of a specific type
    let mayRetrieveAccounts invoker (targetType:string) =
        check invoker "READ" (CheckTarget.Type targetType)

    /// Whether some invoker may read the authentication log info about the account with username {accUser}
    let mayReadAuthTime invoker accUser =
        let check = check invoker "READ_AUTH_INFO"
        match invoker with
        | Invoker.Auth a when a.user == accUser -> check own
        | _                                     -> check any                   

    /// Whether some invoker may reset the password of some account with username {accUser}
    let mayResetPassword invoker accUser =
        let check = check invoker "RESET_PASSWORD"
        try
            let accType = (Account.getByUsername accUser).accType
            match invoker with
            | Invoker.Auth a when a.user == accUser -> check own
            | _                                     -> check (CheckTarget.Type accType)
        with
        | NoSuchUser -> Access.Denied "No account with the target username exists"

    /// Whether some invoker may return all accepted country names of the Account.ExtraAccInfo.address field
    let mayRetrieveCountryList invoker = Access.Accepted // TODO: Add permission check to database
