namespace RentIt.Test
  open Xunit
  open FsUnit.Xunit
  open RentIt
  open RentIt.CreditsTypes

  module TestCheckAccessToProductCr =
    [<Fact>]
    let ``Test check access to product with buy result``() =
      let test = "TestCheckAccessToProductBuyAccessCr"
      try
        let testProd1 = Helper.createTestProduct test
        let testUser = Account.getByUsername testProd1.owner
        let testTrans1 = Helper.createTestBuyTransaction test

        let accessRes = Credits.checkAccessToProduct testUser testProd1

        accessRes |> should equal true
      finally
        Helper.removeTestProduct test

    [<Fact>]
    let ``Test check access to product with rent result``() =
      let test = "TestCheckAccessToProductRentAccessCr"
      try
        let testProd1 = Helper.createTestProduct test
        let testUser = Account.getByUsername testProd1.owner
        let payDate = System.DateTime.Now
        let testTrans = Credits.rentProduct testUser testProd1 1

        let accessRes = Credits.checkAccessToProduct testUser testProd1

        accessRes |> should equal true
      finally
        Helper.removeTestProduct test

    [<Fact>]
    let ``Test check access to product with rent no result``() =
      let test = "TestCheckAccessToProductRentNoAccessCr"
      try
        let testProd1 = Helper.createTestProduct test
        let testUser = Account.getByUsername testProd1.owner
        let testTrans1 = Helper.createTestRentTransaction test

        let accessRes = Credits.checkAccessToProduct testUser testProd1 

        accessRes |> should equal false
      finally
        Helper.removeTestProduct test