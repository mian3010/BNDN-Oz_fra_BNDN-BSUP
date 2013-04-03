namespace RentIt
module TestGetAllByType =

  open Xunit
  open FsUnit.Xunit
  open ProductExceptions
  open ProductTypes

  [<Fact>]
  let ``get product types by name should work``() =
    let p = Product.getAllByType "Movie"
    p |> should be ofExactType<Product list>

  [<Fact>]
  let ``get product types by name should throw arg``() =
    (fun () -> Product.getAllByType "" |> ignore) |> should throw typeof<ArgumentException>
  
  [<Fact>]
  let ``get product types by name should throw no such``() =
    (fun () -> Product.getAllByType "This is very unlikely!" |> ignore) |> should throw typeof<NoSuchProductType>
   