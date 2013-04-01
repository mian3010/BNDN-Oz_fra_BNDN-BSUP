namespace RentIt

module ProductPersistence =

  type Product = ProductTypes.Product
  
  exception NoSuchProduct
  exception NoSuchProductType
  exception NoSuchUser
  exception ProductNotPublished
  exception ProductAlreadyExists
  
  /// <summay>
  /// Creater in persistence layer
  ///</summary>
  /// <typeparam> Product </typeparam>
  /// <returns> Product </returns>
  /// <exception> ProductAlreadyExists </exception>
  /// <exception> NoSuchUser </exception>
  /// <exception> NoSuchProductType </exception>
  val createProduct : Product -> Product

  /// <summay>
  /// Update an exsisting product
  ///</summary>
  /// <typeparam> Product </typeparam>
  /// <returns> Product </returns>
  /// <exception> NoSuchProduct </exception>
  /// <exception> NoSuchUser </exception>
  /// <exception> NoSuchProductType </exception>
  val updateProduct : Product -> Product

  /// <summay>
  /// Get Product by its id
  ///</summary>
  /// <typeparam> Product id </typeparam>
  /// <returns> Product </returns>
  /// <exception> NoSuchProduct </exception>
  val getProductById : string -> Product

  /// <summay>
  /// Get Product by its name and create data
  /// Usefull when you don't know Product id
  ///</summary>
  /// <typeparam> Prodcut name </typeparam>
  /// <typeparam> Prodcut create date </typeparam>
  /// <returns> Product </returns>
  /// <exception> NoSuchProduct </exception>
  val getProductByNameAndCreateData : string -> string -> Product

  /// <summay>
  /// Get a list of Products by Product name
  ///</summary>
  /// <typeparam> Product name </typeparam>
  /// <returns> Product list </returns>
  /// <exception> NoSuchProduct </exception>
  val getProductByName : string -> Product list

  /// <summay>
  /// Get a list of Products by Product type
  ///</summary>
  /// <typeparam> Product type </typeparam>
  /// <returns> Product list </returns>
  /// <exception> NoSuchProduct </exception>
  /// <exception> NoSuchProductType </exception>
  val getProductByType : string -> Product list

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