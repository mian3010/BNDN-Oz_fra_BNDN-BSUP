namespace RentIt

module ControlledAccount =

    open AccountPermissions

    ///////////////////////////////////////////////////////////////////////

    module internal Internal =

        // Raises the correct access denied exception
        let accessDenied (invoker:Invoker) =
            match invoker with
                | Invoker.Auth auth when auth.banned -> raise AccountBanned
                | _ ->                                  raise PermissionDenied

    ///////////////////////////////////////////////////////////////////////

    /// Wrapper for Account.persist, with the addition of {invoker}
    /// {invoker} is of type Permissions.Invoker
    /// Raises PermissionDenied if the {invoker} does not have the rights to perform this action
    /// Raised AccountBanned if the {invoker} is banned, and hence cannot perform actions
    let persist invoker acc =
        let accessOk = AccountPermissions.mayCreateAccount invoker acc
        if not accessOk then Internal.accessDenied invoker
        Account.persist acc
        
    /// Wrapper for Account.getByUsername, with the addition of {invoker}
    /// {invoker} is of type Permissions.Invoker
    /// Raises PermissionDenied if the {invoker} does not have the rights to perform this action
    /// Raised AccountBanned if the {invoker} is banned, and hence cannot perform actions
    let getByUsername invoker user =
        let accessOk = AccountPermissions.mayRetrieveAccount invoker user
        if not accessOk then Internal.accessDenied invoker
        Account.getByUsername user
    
    /// Wrapper for Account.getAllByType, with the addition of {invoker}
    /// {invoker} is of type Permissions.Invoker
    /// Raises PermissionDenied if the {invoker} does not have the rights to perform this action
    /// Raised AccountBanned if the {invoker} is banned, and hence cannot perform actions
    let getAllByType invoker accType =
        let accessOk = AccountPermissions.mayRetrieveAccounts invoker accType
        if not accessOk then Internal.accessDenied invoker
        Account.getAllByType accType

    /// Wrapper for Account.getLastAuthTime, with the addition of {invoker}
    /// {invoker} is of type Permissions.Invoker
    /// Raises PermissionDenied if the {invoker} does not have the rights to perform this action
    /// Raised AccountBanned if the {invoker} is banned, and hence cannot perform actions
    let getLastAuthTime invoker user =
        let accessOk = AccountPermissions.mayReadAuthTime invoker user
        if not accessOk then Internal.accessDenied invoker
        Account.getLastAuthTime user

    /// Wrapper for Account.delete, with the addition of {invoker}
    /// {invoker} is of type Permissions.Invoker
    /// Raises PermissionDenied if the {invoker} does not have the rights to perform this action
    /// Raised AccountBanned if the {invoker} is banned, and hence cannot perform actions
    let delete invoker acc =
        let accessOk = AccountPermissions.mayDeleteAccount invoker acc
        if not accessOk then Internal.accessDenied invoker
        Account.delete acc

    /// Wrapper for Account.update, with the addition of {invoker}
    /// {invoker} is of type Permissions.Invoker
    /// Raises PermissionDenied if the {invoker} does not have the rights to perform this action
    /// Raised AccountBanned if the {invoker} is banned, and hence cannot perform actions
    let update invoker (updatedAcc:Account.Account) =
        let targetAcc = Account.getByUsername updatedAcc.user

        // We do not want to raise PermissionDenied for cases where the invoker did not try to change a restricted field
        // but the account has been updated since its retrieved its copy.
        if targetAcc.version > updatedAcc.version then raise Account.OutdatedData
        elif targetAcc.version < updatedAcc.version then raise Account.BrokenInvariant

        let accessOk = AccountPermissions.mayPerformAccountUpdate invoker targetAcc updatedAcc
        if not accessOk then Internal.accessDenied invoker
        Account.update updatedAcc

    /// Wrapper for Account.resetPassword, with the addition of {invoker}
    /// {invoker} is of type Permissions.Invoker
    /// Raises PermissionDenied if the {invoker} does not have the rights to perform this action
    /// Raised AccountBanned if the {invoker} is banned, and hence cannot perform actions
    let resetPassword invoker user =
        let accessOk = AccountPermissions.mayResetPassword invoker user
        if not accessOk then Internal.accessDenied invoker
        Account.resetPassword user