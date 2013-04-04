namespace RentIt.Test
  open Xunit
  open FsUnit.Xunit
  open RentIt.CreditsPersistence
  open RentIt

  module TestGetTransactionByType =
    [<Fact>]
    let ``Test get transaction by type with result for buy``() =
      let test = "TestGetTransactionByTypeBuy"
      try
        let testProd = Helper.createTestProduct test
        let testTrans = Helper.createTestBuyTransaction test

        let transRes = match (getTransactionsByType false).Head with
                        | CreditsTypes.RentOrBuy.Buy b -> Some b
                        | _ -> None

        testTrans.item.paid |> should equal transRes.Value.item.paid
        testTrans.item.product |> should equal transRes.Value.item.product
        testTrans.item.purchased |> should equal transRes.Value.item.purchased
        testTrans.item.id |> should equal transRes.Value.item.id
      finally
        Helper.removeTestProduct test

    [<Fact>]
    let ``Test get transaction by type with result for rent``() =
      let test = "TestGetTransactionByTypeRent"
      try
        let testProd = Helper.createTestProduct test
        let testTrans = Helper.createTestRentTransaction test

        let transRes = match (getTransactionsByType true).Head with
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
    let ``Test get transaction by type with no result for buy``() =
      let test = "TestGetTransactionByTypeNoResultBuy"
      try
        let testProd = Helper.createTestProduct test
        let testTrans = Helper.createTestBuyTransaction test

        (getTransactionsByType true).Length |> should equal 0
      finally
        Helper.removeTestProduct test

    [<Fact>]
    let ``Test get transaction by type with no result for rent``() =
      let test = "TestGetTransactionByTypeNoResultRent"
      try
        let testProd = Helper.createTestProduct test
        let testTrans = Helper.createTestRentTransaction test

        (getTransactionsByType false).Length |> should equal 0
      finally
        Helper.removeTestProduct test