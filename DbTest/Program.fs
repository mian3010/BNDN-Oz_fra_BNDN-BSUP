// Learn more about F# at http://fsharp.net
// See the 'F# Tutorial' project for more help.

namespace RentIt

module Main =
    [<EntryPoint>]
    let main argv = 
        let acc :AccountTypes.Account = {
                        user = "Username"
                        email = "Email"
                        password = { 
                                    salt = "pass"
                                    hash = "123" }
                        created = System.DateTime.Now
                        banned = false
                        info = {
                                name = Some "Nicolai";
                                address = {
                                            address = Some "Address"
                                            postal = Some 2400
                                            country = Some "Denmark" }
                                birth = Some System.DateTime.Now      
                                about = Some "About_me"
                                credits = Some 666 }
                        accType = "Admin"
                        version = uint32 0 }

        Db.createUser acc
        0 // return an integer exit code
