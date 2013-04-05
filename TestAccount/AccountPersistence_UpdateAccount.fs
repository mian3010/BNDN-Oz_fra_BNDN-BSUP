namespace RentIt.Test
  open Xunit
  open FsUnit.Xunit
  open RentIt.AccountPersistence
  open RentIt

  module TestUpdateAccount =
    
    let user = "Kruger2"
    let email = "mail@mail.mail"
    let pass = "pass123"
    let accType = "Admin"

    [<Fact>]
    let ``Test Update Account``() =
      let test = "TestUpdateAccount"
      try
        let address:AccountTypes.Address = {address = Some "street"
                                            postal = Some 2400
                                            country = Some "Denmark" }
        let extra:AccountTypes.ExtraAccInfo = { name = Some "name"
                                                address = address
                                                birth = Some System.DateTime.Today
                                                about = Some "lol, no u stalker"
                                                credits = Some 0 }

        let acc = Account.make accType user email pass extra
        Account.persist acc

        let acc = Account.make accType user "New@Mail.lol" pass extra
        
        let acc' = Account.update acc
        let acc'' = Account.getByUsername user

        //Test the values
        acc'            |> should equal acc''
        acc'.accType    |> should equal accType
        acc'.banned     |> should equal false
        acc'.email      |> should equal "New@Mail.lol"
        acc'.info       |> should equal extra
        acc'.password   |> should equal (Account.Password.create pass)
        acc'.user       |> should equal user
        acc'.version    |> should equal (uint32 1)
      finally
        Helper.removeTestProduct test