namespace RentIt.Test
  open Xunit
  open FsUnit.Xunit
  open RentIt.ProductTypes
  open RentIt.ProductPersistence

  module TestCreateProduct =
    [<Fact>]
    let ``Test create product``() =
      let test = "TestCreateProduct"
      try
        let result = Helper.createTestProduct test
        let prod = Helper.getTestProduct test
        result.name |> should equal prod.name
        result.createDate |> should equal prod.createDate
        result.productType |> should equal prod.productType
        result.owner |> should equal prod.owner
        result.rating.IsNone |> should equal true
        result.published |> should equal prod.published
        result.id |> should be (greaterThan 0)
        result.thumbnailPath.IsNone |> should equal true
        result.metadata.IsNone |> should equal true
        result.description.Value |> should equal prod.description.Value
        result.rentPrice.Value |> should equal prod.rentPrice.Value
        result.buyPrice.Value |> should equal prod.buyPrice.Value
      finally
        Helper.removeTestProduct test

    [<Fact>]
    let ``Test create product with wrong type``() =
      let test = "TestCreateProductWrongType"
      try
        let testUser = Helper.createTestUser test
        let testProduct = Helper.getTestProduct test
        (fun () -> (createProduct testProduct) |> ignore) |> should throw typeof<NoSuchProductType>
      finally
        Helper.removeTestProduct test

    [<Fact>]
    let ``Test create product with wrong user``() =
      let test = "TestCreateProductWrongUser"
      try
        let testUser = Helper.createTestType test
        let testProduct = Helper.getTestProduct test
        (fun () -> (createProduct testProduct) |> ignore) |> should throw typeof<NoSuchUser>
      finally
        Helper.removeTestProduct test

    [<Fact>]
    let ``Test create product that already exists``() =
      let test = "TestCreateProductAlreadyExists"
      try
        let testProduct = Helper.createTestProduct test
        (fun () -> (createProduct testProduct) |> ignore) |> should throw typeof<ProductAlreadyExists>
      finally
        Helper.removeTestProduct test