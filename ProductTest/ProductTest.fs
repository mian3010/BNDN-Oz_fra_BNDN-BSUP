namespace RentIt

module ProductTest = 
  open Xunit
  open FsUnit.Xunit

  type Product = ProductTypes.Product

  let userId = "2"
  let name = "test product"
  let productType = "Movie"
  let description = "desc"
  let buyPrice = 12
  let rentPrice = 5

  [<Fact>]
  let ``make that works``() =
    let p = Product.make userId name productType (Some description) (Some buyPrice) (Some rentPrice)
    p.buyPrice.Value |> should equal buyPrice