namespace RentIt.Test
module TestUpdate =

  open Xunit
  open FsUnit.Xunit
  open RentIt.ProductExceptions
  open RentIt.ProductTypes
  open RentIt

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
    try
      upd.buyPrice.Value |> should equal uv.Value
    finally
      Helper.removeTestProduct "test upd buy"
    
  [<Fact>]
  let ``update rentPrice should work``() =
    let tp = Helper.createTestProduct "test upd rent"
    let uv = Some 17
    let upd = Product.update {  tp with rentPrice = uv }
    try
      upd.rentPrice.Value |> should equal uv.Value
    finally
      Helper.removeTestProduct "test upd rent"

  [<Fact>]
  let ``update owner should not work``() =
    let tp = Helper.createTestProduct "test upd own fail"
    try
      (fun () -> tp = Product.update tp |> ignore) |> should throw typeof<UpdateNotAllowed>
    finally
      Helper.removeTestProduct "test upd own fail"

  [<Fact>]
  let ``update name should work``() =
    let tp = Helper.createTestProduct "test upd name"
    let uv = "bla bla bla"
    let upd = Product.update {  tp with name = "TESTPRODUCT_"+uv }
    try
      upd.name |> should equal ("TESTPRODUCT_"+uv)
    finally
      Helper.removeTestProduct uv
      Helper.removeTestProduct "test upd name"

  [<Fact>]
  let ``update desc should work``() =
    let tp = Helper.createTestProduct "test upd desc"
    let uv = Some "zzZZzzzz"
    let upd = Product.update {  tp with description = uv }
    try
      upd.description.Value |> should equal uv.Value
    finally
      Helper.removeTestProduct "test upd desc"

  [<Fact>]
  let ``update type should work``() =
    let tp = Helper.createTestProduct "test upd type"
    let uv = Helper.createTestType "test upd type z"
    let upd = Product.update {  tp with productType = uv }
    upd.productType |> should equal uv
    try
      Helper.removeTestProduct "test upd type"
    finally
      Helper.removeTestType "test upd type z"

  [<Fact>]
  let ``update owner should throw arg``() =
    let tp = Helper.createTestProduct "test upd own fail 2"
    try
      (fun () -> tp = Product.update tp |> ignore) |> should throw typeof<ArgumentException>
    finally
      Helper.removeTestProduct "test upd own fail 2"
  
  [<Fact>]
  let ``update name should throw arg``() =
    let tp = {  (Helper.createTestProduct "test upd name fail") with name = null  }
    try
      (fun () -> tp = Product.update tp |> ignore) |> should throw typeof<ArgumentException>
    finally
      Helper.removeTestProduct "test upd name fail"
  
  [<Fact>]
  let ``update productType should throw arg``() =
    let tp = {  (Helper.createTestProduct "test upd ptype fail") with productType = null  }
    try
      (fun () -> tp = Product.update tp |> ignore) |> should throw typeof<ArgumentException>
    finally
      Helper.removeTestProduct "test upd ptype fail"
  
  [<Fact>]
  let ``update productType should throw no such``() =
    let tp = {  (Helper.createTestProduct "test upd ptype fail 2") with productType = "not very likely" }
    try
      (fun () -> tp = Product.update tp |> ignore) |> should throw typeof<ArgumentException>
    finally
      Helper.removeTestProduct "test upd ptype fail 2"
  