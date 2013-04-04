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
        transaction.item.paid |> should equal testProd.buyPrice
        transaction.item.product |> should equal testProd
        transaction.item.purchased |> should equal payDate
        transaction.item.id > 0 |> should equal true
      finally
        Helper.removeTestProduct test

    [<Fact>]
    let ``Test buy product with user that does not exist``() =
      let test = "TestBuyProductUserNotExist"
      let testProd = Helper.createTestProduct test
      try
        let payDate = System.DateTime.Parse "2012-01-01 01:01:01"
        (fun () -> (createBuyTransaction "DoesNotExist" payDate testProd.id) |> ignore) |> should throw typeof<AccountExceptions.NoSuchUser>
      finally
        Helper.removeTestProduct test

    [<Fact>]
    let ``Test buy product that does not exist``() =
      let test = "TestBuyProductNotExist"
      let testUser = Helper.createTestUser test
      try
        let payDate = System.DateTime.Parse "2012-01-01 01:01:01"
        (fun () -> (createBuyTransaction testUser payDate 45345) |> ignore) |> should throw typeof<ProductExceptions.NoSuchProduct>
      finally
        Helper.removeTestUser test