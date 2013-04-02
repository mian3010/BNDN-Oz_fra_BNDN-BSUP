namespace RentIt.Test.ModulePersistence.ProductPersistence
  open Xunit
  open FsUnit.Xunit
  open RentIt.ProductTypes
  open RentIt.ProductPersistence

  module TestCreateProduct =
    [<Fact>]
    let ``Test create product``() =
      let test = "TestCreateProduct"
      let prod = Helper.createTestProduct test
      let result = Helper.getTestProduct test
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
      Helper.removeTestProduct test