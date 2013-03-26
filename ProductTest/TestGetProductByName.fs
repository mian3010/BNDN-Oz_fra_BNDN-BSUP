namespace RentIt

module TestGetProductByName =

  open Xunit
  open FsUnit.Xunit

  type Product = ProductTypes.Product
  exception ArgumentException = RentIt.Product.ArgumentException
  exception NoSuchProduct = RentIt.Product.NoSuchProduct

  [<Fact>]
  let ``get product by name should work``() =
    let p = Product.getProductByName "Test"
    p |> should not' (be null)
    p |> should be ofExactType<Product list>

  [<Fact>]
  let ``get product by name should throw arg``() =
    (Product.getProductByName "" |> ignore) |> should throw typeof<ArgumentException>
  
  [<Fact>]
  let ``get product byname should throw no such``() =
    (Product.getProductByName "This is very unlikely!" |> ignore) |> should throw typeof<NoSuchProduct>
   