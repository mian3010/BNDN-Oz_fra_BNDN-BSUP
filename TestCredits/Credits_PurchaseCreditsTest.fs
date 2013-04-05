namespace RentIt.Test
  open Xunit
  open FsUnit.Xunit
  open RentIt

  module TestPurchaseCredits =
    [<Fact>]
    let ``Test purchase credits``() =
      let test = "TestPurchaseCredits"
      try
        let testUser = Helper.createTestUser test
        ()
      finally
        Helper.removeTestUser test

    [<Fact>]
    let ``Test purchase negative credits``() =
      let test = "TestPurchaseCredits"
      try
        let testUser = Helper.createTestUser test
        ()
      finally
        Helper.removeTestUser test