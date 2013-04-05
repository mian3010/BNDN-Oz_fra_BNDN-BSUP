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