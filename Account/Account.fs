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

    type AccountInfo = unit // Dummy until the actual info to store is decided upon
    type Account = {user : string; email : string; password : Password.Password; info : AccountInfo} // password is hashed
    
    exception UserAlreadyExists // If one tries to create an account whose username already is taken
    exception EmailAddressInUse // If one associate an account with an email address which is used by another account
    exception NoSuchUser        // If one tries to retrieve/update an account, but no account is found for the passed identifier

    /// Creates a new account with the given information. Upon successful invokation, the account will also be persisted
    let create user email password :Account =
        raise (new System.NotImplementedException())
        
    /// Retrieves an account from persistence based on its associated username
    let getByUsername user :Account =
        raise (new System.NotImplementedException())
    
    /// Retrieves an account from persistence based on its associated email address
    let getByEmail email :Account =
        raise (new System.NotImplementedException())

    /// Deletes an previously created account. The account will be removed from persistence.
    let delete acc =
        raise (new System.NotImplementedException())

    /// Updates the persisted account record to the passed {acc} account record
    /// The account which is updated is the one with the identical username
    let update acc =
        raise (new System.NotImplementedException())

    /// True if the unhashed password {password} matches the password hash of the account {acc}
    let hasPassword acc password = Password.verify acc.password password