﻿namespace RentIt

module Product =

  type Product = ProductTypes.Product

  exception NoSuchProduct
  exception NoSuchUser
  exception ProductNotPublished
  exception ArgumentException of string
  exception ProductAlreadyExists
  exception NoSuchProductType
  exception UpdateNotAllowed of string

  /// <summay>
  /// Creater
  ///</summary>
  /// <typeparam> UserId </typeparam>
  /// <typeparam> Name </typeparam>
  /// <typeparam> ProductType </typeparam>
  /// <typeparam> Description (optional) </typeparam>
  /// <typeparam> BuyPrice (optional) </typeparam>
  /// <typeparam> RentPrice (optional) </typeparam>
  /// <returns> Product </returns>
  /// <exception> RentIt.Product.NoSuchUser </exception>
  /// <exception> RentIt.Product.ArgumentException </exception>
  val make : string -> string -> string -> string option -> int option -> int option -> Product 
    
  /// <summary>
  /// Get prodcut by product id
  /// </summary>
  /// <typeparam> Id </typeparam>
  /// <returns> Product </returns>
  /// <exception> RentIt.Product.NoSuchProduct </exception>
  /// <exception> RentIt.Product.ArgumentException </exception>
  val getProductById : string -> Product 

  /// <summary>
  /// Persist a new product, making the product available for publish
  /// </summary>
  // <typeparam> Product </typeparam>
  /// <exception> RentIt.Product.ProductAlreadyExists </exception>
  /// <exception> RentIt.Product.NoSuchProductType </exception>
  /// <exception> RentIt.Product.NoSuchUser </exception>
  /// <exception> RentIt.Product.ArgumentException </exception>
  val persist : Product -> Product

  /// <summary>
  /// Get products by product name
  /// </summary>
  // <typeparam> Product name </typeparam>
  /// <returns> List of products </returns>
  /// <exception> RentIt.Product.NoSuchProduct </exception>
  /// <exception> RentIt.Product.ArgumentException </exception>
  val getProductByName : string -> Product list 

  /// <summary>
  /// Get all products by product type
  /// </summary>
  // <typeparam> Product type name </typeparam>
  /// <returns> List of products </returns>
  /// <exception> RentIt.Product.NoSuchProduct </exception>
  /// <exception> RentIt.Product.ArgumentException </exception>
  val getAllByType : string -> Product list 

  /// <summary>
  /// Update existing product
  /// </summary>
  // <typeparam> Product name </typeparam>
  /// <exception> RentIt.Product.NoSuchProduct </exception>
  /// <exception> RentIt.Product.ArgumentException </exception>
  val update : Product -> Product

  /// <summary>
  /// Buy a product
  /// </summary>
  // <typeparam> Product id </typeparam>
  /// <exception> RentIt.Product.NoSuchProduct </exception>
  /// <exception> RentIt.Product.ArgumentException </exception>
  val buyProduct : string -> 'a

  /// <summary>
  /// Rent a product
  /// </summary>
  // <typeparam> Product id </typeparam>
  // <typeparam> Number of days </typeparam>
  /// <exception> RentIt.Product.NoSuchProduct </exception>
  /// <exception> RentIt.Product.ArgumentException </exception>
  val rentProduct : string -> int -> 'a

  /// <summay>
  /// Rate Product
  ///</summary>
  /// <typeparam> Product id </typeparam>
  /// <typeparam> Rating </typeparam>
  /// <exception> NoSuchProduct </exception>
  val rateProduct : string -> int -> 'a

 /// <summay>
  /// Change Published-flag on Product
  ///</summary>
  /// <typeparam> Product id </typeparam>
  /// <typeparam> Boolean </typeparam>
  /// <exception> NoSuchProduct </exception>
  val publishProduct : string -> bool -> 'a