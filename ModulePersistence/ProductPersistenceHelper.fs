namespace RentIt
open ProductTypes
open System
    module ProductPersistenceHelper =

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
        !dataQ

      let internal convertMetadataFromResult (result:Map<string,string> List) =
        let metadataMap:Map<string,ProductTypes.Meta> ref = ref (Map.empty)
        for row in result do
          metadataMap := (!metadataMap).Add (row.["MetaDataType_Name"],{
            key = row.["MetaDataType_Name"];
            value = row.["Content"]; 
          })
        if ((!metadataMap).Count > 0) then Some !metadataMap else None

      let internal getMetaData productId =
        let objectName = "MetaData"
        let fieldsQ = ref (Persistence.ReadField.createReadField [] objectName "Content")
        fieldsQ := Persistence.ReadField.createReadField !fieldsQ objectName "MetaDataType_Name"
        let joinsQ = []
        let filtersQ = Persistence.Filter.createFilter [] objectName "Product_Id" "=" (string productId)
        let readR = Persistence.Api.read !fieldsQ objectName joinsQ filtersQ
        convertMetadataFromResult readR

      let internal getRating (productId:int) :ProductTypes.Rating option =
        let objectName = "ProductRating"
        let fieldsQ = ref (Persistence.ReadField.createReadFieldProc [] objectName "Rating" Persistence.ReadField.Num)
        fieldsQ := Persistence.ReadField.createReadFieldProc !fieldsQ objectName "Rating" Persistence.ReadField.Avg
        let filtersQ = Persistence.Filter.createFilter [] objectName "Product_Id" "=" (string productId)
        let ratingR = Persistence.Api.read !fieldsQ objectName [] filtersQ
        if (ratingR.Length.Equals 1) then
          Some {
            rating = (int ratingR.Head.[objectName+"_Rating_Avg"]);
            votes = (int ratingR.Head.[objectName+"_Rating_Num"]);
          }
        else None

      let getOptionFromValue value =
        if (value.Equals "null") then Some value else None;

      let getOptionFromIntValue value =
        if (value.Equals "null") then Some (int value) else None;

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
          thumbnailPath = getOptionFromValue result.["Thumbnail_path"];
          metadata = metaData;
          description = getOptionFromValue result.["Description"];
          rentPrice = getOptionFromIntValue result.["Rent_price"];
          buyPrice = getOptionFromIntValue result.["Buy_price"];
        }:ProductTypes.Product

      let internal convertFromResults (result:Map<string,string> List) =
        let productList:ProductTypes.Product List ref = ref []
        for row in result do
          productList := (!productList)@[(convertFromResult row)]
        !productList