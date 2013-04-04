namespace RentIt.Test
  open Xunit
  open FsUnit.Xunit
  open RentIt.CreditsPersistence
  open RentIt

  module TestGetTransactionByTypeAccount =
    [<Fact>]
    let ``Test get transaction by type and account with result for buy``() =
      let test = "TestGetTransactionByTypeAccountBuy"
      try
        let testProd = Helper.createTestProduct test
        let testTrans = Helper.createTestBuyTransaction test

        let transRes = match (getTransactionsByTypeAccount false testProd.owner).Head with
                        | CreditsTypes.RentOrBuy.Buy b -> Some b
                        | _ -> None

        testTrans.item.paid |> should equal transRes.Value.item.paid
        testTrans.item.product |> should equal transRes.Value.item.product
        testTrans.item.purchased |> should equal transRes.Value.item.purchased
        testTrans.item.id |> should equal transRes.Value.item.id
      finally
        Helper.removeTestProduct test

    [<Fact>]
    let ``Test get transaction by type and account with result for rent``() =
      let test = "TestGetTransactionByTypeAccountRent"
      try
        let testProd = Helper.createTestProduct test
        let testTrans = Helper.createTestRentTransaction test

        let transRes = match (getTransactionsByTypeAccount true testProd.owner).Head with
                        | CreditsTypes.RentOrBuy.Rent b -> Some b
                        | _ -> None

        testTrans.item.paid |> should equal transRes.Value.item.paid
        testTrans.item.product |> should equal transRes.Value.item.product
        testTrans.item.purchased |> should equal transRes.Value.item.purchased
        testTrans.item.id |> should equal transRes.Value.item.id
        testTrans.expires |> should equal transRes.Value.expires
      finally
        Helper.removeTestProduct test

    [<Fact>]
    let ``Test get transaction by type and account with no result for buy``() =
      let test = "TestGetTransactionByTypeAccountNoResultBuy"
      try
        let testProd = Helper.createTestProduct test
        let testTrans = Helper.createTestBuyTransaction test

        (getTransactionsByTypeAccount true testProd.owner).Length |> should equal 0
      finally
        Helper.removeTestProduct test

    [<Fact>]
    let ``Test get transaction by type and account with no result for rent``() =
      let test = "TestGetTransactionByTypeAccountNoResultRent"
      try
        let testProd = Helper.createTestProduct test
        let testTrans = Helper.createTestRentTransaction test

        (getTransactionsByTypeAccount false testProd.owner).Length |> should equal 0
      finally
        Helper.removeTestProduct test

    [<Fact>]
    let ``Test get transaction by type and other account with no result for rent``() =
      let test = "TestGetTransactionByTypeAccountWrongNoResultRent"
      try
        let testProd = Helper.createTestProduct test
        let testUser = Helper.createTestUser (test+"2")
        let testTrans = Helper.createTestRentTransaction test

        (getTransactionsByTypeAccount true testUser).Length |> should equal 0
      finally
        Helper.removeTestProduct test
        Helper.removeTestUser (test+"2")

    [<Fact>]
    let ``Test get transaction by type and wrong account``() =
      let test = "TestGetTransactionByTypeAccountNotExist"
      try
        let testProd = Helper.createTestProduct test
        let testTrans = Helper.createTestRentTransaction test

        (fun () -> (getTransactionsByTypeAccount true "DoesNotExist").Length |> ignore) |> should throw typeof<AccountExceptions.NoSuchUser>
      finally
        Helper.removeTestProduct test