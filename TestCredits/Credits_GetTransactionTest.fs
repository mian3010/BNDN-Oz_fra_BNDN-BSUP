namespace RentIt.Test
  open Xunit
  open FsUnit.Xunit
  open RentIt
  open RentIt.CreditsTypes

  module TestGetTransaction =
    [<Fact>]
    let ``Test get transaction``() =
      let test = "TestGetTransaction"
      try
        let testProd = Helper.createTestProduct test
        let testTrans = Helper.createTestBuyTransaction test

        let getTrans = match (Credits.getTransaction testTrans.item.id) with
                        | RentOrBuy.Buy b -> Some b
                        | _ -> None
        getTrans.IsSome |> should equal true
        testTrans.item.paid |> should equal getTrans.Value.item.paid
        testTrans.item.product |> should equal getTrans.Value.item.product
        testTrans.item.purchased |> should equal getTrans.Value.item.purchased
        testTrans.item.id |> should equal getTrans.Value.item.id
      finally
        Helper.removeTestProduct test

    [<Fact>]
    let ``Test get transaction that does not exist``() =
      (fun() -> (Credits.getTransaction 87345) |> ignore) |> should throw typeof<CreditsExceptions.NoSuchTransaction>