namespace RentIt
open ProductTypes
open System
    module ProductPersistenceHelper =

      //Convert a product to a list of datain types for the persistence layer
      let internal convertToDataIn objectName (prod:Product) =
        let dataQ = ref (Persistence.DataIn.createDataIn [] objectName "Name" prod.name)
        dataQ := Persistence.DataIn.createDataIn !dataQ objectName "Created_date" (Persistence.Helper.convertDatetimeToString prod.createDate)
        dataQ := Persistence.DataIn.createDataIn !dataQ objectName "ProductType_Name" prod.productType
        dataQ := Persistence.DataIn.createDataIn !dataQ objectName "User_Username" prod.owner
        dataQ := Persistence.DataIn.createDataIn !dataQ objectName "Published" (string prod.published)
        if (prod.description.IsSome) then dataQ := Persistence.DataIn.createDataIn !dataQ objectName "Description" prod.description.Value
        else dataQ := Persistence.DataIn.createDataInProc !dataQ objectName "Description" "" Persistence.DataIn.Null
        if (prod.rentPrice.IsSome) then dataQ := Persistence.DataIn.createDataIn !dataQ objectName "Rent_price" (string prod.rentPrice.Value)
        else dataQ := Persistence.DataIn.createDataInProc !dataQ objectName "Rent_price" "" Persistence.DataIn.Null
        if (prod.buyPrice.IsSome) then dataQ := Persistence.DataIn.createDataIn !dataQ objectName "Buy_price" (string prod.buyPrice.Value)
        else dataQ := Persistence.DataIn.createDataInProc !dataQ objectName "Buy_price" "" Persistence.DataIn.Null
        !dataQ

      //Convert metadata from persistence result to metadata type in option format
      let internal convertMetadataFromResult (result:Map<string,string> List) =
        let metadataMap:Map<string,ProductTypes.Meta> ref = ref (Map.empty)
        for row in result do
          metadataMap := (!metadataMap).Add (row.["MetaDataType_Name"],{
            key = row.["MetaDataType_Name"];
            value = row.["Content"]; 
          })
        if ((!metadataMap).Count > 0) then Some !metadataMap else None

      //Get metadata from a product id from persistence layer
      let internal getMetaData productId =
        let objectName = "MetaData"
        let fieldsQ = ref (Persistence.ReadField.createReadField [] objectName "Content")
        fieldsQ := Persistence.ReadField.createReadField !fieldsQ objectName "MetaDataType_Name"
        let joinsQ = []
        let filtersQ = Persistence.FilterGroup.createSingleFilterGroup [] objectName "Product_Id" (string productId)
        let readR = Persistence.Api.read !fieldsQ objectName joinsQ filtersQ
        convertMetadataFromResult readR

      //Get the rating from a product id from persistence layer
      let internal getRating (productId:int) :ProductTypes.Rating option =
        let objectName = "ProductRating"
        let fieldsQ = ref (Persistence.ReadField.createReadFieldProc [] objectName "Rating" Persistence.ReadField.Num)
        fieldsQ := Persistence.ReadField.createReadFieldProc !fieldsQ objectName "Rating" Persistence.ReadField.Avg
        let filtersQ = Persistence.FilterGroup.createSingleFilterGroup [] objectName "Product_Id" (string productId)
        let ratingR = Persistence.Api.read !fieldsQ objectName [] filtersQ
        if (ratingR.Length.Equals 1) && not (ratingR.Head.[objectName+"_Rating_Avg"].Equals "") && not (ratingR.Head.[objectName+"_Rating_Num"].Equals "") then
          Some {
            rating = (int ratingR.Head.[objectName+"_Rating_Avg"]);
            votes = (int ratingR.Head.[objectName+"_Rating_Num"]);
          }
        else None

      //Get product type from string
      let internal getProductType pType =
        let objectName = "ProductType"
        let fieldsQ = Persistence.ReadField.createReadFieldProc [] "" "" Persistence.ReadField.All
        let filtersQ = Persistence.FilterGroup.createSingleFilterGroup [] objectName "Name" pType
        let prodType = Persistence.Api.read fieldsQ objectName [] filtersQ
        if (prodType.Length < 1) then raise ProductExceptions.NoSuchProductType
        else prodType.Head

      //Get option from a database format string
      let internal getOptionFromValue value =
        if not (value.Equals "") then Some value else None;

      //Get option from a database format string and convert to int
      let internal getOptionFromIntValue value =
        if not (value.Equals "") then Some (int value) else None;

      //Convert product from persistence layer result to type
      let internal convertFromResult (result:Map<string,string>) =
        let metaData = getMetaData result.["Id"]
        {
          name = result.["Name"];
          createDate = (DateTime.Parse result.["Created_date"]);
          productType = result.["ProductType_Name"];
          owner = result.["User_Username"];
          rating = getRating (int result.["Id"])
          published = (Boolean.Parse result.["Published"]);
          id = (int result.["Id"]);
          metadata = metaData;
          description = getOptionFromValue result.["Description"];
          rentPrice = getOptionFromIntValue result.["Rent_price"];
          buyPrice = getOptionFromIntValue result.["Buy_price"];
        }:ProductTypes.Product

      //Convert entire result set from persistence layer to product type
      let internal convertFromResults (result:Map<string,string> List) =
        let productList:ProductTypes.Product List ref = ref []
        for row in result do
          productList := (!productList)@[(convertFromResult row)]
        !productList