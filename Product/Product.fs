namespace RentIt

module Product =

  type Product = ProductTypes.Product

  // Exceptions
  exception NoSuchProduct
  exception NoSuchUser
  exception ProductNotPublished
  exception ArgumentException of string
  exception ProductAlreadyExists
  exception NoSuchProductType

  /// <summay>
  /// Creater
  ///</summary>
  /// <typeparam> UserId </typeparam>
  /// <typeparam> Name </typeparam>
  /// <typeparam> ProductType </typeparam>
  /// <typeparam> Description </typeparam>
  /// <typeparam> BuyPrice </typeparam>
  /// <typeparam> RentPrice </typeparam>
  /// <exception> RentIt.Product.NoSuchUser </exception>
  /// <exception> RentIt.Product.ArgumentException </exception>
  let make (userId:string) (name:string) (productType:string) (description:string option) (buyPrice:int option) (rentPrice:int option) : Product =
    if (userId = null || userId.Trim().Length = 0) then raise (ArgumentException "UserId empty")
    if (name = null || name.Trim().Length = 0) then raise (ArgumentException "Name empty")
    if (productType = null || productType.Trim().Length = 0) then raise (ArgumentException "ProductType empty")
    {
      name = name;
      createDate = System.DateTime.Now;
      productType = productType;
      owner = userId;
      description = description;
      rentPrice = rentPrice;
      buyPrice = buyPrice;
    }
    
  /// <summary>
  /// Get prodcut by product id
  /// </summary>
  /// <typeparam> Id </typeparam>
  /// <returns> Product </returns>
  /// <exception> RentIt.Product.NoSuchProduct </exception>
  /// <exception> RentIt.Product.ArgumentException </exception>
  let getProductById (id:string) : Product =
    raise (new System.NotImplementedException())

  /// <summary>
  /// Persist a new product, making the product available for publish
  /// </summary>
  // <typeparam> Product </typeparam>
  /// <exception> RentIt.Product.ProductAlreadyExists </exception>
  /// <exception> RentIt.Product.NoSuchProductType </exception>
  /// <exception> RentIt.Product.NoSuchUser </exception>
  /// <exception> RentIt.Product.ArgumentException </exception>
  let persist (p:Product) = 
    raise (new System.NotImplementedException())

  /// <summary>
  /// Get products by product name
  /// </summary>
  // <typeparam> Product name </typeparam>
  /// <returns> List of products </returns>
  /// <exception> RentIt.Product.NoSuchProduct </exception>
  /// <exception> RentIt.Product.ArgumentException </exception>
  let getProductByName (pName:string) : Product list =
    raise (new System.NotImplementedException())

  /// <summary>
  /// Get all products by product type
  /// </summary>
  // <typeparam> Product type name </typeparam>
  /// <returns> List of products </returns>
  /// <exception> RentIt.Product.NoSuchProduct </exception>
  /// <exception> RentIt.Product.ArgumentException </exception>
  let getAllByType (typeName:string) : Product list =
    raise (new System.NotImplementedException())

  /// <summary>
  /// Update existing product
  /// </summary>
  // <typeparam> Product name </typeparam>
  /// <exception> RentIt.Product.NoSuchProduct </exception>
  /// <exception> RentIt.Product.ArgumentException </exception>
  let update (p:Product) =
    raise (new System.NotImplementedException())

  /// <summary>
  /// Buy a product
  /// </summary>
  // <typeparam> Product id </typeparam>
  /// <exception> RentIt.Product.NoSuchProduct </exception>
  /// <exception> RentIt.Product.ArgumentException </exception>
  let buyProduct (pId:string) =
    raise (new System.NotImplementedException())

  /// <summary>
  /// Rent a product
  /// </summary>
  // <typeparam> Product id </typeparam>
  // <typeparam> Number of days </typeparam>
  /// <exception> RentIt.Product.NoSuchProduct </exception>
  /// <exception> RentIt.Product.ArgumentException </exception>
  let rentProduct (pId:string) (days:int) =
    raise (new System.NotImplementedException())
