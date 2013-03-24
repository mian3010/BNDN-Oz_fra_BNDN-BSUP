namespace RentIt

module ProductTest = 
  open Xunit
  open FsUnit.Xunit

  type Product = ProductTypes.Product
  //exception ArgumentException = RentIt.Product.ArgumentException

  let userId = "2"
  let name = "test product"
  let productType = "Movie"
  let description = Some "desc"
  let buyPrice = Some 12
  let rentPrice = Some 5

  [<Fact>]
  let ``buyPrice should work``() =
    let p = Product.make userId name productType description buyPrice rentPrice
    p.buyPrice.Value |> should equal buyPrice.Value

  [<Fact>]
  let ``rentPrice should work``() =
    let p = Product.make userId name productType description buyPrice rentPrice
    p.rentPrice.Value |> should equal rentPrice.Value

  [<Fact>]
  let ``owner should work``() =
    let p = Product.make userId name productType description buyPrice rentPrice
    p.owner |> should equal userId

  [<Fact>]
  let ``name should work``() =
    let p = Product.make userId name productType description buyPrice rentPrice
    p.name |> should equal name

  [<Fact>]
  let ``desc should work``() =
    let p = Product.make userId name productType description buyPrice rentPrice
    p.description.Value |> should equal description.Value

  [<Fact>]
  let ``type should work``() =
    let p = Product.make userId name productType description buyPrice rentPrice
    p.productType |> should equal productType

  [<Fact>]
  let ``owner should throw``() =
  //  Product.make null name productType description buyPrice rentPrice |> should throw typeof<ArgumentException>
    Product.make null name productType description buyPrice rentPrice |> should throw typeof<System.Exception>
  
  [<Fact>]
  let ``name should throw``() =
  //  Product.make null name productType description buyPrice rentPrice |> should throw typeof<ArgumentException>
    Product.make userId null productType description buyPrice rentPrice |> should throw typeof<System.Exception>
  
  [<Fact>]
  let ``productType should throw``() =
  //  Product.make null name productType description buyPrice rentPrice |> should throw typeof<ArgumentException>
    Product.make userId name null description buyPrice rentPrice |> should throw typeof<System.Exception>
  
  [<Fact>]
  let ``productType should throw no such``() =
  //  Product.make null name productType description buyPrice rentPrice |> should throw typeof<NoSuchProductType>
    Product.make userId name "muhaha" description buyPrice rentPrice |> should throw typeof<System.Exception>
      