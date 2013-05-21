namespace RentIt.Test

module ControlledProductTest =
    open Xunit
    open FsUnit.Xunit
    open RentIt.GeneralExceptions
    open RentIt.ControlledProduct
    open RentIt

    let admin = "Admin"
    let contentProvider = "ContenProvider"
    
    let acc:AccountTypes.Account = {
        user = "TESTUSER"
        email = "test@example.com";
        password = Account.Password.create "Testpass"
        created = (System.DateTime.Parse "2012-10-10 02:04:00");
        banned = false;
        info = ({
                name = Some "Testuser";
                address = ({
                            address = Some "Testaddress";
                            postal = Some 1123;
                            country = Some "Denmark"
                }:AccountTypes.Address);
                birth = Some (System.DateTime.Parse "2012-10-10 02:04:00");
                about = Some "Testing";
                credits = Some 0;
        }:AccountTypes.ExtraAccInfo);
        accType = "Customer";
        version = (uint32 0);
      }



    let token = ControlledAuth.authenticate acc.user acc.accType 
    
    (*let invoker = PermissionsUtil.Invoker.newAuth token.token

    [<Fact>]
    let ``ContentProvider should work´´() =
        let p = ControlledProduct.make invoker 
            *)

