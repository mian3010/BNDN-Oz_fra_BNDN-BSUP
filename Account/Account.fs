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
        
        type Password = {salt : string; hash : string;}

        /// Produces a random password of {length} characters
        let produce length :Password =
            raise (new System.NotImplementedException())

        /// Produces a salted hash of the given unhashed password
        let create password =
            let salt = Internal.produceSalt
            let password = Internal.toBase64 password
            {salt = salt; hash = Internal.hash (password+salt)}

        /// Tests whether a hashed password matches a plain password
        let verify hashed password =
            let password = Internal.toBase64 password
            let hash = Internal.hash (password+hashed.salt)
            hashed.hash = hash
    
    ///////////////////////////////////////////////////////////////////////
    
    ////// TYPES

    type Address =              {
                                    address : string;
                                    postal : int;
                                    country : string;
                                }

    // An 'enum' of the different account types
    type AccountType =            Customer
                                | ContentProvider
                                | Admin

    // Custom data for each type of account

    type Customer =             {
                                    name : string;              // null if not set
                                    address : Address;          // null if not set
                                    birth : System.DateTime;    // Date of Birth, null if not set
                                    about : string;             // About Me
                                    credits : int;
                                }
    type ContentProvider =      {
                                    name : string;              // null if not set
                                    address : Address;          // null if not set
                                }
    type Admin =                unit

    // Like a 'supertype' of each kind of additional account info
    type TypeInfo =               Customer of Customer
                                | ContentProvider of ContentProvider
                                | Admin of Admin

    // Common data for each account type

    type Account =              {
                                    user : string; // Usernames are case insensitive
                                    email : string;
                                    password : Password.Password; // password is hashed
                                    created : System.DateTime;
                                    banned : bool;
                                    info : TypeInfo;
                                    version : System.UInt32; // to be used by persistence API
                                }
    
    // Exceptions

    exception UserAlreadyExists // If one tries to create an account whose username already is taken
    exception EmailAddressInUse // If one associate an account with an email address which is used by another account
    exception NoSuchUser        // If one tries to retrieve/update an account, but no account is found for the passed identifier
    exception OutdatedData      // If a call to 'update' cannot succeed, because the changes conflict with more recent changes


    ////// CREDO FUNCTIONS

    /// Creates a new account with the given information. Upon successful invokation, the account will also be persisted.
    let create typeinfo user email password :Account =
        raise (new System.NotImplementedException())
        
    /// Retrieves an account from persistence based on its associated username
    let getByUsername user :Account =
        raise (new System.NotImplementedException())
    
    /// Retrieves all accounts of a specific type
    let getAllByType (accType:AccountType) :Account list =
        raise (new System.NotImplementedException())

    /// Retrieves the date and time which the user {user} last authenticated
    /// null means "never"
    let getLastAuthTime user :System.DateTime =
        raise (new System.NotImplementedException())

    /// Deletes an previously created account. The account will be removed from persistence.
    let delete acc =
        raise (new System.NotImplementedException())

    /// Updates the persisted account record to the passed {acc} account record
    /// The account which is updated is the one with the identical username
    let update acc =
        raise (new System.NotImplementedException())

    ////// HELPER FUNCTIONS

    /// True if the unhashed password {password} matches the password hash of the account {acc}
    let hasPassword acc password = Password.verify acc.password password