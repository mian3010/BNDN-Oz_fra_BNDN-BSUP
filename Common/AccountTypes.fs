namespace RentIt

/// All types used by account
module AccountTypes =
    
    type Password = {salt : string; hash : string;}

    type Address =              {
                                    address : string option;
                                    postal : int option;
                                    country : string option;
                                }

    type ExtraAccInfo =         {
                                    name : string option;
                                    address : Address;
                                    birth : System.DateTime option;    // Date of Birth
                                    about : string option;             // About Me
                                    credits : int option;
                                }

    type Account =              {
                                    user : string; // Usernames are case insensitive
                                    email : string;
                                    password : Password; // password is hashed
                                    created : System.DateTime;
                                    banned : bool;
                                    info : ExtraAccInfo;
                                    accType : string;
                                    version : uint32; // to be used by persistence API. 0 if not persisted yet
                                }