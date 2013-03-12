namespace RentIt

module ControlledAuth =
    
    // Making it accessible, just as if the client had used the Auth module directly
    module Token = Auth.Token

    /// Wrapper for Auth.authenticate
    /// Adds a check for banned accounts, meaning that banned users cannot authenticate (Permissions.AccountBanned is raised)
    let authenticate user pass =
        let token = Auth.authenticate user pass
        let user = Account.getByUsername user
        if user.banned then raise Permissions.AccountBanned
        token

    /// Wrapper for Auth.accessAccount
    /// Adds a check for banned accounts, meaning that banned users cannot authenticate (Permissions.AccountBanned is raised)
    let accessAccount token =
        let user = Auth.accessAccount token
        if user.banned then raise Permissions.AccountBanned
        user