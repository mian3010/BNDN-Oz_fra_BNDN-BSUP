namespace RentIt
open ProductTypes
open ProductExceptions

module Product =

  let internal getLocalProductFile (id:int) owner =
      let path = System.AppDomain.CurrentDomain.BaseDirectory + "\\Uploads\\" + owner
      let dir = new System.IO.DirectoryInfo(path);
      try
        dir.GetFiles(id.ToString() + ".*").[0]
      with
        | :? System.IO.DirectoryNotFoundException -> raise ProductExceptions.NoSuchMedia

  let internal getLocalThumbnailFile (id:int) owner =
      let path = System.AppDomain.CurrentDomain.BaseDirectory + "\\Uploads\\" + owner + "\\Thumbnails"
      let dir = new System.IO.DirectoryInfo(path);
      dir.GetFiles(id.ToString() + ".*").[0]

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
      | ProductAlreadyExists -> raise ProductAlreadyExists
      | NoSuchUser           -> raise NoSuchUser
      | NoSuchProductType    -> raise NoSuchProductType

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
    {
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
    }

  /// <summary>
  /// Get a list of product types 
  /// </summary>
  /// <returns> String list of product types </returns>
  let getListOfProductTypes () : string[] =
    ProductPersistence.getListOfProductTypes ()

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
      | NoSuchProduct -> raise NoSuchProduct
    
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
      | NoSuchProduct -> raise NoSuchProduct

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
      | ProductAlreadyExists -> raise ProductAlreadyExists
      | NoSuchUser           -> raise NoSuchUser
      | NoSuchProductType    -> raise NoSuchProductType

  /// <summary>
  /// Rate Product
  ///</summary>
  /// <typeparam> Product id </typeparam>
  /// <typeparam> Rating </typeparam>
  /// <exception> NoSuchProduct </exception>
  let rateProduct (pId:int) (user:string) (rating:int) = 
    // Defens
    if (pId < 1) then raise (ArgumentException "ProductId invalid")
    if (user = null || user.Trim().Length = 0) then raise (ArgumentException "User invalid")
    if (-5 > rating || rating > 5) then raise (ArgumentException "Rating must be between -5 and 5")

    try
      ProductPersistence.rateProduct pId user rating
    with
      | NoSuchProduct -> raise NoSuchProduct

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
      | NoSuchProduct -> raise NoSuchProduct

  /// <summary>
  /// Returns all MIME types supported for a given product type
  ///</summary>
  /// <typeparam> product type </typeparam>
  let getMimesForProductType (productType:string) : string list = 
    ProductPersistence.getMimeTypesForProductType productType

  /// <summary>
  /// Persist a new media
  ///</summary>
  let persistMedia (id:uint32) (mime:string) (stream:System.IO.Stream) =
    let p = getProductById (int id)
    let fileName = p.id.ToString() + "." + mime.Replace(@"/", "_");
    let filePath = System.AppDomain.CurrentDomain.BaseDirectory + "\\Uploads\\" + p.owner + "\\" + fileName

    let fs = new System.IO.FileStream(filePath, System.IO.FileMode.Create);
    stream.CopyTo(fs);
    stream.Close();
    ()

  /// <summary>
  /// Persist a new media thumbnail
  ///</summary>
  let persistMediaThumbnail (id:uint32) (mime:string) (stream:System.IO.Stream) =
    let p = getProductById (int id)
    let fileType = mime.Substring(mime.IndexOf(@"/") + 1).ToLower();
    let allowedTypes = Set.ofList ["jpeg"; "jpg"; "gif"; "png"]
    if not (allowedTypes.Contains fileType) then raise ProductExceptions.MimeTypeNotAllowed
    
    let fileName = p.id.ToString() + "." + mime.Replace(@"/", "_");
    let filePath = System.AppDomain.CurrentDomain.BaseDirectory + "\\Uploads\\" + p.owner + "\\Thumbnails\\" + fileName

    let fs = new System.IO.FileStream(filePath, System.IO.FileMode.Create);
    stream.CopyTo(fs);
    stream.Close();
    ()

  /// <summary>
  /// Gets a stream to the requeste media and the media MIME type
  ///</summary>
  /// <exception> MediaNotFound </exception>
  let getMedia (id:uint32) =
    let product = getProductById (int id)
    let info = getLocalProductFile (int id) product.owner
    try
      System.IO.File.OpenRead(info.FullName), info.Extension.Substring(1)
    with
      | :? System.IO.FileNotFoundException -> raise ProductExceptions.NoSuchMedia

  /// <summary>
  /// Gets a stream to the requeste media thumbnail and the media MIME type
  ///</summary>
  /// <exception> MediaNotFound </exception>
  let getMediaThumbnail (id:uint32) = 
    let product = getProductById (int id)
    let info = getLocalThumbnailFile (int id) product.owner
    try
      System.IO.File.OpenRead(info.FullName), info.Extension.Substring(1)
    with
      | :? System.IO.FileNotFoundException -> raise ProductExceptions.NoSuchMedia

  let searchProducts search =
    ProductPersistence.searchProducts search

  let getAllProducts (showPublished:PublishedStatus) =
    ProductPersistence.getAllProducts showPublished

  let getAllProductsByUser (userName:string) (showPublished:PublishedStatus) =
    ProductPersistence.getAllProductsByUser userName showPublished

  let getAllProductsByType (pType:string) (showPublished:PublishedStatus) =
    ProductPersistence.getAllProductsByType pType showPublished

  let getAllProductsByUserAndTitle (userName:string) (title:string) (showPublished:PublishedStatus) =
    ProductPersistence.getAllProductsByUserAndTitle userName title showPublished

  /// <summary>
  /// Checks if a media file is avalible for the given product
  ///</summary>
  let hasMedia (id:uint32) =
    try
      match getMedia id with
      | (s, _) -> s.Close(); true
    with
      | ProductExceptions.NoSuchMedia -> false

  /// <summary>
  /// Checks if a thumbnail file is avalible for the given product
  ///</summary>
  let hasThumbnail (id:uint32) =
    try
      match getMediaThumbnail id with
      | (s, _) -> s.Close(); true
    with
      | ProductExceptions.NoSuchMedia -> false