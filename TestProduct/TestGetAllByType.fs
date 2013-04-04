namespace RentIt.Test
module TestGetAllByType =

  open Xunit
  open FsUnit.Xunit
  open RentIt.ProductExceptions
  open RentIt.ProductTypes
  open RentIt

  [<Fact>]
  let ``get products by type name should work``() =
    let tp = Helper.createTestProduct "test get p by type"
    let p = Product.getAllByType tp.productType
    try
      p |> should be ofExactType<Product list>
    finally
      Helper.removeTestProduct "test get p by type"

  [<Fact>]
  let ``get products by type name should throw arg``() =
    (fun () -> Product.getAllByType "" |> ignore) |> should throw typeof<ArgumentException>
  
  [<Fact>]
  let ``get products by type name should throw no such``() =
    (fun () -> Product.getAllByType "This is very unlikely!" |> ignore) |> should throw typeof<NoSuchProductType>
   