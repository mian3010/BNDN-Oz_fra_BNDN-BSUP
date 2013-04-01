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
  let createProduct (p:Product) : Product =
    raise (new System.NotImplementedException())

  /// <summay>
  /// Update an exsisting product
  ///</summary>
  /// <typeparam> Product </typeparam>
  /// <returns> Product </returns>
  /// <exception> NoSuchProduct </exception>
  /// <exception> NoSuchUser </exception>
  /// <exception> NoSuchProductType </exception>
  let updateProduct (p:Product) : Product =
    raise (new System.NotImplementedException())

  /// <summay>
  /// Get Product by its id
  ///</summary>
  /// <typeparam> Product id </typeparam>
  /// <returns> Product </returns>
  /// <exception> NoSuchProduct </exception>
  let getProductById (id:string) : Product =
    raise (new System.NotImplementedException())

  /// <summay>
  /// Get Product by its name and create data
  /// Usefull when you don't know Product id
  ///</summary>
  /// <typeparam> Prodcut name </typeparam>
  /// <typeparam> Prodcut create date </typeparam>
  /// <returns> Product </returns>
  /// <exception> NoSuchProduct </exception>
  let getProductByNameAndCreateData (name:string) (date:string) : Product =
    raise (new System.NotImplementedException())

  /// <summay>
  /// Get a list of Products by Product name
  ///</summary>
  /// <typeparam> Product name </typeparam>
  /// <returns> Product list </returns>
  /// <exception> NoSuchProduct </exception>
  let getProductByName (name:string) : Product list =
    raise (new System.NotImplementedException())

  /// <summay>
  /// Get a list of Products by Product type
  ///</summary>
  /// <typeparam> Product type </typeparam>
  /// <returns> Product list </returns>
  /// <exception> NoSuchProduct </exception>
  /// <exception> NoSuchProductType </exception>
  let getProductByType (pType:string) : Product list =
    raise (new System.NotImplementedException())

  /// <summay>
  /// Rate Product
  ///</summary>
  /// <typeparam> Product id </typeparam>
  /// <typeparam> Rating </typeparam>
  /// <exception> NoSuchProduct </exception>
  let rateProduct (pId:string) (rating:int) = 
    raise (new System.NotImplementedException())

  /// <summay>
  /// Change Published-flag on Product
  ///</summary>
  /// <typeparam> Product id </typeparam>
  /// <typeparam> Boolean </typeparam>
  /// <exception> NoSuchProduct </exception>
  let publishProduct (pId:string) (status:bool) =
    raise (new System.NotImplementedException())