namespace RentIt.Test.ModulePersistence.ProductPersistence
  open Xunit
  open FsUnit.Xunit
  open RentIt.ProductTypes
  open RentIt.ProductPersistence

  module TestGetProduct =
    [<Fact>]
    let ``Test get product by id``() =
      let test = "TestGetProductById"
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
      getProd.thumbnailPath.IsNone |> should equal true
      getProd.metadata.IsNone |> should equal true
      getProd.description.Value |> should equal testProd.description.Value
      getProd.rentPrice.Value |> should equal testProd.rentPrice.Value
      getProd.buyPrice.Value |> should equal testProd.buyPrice.Value
      Helper.removeTestProduct test

    [<Fact>]
    let ``Test get product by name``() =
      let test = "TestGetProductByName"
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
      getProd.thumbnailPath.IsNone |> should equal true
      getProd.metadata.IsNone |> should equal true
      getProd.description.Value |> should equal testProd.description.Value
      getProd.rentPrice.Value |> should equal testProd.rentPrice.Value
      getProd.buyPrice.Value |> should equal testProd.buyPrice.Value
      Helper.removeTestProduct test

    [<Fact>]
    let ``Test get product by type``() =
      let test = "TestGetProductByType"
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
      getProd.thumbnailPath.IsNone |> should equal true
      getProd.metadata.IsNone |> should equal true
      getProd.description.Value |> should equal testProd.description.Value
      getProd.rentPrice.Value |> should equal testProd.rentPrice.Value
      getProd.buyPrice.Value |> should equal testProd.buyPrice.Value
      Helper.removeTestProduct test