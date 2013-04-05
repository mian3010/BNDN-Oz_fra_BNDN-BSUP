namespace RentIt.Test
  open Xunit
  open FsUnit.Xunit
  open RentIt.ProductTypes
  open RentIt.ProductPersistence
  open RentIt.ProductExceptions

  module TestPublishProduct =
    [<Fact>]
    let ``Test publish product``() =
      let test = "TestPublishProduct"
      try
        //Create test data
        let testProd = ref (Helper.createTestProduct test)
        //Unpublish product
        testProd := publishProduct (!testProd).id false
        //Test that product has been unpublished
        (!testProd).published |> should equal false
        //Set published again
        testProd := publishProduct (!testProd).id true
        //Test that product has been published
        (!testProd).published |> should equal true
      finally
        //Clean up after ourselves
        Helper.removeTestProduct test

    [<Fact>]
    let ``Test publish product that does not exist``() =
     let test = "TestPublishNotExist"
     (fun () -> (publishProduct 7833 true) |> ignore) |> should throw typeof<NoSuchProduct>