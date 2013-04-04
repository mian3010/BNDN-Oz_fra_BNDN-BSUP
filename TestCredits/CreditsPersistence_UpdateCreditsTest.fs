namespace RentIt.Test
  open Xunit
  open FsUnit.Xunit
  open RentIt.CreditsPersistence
  open RentIt

  module TestUpdateCredits =
    [<Fact>]
    let ``Test add credits to account``() =
      let test = "TestAddCreditsToAccount"
      try
        let account = Helper.createTestUser test
        let success = updateCredits account 20
        let updAccount = Account.getByUsername account
        success |> should equal true
        updAccount.info.credits.Value |> should equal 20
      finally
        Helper.removeTestUser test

    [<Fact>]
    let ``Test remove credits from account``() =
      let test = "TestRemoveCreditsFromAccount"
      try
        let account = Helper.createTestUser test
        updateCredits account 100 |> ignore
        let success = updateCredits account -20
        let updAccount = Account.getByUsername account
        success |> should equal true
        updAccount.info.credits.Value |> should equal 80
      finally
        Helper.removeTestUser test

    [<Fact>]
    let ``Test remove to many credits from account``() =
      let test = "TestRemoveToManyCreditsFromAccount"
      try
        let account = Helper.createTestUser test
        (fun () -> (updateCredits account -20) |> ignore) |> should throw typeof<CreditsExceptions.NotEnoughCredits>
      finally
        Helper.removeTestUser test

    [<Fact>]
    let ``Test add credits to wrong account``() =
      (fun () -> (updateCredits "DoesNotExist" 20) |> ignore) |> should throw typeof<AccountExceptions.NoSuchUser>