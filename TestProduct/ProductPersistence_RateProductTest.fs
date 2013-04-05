namespace RentIt.Test
  open Xunit
  open FsUnit.Xunit
  open RentIt.ProductTypes
  open RentIt.ProductPersistence
  open RentIt.ProductExceptions

  module TestRateProduct =
    [<Fact>]
    let ``Test rate product``() =
      let test = "TestRateProduct"
      try
        //Create test data
        let testProd = ref (Helper.createTestProduct test)
        //Create three test users
        let testUser1 = Helper.createTestUser (test+"2")
        let testUser2 = Helper.createTestUser (test+"3")
        let testUser3 = Helper.createTestUser (test+"4")
        //Rate for them
        testProd := rateProduct (!testProd).id testUser1 4
        testProd := rateProduct (!testProd).id testUser2 -2
        testProd := rateProduct (!testProd).id testUser3 1
        //Test that number of votes and rating are correct
        (!testProd).rating.Value.votes |> should equal 3
        (!testProd).rating.Value.rating |> should equal 1
        //Change one of the users ratings
        testProd := rateProduct (!testProd).id testUser2 1
        //Test that number of votes and rating are correct
        (!testProd).rating.Value.votes |> should equal 3
        (!testProd).rating.Value.rating |> should equal 2
      finally
        //Clean up
        Helper.removeTestUser (test+"2")
        Helper.removeTestUser (test+"3")
        Helper.removeTestUser (test+"4")
        Helper.removeTestProduct test

    [<Fact>]
    let ``Test rate product that does not exist``() =
      let test = "TestRateDoesNotExist"
      try
        let testUser = Helper.createTestUser test
        (fun () -> (rateProduct 7833 testUser 5) |> ignore) |> should throw typeof<NoSuchProduct>
      finally
        Helper.removeTestUser test

    [<Fact>]
    let ``Test rate product with user that does not exist``() =
      let test = "TestRateUserDoesNotExist"
      try
        let testProd = Helper.createTestProduct test
        (fun () -> (rateProduct testProd.id "DoesNotExist" 5) |> ignore) |> should throw typeof<NoSuchUser>
      finally
        Helper.removeTestProduct test