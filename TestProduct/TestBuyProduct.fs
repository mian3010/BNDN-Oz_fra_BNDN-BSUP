namespace RentIt
module TestBuyProduct =

  open Xunit
  open FsUnit.Xunit
  open ProductExceptions
  open ProductTypes
  
  [<Fact>]
  let ``something with buy``() =
    0 |> should not' (be 0)
