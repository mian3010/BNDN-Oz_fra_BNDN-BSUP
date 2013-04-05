namespace RentIt.Test
  open Xunit
  open FsUnit.Xunit
  open RentIt
  open RentIt.CreditsTypes

  module TestGetTransactions =
    [<Fact>]
    let ``Test get transactions``() =
      let test = "TestGetTransactions"
      try
        let testProd = Helper.createTestProduct test
        let testProd2 = Helper.createTestProduct (test+"2")
        let testUser = Account.getByUsername testProd.owner
        let testTrans1 = Helper.createTestBuyTransaction test
        let testTrans2 = Helper.createTestRentTransaction test
        let testTrans3 = Helper.createTestBuyTransaction (test+"2")

        let trans = Credits.getTransactions testUser

        trans.Length |> should equal 2
      finally
        Helper.removeTestProduct test

    [<Fact>]
    let ``Test get transactions for user that does not exist``() =
      let test = "TestGetTransactionsUserNotExist"
      let testUser = Account.getByUsername (Helper.createTestUser test)
      Helper.removeTestUser test
      (fun() -> (Credits.getTransactions testUser) |> ignore) |> should throw typeof<AccountExceptions.NoSuchUser>