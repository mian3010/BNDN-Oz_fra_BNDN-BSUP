namespace RentIt.Test
  open Xunit
  open FsUnit.Xunit
  open RentIt.CreditsPersistence
  open RentIt

  module TestGetTransactionById =
    [<Fact>]
    let ``Test get transaction by id``() =
      let test = "TestGetTransactionById"
      try
        let testProd = Helper.createTestProduct test
        let testTrans = Helper.createTestBuyTransaction test

        let transaction = match (getTransactionById testTrans.item.id) with
                            | CreditsTypes.RentOrBuy.Buy b -> Some b
                            | _ -> None

        testTrans.item.paid |> should equal transaction.Value.item.paid
        testTrans.item.product |> should equal transaction.Value.item.product
        testTrans.item.purchased |> should equal transaction.Value.item.purchased
        testTrans.item.id |> should equal transaction.Value.item.id
      finally
        Helper.removeTestProduct test

    [<Fact>]
    let ``Test get transaction by id that does not exist``() =
      let test = "TestGetTransactionByIdNotExist"
      try
        let payDate = System.DateTime.Parse "2012-01-01 01:01:01"
        let testProd = Helper.createTestProduct test
        (fun () -> (getTransactionById 234234) |> ignore) |> should throw typeof<CreditsExceptions.NoSuchTransaction>
      finally
        Helper.removeTestProduct test
