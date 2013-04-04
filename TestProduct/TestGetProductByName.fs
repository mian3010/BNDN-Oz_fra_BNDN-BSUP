namespace RentIt.Test

module TestGetProductByName =

  open Xunit
  open FsUnit.Xunit
  open RentIt.ProductExceptions
  open RentIt.ProductTypes
  open RentIt

  [<Fact>]
  let ``get product by name should work``() =
    let p2 = Helper.createTestProduct "test get p by name"
    let p = Product.getProductByName p2.name
    try
      p |> should be ofExactType<Product list>
    finally
      Helper.removeTestProduct "test get p by name"

  [<Fact>]
  let ``get product by name should throw arg``() =
    (fun () -> Product.getProductByName "" |> ignore) |> should throw typeof<ArgumentException>
  
  [<Fact>]
  let ``get product byname should throw no such``() =
    (fun () -> Product.getProductByName "This is very unlikely!" |> ignore) |> should throw typeof<NoSuchProduct>
   