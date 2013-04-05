namespace RentIt.Test
  open Xunit
  open FsUnit.Xunit
  open RentIt.AccountPersistence
  open RentIt

  module TestPersistAccount =
    let address:AccountTypes.Address = {address = Some "street"
                                        postal = Some 2400
                                        country = Some "Denmark" }
    let extra:AccountTypes.ExtraAccInfo = { name = Some "name"
                                            address = address
                                            birth = Some System.DateTime.Today
                                            about = Some "lol, no u stalker"
                                            credits = Some 0 }
    let user = "Kruger"
    let email = "mail@mail.mail"
    let pass = "pass123"
    let accType = "Admin"

    [<Fact>]
    let ``Test Persist Account``() =
      let test = "TestPersistAccount"
      try    
        let acc = Account.make accType user email pass extra
        Account.persist acc

        let acc' = Account.getByUsername user

        //Test the values
        acc'    |> should equal acc
      finally
        Helper.removeTestProduct test