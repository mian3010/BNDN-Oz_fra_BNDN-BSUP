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

    let internal invokerToIdentity = function
    | Auth acc -> Permissions.Auth acc.user
    | Unauth -> Permissions.Unauth
    
    open Permissions
    open AccountTypes
    open Ops

    ///////////////////////////////////////////////////////////////////////

    module internal Internal =

        /// Returns a copy of A with all fields having default values set to their default values
        let defaultFrom (A:Account) :Account =
            let info =  match A.info with
                        | TypeInfo.Admin a -> TypeInfo.Admin a
                        | TypeInfo.ContentProvider cp -> TypeInfo.ContentProvider cp
                        | TypeInfo.Customer c -> TypeInfo.Customer  {
                                                                        name = c.name;
                                                                        address = c.address;
                                                                        birth = c.birth;
                                                                        about = c.about;
                                                                        credits = 0; // By default accounts start with 0 credits
                                                                    }
            {
                user = A.user;
                email = A.email;
                password = A.password;
                created = A.created;
                banned = false;         // Default: Not banned
                info = info;            // Default info
                version = uint32(0);    // Default: 0
            }
        
        /// Checks whether the banned status of account A and B is the same, and if not, whether the change is allowed
        let bannedOk check A B =
            if not (A.banned = B.banned) then
                match A.info with
                                | Customer c             -> check BAN_UNBAN_CUSTOMER
                                | ContentProvider cp     -> check BAN_UNBAN_CONTENT_PROVIDER
                                | Admin a                -> check BAN_UNBAN_ADMIN
            else true

        /// Checks whether the email field of account A and B is the same, and if not, whether the change is allowed
        /// {invoker} is the user trying to carry out this change
        let emailOk invoker check A B =
            if not (A.email = B.email) then
                match invoker with
                | Invoker.Auth a when a.user == A.user   -> check CHANGE_OWN_EMAIL // Only authenticated users "own" something
                | _ ->          match A.info with
                                | Customer c             -> check CHANGE_CUSTOMER_EMAIL
                                | ContentProvider cp     -> check CHANGE_CONTENT_PROVIDER_EMAIL
                                | Admin a                -> check CHANGE_ADMIN_EMAIL
            else not (A.email = null)

        /// Checks whether the password of account A and B is the same, and if not, whether the change is allowed
        /// {invoker} is the user trying to carry out this change
        let passwordOk invoker check A B =
            if not (A.password = B.password) then
                match invoker with
                | Invoker.Auth a when a.user == A.user   -> check CHANGE_OWN_PASSWORD // Only authenticated users "own" something
                | _ ->          match A.info with
                                | Customer c             -> check CHANGE_CUSTOMER_PASSWORD
                                | ContentProvider cp     -> check CHANGE_CONTENT_PROVIDER_PASSWORD
                                | Admin a                -> check CHANGE_ADMIN_PASSWORD
            else true

        /// Checks whether the credits of the customer account described by A may change to the credits described by B, if they are different
        /// {invoker} is the user trying to carry out this change
        /// {user} is the username associated with the account partially described by A
        let creditsOk invoker check user (A:Customer) (B:Customer) =
            if (B.credits > A.credits) then
                match invoker with
                | Invoker.Auth a when a.user == user     -> check GIVE_OWN_CREDITS // Only authenticated users "own" something
                | _                                      -> check GIVE_CREDITS
            elif (B.credits < A.credits) then
                match invoker with
                | Invoker.Auth a when a.user == user     -> check TAKE_OWN_CREDITS // Only authenticated users "own" something
                | _                                      -> check TAKE_CREDITS
            else true

        /// Checks whether the info of the customer account described by A may change to the info described by I, if they are different
        /// {invoker} is the user trying to carry out this change
        /// {user} is the username associated with the account partially described by A
        /// Raises an exception if I does not describe Customer info
        let customerOk invoker check user (A:Customer) (I:TypeInfo) =
            match I with
            | Customer i        ->  if not (A = i) then
                                        let otherDiff = (A.credits = i.credits)            // if credits are the same then other fields changed between A and i
                                        let creditsOk = (creditsOk invoker check user A i) // credits cannot change without permission. Check it!
                                        match invoker with
                                        | Invoker.Auth a when a.user == user     -> creditsOk && 
                                                                                    (not otherDiff || check EDIT_OWN) // Only authenticated users "own" something
                                        | _                                      -> creditsOk &&
                                                                                    (not otherDiff || check EDIT_CUSTOMER) // 'not otherDiff' = no edit (besides credits?) occured
                                    else true
            | _                 ->  failwith "I was not of type Customer"

        /// Checks whether the info of the content provider account described by A may change to the info described by I, if they are different
        /// {invoker} is the user trying to carry out this change
        /// {user} is the username associated with the account partially described by A
        /// Raises an exception if I does not describe ContentProvider info
        let contentProviderOk invoker check user (A:ContentProvider) (I:TypeInfo) =
            match I with
            | ContentProvider i ->  if not (A = i) then
                                        match invoker with
                                        | Invoker.Auth a when a.user == user     -> check EDIT_OWN // Only authenticated users "own" something
                                        | _                                      -> check EDIT_CONTENT_PROVIDER
                                    else true
            | _                 ->  failwith "I was not of type ContentProvider"
    
        /// Checks whether the info of the admin account described by A may change to the info described by I, if they are different
        /// {invoker} is the user trying to carry out this change
        /// {user} is the username associated with the account partially described by A
        /// Raises an exception if I does not describe Admin info
        let adminOk invoker check user (A:Admin) (I:TypeInfo) = 
            match I with
            | Admin a   -> true     // EDIT_OWN and EDIT_ADMIN if Admin is given any info
            | _         -> failwith "I was not of type Admin"
    

    ///////////////////////////////////////////////////////////////////////

    // Different helper functions to test access rights for various kinds of actions on accounts

    ///////////////////////////////////////////////////////////////////////
    
    open Internal

    /// Whether some invoker may change the info of some account to something else
    let mayPerformAccountUpdate invoker (targetAcc:Account) (updatedAcc:Account) =
        let A = targetAcc
        let B = updatedAcc
        // All immtable fields are not allowed to change!
        if     not (A.user == B.user)                                      then false
        elif   not (A.created = B.created)                                 then false
        elif   not ((typeInfoToString A.info) = (typeInfoToString B.info)) then false
        else // Test the fields, which may change, against permissions
            let check = Permissions.checkPermissions (invokerToIdentity invoker)
            let accOk = (bannedOk           check A B) &&
                        (emailOk    invoker check A B) &&
                        (passwordOk invoker check A B)
            match A.info with
            | Customer c            -> accOk && (customerOk          invoker check A.user c  B.info)
            | ContentProvider cp    -> accOk && (contentProviderOk   invoker check A.user cp B.info)
            | Admin a               -> accOk && (adminOk             invoker check A.user a  B.info)

    /// Whether some invoker may create the account {newAcc} incl. all stated info
    let mayCreateAccount invoker (newAcc:Account) =
        let N = newAcc
        // Verify required and/or immutable fields
        if   N.user = null then                     false
        elif N.email = null then                    false
        elif N.created > (System.DateTime.Now) then false
        else
            let check = Permissions.checkPermissions (invokerToIdentity invoker)
            let nonDefaultsOk = mayPerformAccountUpdate invoker (defaultFrom N) N // Creator must have permissions to create accounts with non-default values (credits, banned, ...)
            let createPermissions = match N.info with
                                    | Customer c            -> check CREATE_CUSTOMER
                                    | Admin a               -> check CREATE_ADMIN
                                    | ContentProvider cp    -> check CREATE_CONTENT_PROVIDER
            createPermissions && nonDefaultsOk

    /// Whether some invoker may delete some account
    let mayDeleteAccount invoker targetAcc = false // Not supported

    /// Whether some invoker may read the account info of the account with username {accUser}
    let mayRetrieveAccount invoker accUser =
        let check = Permissions.checkPermissions (invokerToIdentity invoker)
        let targetAcc = Account.getByUsername accUser
        match invoker with
        | Invoker.Auth a when a.user == accUser -> check READ_OWN // Only authenticated users "own" something
        | _ ->         match targetAcc.info with
                       | Customer c             -> check READ_CUSTOMER
                       | ContentProvider cp     -> check READ_CONTENT_PROVIDER
                       | Admin a                -> check READ_ADMIN

    /// Whether some invoker may retrieve all accounts of a specific type
    let mayRetrieveAccounts invoker (targetType:AccountType) =
        let check = Permissions.checkPermissions (invokerToIdentity invoker)
        match targetType with
        | AccountType.Customer          -> check READ_CUSTOMER
        | AccountType.ContentProvider   -> check READ_CONTENT_PROVIDER
        | AccountType.Admin             -> check READ_ADMIN

    /// Whether some invoker may read the authentication log info about the account with username {accUser}
    let mayReadAuthTime invoker accUser =
        let check = Permissions.checkPermissions (invokerToIdentity invoker)
        let targetAcc = Account.getByUsername accUser
        match invoker with
        | Invoker.Auth a when a.user == accUser -> check READ_OWN_AUTH_INFO // Only authenticated users "own" something
        | _                                     -> check READ_AUTH_INFO                                   

    /// Whether some invoker may reset the password of some account with username {accUser}
    let mayResetPassword invoker accUser =
        let check = Permissions.checkPermissions (invokerToIdentity invoker)
        let targetAcc = Account.getByUsername accUser
        match invoker with
        | Invoker.Auth a when a.user == accUser -> check RESET_OWN_PASSWORD // Only authenticated users "own" something
        | _ ->         match targetAcc.info with
                       | Customer c             -> check RESET_CUSTOMER_PASSWORD
                       | ContentProvider cp     -> check RESET_CONTENT_PROVIDER_PASSWORD
                       | Admin a                -> check RESET_ADMIN_PASSWORD