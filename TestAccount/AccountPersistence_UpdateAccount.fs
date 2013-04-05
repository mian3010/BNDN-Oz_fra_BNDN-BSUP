namespace RentIt.Test
  open Xunit
  open FsUnit.Xunit
  open RentIt.AccountPersistence
  open RentIt

  module TestUpdateAccount =
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
    let ``Test Update Account``() =
      let test = "TestUpdateAccount"
      try    
        let acc = ref <| Account.make accType user email pass extra
        Account.persist acc.Value

        acc := Account.make accType user "New@Mail.lol" pass extra
        
        let acc' = Account.update acc.Value
        let acc'' = Account.getByUsername user

        //Test the values
        acc'            |> should equal acc''
        acc'.accType    |> should equal accType
        acc'.banned     |> should equal false
        acc'.email      |> should equal "New@Mail.lol"
        acc'.info       |> should equal extra
        acc'.password   |> should equal pass
        acc'.user       |> should equal user
        acc'.version    |> should equal 2
      finally
        Helper.removeTestProduct test