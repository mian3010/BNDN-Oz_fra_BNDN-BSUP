namespace RentIt.Test
  open Xunit
  open FsUnit.Xunit
  open RentIt.CreditsPersistence
  open RentIt

  module TestGetTransactionAccountAccess =
    [<Fact>]
    let ``Test get transaction by account access with result``() =
      let test = "TestGetTransByTypeAccAccs"
      try
        let testProd1 = Helper.createTestProduct test
        let testProd2 = Helper.createTestProduct (test+"2")
        let testTrans1 = Helper.createTestBuyTransaction test
        let testTrans2 = Helper.createTestRentTransaction test
        let testTrans3 = Helper.createTestBuyTransaction (test+"2")

        let accessRes = getTransactionByAccountAccess testProd1.owner

        accessRes.Length |> should equal 1
      finally
        Helper.removeTestProduct test
        Helper.removeTestProduct (test+"2")

    [<Fact>]
    let ``Test get transaction by account access with no result``() =
      let test = "TestGetTransByTypeAccAccs"
      try
        let testProd1 = Helper.createTestProduct test
        let testProd2 = Helper.createTestProduct (test+"2")
        let testTrans2 = Helper.createTestRentTransaction test
        let testTrans3 = Helper.createTestBuyTransaction (test+"2")

        let accessRes = getTransactionByAccountAccess testProd1.owner

        accessRes.Length |> should equal 0
      finally
        Helper.removeTestProduct test
        Helper.removeTestProduct (test+"2")

    [<Fact>]
    let ``Test get transaction by account access that does not exist``() =
      let test = "TestGetTransByTypeAcctAccsNo"
      try
        let testProd1 = Helper.createTestProduct test
        let testProd2 = Helper.createTestProduct (test+"2")
        let testTrans1 = Helper.createTestBuyTransaction test
        let testTrans2 = Helper.createTestRentTransaction test
        let testTrans3 = Helper.createTestBuyTransaction (test+"2")

        (fun () -> (getTransactionByAccountAccess "DoesNotExist") |> ignore) |> should throw typeof<AccountExceptions.NoSuchUser>
      finally
        Helper.removeTestProduct test
        Helper.removeTestProduct (test+"2")