namespace RentIt.Test.ModulePersistence.ProductPersistence
  open Xunit
  open FsUnit.Xunit
  open RentIt.ProductTypes
  open RentIt.ProductPersistence

  module TestCreateProductType =
    [<Fact>]
    let ``Test create product type``() =
      //Create test type
      let test = "TestCreateProductType"
      let prod = Helper.createTestType
      let result = Helper.getProductType test
      //Test that it is the right one that are returned
      result |> should equal prod
      //Clean up
      Helper.removeTestType test