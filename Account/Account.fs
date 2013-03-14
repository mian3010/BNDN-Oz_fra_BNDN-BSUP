namespace RentIt

module Account =
    
    ///////////////////////////////////////////////////////////////////////

    module Password =

        ///////////////////////////////////////////////////////////////////////

        /// Implementation inspired by:
        /// http://geekswithblogs.net/Nettuce/archive/2012/06/14/salt-and-hash-a-password-in.net.aspx
        module internal Internal =
        
            let random = new System.Random()
            let hasher = System.Security.Cryptography.SHA512.Create()

            /// Converts a string to base64
            let toBase64 (str:string) =
                let bytes = System.Text.Encoding.UTF8.GetBytes(str)
                System.Convert.ToBase64String(bytes)

            /// Creates a cryptographic hash of a string
            let hash (str:string) =
                let bytes = System.Text.Encoding.UTF8.GetBytes(str)
                System.Convert.ToBase64String(hasher.ComputeHash(bytes))

            /// Generates a pseudo random salt
            let produceSalt =
                let bytes = Array.zeroCreate 32
                random.NextBytes(bytes) // Fill with random bytes
                System.Convert.ToBase64String(bytes) // return salt
    
        ///////////////////////////////////////////////////////////////////////

        type Password = AccountTypes.PasswordTypes.Password

        /// Produces a salted hash of the given unhashed password
        let create password :Password =
            let salt = Internal.produceSalt
            let password = Internal.toBase64 password
            {salt = salt; hash = Internal.hash (password+salt)}

        /// Produces a random password of {length} ASCII characters
        let produce length :Password =
            let randomChar =
                let codepoint = Internal.random.Next(33, 127) // From "!" to "~" in the ASCII range
                char(codepoint)
            let rec helper = function
                | (0, str) -> str
                | (x, str) -> helper (x-1, string(randomChar))
            create (helper (length, ""))

        /// Tests whether a hashed password matches a plain password
        let verify (hashed:Password) password =
            let password = Internal.toBase64 password
            let hash = Internal.hash (password+hashed.salt)
            hashed.hash = hash
    
    ///////////////////////////////////////////////////////////////////////
    
    ////// TYPES

    type Address = AccountTypes.Address

    // An 'enum' of the different account types
    type AccountType =          AccountTypes.AccountType

    // Custom data for each type of account

    type Customer =             AccountTypes.Customer
    type ContentProvider =      AccountTypes.ContentProvider
    type Admin =                AccountTypes.Admin

    // Like a 'supertype' of each kind of additional account info
    type TypeInfo =             AccountTypes.TypeInfo

    // Common data for each account type

    type Account =              AccountTypes.Account

    // Exceptions

    exception UserAlreadyExists // If one tries to create/persist an account whose username already is taken
    exception NoSuchUser        // If one tries to retrieve/update an account, but no account is found for the passed identifier
    exception OutdatedData      // If a call to 'update' cannot succeed, because the changes conflict with more recent changes
    exception BrokenInvariant   // If a function is invoked on an account whose invariants have been broken


    ////// CONSTRUCTOR FUNCTIONS

    /// Constructs a new Account record with the given information.
    let make (typeinfo:TypeInfo) (user:string) (email:string) (password:string) :Account =
        {
            user = user;
            email = email;
            password = Password.create password;
            created = System.DateTime.Now;
            banned = false;
            info = typeinfo;
            version = uint32(0);    
        }

    ////// CREDO FUNCTIONS

    /// Persists a new Account, making the account visible to the outside world
    /// Raises UserAlreadyExists if the username already is occupied
    let persist (acc:Account) =
        try
            DB.createUser acc
        with
            | DB.UsernameAlreadyInUse -> raise UserAlreadyExists
        
    /// Retrieves an account from persistence based on its associated username
    /// Raises NoSuchUser if no account is associated with the given username
    let getByUsername (user:string) :Account =
        try
            DB.getUserByName user
        with
            | DB.NoUserWithSuchName -> raise NoSuchUser
    
    /// Retrieves all accounts of a specific type
    let getAllByType (accType:AccountType) :Account list = DB.getAllUsersByType accType

    /// Retrieves the date and time which the user {user} last authenticated
    /// 'None' means that the user never has authenticated
    /// Raises NoSuchUser if no account is associated with the given username
    let getLastAuthTime (user:string) :System.DateTime option =
        try
            DB.getUsersLastAuthTime user
        with
            | DB.NoUserWithSuchName -> raise NoSuchUser

    /// Deletes an previously created account. The account will be removed from persistence.
    let delete (acc:Account) =
        raise (new System.NotImplementedException())

    /// Updates the persisted account record to the passed {acc} account record
    /// The account which is updated is the one with the identical username.
    /// The function returns an Account record which has the data of the given record, but with an updated version binding.
    /// The return value should be used as base for future updates to avoid OutdatedData exceptions
    /// Raises NoSuchUser if no account is associated with the given username
    /// Raises OutdatedData the account has been updated/changed since it was read (which could mean that the update is based on old data)
    let update (acc:Account) :Account =
        try
            DB.update acc
        with
            | DB.NoUserWithSuchName -> raise NoSuchUser
            | DB.NewerVersionExist -> raise OutdatedData
            | DB.IllegalAccountVersion -> raise BrokenInvariant
    ////// HELPER FUNCTIONS

    /// True if the unhashed password {password} matches the password hash of the account {acc}
    let hasPassword (acc:Account) (password:string) = Password.verify acc.password password

    /// Returns a list where every banned account of {accs} has been removed
    let rec filterBanned (accs:Account list) =
        match accs with
            | []                            -> accs
            | acc :: accs when acc.banned   -> filterBanned accs
            | acc :: accs                   -> [acc] @ (filterBanned accs)
