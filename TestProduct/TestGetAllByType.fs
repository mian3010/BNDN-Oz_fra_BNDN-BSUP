namespace RentIt
module TestGetAllByType =

  open Xunit
  open FsUnit.Xunit
  open ProductExceptions
  open ProductTypes

  [<Fact>]
  let ``get product types by name should work``() =
    Helper.createTestProduct "Test" |> ignore
    let p = Product.getAllByType "TESTTYPE_Test"
    p |> should be ofExactType<Product list>
    Helper.removeTestProduct "Test"

  [<Fact>]
  let ``get product types by name should throw arg``() =
    (fun () -> Product.getAllByType "" |> ignore) |> should throw typeof<ArgumentException>
  
  [<Fact>]
  let ``get product types by name should throw no such``() =
    (fun () -> Product.getAllByType "This is very unlikely!" |> ignore) |> should throw typeof<NoSuchProductType>
   