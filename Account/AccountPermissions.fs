namespace RentIt

module AccountPermissions =
    
    // Different helper functions to test access rights for various kinds of actions on accounts

    ///////////////////////////////////////////////////////////////////////
    
    // The different kinds of users which may wish to attempt actions
    type Invoker =    Auth of Account.Account   // authenticated invokers
                    | Unauth                    // anonymous invokers

    exception AccountBanned    // Raised when a banned invoker attempts to perform an action
    exception PermissionDenied // Raised when an invoker attempts to perform an inaccessible action


    
    /// Whether some invoker may create the account {newAcc} incl. all stated info
    let mayCreateAccount invoker newAcc = false
    // CREATE_CUSTOMER
    // CREATE_CONTENT_PROVIDER
    // CREATE_ADMIN
    // BAN_UNBAN_CUSTOMER
    // BAN_UNBAN_CONTENT_PROVIDER
    // BAN_UNBAN_ADMIN
    // GIVE_CREDITS
    // TAKE_CREDITS

    /// Whether some invoker may change the info of some account to something else
    let mayPerformAccountUpdate invoker updatedAcc targetAcc = false
    // EDIT_OWN
    // EDIT_CUSTOMER
    // EDIT_CONTENT_PROVIDER
    // EDIT_ADMIN

    // CHANGE_OWN_PASSWORD
    // CHANGE_CUSTOMER_PASSWORD
    // CHANGE_CONTENT_PROVIDER_PASSWORD
    // CHANGE_ADMIN_PASSWORD

    // CHANGE_OWN_EMAIL
    // CHANGE_CUSTOMER_EMAIL
    // CHANGE_CONTENT_PROVIDER_EMAIL
    // CHANGE_ADMIN_EMAIL

    // Also the BAN_UNBAN_* and GIVE/TAKE_CREDITS

    /// Whether some invoker may delete some account
    let mayDeleteAccount invoker targetAcc = false

    /// Whether some invoker may read the account info of the account with username {accUser}
    let mayRetrieveAccount invoker accUser = false
    // READ_OWN
    // READ_CUSTOMER
    // READ_CONTENT_PROVIDER
    // READ_ADMIN

    /// Whether some invoker may retrieve all accounts of a specific type
    let mayRetrieveAccounts invoker targetType = false
    // READ_* (* = type)

    /// Whether some invoker may read the authentication log info about the account with username {accUser}
    let mayReadAuthTime invoker accUser = false
    // READ_OWN_AUTH_INFO
    // READ_AUTH_INFO

    /// Whether some invoker may reset the password of some account with username {accUser}
    let mayResetPassword invoker accUser = false
    // RESET_OWN_PASSWORD
    // RESET_CUSTOMER_PASSWORD
    // RESET_CONTENT_PROVIDER_PASSWORD
    // RESET_ADMIN_PASSWORD