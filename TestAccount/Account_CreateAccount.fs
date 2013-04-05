namespace RentIt.Test
  open Xunit
  open FsUnit.Xunit
  open RentIt.AccountPersistence
  open RentIt

  module TestCreateAccount =

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

    [<Fact>]
    let ``Test Create Admin Account``() =
      let test = "TestCreateAdminAccount"
      try
        let accType = "Admin"
        let acc = Account.make accType user email pass extra
        
        //Test the values
        acc.accType     |> should equal accType
        acc.banned      |> should equal false
        acc.email       |> should equal email
        acc.info        |> should equal extra
        acc.password    |> should equal pass
        acc.user        |> should equal user
        acc.version     |> should equal 1
      finally
        Helper.removeTestProduct test

    [<Fact>]
    let ``Test Create Provider Account``() =
      let test = "TestCreateProviderAccount"
      try
        let accType = "Content Provider"
        let acc = Account.make accType user email pass extra
        
        //Test the values
        acc.accType     |> should equal accType
        acc.banned      |> should equal false
        acc.email       |> should equal email
        acc.info        |> should equal extra
        acc.password    |> should equal pass
        acc.user        |> should equal user
        acc.version     |> should equal 1
      finally
        Helper.removeTestProduct test

    [<Fact>]
    let ``Test Create Customer Account``() =
      let test = "TestCreateCustomerAccount"
      try
        let accType = "Customer"
        let acc = Account.make accType user email pass extra
        
        //Test the values
        acc.accType     |> should equal accType
        acc.banned      |> should equal false
        acc.email       |> should equal email
        acc.info        |> should equal extra
        acc.password    |> should equal pass
        acc.user        |> should equal user
        acc.version     |> should equal 1
      finally
        Helper.removeTestProduct test