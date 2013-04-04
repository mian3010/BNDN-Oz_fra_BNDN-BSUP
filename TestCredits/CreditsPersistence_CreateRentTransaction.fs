namespace RentIt.Test
  open Xunit
  open FsUnit.Xunit
  open RentIt.CreditsPersistence
  open RentIt

  module TestCreateRentTransaction =
    [<Fact>]
    let ``Test rent product``() =
      let test = "TestRentProduct"
      try
        let payDate = System.DateTime.Parse "2012-01-01 01:01:01"
        let expireDate = System.DateTime.Parse "2012-01-02 01:01:01"
        let testProd = Helper.createTestProduct test
        let transaction = Helper.createTestRentTransaction test

        //Test the values
        transaction.item.paid |> should equal testProd.buyPrice
        transaction.item.product |> should equal testProd
        transaction.item.purchased |> should equal payDate
        transaction.item.id > 0 |> should equal true
        transaction.expires |> should equal expireDate
      finally
        Helper.removeTestProduct test

    [<Fact>]
    let ``Test rent product with user that does not exist``() =
      let test = "TestRentProductUserNotExist"
      let testProd = Helper.createTestProduct test
      try
        let payDate = System.DateTime.Parse "2012-01-01 01:01:01"
        let expireDate = System.DateTime.Parse "2012-01-02 01:01:01"
        (fun () -> (createRentTransaction "DoesNotExist" payDate testProd.id expireDate) |> ignore) |> should throw typeof<AccountExceptions.NoSuchUser>
      finally
        Helper.removeTestProduct test

    [<Fact>]
    let ``Test rent product that does not exist``() =
      let test = "TestRentProductNotExist"
      let testUser = Helper.createTestUser test
      try
        let payDate = System.DateTime.Parse "2012-01-01 01:01:01"
        let expireDate = System.DateTime.Parse "2012-01-02 01:01:01"
        (fun () -> (createRentTransaction testUser payDate 45345 expireDate) |> ignore) |> should throw typeof<ProductExceptions.NoSuchProduct>
      finally
        Helper.removeTestUser test