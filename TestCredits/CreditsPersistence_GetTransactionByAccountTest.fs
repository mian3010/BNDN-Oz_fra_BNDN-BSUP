namespace RentIt.Test
  open Xunit
  open FsUnit.Xunit
  open RentIt.CreditsPersistence
  open RentIt

  module TestGetTransactionByAccount =
    [<Fact>]
    let ``Test get transaction by account``() =
      let test = "TestGetTransactionByAccount"
      try
        let testProd = Helper.createTestProduct test
        let testTrans = Helper.createTestBuyTransaction test
        let transRes = match (getTransactionByAccount testProd.owner).Head with
                        | CreditsTypes.RentOrBuy.Buy b -> Some b
                        | _ -> None

        testTrans.item.paid |> should equal transRes.Value.item.paid
        testTrans.item.product |> should equal transRes.Value.item.product
        testTrans.item.purchased |> should equal transRes.Value.item.purchased
        testTrans.item.id |> should equal transRes.Value.item.id
      finally
        Helper.removeTestProduct test

    [<Fact>]
    let ``Test get transaction by account that does not exist``() =
      let test = "TestGetTransactionByAccountNotExist"
      try
        let testProd = Helper.createTestProduct test
        let testTrans = Helper.createTestBuyTransaction test
        (fun () -> (getTransactionByAccount "DoesNotExist") |> ignore) |> should throw typeof<AccountExceptions.NoSuchUser>
      finally
        Helper.removeTestProduct test