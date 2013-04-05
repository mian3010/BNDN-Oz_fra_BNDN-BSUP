namespace RentIt.Test
  open Xunit
  open FsUnit.Xunit
  open RentIt.ProductTypes
  open RentIt.ProductPersistence
  open RentIt.ProductExceptions

  module TestGetProduct =
    [<Fact>]
    let ``Test get product by id``() =
      let test = "TestGetProductById"
      try
        //Create test data
        let testProd = Helper.createTestProduct test
        let getProd = getProductById testProd.id
        getProd.name |> should equal testProd.name
        getProd.createDate |> should equal testProd.createDate
        getProd.productType |> should equal testProd.productType
        getProd.owner |> should equal testProd.owner
        getProd.rating.IsNone |> should equal true
        getProd.published |> should equal testProd.published
        getProd.id |> should be (greaterThan 0)
        getProd.metadata.IsNone |> should equal true
        getProd.description.Value |> should equal testProd.description.Value
        getProd.rentPrice.Value |> should equal testProd.rentPrice.Value
        getProd.buyPrice.Value |> should equal testProd.buyPrice.Value
      finally
        Helper.removeTestProduct test

    [<Fact>]
    let ``Test get product by name``() =
      let test = "TestGetProductByName"
      try
        //Create test data
        let testProd = Helper.createTestProduct test
        let getProd = (getProductByName testProd.name).Head
        getProd.name |> should equal testProd.name
        getProd.createDate |> should equal testProd.createDate
        getProd.productType |> should equal testProd.productType
        getProd.owner |> should equal testProd.owner
        getProd.rating.IsNone |> should equal true
        getProd.published |> should equal testProd.published
        getProd.id |> should be (greaterThan 0)
        getProd.metadata.IsNone |> should equal true
        getProd.description.Value |> should equal testProd.description.Value
        getProd.rentPrice.Value |> should equal testProd.rentPrice.Value
        getProd.buyPrice.Value |> should equal testProd.buyPrice.Value
      finally
        Helper.removeTestProduct test

    [<Fact>]
    let ``Test get product by type``() =
      let test = "TestGetProductByType"
      try
        //Create test data
        let testProd = Helper.createTestProduct test
        let getProd = (getProductByType testProd.productType).Head
        getProd.name |> should equal testProd.name
        getProd.createDate |> should equal testProd.createDate
        getProd.productType |> should equal testProd.productType
        getProd.owner |> should equal testProd.owner
        getProd.rating.IsNone |> should equal true
        getProd.published |> should equal testProd.published
        getProd.id |> should be (greaterThan 0)
        getProd.metadata.IsNone |> should equal true
        getProd.description.Value |> should equal testProd.description.Value
        getProd.rentPrice.Value |> should equal testProd.rentPrice.Value
        getProd.buyPrice.Value |> should equal testProd.buyPrice.Value
      finally
        Helper.removeTestProduct test

    [<Fact>]
    let ``Test get product that does not exist``() =
     let test = "TestGetProductThatDoesNotExist"
     (fun () -> (getProductById 78723) |> ignore) |> should throw typeof<NoSuchProduct>

    [<Fact>]
    let ``Test get products that does not exist``() =
     let test = "TestGetProductsThatDoesNotExist"
     (fun () -> (getProductByName "DoesNotExist") |> ignore) |> should throw typeof<NoSuchProduct>