namespace RentIt.Test
  open Xunit
  open FsUnit.Xunit
  open RentIt

  module TestPurchaseCredits =
    [<Fact>]
    let ``Test purchase credits``() =
      let test = "TestPurchaseCredits"
      try
        let testUser = Account.getByUsername (Helper.createTestUser test)
        let success = Credits.purchaseCredits testUser 20
        success |> should equal true
        let testUser = Account.getByUsername testUser.user
        testUser.info.credits.Value |> should equal 20
      finally
        Helper.removeTestUser test

    [<Fact>]
    let ``Test purchase negative credits``() =
      let test = "TestPurchaseNegativeCredits"
      try
        let testUser = Account.getByUsername (Helper.createTestUser test)
        (fun () -> (Credits.purchaseCredits testUser -20) |> ignore) |> should throw typeof<CreditsExceptions.InvalidCredits>
      finally
        Helper.removeTestUser test