namespace RentIt

module TestPersist = 

  open Xunit
  open FsUnit.Xunit
  open ProductExceptions
  open ProductTypes

  [<Fact>]
  let ``something with persist``() =
    let user = Helper.createTestUser "test persist"
    let typ = Helper.createTestType "test persist"
    let tp = Product.persist (Helper.getTestProduct "test persist")
    let p = (Product.getProductByName tp.name).Head
    p |> should equal tp
    Helper.removeTestProduct "test persist"
