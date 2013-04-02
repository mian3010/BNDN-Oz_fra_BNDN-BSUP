namespace RentIt
module TestRentProduct =

  open Xunit
  open FsUnit.Xunit
  open ProductExceptions
  open ProductTypes
  
  [<Fact>]
  let ``something with rent``() =
    0 |> should not' (be 0)
