namespace RentIt
module TestUpdate =

  open Xunit
  open FsUnit.Xunit
  open ProductExceptions
  open ProductTypes

  // TODO: Add test cases for following:
  let rating:Rating = {  rating=0; votes=0;  }
  let published = true
  let id = 5
  let metadata = None
  let thumbnailPath = None

  [<Fact>]
  let ``update buyPrice should work``() =
    let tp = Helper.createTestProduct "test upd buy"
    let uv = Some 17
    let upd = Product.update {  tp with buyPrice = uv }
    upd.buyPrice.Value |> should equal uv.Value
    Helper.removeTestProduct "test upd buy"
    
  [<Fact>]
  let ``update rentPrice should work``() =
    let tp = Helper.createTestProduct "test upd rent"
    let uv = Some 17
    let upd = Product.update {  tp with rentPrice = uv }
    upd.rentPrice.Value |> should equal uv.Value
    Helper.removeTestProduct "test upd rent"

  [<Fact>]
  let ``update owner should not work``() =
    let tp = Helper.createTestProduct "test upd own fail"
    (fun () -> tp = Product.update tp |> ignore) |> should throw typeof<UpdateNotAllowed>
    Helper.removeTestProduct "test upd own fail"

  [<Fact>]
  let ``update name should work``() =
    let tp = Helper.createTestProduct "test upd name"
    let uv = "bla bla bla"
    let upd = Product.update {  tp with name = "TESTPRODUCT_"+uv }
    upd.name |> should equal ("TESTPRODUCT_"+uv)
    Helper.removeTestProduct uv

  [<Fact>]
  let ``update desc should work``() =
    let tp = Helper.createTestProduct "test upd desc"
    let uv = Some "zzZZzzzz"
    let upd = Product.update {  tp with description = uv }
    upd.description.Value |> should equal uv.Value
    Helper.removeTestProduct "test upd desc"

  [<Fact>]
  let ``update type should work``() =
    let tp = Helper.createTestProduct "test upd type"
    let uv = Helper.createTestType "test upd type z"
    let upd = Product.update {  tp with productType = uv }
    upd.productType |> should equal uv
    Helper.removeTestProduct "test upd type"
    Helper.removeTestType "test upd type z"

  [<Fact>]
  let ``update owner should throw arg``() =
    let tp = Helper.createTestProduct "test upd own fail 2"
    (fun () -> tp = Product.update tp |> ignore) |> should throw typeof<ArgumentException>
    Helper.removeTestProduct "test upd own fail 2"
  
  [<Fact>]
  let ``update name should throw arg``() =
    let tp = {  (Helper.createTestProduct "test upd name fail") with name = null  }
    (fun () -> tp = Product.update tp |> ignore) |> should throw typeof<ArgumentException>
    Helper.removeTestProduct "test upd name fail"
  
  [<Fact>]
  let ``update productType should throw arg``() =
    let tp = {  (Helper.createTestProduct "test upd ptype fail") with productType = null  }
    (fun () -> tp = Product.update tp |> ignore) |> should throw typeof<ArgumentException>
    Helper.removeTestProduct "test upd ptype fail"
  
  [<Fact>]
  let ``update productType should throw no such``() =
    let tp = {  (Helper.createTestProduct "test upd ptype fail 2") with productType = "not very likely" }
    (fun () -> tp = Product.update tp |> ignore) |> should throw typeof<ArgumentException>
    Helper.removeTestProduct "test upd ptype fail 2"
  