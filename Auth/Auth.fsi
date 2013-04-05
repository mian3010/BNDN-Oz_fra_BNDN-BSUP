namespace RentIt

module Auth =

    /// Token manipulator functions
    module Token =

        type AuthToken;

        /// Creates a new token for {user} which expires in {hours} hours
        val create : int -> string -> AuthToken

        /// True if the token no longer is legal, otherwise false
        val isExpired : AuthToken -> bool
    
        /// Encodes a {token} record into a string for transport
        val encode : AuthToken -> string

        /// Decodes a {token} string into a token record
        /// Raises an IllegalToken exception if its checksum does not match the content or the token is illegally structured
        val decode : string -> AuthToken

    ///////////////////////////////////////////////////////////////////////

    /// Retrieves an encoded token for the user {user}, together with an expiration date/time.
    /// Raises AuthExceptions.AuthenticationFailed if the credentials are incorrect
    /// The result is a tuple: (encodedToken, tokenExpiration) aka (string * System.DateTime)
    val authenticate : string -> string -> string * System.DateTime

    /// Retrieves the account for which {token} is a reference to.
    /// Raises AuthExceptions.IllegalToken if the token is malformed
    /// Raises AuthExceptions.TokenExpired if the token has expired
    val accessAccount : string -> AccountTypes.Account
