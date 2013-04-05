namespace RentIt.Test

module TestSearch = 
  open Xunit
  open FsUnit.Xunit
  open RentIt

  [<Fact>]
  let ``search products name should work``() =
    try
      Helper.createTestType "search test" |> ignore
      Helper.createTestUser "search test" |> ignore
      let p1 = Product.persist ({(Helper.getTestProduct "search test") with name = "TESTPRODUCT_hihi"})
      let res = Product.searchProducts "hi"
      res.IsEmpty |> should equal false
    finally
      Helper.removeTestProduct "hihi"
      Helper.removeTestProduct "search test"

  [<Fact>]
  let ``search products desc should work``() =
    try
      Helper.createTestType "search test2" |> ignore
      Helper.createTestUser "search test2" |> ignore
      let p2 = Product.persist ({(Helper.getTestProduct "search test2") with description = Some "carsomething"})
      let res = Product.searchProducts "car"
      res.IsEmpty |> should equal false
    finally
      Helper.removeTestProduct "search test2"