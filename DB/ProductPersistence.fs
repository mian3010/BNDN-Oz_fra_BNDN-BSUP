namespace RentIt

module ProductPersistence =

  type Product = ProductTypes.Product
  
  exception NoSuchProduct
  exception NoSuchProductType
  exception NoSuchUser
  exception ProductNotPublished
  exception ProductAlreadyExists

  let internal convertDatetimeToString (date:System.DateTime) =
    (string date.Year)+"-"+(string date.Month)+"-"+(string date.Day)

  let internal convertToDataIn objectName (prod:Product) =
    let dataQ = ref (Persistence.DataIn.createDataIn [] objectName "Name" prod.name)
    dataQ := Persistence.DataIn.createDataIn !dataQ objectName "Created_date" (convertDatetimeToString prod.createDate)
    dataQ := Persistence.DataIn.createDataIn !dataQ objectName "ProductType_Name" prod.productType
    dataQ := Persistence.DataIn.createDataIn !dataQ objectName "User_Username" prod.owner
    dataQ := Persistence.DataIn.createDataIn !dataQ objectName "Published" (string prod.published)
    if (prod.description.IsSome) then dataQ := Persistence.DataIn.createDataIn !dataQ objectName "Description" prod.description.Value
    if (prod.rentPrice.IsSome) then dataQ := Persistence.DataIn.createDataIn !dataQ objectName "Rent_price" (string prod.rentPrice.Value)
    if (prod.buyPrice.IsSome) then dataQ := Persistence.DataIn.createDataIn !dataQ objectName "Buy_price" (string prod.buyPrice.Value)
    dataQ

  let internal convertMetadataFromResult (result:Map<string,string> List) =
    let metadataMap:Map<string,ProductTypes.Meta> = Map.empty
    for row in result do
      metadataMap.Add 

  let internal getMetaData productId =
    let objectName = "MetaData"
    let fieldsQ = ref (Persistence.ReadField.createReadField [] objectName "Content")
    fieldsQ := Persistence.ReadField.createReadField !fieldsQ objectName "MetaDataType_Name"
    let joinsQ = []
    let filtersQ = Persistence.Filter.createFilter [] objectName "Product_Id" "=" (string productId)
    let readR = Persistence.Api.read !fieldsQ objectName joinsQ filtersQ
    convertMetadataFromResult readR

  let internal convertFromResult (result:Map<string,string> List) =
    
  
  /// <summay>
  /// Creater in persistence layer
  ///</summary>
  /// <typeparam> Product </typeparam>
  /// <returns> Product </returns>
  /// <exception> ProductAlreadyExists </exception>
  /// <exception> NoSuchUser </exception>
  /// <exception> NoSuchProductType </exception>
  let createProduct (p:Product) : Product =
    let objectName = "Product"
    let dataQ = convertToDataIn objectName p
    let updateR = Persistence.Api.create objectName dataQ
    convertFromResult updateR
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
  