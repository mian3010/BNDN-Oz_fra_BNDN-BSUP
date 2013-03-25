namespace RentIt

module TestGetProductById =

  open Xunit
  open FsUnit.Xunit

  type Product = ProductTypes.Product
  exception ArgumentException = RentIt.Product.ArgumentException
  exception NoSuchProduct = RentIt.Product.NoSuchProduct

  [<Fact>]
  let ``get product by id should work``() =
    let p = Product.getProductById "1"
    p |> should not' (be null)
    p |> should be ofExactType<Product>

  [<Fact>]
  let ``get product by id should throw arg``() =
    (Product.getProductById "" |> ignore) |> should throw typeof<ArgumentException>
  
  [<Fact>]
  let ``get product by id should throw no such``() =
    (Product.getProductById "05897" |> ignore) |> should throw typeof<ArgumentException>
   
  [<Fact>]
  let ``get product by id should throw no such #2``() =
    (Product.getProductById "muhaHA" |> ignore) |> should throw typeof<NoSuchProduct>
   