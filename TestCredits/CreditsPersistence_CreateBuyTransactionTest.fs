namespace RentIt.Test
  open Xunit
  open FsUnit.Xunit
  open RentIt.CreditsPersistence
  open RentIt

  module TestCreateBuyTransaction =
    [<Fact>]
    let ``Test buy product``() =
      let test = "TestBuyProduct"
      try
        let payDate = System.DateTime.Parse "2012-01-01 01:01:01"
        let testProd = Helper.createTestProduct test
        let transaction = Helper.createTestBuyTransaction test
        
        //Test the values
        transaction.item.paid |> should equal 20
        transaction.item.product |> should equal testProd
        transaction.item.purchased |> should equal payDate
      finally
        Helper.removeTestProduct test

    [<Fact>]
    let ``Test buy product with user that does not exist``() =
      let test = "TestBuyProductUserNotExist"
      let testProd = Helper.createTestProduct test
      try
        let payDate = System.DateTime.Parse "2012-01-01 01:01:01"
        (fun () -> (createBuyTransaction (Helper.createTestBuyTransactionType "DoesNotExist" payDate testProd)) |> ignore) |> should throw typeof<AccountExceptions.NoSuchUser>
      finally
        Helper.removeTestProduct test

    [<Fact>]
    let ``Test buy product that does not exist``() =
      let test = "TestBuyProductNotExist"
      let testUser = Helper.createTestUser test
      let testProduct = Helper.createTestProduct (test+"2")
      Helper.removeTestProduct (test+"2")
      try
        let payDate = System.DateTime.Parse "2012-01-01 01:01:01"
        (fun () -> (createBuyTransaction (Helper.createTestBuyTransactionType testUser payDate testProduct)) |> ignore) |> should throw typeof<ProductExceptions.NoSuchProduct>
      finally
        Helper.removeTestUser test