namespace RentIt.Test
  open Xunit
  open FsUnit.Xunit
  open RentIt.CreditsPersistence
  open RentIt

  module TestCheckAccessToProduct =
    [<Fact>]
    let ``Test check access to product with buy result``() =
      let test = "TestCheckAccessToProductBuyAccess"
      try
        let testProd1 = Helper.createTestProduct test
        let testTrans1 = Helper.createTestBuyTransaction test

        let accessRes = checkAccessToProduct testProd1.owner testProd1.id 

        accessRes |> should equal true
      finally
        Helper.removeTestProduct test

    [<Fact>]
    let ``Test check access to product with rent result``() =
      let test = "TestCheckAccessToProductRentAccess"
      try
        let testProd1 = Helper.createTestProduct test
        let payDate = System.DateTime.Parse "2012-01-01 01:01:01"
        let expireDate = (System.DateTime.Now.AddDays 1.0)
        let testTrans = createRentTransaction (Helper.createTestRentTransactionType ("TESTUSER_"+test) payDate (Product.getProductByName ("TESTPRODUCT_"+test)).Head expireDate)

        let accessRes = checkAccessToProduct testProd1.owner testProd1.id 

        accessRes |> should equal true
      finally
        Helper.removeTestProduct test

    [<Fact>]
    let ``Test check access to product with rent no result``() =
      let test = "TestCheckAccessToProductRentNoAccess"
      try
        let testProd1 = Helper.createTestProduct test
        let testTrans1 = Helper.createTestRentTransaction test

        let accessRes = checkAccessToProduct testProd1.owner testProd1.id 

        accessRes |> should equal false
      finally
        Helper.removeTestProduct test