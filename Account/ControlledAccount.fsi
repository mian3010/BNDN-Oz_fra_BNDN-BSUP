namespace RentIt

module ControlledAccount =

    open AccountTypes
    open PermissionsUtil

    /// Persists a new Account, making the account visible to the outside world
    /// Raises UserAlreadyExists if the username already is occupied
    /// Raises UnknownAccType if the specified account type does not exist
    /// Raises TooLargeData if one or more of the account fields were too large to be persisted
    val persist : Invoker -> Account -> Account
        
    /// Retrieves an account from persistence based on its associated username
    /// Raises NoSuchUser if no account is associated with the given username
    val getByUsername : Invoker -> string -> Account
    
    /// Retrieves all accounts of a specific type
    val getAllByType : Invoker -> string -> Account list

    /// Retrieves the date and time which the user {user} last authenticated
    /// 'None' means that the user never has authenticated
    /// Raises NoSuchUser if no account is associated with the given username
    val getLastAuthTime : Invoker -> string -> System.DateTime option

    /// Deletes an previously created account. The account will be removed from persistence.
    val delete : Invoker -> Account -> unit

    /// Updates the persisted account record to the passed {acc} account record
    /// The account which is updated is the one with the identical username.
    /// The function returns an Account record which has the data of the given record, but with an updated version binding.
    /// The return value should be used as base for future updates to avoid OutdatedData exceptions
    /// Raises NoSuchUser if no account is associated with the given username
    /// Raises OutdatedData the account has been updated/changed since it was read (which could mean that the update is based on old data)
    /// Raises TooLargeData if one or more of the new account fields were too large to be persisted
    val update : Invoker -> Account -> Account

