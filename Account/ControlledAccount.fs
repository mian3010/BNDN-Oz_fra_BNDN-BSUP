namespace RentIt

module ControlledAccount =

    ///////////////////////////////////////////////////////////////////////

    module internal Internal =

        // Raises the correct access denied exception
        let accessDenied (invoker:Permissions.Invoker) =
            match invoker with
                | Permissions.Invoker.Auth auth when auth.banned -> raise Permissions.AccountBanned
                | _ ->                                              raise Permissions.PermissionDenied

    ///////////////////////////////////////////////////////////////////////

    /// Wrapper for Account.persist, with the addition of {invoker}
    /// {invoker} is of type Permissions.Invoker
    /// Raises PermissionDenied if the {invoker} does not have the rights to perform this action
    /// Raised AccountBanned if the {invoker} is banned, and hence cannot perform actions
    let persist invoker acc =
        let accessOk = Permissions.Account.mayCreateAccount invoker acc
        if not accessOk then Internal.accessDenied invoker
        Account.persist acc
        
    /// Wrapper for Account.getByUsername, with the addition of {invoker}
    /// {invoker} is of type Permissions.Invoker
    /// Raises PermissionDenied if the {invoker} does not have the rights to perform this action
    /// Raised AccountBanned if the {invoker} is banned, and hence cannot perform actions
    let getByUsername invoker user =
        let accessOk = Permissions.Account.mayRetrieveAccount invoker user
        if not accessOk then Internal.accessDenied invoker
        Account.getByUsername user
    
    /// Wrapper for Account.getAllByType, with the addition of {invoker}
    /// {invoker} is of type Permissions.Invoker
    /// Raises PermissionDenied if the {invoker} does not have the rights to perform this action
    /// Raised AccountBanned if the {invoker} is banned, and hence cannot perform actions
    let getAllByType invoker accType =
        let accessOk = Permissions.Account.mayRetrieveAccounts invoker accType
        if not accessOk then Internal.accessDenied invoker
        Account.getAllByType accType

    /// Wrapper for Account.getLastAuthTime, with the addition of {invoker}
    /// {invoker} is of type Permissions.Invoker
    /// Raises PermissionDenied if the {invoker} does not have the rights to perform this action
    /// Raised AccountBanned if the {invoker} is banned, and hence cannot perform actions
    let getLastAuthTime invoker user =
        let accessOk = Permissions.Account.mayReadAuthTime invoker user
        if not accessOk then Internal.accessDenied invoker
        Account.getLastAuthTime user

    /// Wrapper for Account.delete, with the addition of {invoker}
    /// {invoker} is of type Permissions.Invoker
    /// Raises PermissionDenied if the {invoker} does not have the rights to perform this action
    /// Raised AccountBanned if the {invoker} is banned, and hence cannot perform actions
    let delete invoker acc =
        let accessOk = Permissions.Account.mayDeleteAccount invoker acc
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

        let accessOk = Permissions.Account.mayPerformAccountUpdate invoker updatedAcc targetAcc
        if not accessOk then Internal.accessDenied invoker
        Account.update updatedAcc