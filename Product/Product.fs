namespace RentIt

module Product =

  // Should this catch Exception and raise UnkownException?

  type Product = ProductTypes.Product
  type Meta = ProductTypes.Meta

  // Exceptions
  exception NoSuchProduct
  exception NoSuchUser
  exception ProductNotPublished
  exception ArgumentException of string
  exception ProductAlreadyExists
  exception NoSuchProductType
  exception UpdateNotAllowed of string

   /// <summary>
  /// Persist a new product, making the product available for publish
  /// </summary>
  // <typeparam> Product </typeparam>
  /// <exception> RentIt.Product.ProductAlreadyExists </exception>
  /// <exception> RentIt.Product.NoSuchProductType </exception>
  /// <exception> RentIt.Product.NoSuchUser </exception>
  /// <exception> RentIt.Product.ArgumentException </exception>
  let persist (p:Product) = 
    // Defens
    if (p.owner = null || p.owner.Trim().Length = 0) then raise (ArgumentException "UserId empty")
    if (p.name = null || p.name.Trim().Length = 0) then raise (ArgumentException "Name empty")
    if (p.productType = null || p.productType.Trim().Length = 0) then raise (ArgumentException "ProductType empty")
    
    // Persist
    try
      ProductPersistence.createProduct p
    with
      | ProductPersistence.ProductAlreadyExists -> raise ProductAlreadyExists
      | ProductPersistence.NoSuchUser           -> raise NoSuchUser
      | ProductPersistence.NoSuchProductType    -> raise NoSuchProductType

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
    persist {
      name = name;
      createDate = System.DateTime.Now;
      productType = productType;
      owner = userId;
      description = description;
      rentPrice = rentPrice;
      buyPrice = buyPrice;
      id = -1;
      rating = None;
      published = false;
      metadata = None;
      thumbnailPath = None;
    }
  
  /// <summary>
  /// Get prodcut by product id
  /// </summary>
  /// <typeparam> Id </typeparam>
  /// <returns> Product </returns>
  /// <exception> RentIt.Product.NoSuchProduct </exception>
  /// <exception> RentIt.Product.ArgumentException </exception>
  let getProductById (id:string) : Product =
    if (id = null || id.Trim().Length = 0) then raise (ArgumentException "Product id empty")
    try
      ProductPersistence.getProductById id
    with
      | ProductPersistence.NoSuchProduct -> raise NoSuchProduct
    
  /// <summary>
  /// Get products by product name
  /// </summary>
  // <typeparam> Product name </typeparam>
  /// <returns> List of products </returns>
  /// <exception> RentIt.Product.NoSuchProduct </exception>
  /// <exception> RentIt.Product.ArgumentException </exception>
  let getProductByName (pName:string) : Product list =
    if (pName = null || pName.Trim().Length = 0) then raise (ArgumentException "Name empty")

    try
      ProductPersistence.getProductByName pName
    with
      | ProductPersistence.NoSuchProduct -> raise NoSuchProduct

  /// <summary>
  /// Get all products by product type
  /// </summary>
  // <typeparam> Product type name </typeparam>
  /// <returns> List of products </returns>
  /// <exception> RentIt.Product.NoSuchProductType </exception>
  /// <exception> RentIt.Product.ArgumentException </exception>
  let getAllByType (typeName:string) : Product list =
    if (typeName = null || typeName.Trim().Length = 0) then raise (ArgumentException "Product type name empty")

    try
      ProductPersistence.getProductByType typeName
    with
      | ProductPersistence.NoSuchProduct     -> raise NoSuchProduct
      | ProductPersistence.NoSuchProductType -> raise NoSuchProductType

  /// <summary>
  /// Update existing product
  /// </summary>
  // <typeparam> Product name </typeparam>
  /// <exception> RentIt.Product.NoSuchProduct </exception>
  /// <exception> RentIt.Product.UpdateNotAllowed </exception>
  /// <exception> RentIt.Product.ArgumentException </exception>
  let update (p:Product) : Product =
    // Defens
    if (p.owner = null || p.owner.Trim().Length = 0) then raise (ArgumentException "UserId empty")
    if (p.name = null || p.name.Trim().Length = 0) then raise (ArgumentException "Name empty")
    if (p.productType = null || p.productType.Trim().Length = 0) then raise (ArgumentException "ProductType empty")
    
    // Update
    try
      ProductPersistence.updateProduct p
    with
      | ProductPersistence.ProductAlreadyExists -> raise ProductAlreadyExists
      | ProductPersistence.NoSuchUser           -> raise NoSuchUser
      | ProductPersistence.NoSuchProductType    -> raise NoSuchProductType

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

  /// <summay>
  /// Rate Product
  ///</summary>
  /// <typeparam> Product id </typeparam>
  /// <typeparam> Rating </typeparam>
  /// <exception> NoSuchProduct </exception>
  let rateProduct (pId:string) (user:string) (rating:int) = 
    // Defens
    if (pId = null || pId.Trim().Length = 0) then raise (ArgumentException "ProductId empty")
    if (-5 > rating || rating > 5) then raise (ArgumentException "Rating must be between -5 and 5")

    try
      ProductPersistence.rateProduct pId user rating
    with
      | ProductPersistence.NoSuchProduct -> raise NoSuchProduct

  /// <summay>
  /// Change Published-flag on Product
  ///</summary>
  /// <typeparam> Product id </typeparam>
  /// <typeparam> Boolean </typeparam>
  /// <exception> NoSuchProduct </exception>
  let publishProduct (pId:string) (status:bool) =
    if (pId = null || pId.Trim().Length = 0) then raise (ArgumentException "ProductId empty")

    try
      ProductPersistence.publishProduct pId status
    with
      | ProductPersistence.NoSuchProduct -> raise NoSuchProduct