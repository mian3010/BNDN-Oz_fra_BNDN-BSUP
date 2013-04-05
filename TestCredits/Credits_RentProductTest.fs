namespace RentIt.Test
  open Xunit
  open FsUnit.Xunit
  open RentIt

  module TestRentProduct =
    [<Fact>]
    let ``Test rent product``() =
      let test = "TestRentProduct"
      try
        let testProd = Helper.createTestProduct test
        let testUser = Account.getByUsername testProd.owner
        let successCred = Credits.purchaseCredits testUser 1000
        let testUser = Account.getByUsername testUser.user
        let rent = Credits.rentProduct testUser testProd 2
        rent > System.DateTime.Now.AddDays 1.0 |> should equal true
      finally
        Helper.removeTestProduct test

    [<Fact>]
    let ``Test rent product with not enough credits``() =
      let test = "TestRentProductNotEnough"
      try
        let testProd = Helper.createTestProduct test
        let testUser = Account.getByUsername testProd.owner
        (fun() -> (Credits.rentProduct testUser testProd 2) |> ignore) |> should throw typeof<CreditsExceptions.NotEnoughCredits>
      finally
        Helper.removeTestProduct test

    [<Fact>]
    let ``Test rent product with wrong product``() =
      let test = "TestBuyProductWrongProduct"
      try
        let testProd = Helper.createTestProduct test
        let testProd2 = Helper.createTestProduct (test+"2")
        Helper.removeTestProduct (test+"2")
        let testUser = Account.getByUsername testProd.owner
        let successCred = Credits.purchaseCredits testUser 1000
        let testUser = Account.getByUsername testUser.user
        (fun() -> (Credits.rentProduct testUser testProd2 2) |> ignore) |> should throw typeof<ProductExceptions.NoSuchProduct>
      finally
        Helper.removeTestProduct test

    [<Fact>]
    let ``Test rent product with wrong user``() =
      let test = "TestBuyProductWrongUser"
      try
        let testProd = Helper.createTestProduct test
        let testUser = Account.getByUsername testProd.owner
        let testUser2 = Account.getByUsername (Helper.createTestUser (test+"2"))
        Helper.removeTestUser (test+"2")
        (fun() -> (Credits.rentProduct testUser2 testProd 2) |> ignore) |> should throw typeof<AccountExceptions.NoSuchUser>
      finally
        Helper.removeTestProduct test