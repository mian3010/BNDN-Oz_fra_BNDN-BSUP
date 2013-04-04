namespace RentIt.Test

module TestPersist = 

  open Xunit
  open FsUnit.Xunit
  open RentIt.ProductExceptions
  open RentIt.ProductTypes
  open RentIt

  [<Fact>]
  let ``something with persist``() =
    let user = Helper.createTestUser "test persist"
    let typ = Helper.createTestType "test persist"
    let tp = Product.persist (Helper.getTestProduct "test persist")
    let p = (Product.getProductByName tp.name).Head
    try
      p |> should equal tp
    finally
      Helper.removeTestProduct "test persist"
