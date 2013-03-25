namespace RentIt

module TestPersist = 

  open Xunit
  open FsUnit.Xunit

  type Product = ProductTypes.Product
  exception ArgumentException = RentIt.Product.ArgumentException
  exception NoSuchProduct = RentIt.Product.NoSuchProduct
  exception NoSuchUser = RentIt.Product.NoSuchUser
  exception ProductAlreadyExists = RentIt.Product.ProductAlreadyExists

  [<Fact>]
  let ``something with persist``() =
    0 |> should not' (be 0)
