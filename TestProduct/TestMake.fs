namespace RentIt

module ProductTest = 
  open Xunit
  open FsUnit.Xunit
  open ProductExceptions
  open ProductTypes

  let userName = "Lynette"
  let name = "test product"
  let productType = "Movie"
  let description = Some "desc"
  let buyPrice = Some 12
  let rentPrice = Some 5

  [<Fact>]
  let ``buyPrice should work``() =
    let p = Product.make userName name productType description buyPrice rentPrice
    p.buyPrice.Value |> should equal buyPrice.Value

  [<Fact>]
  let ``rentPrice should work``() =
    let p = Product.make userName name productType description buyPrice rentPrice
    p.rentPrice.Value |> should equal rentPrice.Value

  [<Fact>]
  let ``owner should work``() =
    let p = Product.make userName name productType description buyPrice rentPrice
    p.owner |> should equal userName

  [<Fact>]
  let ``name should work``() =
    let p = Product.make userName name productType description buyPrice rentPrice
    p.name |> should equal name

  [<Fact>]
  let ``desc should work``() =
    let p = Product.make userName name productType description buyPrice rentPrice
    p.description.Value |> should equal description.Value

  [<Fact>]
  let ``type should work``() =
    let p = Product.make userName name productType description buyPrice rentPrice
    p.productType |> should equal productType

  [<Fact>]
  let ``owner should throw arg``() =
    (fun () -> Product.make null name productType description buyPrice rentPrice |> ignore) |> should throw typeof<ArgumentException>
  
  [<Fact>]
  let ``name should throw arg``() =
    (fun () -> Product.make userName null productType description buyPrice rentPrice |> ignore) |> should throw typeof<ArgumentException>
  
  [<Fact>]
  let ``productType should throw arg``() =
    (fun () -> Product.make userName name null description buyPrice rentPrice |> ignore) |> should throw typeof<ArgumentException>
  
  [<Fact>]
  let ``productType should throw no such``() =
    (fun () -> Product.make userName name "asdlkfj s" description buyPrice rentPrice |> ignore) |> should throw typeof<NoSuchProductType>
  