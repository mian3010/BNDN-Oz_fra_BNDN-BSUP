namespace RentIt

module Account =

    open AccountTypes

    module Password =

        type Password

        /// Produces a salted hash of the given unhashed password
        val create : string -> Password

        /// Produces a random password of {length} ASCII characters
        val produce : int -> Password

        /// Tests whether a hashed password matches a plain password
        val verify : Password -> string -> bool

    ////// CONSTRUCTOR FUNCTIONS

    /// Constructs a new Account record with the given information.
    /// Raises BrokenInvariant if any of the passed strings are null
    val make : string -> string -> string -> string -> ExtraAccInfo -> Account

    ////// CREDO FUNCTIONS

    /// Persists a new Account, making the account visible to the outside world
    /// Raises UserAlreadyExists if the username already is occupied
    /// Raises UnknownAccType if the specified account type does not exist
    /// Raises TooLargeData if one or more of the account fields were too large to be persisted
    val persist : Account -> Account
        
    /// Retrieves an account from persistence based on its associated username
    /// Raises NoSuchUser if no account is associated with the given username
    val getByUsername : string -> Account
    
    /// Retrieves all accounts of a specific type
    val getAllByType : string -> Account list

    /// Retrieves the date and time which the user {user} last authenticated
    /// 'None' means that the user never has authenticated
    /// Raises NoSuchUser if no account is associated with the given username
    val getLastAuthTime : string -> System.DateTime option

    /// Deletes an previously created account. The account will be removed from persistence.
    val delete : Account -> unit

    /// Updates the persisted account record to the passed {acc} account record
    /// The account which is updated is the one with the identical username.
    /// The function returns an Account record which has the data of the given record, but with an updated version binding.
    /// The return value should be used as base for future updates to avoid OutdatedData exceptions
    /// Raises NoSuchUser if no account is associated with the given username
    /// Raises OutdatedData the account has been updated/changed since it was read (which could mean that the update is based on old data)
    /// Raises TooLargeData if one or more of the new account fields were too large to be persisted
    val update : Account -> Account

    ////// HELPER FUNCTIONS

    /// Resets the password of the account with the specified username {user}
    /// The new, randomly generated password will be emailed to the account associated with the username
    /// Raises NoSuchUser if no account is associated with the given username
    val resetPassword : string -> unit

    /// True if the unhashed password {password} matches the password hash of the account {acc}
    val hasPassword : Account -> string -> bool

    /// Returns a list where every banned account of {accs} has been removed
    val filterBanned : Account list -> Account list

    /// Returns a list of every accepted country name of the system
    val getAcceptedCountries : unit -> string list

