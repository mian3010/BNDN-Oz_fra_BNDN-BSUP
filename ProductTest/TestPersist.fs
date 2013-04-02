namespace RentIt

module TestPersist = 

  open Xunit
  open FsUnit.Xunit
  open ProductExceptions
  open ProductTypes

  [<Fact>]
  let ``something with persist``() =
    0 |> should not' (be 0)
