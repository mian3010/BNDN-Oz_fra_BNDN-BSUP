namespace RentIt
module TestBuyProduct =

  open Xunit
  open FsUnit.Xunit

  type Product = ProductTypes.Product
  exception ArgumentException = RentIt.Product.ArgumentException
  exception NoSuchProductType = RentIt.Product.NoSuchProductType
  
  [<Fact>]
  let ``something with buy``() =
    0 |> should not' (be 0)
