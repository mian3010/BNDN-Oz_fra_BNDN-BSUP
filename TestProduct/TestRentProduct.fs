namespace RentIt.Test
module TestRentProduct =

  open Xunit
  open FsUnit.Xunit
  open RentIt.ProductExceptions
  open RentIt.ProductTypes
  open RentIt
  
  [<Fact>]
  let ``something with rent``() =
    0 |> should not' (be 0)
