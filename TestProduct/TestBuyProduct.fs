namespace RentIt.Test
module TestBuyProduct =

  open Xunit
  open FsUnit.Xunit
  open RentIt.ProductExceptions
  open RentIt.ProductTypes
  open RentIt
  
  [<Fact>]
  let ``something with buy``() =
    0 |> should not' (be 0)
