namespace RentIt.Test
  open Xunit
  open FsUnit.Xunit
  open RentIt

  module TestBuyProduct =
    [<Fact>]
    let ``Test buy product``() =
      let test = "TestBuyProduct"
      try
        let testProd = Helper.createTestProduct test
        let testUser = Account.getByUsername testProd.owner
        let successCred = Credits.purchaseCredits testUser 1000
        let buy = Credits.buyProduct testUser testProd
        buy |> should equal true
      finally
        Helper.removeTestProduct test

    [<Fact>]
    let ``Test buy product with not enough credits``() =
      let test = "TestBuyProductNotEnough"
      try
        let testProd = Helper.createTestProduct test
        let testUser = Account.getByUsername testProd.owner
        (fun() -> (Credits.buyProduct testUser testProd) |> ignore) |> should throw typeof<CreditsExceptions.NotEnoughCredits>
      finally
        Helper.removeTestProduct test

    [<Fact>]
    let ``Test buy product with wrong product``() =
      let test = "TestBuyProductWrongProduct"
      try
        let testProd = Helper.createTestProduct test
        let testProd2 = Helper.createTestProduct (test+"2")
        Helper.removeTestProduct (test+"2")
        let testUser = Account.getByUsername testProd.owner
        let successCred = Credits.purchaseCredits testUser 1000
        let testUser = Account.getByUsername testUser.user
        (fun() -> (Credits.buyProduct testUser testProd2) |> ignore) |> should throw typeof<ProductExceptions.NoSuchProduct>
      finally
        Helper.removeTestProduct test

    [<Fact>]
    let ``Test buy product with wrong user``() =
      let test = "TestBuyProductWrongUser"
      try
        let testProd = Helper.createTestProduct test
        let testUser = Account.getByUsername testProd.owner
        let testUser2 = Account.getByUsername (Helper.createTestUser (test+"2"))
        Helper.removeTestUser (test+"2")
        (fun() -> (Credits.buyProduct testUser2 testProd) |> ignore) |> should throw typeof<AccountExceptions.NoSuchUser>
      finally
        Helper.removeTestProduct test