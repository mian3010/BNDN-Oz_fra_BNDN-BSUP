namespace RentIt

module ControlledAccount =

    open PermissionExceptions
    open PermissionsUtil
    open AccountPermissions

    ///////////////////////////////////////////////////////////////////////

    module internal Internal =

        // Raises the correct access denied exception
        let check (invoker:Invoker) (access:Access) =
            match invoker with
                | Invoker.Auth auth when auth.banned -> raise AccountBanned
                | _                                  -> match access with
                                                        | Access.Denied reason  -> raise (PermissionDenied reason)
                                                        | Access.Accepted       -> ignore; // Return normally in this case

    ///////////////////////////////////////////////////////////////////////

    /// Wrapper for Account.persist, with the addition of {invoker}
    /// {invoker} is of type Permissions.Invoker
    /// Raises PermissionDenied if the {invoker} does not have the rights to perform this action
    /// Raised AccountBanned if the {invoker} is banned, and hence cannot perform actions
    let persist invoker acc =
        let allowed = AccountPermissions.mayCreateAccount invoker acc
        Internal.check invoker allowed |> ignore
        Account.persist acc
        
    /// Wrapper for Account.getByUsername, with the addition of {invoker}
    /// {invoker} is of type Permissions.Invoker
    /// Raises PermissionDenied if the {invoker} does not have the rights to perform this action
    /// Raised AccountBanned if the {invoker} is banned, and hence cannot perform actions
    let getByUsername invoker user =
        let allowed = AccountPermissions.mayRetrieveAccount invoker user
        Internal.check invoker allowed |> ignore
        Account.getByUsername user
    
    /// Wrapper for Account.getAllByType, with the addition of {invoker}
    /// {invoker} is of type Permissions.Invoker
    /// Raises PermissionDenied if the {invoker} does not have the rights to perform this action
    /// Raised AccountBanned if the {invoker} is banned, and hence cannot perform actions
    let getAllByType invoker accType =
        let allowed = AccountPermissions.mayRetrieveAccounts invoker accType
        Internal.check invoker allowed |> ignore
        Account.getAllByType accType

    /// Wrapper for Account.getLastAuthTime, with the addition of {invoker}
    /// {invoker} is of type Permissions.Invoker
    /// Raises PermissionDenied if the {invoker} does not have the rights to perform this action
    /// Raised AccountBanned if the {invoker} is banned, and hence cannot perform actions
    let getLastAuthTime invoker user =
        let allowed = AccountPermissions.mayReadAuthTime invoker user
        Internal.check invoker allowed |> ignore
        Account.getLastAuthTime user

    /// Wrapper for Account.delete, with the addition of {invoker}
    /// {invoker} is of type Permissions.Invoker
    /// Raises PermissionDenied if the {invoker} does not have the rights to perform this action
    /// Raised AccountBanned if the {invoker} is banned, and hence cannot perform actions
    let delete invoker acc =
        let allowed = AccountPermissions.mayDeleteAccount invoker acc
        Internal.check invoker allowed |> ignore
        Account.delete acc

    /// Wrapper for Account.update, with the addition of {invoker}
    /// {invoker} is of type Permissions.Invoker
    /// Raises PermissionDenied if the {invoker} does not have the rights to perform this action
    /// Raised AccountBanned if the {invoker} is banned, and hence cannot perform actions
    let update invoker (updatedAcc:AccountTypes.Account) =
        let targetAcc = Account.getByUsername updatedAcc.user

        // We do not want to raise PermissionDenied for cases where the invoker did not try to change a restricted field
        // but the account has been updated since its retrieved its copy.
        if targetAcc.version > updatedAcc.version then raise AccountExceptions.OutdatedData
        elif targetAcc.version < updatedAcc.version then raise AccountExceptions.BrokenInvariant

        let allowed = AccountPermissions.mayPerformAccountUpdate invoker targetAcc updatedAcc
        Internal.check invoker allowed |> ignore
        Account.update updatedAcc

    /// Wrapper for Account.resetPassword, with the addition of {invoker}
    /// {invoker} is of type Permissions.Invoker
    /// Raises PermissionDenied if the {invoker} does not have the rights to perform this action
    /// Raised AccountBanned if the {invoker} is banned, and hence cannot perform actions
    let resetPassword invoker user =
        let allowed = AccountPermissions.mayResetPassword invoker user
        Internal.check invoker allowed |> ignore
        Account.resetPassword user

    /// Wrapper for Account.getAcceptedCountries, with the addition of {invoker}
    /// {invoker} is of type Permissions.Invoker
    /// Raises PermissionDenied if the {invoker} does not have the rights to perform this action
    /// Raised AccountBanned if the {invoker} is banned, and hence cannot perform actions
    let getAcceptedCountries invoker = 
        let allowed = AccountPermissions.mayRetrieveCountryList invoker
        Internal.check invoker allowed |> ignore
        Account.getAcceptedCountries