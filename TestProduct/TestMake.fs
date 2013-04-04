namespace RentIt.Test

module ProductTest = 
  open Xunit
  open FsUnit.Xunit
  open RentIt.ProductExceptions
  open RentIt.ProductTypes
  open RentIt

  let userName = "Lynette"
  let name = "test product"
  let productType = "Movie"
  let description = Some "desc"
  let buyPrice = Some 12
  let rentPrice = Some 5

  let removeProduct name =
    let filtersQ = Persistence.Filter.createFilter [] "Product" "Name" name
    Persistence.Api.delete "Product" filtersQ |> ignore

  [<Fact>]
  let ``buyPrice should work``() =
    let p = Product.make userName "test make b" productType description buyPrice rentPrice
    try
      p.buyPrice.Value |> should equal buyPrice.Value
    finally
      removeProduct "test make b"

  [<Fact>]
  let ``rentPrice should work``() =
    let p = Product.make userName "test make r" productType description buyPrice rentPrice
    try
      p.rentPrice.Value |> should equal rentPrice.Value
    finally
      removeProduct "test make r"

  [<Fact>]
  let ``owner should work``() =
    let p = Product.make userName "test make own" productType description buyPrice rentPrice
    try
      p.owner |> should equal userName
    finally
      removeProduct "test make own"

  [<Fact>]
  let ``name should work``() =
    let p = Product.make userName "test make n" productType description buyPrice rentPrice
    try
      p.name |> should equal "test make n"
    finally
      removeProduct "test make n"

  [<Fact>]
  let ``desc should work``() =
    let p = Product.make userName "test make d" productType description buyPrice rentPrice
    try
      p.description.Value |> should equal description.Value
    finally
      removeProduct "test make d"

  [<Fact>]
  let ``type should work``() =
    let p = Product.make userName "test make t" productType description buyPrice rentPrice
    try
      p.productType |> should equal productType
    finally
      removeProduct "test make t"

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
  