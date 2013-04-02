namespace RentIt

module TestGetProductById =

  open Xunit
  open FsUnit.Xunit
  open ProductExceptions
  open ProductTypes

  [<Fact>]
  let ``get product by id should work``() =
    let p = Product.getProductById 1
    p |> should not' (be null)
    p |> should be ofExactType<Product>

  [<Fact>]
  let ``get product by id should throw arg``() =
    (Product.getProductById -2 |> ignore) |> should throw typeof<ArgumentException>
  
  [<Fact>]
  let ``get product by id should throw no such``() =
    (Product.getProductById 05897 |> ignore) |> should throw typeof<NoSuchProduct> 