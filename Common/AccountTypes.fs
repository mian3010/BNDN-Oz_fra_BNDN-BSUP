namespace RentIt

/// All types used by account
module AccountTypes =
    
    ///////////////////////////////////////////////////////////////////////

    module PasswordTypes = 

        type Password = {salt : string; hash : string;}

    ///////////////////////////////////////////////////////////////////////

    type Address =              {
                                    address : string option;
                                    postal : int option;
                                    country : string option;
                                }

    // An 'enum' of the different account types
    type AccountType =            Customer
                                | ContentProvider
                                | Admin

    // Custom data for each type of account

    type Customer =             {
                                    name : string option;
                                    address : Address;
                                    birth : System.DateTime option;    // Date of Birth
                                    about : string option;             // About Me
                                    credits : int;
                                }
    type ContentProvider =      {
                                    name : string option;
                                    address : Address;
                                }
    type Admin =                unit

    // Like a 'supertype' of each kind of additional account info
    type TypeInfo =               Customer of Customer
                                | ContentProvider of ContentProvider
                                | Admin of Admin

    // Common data for each account type

    type Account =              {
                                    user : string; // Usernames are case insensitive - record invariant: lower case
                                    email : string;
                                    password : PasswordTypes.Password; // password is hashed
                                    created : System.DateTime;
                                    banned : bool;
                                    info : TypeInfo;
                                    version : System.UInt32; // to be used by persistence API. 0 if not persisted yet
                                }