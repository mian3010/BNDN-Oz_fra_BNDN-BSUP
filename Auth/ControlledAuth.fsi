namespace RentIt

module ControlledAuth =
    
    /// Wrapper for Auth.authenticate
    /// Adds a check for banned accounts, meaning that banned users cannot authenticate (Permissions.AccountBanned is raised)
    val authenticate : string -> string -> string * System.DateTime

    /// Wrapper for Auth.accessAccount
    /// Adds a check for banned accounts, meaning that banned users cannot authenticate (Permissions.AccountBanned is raised)
    val accessAccount : string -> AccountTypes.Account