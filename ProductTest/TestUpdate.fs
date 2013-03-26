namespace RentIt
module TestUpdate =

  open Xunit
  open FsUnit.Xunit

  type Product = ProductTypes.Product
  exception ArgumentException = RentIt.Product.ArgumentException
  exception NoSuchProductType = RentIt.Product.NoSuchProductType
  exception UpdateNotAllowed  = RentIt.Product.UpdateNotAllowed

  let userId = "2"
  let name = "test product"
  let productType = "Movie"
  let description = Some "desc"
  let buyPrice = Some 12
  let rentPrice = Some 5

  let p = Product.make userId name productType description buyPrice rentPrice

  [<Fact>]
  let ``update buyPrice should work``() =
    let tp:Product =
            {
              name = p.name;
              createDate = p.createDate;
              productType = p.productType;
              owner = p.owner;
              description = p.description;
              rentPrice = p.rentPrice;
              buyPrice = Some 15;
            }
    let tpu = Product.update tp
    tp.buyPrice.Value |> should equal tpu.buyPrice.Value

  [<Fact>]
  let ``update rentPrice should work``() =
    let tp:Product =
            {
              name = p.name;
              createDate = p.createDate;
              productType = p.productType;
              owner = p.owner;
              description = p.description;
              rentPrice = Some 8;
              buyPrice = p.buyPrice;
            }
    let tpu = Product.update tp
    tp.rentPrice.Value |> should equal tpu.rentPrice.Value

  [<Fact>]
  let ``update owner should not work``() =
    let tp:Product =
            {
              name = p.name;
              createDate = p.createDate;
              productType = p.productType;
              owner = "This is not legal";
              description = p.description;
              rentPrice = p.rentPrice;
              buyPrice = p.buyPrice;
            }
    (tp = Product.update tp |> ignore) |> should throw typeof<UpdateNotAllowed>

  [<Fact>]
  let ``update name should work``() =
    let tp:Product =
            {
              name = "This is a great movie title";
              createDate = p.createDate;
              productType = p.productType;
              owner = p.owner;
              description = p.description;
              rentPrice = p.rentPrice;
              buyPrice = p.buyPrice;
            }
    let tpu = Product.update tp
    tp.name |> should equal tpu.name

  [<Fact>]
  let ``update desc should work``() =
    let tp:Product =
            {
              name = p.name;
              createDate = p.createDate;
              productType = p.productType;
              owner = p.owner;
              description = Some "other desc";
              rentPrice = p.rentPrice;
              buyPrice = p.buyPrice;
            }
    let tpu = Product.update tp
    tp.description |> should equal tpu.description

  [<Fact>]
  let ``update type should work``() =
    let tp:Product =
            {
              name = p.name;
              createDate = p.createDate;
              productType = "Music";
              owner = p.owner;
              description = p.description;
              rentPrice = p.rentPrice;
              buyPrice = p.buyPrice;
            }
    let tpu = Product.update tp
    tp.productType |> should equal tpu.productType

  [<Fact>]
  let ``update owner should throw arg``() =
    let tp:Product =
            {
              name = p.name;
              createDate = p.createDate;
              productType = p.productType;
              owner = "";
              description = p.description;
              rentPrice = p.rentPrice;
              buyPrice = p.buyPrice;
            }
    (tp = Product.update tp |> ignore) |> should throw typeof<ArgumentException>
  
  [<Fact>]
  let ``update name should throw arg``() =
    let tp:Product =
            {
              name = "";
              createDate = p.createDate;
              productType = p.productType;
              owner = p.owner;
              description = p.description;
              rentPrice = p.rentPrice;
              buyPrice = p.buyPrice;
            }
    (tp = Product.update tp |> ignore) |> should throw typeof<ArgumentException>
  
  [<Fact>]
  let ``update productType should throw arg``() =
    let tp:Product =
            {
              name = p.name;
              createDate = p.createDate;
              productType = "";
              owner = p.owner;
              description = p.description;
              rentPrice = p.rentPrice;
              buyPrice = p.buyPrice;
            }
    (tp = Product.update tp |> ignore) |> should throw typeof<ArgumentException>
  
  [<Fact>]
  let ``update productType should throw no such``() =
    let tp:Product =
            {
              name = p.name;
              createDate = p.createDate;
              productType = "This type is very unlikely to exsist";
              owner = p.owner;
              description = p.description;
              rentPrice = p.rentPrice;
              buyPrice = p.buyPrice;
            }
    (tp = Product.update tp |> ignore) |> should throw typeof<ArgumentException>
  