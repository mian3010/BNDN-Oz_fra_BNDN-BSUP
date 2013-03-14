namespace RentIt

module AccountPermissions =
    
    ///////////////////////////////////////////////////////////////////////
    
    // The different kinds of users which may wish to attempt actions
    type Invoker =    Auth of Account.Account   // authenticated invokers
                    | Unauth of unit            // anonymous invokers

    exception AccountBanned    // Raised when a banned invoker attempts to perform an action
    exception PermissionDenied // Raised when an invoker attempts to perform an inaccessible action


    
    /// Whether some invoker may create the account {newAcc} incl. all stated info
    let mayCreateAccount invoker newAcc = false

    /// Whether some invoker may change the info of some account to something else
    let mayPerformAccountUpdate invoker updatedAcc targetAcc = false

    /// Whether some invoker may delete some account
    let mayDeleteAccount invoker targetAcc = false

    /// Whether some invoker may read the account info of the account with username {accUser}
    let mayRetrieveAccount invoker accUser = false

    /// Whether some invoker may retrieve all accounts of a specific type
    let mayRetrieveAccounts invoker targetType = false

    /// Whether some invoker may read the authentication log info about the account with username {accUser}
    let mayReadAuthTime invoker accUser = false