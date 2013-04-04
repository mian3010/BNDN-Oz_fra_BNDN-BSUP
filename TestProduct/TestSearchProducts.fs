namespace RentIt.Test

module TestSearch = 
  open Xunit
  open FsUnit.Xunit
  open RentIt

  [<Fact>]
  let ``search products should work``() =
    try
      Helper.createTestType "search test" |> ignore
      Helper.createTestUser "search test" |> ignore
      let p1 = Product.persist ({(Helper.getTestProduct "search test") with name = "hihi"})
      Helper.createTestType "search test2" |> ignore
      Helper.createTestUser "search test2" |> ignore
      let p2 = Product.persist ({(Helper.getTestProduct "search test2") with description = Some "car"})
      let res = Product.searchProducts "hi"
      printfn "%A" res
      res.IsEmpty |> should equal false
    finally
      Helper.removeTestProduct "hihi"
      Helper.removeTestProduct "search test2"
      Helper.removeTestType "search test"
      Helper.removeTestType "search test2"
      Helper.removeTestUser "search test"
      Helper.removeTestUser "search test2"