namespace RentIt

open System
open ProductPersistenceHelper

module ProductPersistence =
  open ProductTypes
  open ProductExceptions
    
  let objectName = "Product"
  /// <summary>
  /// Creater in persistence layer
  /// </summary>
  /// <typeparam> Product </typeparam>
  /// <returns> Product </returns>
  /// <exception> ProductAlreadyExists </exception>
  /// <exception> NoSuchUser </exception>
  /// <exception> NoSuchProductType </exception>
  let createProduct (p:Product) : Product =
    if (p.id > 0) then raise ProductAlreadyExists
    let reader = Persistence.Api.create "Loggable" []
    let dataQ = ref (convertToDataIn objectName p)
    dataQ := Persistence.DataIn.createDataIn !dataQ objectName "Id" ((reader.Item 0).Item "Id")
    try 
      let createR = Persistence.Api.create objectName !dataQ
      convertFromResult createR.Head
    with
      | PersistenceExceptions.ReferenceDoesNotExist -> 
        try
          let user = AccountPersistence.getUserByName p.owner
          raise NoSuchProductType
        with 
          | AccountExceptions.NoSuchUser|AccountPersistenceExceptions.NoUserWithSuchName -> raise NoSuchUser
          | _ as e -> raise e

  /// <summary>
  /// Get Product by its id
  /// </summary>
  /// <typeparam> Product id </typeparam>
  /// <returns> Product </returns>
  /// <exception> NoSuchProduct </exception>
  let getProductById (id:int) : Product =
    let fieldsQ = Persistence.ReadField.createReadFieldProc [] objectName "" Persistence.ReadField.All
    let joinsQ = [];
    let filtersQ = Persistence.FilterGroup.createSingleFilterGroup [] objectName "Id" (string id)
    let readR = Persistence.Api.read fieldsQ objectName joinsQ filtersQ
    if (readR.Length < 1) then raise NoSuchProduct
    convertFromResult readR.Head

  /// <summary>
  /// Update an existing product
  /// </summary>
  /// <typeparam> Product </typeparam>
  /// <returns> Product </returns>
  /// <exception> NoSuchProduct </exception>
  /// <exception> NoSuchUser </exception>
  /// <exception> NoSuchProductType </exception>
  let updateProduct (p:Product) : Product =
    let dataQ = convertToDataIn objectName p
    let filtersQ = Persistence.FilterGroup.createSingleFilterGroup [] objectName "Id" (string p.id)
    try
      let updateR = Persistence.Api.update objectName filtersQ dataQ
      if (updateR.Length < 1) then raise NoSuchProduct 
      else convertFromResult updateR.Head
    with
      | PersistenceExceptions.ReferenceDoesNotExist -> 
        try
          let user = AccountPersistence.getUserByName p.owner
          let prod = getProductById p.id
          raise NoSuchProductType
        with 
          | AccountExceptions.NoSuchUser|AccountPersistenceExceptions.NoUserWithSuchName -> raise NoSuchUser
          | _ as e -> raise e
      | _ as e -> raise e

  /// <summary>
  /// Get a list of Products by Product name
  /// </summary>
  /// <typeparam> Product name </typeparam>
  /// <returns> Product list </returns>
  /// <exception> NoSuchProduct </exception>
  let getProductByName (name:string) : Product list =
    let fieldsQ = Persistence.ReadField.createReadFieldProc [] objectName "" Persistence.ReadField.All
    let joinsQ = [];
    let filtersQ = Persistence.FilterGroup.createSingleFilterGroup [] objectName "Name" name
    let readR = Persistence.Api.read fieldsQ objectName joinsQ filtersQ
    if (readR.Length < 1) then raise NoSuchProduct
    convertFromResults readR

  /// <summary>
  /// Get a list of Products by Product type
  /// </summary>
  /// <typeparam> Product type </typeparam>
  /// <returns> Product list </returns>
  /// <exception> NoSuchProduct </exception>
  /// <exception> NoSuchProductType </exception>
  let getProductByType (pType:string) : Product list =
    let prodType = getProductType pType
    let fieldsQ = Persistence.ReadField.createReadFieldProc [] objectName "" Persistence.ReadField.All
    let joinsQ = [];
    let filtersQ = Persistence.FilterGroup.createSingleFilterGroup [] objectName "ProductType_Name" pType
    let readR = Persistence.Api.read fieldsQ objectName joinsQ filtersQ
    convertFromResults readR

  /// <summary>
  /// Rate Product
  /// </summary>
  /// <typeparam> Product id </typeparam>
  /// <typeparam> Rating </typeparam>
  /// <exception> NoSuchProduct </exception>
  /// <exception> NoSuchUser </exception>
  let rateProduct (pId:int) (user:string) (rating:int) = 
    let objectName = "ProductRating"
    let fieldsQ = Persistence.ReadField.createReadField [] objectName "Rating"
    let joinsQ = [];
    let filtersQ = ref (Persistence.FilterGroup.createSingleFilterGroup [] objectName "User_Username" user)
    filtersQ := Persistence.FilterGroup.createSingleFilterGroup (!filtersQ).Head.filters objectName "Product_Id" (string pId)
    let readR = Persistence.Api.read fieldsQ objectName joinsQ !filtersQ
    
    let dataQ = ref (Persistence.DataIn.createDataIn [] objectName "Rating" (string rating))
    try
      let rateR = if (readR.Length.Equals 0) then
                    dataQ := Persistence.DataIn.createDataIn !dataQ objectName "Product_Id" (string pId)
                    dataQ := Persistence.DataIn.createDataIn !dataQ objectName "User_Username" user
                    Persistence.Api.create objectName !dataQ
                  else
                    Persistence.Api.update objectName !filtersQ !dataQ

      getProductById pId
    with
      | PersistenceExceptions.ReferenceDoesNotExist -> 
        try
          let user = AccountPersistence.getUserByName user
          raise NoSuchProduct
        with 
          | AccountExceptions.NoSuchUser|AccountPersistenceExceptions.NoUserWithSuchName -> raise NoSuchUser
          | _ as e -> raise e
    

  /// <summary>
  /// Change Published-flag on Product
  /// </summary>
  /// <typeparam> Product id </typeparam>
  /// <typeparam> Boolean </typeparam>
  /// <exception> NoSuchProduct </exception>
  let publishProduct (pId:int) (status:bool) =
    let filtersQ = Persistence.FilterGroup.createSingleFilterGroup [] objectName "Id" (string pId)
    let dataQ = Persistence.DataIn.createDataIn [] objectName "Published" (string status)
    try
      let publishR = Persistence.Api.update objectName filtersQ dataQ
      if (publishR.Length < 1) then raise NoSuchProduct
      else convertFromResult publishR.Head
    with
      | PersistenceExceptions.ReferenceDoesNotExist -> raise NoSuchProduct
      | _ as e -> raise e

  /// <summary>
  /// Create a product type
  /// </summary>
  /// <typeparam> Name of the product  type <//typeparam>
  /// <returns> Bool depending on success </returns>
  let createProductType typeName = 
    let objectName = "ProductType"
    let dataQ = Persistence.DataIn.createDataIn [] objectName "Name" typeName
    let typeR = Persistence.Api.create objectName dataQ
    if (typeR.Length > 0) then true
    else false

  /// <summary>
  /// Get a list of product types 
  /// </summary>
  /// <returns> String list of product types </returns>
  let getListOfProductTypes () =
    let fieldsQ = Persistence.ReadField.createReadFieldProc [] "" "" Persistence.ReadField.All
    let v = Persistence.Api.read fieldsQ "ProductType" [] []
    let mutable l:string list  = []
    for c in v do
      l <- l@[c.["Name"]]
    List.toArray l

  let searchProducts search =
    let objectName = "Product"
    let fieldsQ = Persistence.ReadField.createReadFieldProc [] "" "" Persistence.ReadField.All
    let joinsQ = Persistence.ObjectJoin.createObjectJoinProc [] "Product" "Id" "MetaData" "Product_Id" Persistence.ObjectJoin.leftJoin
    let filtersQ = ref (Persistence.Filter.createFilterProc [] objectName "Name" search Persistence.Filter.anyBeforeAndAfter)
    filtersQ := Persistence.Filter.createFilterProc !filtersQ objectName "Description" search Persistence.Filter.anyBeforeAndAfter
    filtersQ := Persistence.Filter.createFilterProc !filtersQ "MetaData" "Content" search Persistence.Filter.anyBeforeAndAfter
    let filterGroup = Persistence.FilterGroup.createFilterGroupFiltersProc [] !filtersQ Persistence.FilterGroup.orCondition
    convertFromResults (Persistence.Api.read fieldsQ objectName joinsQ filterGroup)

  let getAllProducts (showPublished:PublishedStatus) =
    let objectName = "Product"
    let filtersQ = getPublishedFilter showPublished
    let fieldsQ = Persistence.ReadField.createReadFieldProc [] "" "" Persistence.ReadField.All
    convertFromResults (Persistence.Api.read fieldsQ objectName [] filtersQ)

  let getAllProductsByUser (userName:string) (showPublished:PublishedStatus) =
    let objectName = "Product"
    let filtersQ = ref (getPublishedFilter showPublished)
    filtersQ := Persistence.FilterGroup.createSingleFilterGroup (!filtersQ).Head.filters objectName "User_Username" userName
    let fieldsQ = Persistence.ReadField.createReadFieldProc [] "" "" Persistence.ReadField.All
    convertFromResults (Persistence.Api.read fieldsQ objectName [] !filtersQ)
  
  let getAllProductsByType (pType:string) (showPublished:PublishedStatus) =
    let objectName = "Product"
    let filtersQ = ref (getPublishedFilter showPublished)
    filtersQ := Persistence.FilterGroup.createSingleFilterGroup (!filtersQ).Head.filters objectName "ProductType_Name" pType
    let fieldsQ = Persistence.ReadField.createReadFieldProc [] "" "" Persistence.ReadField.All
    convertFromResults (Persistence.Api.read fieldsQ objectName [] !filtersQ)

  let getAllProductsByUserAndName (userName:string) (name:string) (showPublished:PublishedStatus) =
    let objectName = "Product"
    let filtersQ = ref (getPublishedFilter showPublished)
    filtersQ := Persistence.FilterGroup.createSingleFilterGroup (!filtersQ).Head.filters objectName "User_Username" userName
    filtersQ := Persistence.FilterGroup.createSingleFilterGroup (!filtersQ).Head.filters objectName "Name" name
    let fieldsQ = Persistence.ReadField.createReadFieldProc [] "" "" Persistence.ReadField.All
    convertFromResults (Persistence.Api.read fieldsQ objectName [] !filtersQ)