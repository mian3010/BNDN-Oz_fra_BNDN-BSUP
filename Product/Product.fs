namespace RentIt

module ProductTypes = 

  type Product = {
                    name : string;
                    createDate : System.DateTime;
                    productType : string;
                    owner : string;
                    description : string option;
                    rentPrice : int option;
                    buyPrice : int option;
                 }

module Product =

  type Product = ProductTypes.Product

  // Exceptions
  exception NoSuchProduct
  exception NoSuchUser
  exception ProductNotPublished
  exception ArgumentException of string
  exception ProductAlreadyExists
  exception UnknownProductType

  /// <summay>
  /// Creater
  ///</summary>
  /// <typeparam> UserId </typeparam>
  /// <typeparam> Name </typeparam>
  /// <typeparam> Description </typeparam>
  /// <typeparam> ProductType </typeparam>
  /// <typeparam> BuyPrice </typeparam>
  /// <typeparam> RentPrice </typeparam>
  /// <exception> RentIt.Product.NoSuchUser </exception>
  /// <exception> RentIt.Product.ArgumentException </exception>
  let make : string -> string -> string -> string -> string -> string : Product =
    raise (new System.NotImplementedException())
    
  /// <summary>
  /// Get prodcut by product id
  /// </summary>
  /// <typeparam> Id </typeparam>
  /// <returns> Product </returns>
  /// <exception> RentIt.Product.NoSuchProduct </exception>
  /// <exception> RentIt.Product.ArgumentException </exception>
  let getProductById : string -> Product =
    raise (new System.NotImplementedException())

  /// <summary>
  /// Persist a new product, making the product available for publish
  /// </summary>
  // <typeparam> Product </typeparam>
  /// <exception> RentIt.Product.ProductAlreadyExists </exception>
  /// <exception> RentIt.Product.UnknownProductType </exception>
  /// <exception> RentIt.Product.NoSuchUser </exception>
  /// <exception> RentIt.Product.ArgumentException </exception>
  let persist : Product = 
    raise (new System.NotImplementedException())

  /// <summary>
  /// Get products by product name
  /// </summary>
  // <typeparam> Product name </typeparam>
  /// <returns> List of products </returns>
  /// <exception> RentIt.Product.NoSuchProduct </exception>
  /// <exception> RentIt.Product.ArgumentException </exception>
  let getProductByName : string -> Product list =
    raise (new System.NotImplementedException())

  /// <summary>
  /// Get all products by product type
  /// </summary>
  // <typeparam> Product type name </typeparam>
  /// <returns> List of products </returns>
  /// <exception> RentIt.Product.NoSuchProduct </exception>
  /// <exception> RentIt.Product.ArgumentException </exception>
  let getAllByType : string -> Product list =
    raise (new System.NotImplementedException())

  /// <summary>
  /// Update existing product
  /// </summary>
  // <typeparam> Product name </typeparam>
  /// <exception> RentIt.Product.NoSuchProduct </exception>
  /// <exception> RentIt.Product.ArgumentException </exception>
  let update : Product =
    raise (new System.NotImplementedException())

  /// <summary>
  /// Buy a product
  /// </summary>
  // <typeparam> Product id </typeparam>
  /// <exception> RentIt.Product.NoSuchProduct </exception>
  /// <exception> RentIt.Product.ArgumentException </exception>
  let buyProduct : string =
    raise (new System.NotImplementedException())

  /// <summary>
  /// Rent a product
  /// </summary>
  // <typeparam> Product id </typeparam>
  // <typeparam> Number of days </typeparam>
  /// <exception> RentIt.Product.NoSuchProduct </exception>
  /// <exception> RentIt.Product.ArgumentException </exception>
  let rentProduct : string -> int =
    raise (new System.NotImplementedException())
