namespace RentIt
  open System
  open Xunit
  open FsUnit.Xunit
  open ProductExceptions
  open ProductTypes

  module TestGetProductById =
    [<Fact>]
    let ``get product by id should work``() =
      Helper.createTestProduct "test get p id" |> ignore
      let p = Product.getProductById 4
      p |> should be ofExactType<Product>
      Helper.removeTestProduct "test get p id"

    [<Fact>]
    let ``get product by id should throw arg``() =
      (fun () -> (Product.getProductById -2) |> ignore)|> should throw typeof<ArgumentException>
  
    [<Fact>]
    let ``get product by id should throw no such``() =
      (fun () -> (Product.getProductById 05897) |> ignore) |> should throw typeof<NoSuchProduct> 