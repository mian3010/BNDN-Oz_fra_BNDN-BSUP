namespace RentIt
open System
open RentIt.CreditsTypes
    module CreditsPersistenceHelper =

      let objectName = "Transaction"

      let internal createDataInFromRentOrBuy userName purchased paid productId =
        let dataQ = ref (Persistence.DataIn.createDataIn [] objectName "User_Username" userName)
        dataQ := Persistence.DataIn.createDataIn !dataQ objectName "Purchased_Date" (Persistence.Helper.convertDatetimeToString purchased)
        dataQ := Persistence.DataIn.createDataIn !dataQ objectName "PricePaid" (string paid)
        dataQ := Persistence.DataIn.createDataIn !dataQ objectName "Product_Id" (string productId)
        !dataQ

      //Convert a product to a list of datain types for the persistence layer
      let internal convertToDataIn (transaction:RentOrBuy) =
        match transaction with 
          | RentOrBuy.Buy b -> createDataInFromRentOrBuy b.item.user b.item.purchased b.item.paid b.item.product.id
          | RentOrBuy.Rent r -> Persistence.DataIn.createDataIn (createDataInFromRentOrBuy r.item.user r.item.purchased r.item.paid r.item.product.id) objectName "Expires_Date" (Persistence.Helper.convertDatetimeToString r.expires)


      let internal convertFromResultToBuy (result:Map<string,string>) =
        {
                          item=({
                                id=(int result.["Id"]);
                                user=result.["User_Username"];
                                purchased=(System.DateTime.Parse result.["Purchased_Date"]);
                                paid=(int result.["PricePaid"]);
                                product=(ProductPersistence.getProductById (int result.["Product_Id"]));
                          }:Transaction)
        }:Buy

      let internal convertFromResultToRent (result:Map<string,string>) =
        {
                          item=({
                                id=(int result.["Id"]);
                                user=result.["User_Username"];
                                purchased=(System.DateTime.Parse result.["Purchased_Date"]);
                                paid=(int result.["PricePaid"]);
                                product=(ProductPersistence.getProductById (int result.["Product_Id"]));
                          }:Transaction);
                          expires=(System.DateTime.Parse result.["Expires_Date"]);
        }:Rent
         

      let internal convertFromResult (result:Map<string,string>) =
        match result.["Type"] with
          | "B" -> (RentOrBuy.Buy (convertFromResultToBuy result))
          | "R" -> (RentOrBuy.Rent (convertFromResultToRent result))
          | _ -> raise CreditsExceptions.UnexpectedType

      let internal convertFromResults (result:Map<string,string> List) =
        let transactionList:CreditsTypes.RentOrBuy List ref = ref []
        for row in result do
          transactionList := (!transactionList)@[(convertFromResult row)]
        !transactionList

      let createFiltersFromAccountAccess username = 
        let filterAccount = ref (Persistence.FilterGroup.createFilterGroup [] objectName "User_Username" username)

        let filterAccess1 = Persistence.FilterGroup.createFilterGroup [] objectName "Type" "B"
        let filterAccessGroup1:Persistence.Types.FilterGroup List = [{filterAccess1.Head with joiner=Some Persistence.FilterGroup.orJoiner}]

        let filterAccess2 = ref (Persistence.Filter.createFilter [] objectName "Type" "R")
        filterAccess2 := Persistence.Filter.createFilterProc [] objectName "Expires_Date" (Persistence.Helper.convertDatetimeToString System.DateTime.Now) Persistence.Filter.lessThanOrEqual
        let filterAccessGroup2 = Persistence.FilterGroup.createFilterGroupFilters [] !filterAccess2

        [(!filterAccount).Head;filterAccessGroup1.Head;filterAccessGroup2.Head]