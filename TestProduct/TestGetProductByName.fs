namespace RentIt

module TestGetProductByName =

  open Xunit
  open FsUnit.Xunit
  open ProductExceptions
  open ProductTypes

  [<Fact>]
  let ``get product by name should work``() =
    let p2 = Helper.createTestProduct "Test"
    let p = Product.getProductByName p2.name
    p |> should be ofExactType<Product list>
    Helper.removeTestProduct "Test"

  [<Fact>]
  let ``get product by name should throw arg``() =
    (fun () -> Product.getProductByName "" |> ignore) |> should throw typeof<ArgumentException>
  
  [<Fact>]
  let ``get product byname should throw no such``() =
    (fun () -> Product.getProductByName "This is very unlikely!" |> ignore) |> should throw typeof<NoSuchProduct>
   