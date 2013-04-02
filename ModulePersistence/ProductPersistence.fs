namespace RentIt

open System
open ProductPersistenceHelper

module ProductPersistence =

  type Product = ProductTypes.Product
  
  exception NoSuchProduct
  exception NoSuchProductType
  exception NoSuchUser
  exception ProductNotPublished
  exception ProductAlreadyExists
    
  let objectName = "Product"
  /// <summay>
  /// Creater in persistence layer
  /// </summary>
  /// <typeparam> Product </typeparam>
  /// <returns> Product </returns>
  /// <exception> ProductAlreadyExists </exception>
  /// <exception> NoSuchUser </exception>
  /// <exception> NoSuchProductType </exception>
  let createProduct (p:Product) : Product =
    let reader = Persistence.Api.create "Loggable" []
    let dataQ = ref (convertToDataIn objectName p)
    dataQ := Persistence.DataIn.createDataIn !dataQ objectName "Id" ((reader.Item 0).Item "Id")
    let createR = Persistence.Api.create objectName !dataQ
    convertFromResult createR.Head

  /// <summay>
  /// Update an existing product
  /// </summary>
  /// <typeparam> Product </typeparam>
  /// <returns> Product </returns>
  /// <exception> NoSuchProduct </exception>
  /// <exception> NoSuchUser </exception>
  /// <exception> NoSuchProductType </exception>
  let updateProduct (p:Product) : Product =
    let dataQ = convertToDataIn objectName p
    let filtersQ = Persistence.Filter.createFilter [] objectName "Id" "=" (string p.id)
    let updateR = Persistence.Api.update objectName filtersQ dataQ
    convertFromResult updateR.Head

  /// <summay>
  /// Get Product by its id
  /// </summary>
  /// <typeparam> Product id </typeparam>
  /// <returns> Product </returns>
  /// <exception> NoSuchProduct </exception>
  let getProductById (id:int) : Product =
    let fieldsQ = Persistence.ReadField.createReadFieldProc [] objectName "" Persistence.ReadField.All
    let joinsQ = [];
    let filtersQ = Persistence.Filter.createFilter [] objectName "Id" "=" (string id)
    let readR = Persistence.Api.read fieldsQ objectName joinsQ filtersQ
    convertFromResult readR.Head

  /// <summay>
  /// Get a list of Products by Product name
  /// </summary>
  /// <typeparam> Product name </typeparam>
  /// <returns> Product list </returns>
  /// <exception> NoSuchProduct </exception>
  let getProductByName (name:string) : Product list =
    let fieldsQ = Persistence.ReadField.createReadFieldProc [] objectName "" Persistence.ReadField.All
    let joinsQ = [];
    let filtersQ = Persistence.Filter.createFilter [] objectName "Name" "=" name
    let readR = Persistence.Api.read fieldsQ objectName joinsQ filtersQ
    convertFromResults readR

  /// <summay>
  /// Get a list of Products by Product type
  /// </summary>
  /// <typeparam> Product type </typeparam>
  /// <returns> Product list </returns>
  /// <exception> NoSuchProduct </exception>
  /// <exception> NoSuchProductType </exception>
  let getProductByType (pType:string) : Product list =
    let fieldsQ = Persistence.ReadField.createReadFieldProc [] objectName "" Persistence.ReadField.All
    let joinsQ = [];
    let filtersQ = Persistence.Filter.createFilter [] objectName "ProductType_Name" "=" pType
    let readR = Persistence.Api.read fieldsQ objectName joinsQ filtersQ
    convertFromResults readR

  /// <summay>
  /// Rate Product
  /// </summary>
  /// <typeparam> Product id </typeparam>
  /// <typeparam> Rating </typeparam>
  /// <exception> NoSuchProduct </exception>
  let rateProduct (pId:int) (user:string) (rating:int) = 
    let objectName = "ProductRating"
    let fieldsQ = Persistence.ReadField.createReadField [] objectName "Rating"
    let joinsQ = [];
    let filtersQ = ref (Persistence.Filter.createFilter [] objectName "User_Username" "=" user)
    filtersQ := Persistence.Filter.createFilter !filtersQ objectName "Product_Id" "=" (string pId)
    let readR = Persistence.Api.read fieldsQ objectName joinsQ !filtersQ
    
    let dataQ = ref (Persistence.DataIn.createDataIn [] objectName "Rating" (string rating))
    let rateR = if (readR.Length.Equals 0) then
                  dataQ := Persistence.DataIn.createDataIn !dataQ objectName "Product_Id" (string pId)
                  dataQ := Persistence.DataIn.createDataIn !dataQ objectName "User_Username" user
                  Persistence.Api.create objectName !dataQ
                else
                  Persistence.Api.update objectName !filtersQ !dataQ

    getProductById pId
    

  /// <summay>
  /// Change Published-flag on Product
  /// </summary>
  /// <typeparam> Product id </typeparam>
  /// <typeparam> Boolean </typeparam>
  /// <exception> NoSuchProduct </exception>
  let publishProduct (pId:int) (status:bool) =
    let filtersQ = Persistence.Filter.createFilter [] objectName "Id" "=" (string pId)
    let dataQ = Persistence.DataIn.createDataIn [] objectName "Published" (string status)
    let publishR = Persistence.Api.update objectName filtersQ dataQ
    convertFromResult publishR.Head

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