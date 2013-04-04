namespace RentIt.Test
  open System
  open Xunit
  open FsUnit.Xunit
  open RentIt.ProductExceptions
  open RentIt.ProductTypes
  open RentIt

  module TestGetProductById =
    [<Fact>]
    let ``get product by id should work``() =
      let testProd = Helper.createTestProduct "test get p id"
      let p = Product.getProductById testProd.id
      try
        p |> should be ofExactType<Product>
      finally
        Helper.removeTestProduct "test get p id"

    [<Fact>]
    let ``get product by id should throw arg``() =
      (fun () -> (Product.getProductById -2) |> ignore)|> should throw typeof<ArgumentException>
  
    [<Fact>]
    let ``get product by id should throw no such``() =
      (fun () -> (Product.getProductById 05897) |> ignore) |> should throw typeof<NoSuchProduct> 