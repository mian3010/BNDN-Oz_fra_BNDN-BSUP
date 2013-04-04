namespace RentIt
  open ProductTypes
  open AccountTypes
  open CreditsTypes

  module Credits = 
    let purchaseCredits (account:Account) (credits:int) :bool =
      raise (new System.NotImplementedException())

    let buyProduct (account:Account) (product:Product) :bool =
      raise (new System.NotImplementedException())

    let rentProduct (account:Account) (product:Product) (days:int) :System.DateTime =
      raise (new System.NotImplementedException())

    let getTransaction (id:int) :RentOrBuy =
      raise (new System.NotImplementedException())

    let getTransactions (account:Account) :RentOrBuy List =
      raise (new System.NotImplementedException())

    let getTransactionsByType (account:Account) (isRent:bool) :RentOrBuy List =
      raise (new System.NotImplementedException())

    let checkAccessToProduct (account:Account) (product:Product) :bool =
      raise (new System.NotImplementedException())