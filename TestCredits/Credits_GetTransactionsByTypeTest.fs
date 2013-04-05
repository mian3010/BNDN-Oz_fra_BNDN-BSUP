namespace RentIt.Test
  open Xunit
  open FsUnit.Xunit
  open RentIt
  open RentIt.CreditsTypes

  module TestGetTransactinsByType =
    [<Fact>]
    let ``Test get transactions by type buy``() =
      let test = "TestGetTransByTypeBuy"
      try
        let testProd = Helper.createTestProduct test
        let testProd2 = Helper.createTestProduct (test+"2")
        let testUser = Account.getByUsername testProd.owner
        let testTrans1 = Helper.createTestBuyTransaction test
        let testTrans2 = Helper.createTestBuyTransaction test
        let testTrans3 = Helper.createTestRentTransaction test
        let testTrans4 = Helper.createTestBuyTransaction (test+"2")

        let trans = Credits.getTransactionsByType testUser false

        trans.Length |> should equal 2
      finally
        Helper.removeTestProduct test
        Helper.removeTestProduct (test+"2")

    [<Fact>]
    let ``Test get transactions by type rent``() =
      let test = "TestGetTransByTypeRent"
      try
        let testProd = Helper.createTestProduct test
        let testProd2 = Helper.createTestProduct (test+"2")
        let testUser = Account.getByUsername testProd.owner
        let testTrans1 = Helper.createTestBuyTransaction test
        let testTrans2 = Helper.createTestBuyTransaction test
        let testTrans3 = Helper.createTestRentTransaction test
        let testTrans4 = Helper.createTestBuyTransaction (test+"2")

        let trans = Credits.getTransactionsByType testUser true

        trans.Length |> should equal 1
      finally
        Helper.removeTestProduct test
        Helper.removeTestProduct (test+"2")

    [<Fact>]
    let ``Test get transactions by type for user that does not exist``() =
      let test = "TestGetTransByTypeUserNotExist"
      let testUser = Account.getByUsername (Helper.createTestUser test)
      Helper.removeTestUser test
      (fun() -> (Credits.getTransactions testUser) |> ignore) |> should throw typeof<AccountExceptions.NoSuchUser>