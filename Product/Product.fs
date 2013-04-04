namespace RentIt
open ProductTypes
open ProductExceptions

module Product =

  // Should this catch Exception and raise UnkownException?

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
    if (p.owner = null || p.owner.Trim().Length = 0) then raise (ArgumentException "userName empty")
    if (p.name = null || p.name.Trim().Length = 0) then raise (ArgumentException "Name empty")
    if (p.productType = null || p.productType.Trim().Length = 0) then raise (ArgumentException "ProductType empty")
    
    // Persist
    try
      ProductPersistence.createProduct p
    with
      | ProductPersistence.ProductAlreadyExists -> raise ProductAlreadyExists
      | ProductPersistence.NoSuchUser           -> raise NoSuchUser
      | ProductPersistence.NoSuchProductType    -> raise NoSuchProductType

  /// <summary>
  /// Creater
  ///</summary>
  /// <typeparam> userName </typeparam>
  /// <typeparam> Name </typeparam>
  /// <typeparam> ProductType </typeparam>
  /// <typeparam> Description </typeparam>
  /// <typeparam> BuyPrice </typeparam>
  /// <typeparam> RentPrice </typeparam>
  /// <exception> RentIt.Product.NoSuchUser </exception>
  /// <exception> RentIt.Product.ArgumentException </exception>
  let make (userName:string) (name:string) (productType:string) (description:string option) (buyPrice:int option) (rentPrice:int option) : Product =
    if (userName = null || userName.Trim().Length = 0) then raise (ArgumentException "userName empty")
    if (name = null || name.Trim().Length = 0) then raise (ArgumentException "Name empty")
    if (productType = null || productType.Trim().Length = 0) then raise (ArgumentException "ProductType empty")
    persist {
      name = name;
      createDate = System.DateTime.Now;
      productType = productType;
      owner = userName;
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
  /// Get a list of product types 
  /// </summary>
  /// <returns> String list of product types </returns>
  let getListOfProductTypes () : string[] =
    ProductPersistence.getListOfProductTypes ()

  /// <summary>
  /// Get all products
  /// </summary>
  /// <returns> List of products </returns>
  let getAll () : Product list =
    let types = getListOfProductTypes ()
    let returnList = ref []
    try
      for s in types do
        returnList := returnList.Value @ ProductPersistence.getProductByType s
      returnList.Value
    with
      | ProductPersistence.NoSuchProduct     -> raise NoSuchProduct
      | ProductPersistence.NoSuchProductType -> raise NoSuchProductType
  
  /// <summary>
  /// Get prodcut by product id
  /// </summary>
  /// <typeparam> Id </typeparam>
  /// <returns> Product </returns>
  /// <exception> RentIt.Product.NoSuchProduct </exception>
  /// <exception> RentIt.Product.ArgumentException </exception>
  let getProductById (id:int) : Product =
    if (id < 1) then raise (ArgumentException "Product id invalid")
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
  /// If the type argument is empty, all products will be returned
  /// </summary>
  // <typeparam> Product type name </typeparam>
  /// <returns> List of products </returns>
  /// <exception> RentIt.Product.NoSuchProductType </exception>
  /// <exception> RentIt.Product.ArgumentException </exception>
  let getAllByType (typeName:string) : Product list =
    if (typeName = null) then raise (ArgumentException "Product type name empty")

    try
      if typeName.Trim().Length = 0 then getAll() else ProductPersistence.getProductByType typeName
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
    if (p.owner = null || p.owner.Trim().Length = 0) then raise (ArgumentException "userName empty")
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
  let buyProduct (pId:int) =
    raise (new System.NotImplementedException())

  /// <summary>
  /// Rent a product
  /// </summary>
  // <typeparam> Product id </typeparam>
  // <typeparam> Number of days </typeparam>
  /// <exception> RentIt.Product.NoSuchProduct </exception>
  /// <exception> RentIt.Product.ArgumentException </exception>
  let rentProduct (pId:int) (days:int) =
    raise (new System.NotImplementedException())

  /// <summary>
  /// Rate Product
  ///</summary>
  /// <typeparam> Product id </typeparam>
  /// <typeparam> Rating </typeparam>
  /// <exception> NoSuchProduct </exception>
  let rateProduct (pId:int) (user:string) (rating:int) = 
    // Defens
    if (pId < 1) then raise (ArgumentException "ProductId invalid")
    if (-5 > rating || rating > 5) then raise (ArgumentException "Rating must be between -5 and 5")

    try
      ProductPersistence.rateProduct pId user rating
    with
      | ProductPersistence.NoSuchProduct -> raise NoSuchProduct

  /// <summary>
  /// Change Published-flag on Product
  ///</summary>
  /// <typeparam> Product id </typeparam>
  /// <typeparam> Boolean </typeparam>
  /// <exception> NoSuchProduct </exception>
  let publishProduct (pId:int) (status:bool) =
    if (pId < 1) then raise (ArgumentException "ProductId invalid")

    try
      ProductPersistence.publishProduct pId status
    with
      | ProductPersistence.NoSuchProduct -> raise NoSuchProduct

