namespace RentIt
  open ProductTypes
  open AccountTypes
  open CreditsTypes
  open CreditsPersistenceHelper

  module CreditsPersistence =
    let objectName = "Transaction"

    /// <summary>
    /// Update credits
    /// </summary>
    /// <typeparam> Username of account to update </typeparam>
    /// <typeparam> Amount to modify by </typeparam>
    /// <returns> Whether or not modify was successfull </returns>
    /// <exception> RentIt.AccountExceptions.NoSuchUser </exception>
    /// <exception> RentIt.CreditsException.NotEnoughCredits </exception>
    let updateCredits (username:string) (amount:int) :bool =
      let transactionQ = ref (Persistence.Transaction.createTransaction)

      let objectName = "User"
      let filtersQ = ref (Persistence.FilterGroup.createSingleFilterGroup [] objectName "Username" username)
      filtersQ := Persistence.FilterGroup.createFilterGroupFilterProc !filtersQ objectName "Balance" (string (amount*(-1))) Persistence.Filter.greaterThanOrEqual
      let fieldValQ = Persistence.Field.createField [] objectName "Balance"
      let dataQ = Persistence.DataIn.createDataInFieldValueWithMod [] objectName "Balance" fieldValQ.Head ("+"+(string amount))
      try
        let updateR = Persistence.Api.update objectName !filtersQ dataQ
        let user = Account.getByUsername username
        if (updateR.Length.Equals 0) then raise CreditsExceptions.NotEnoughCredits
        AccountPersistence.updateUserInCache username
        true
      with
        | AccountExceptions.NoSuchUser|CreditsExceptions.NotEnoughCredits as e -> raise e
        | _ -> false


    /// <summary>
    /// Create a buy transaction
    /// </summary>
    /// <typeparam> The buy transaction to create </typeparam>
    /// <typeparam> The time of the transaction </typeparam>
    /// <typeparam> The product id </typeparam>
    /// <returns> The new transaction. Persisted </returns>
    /// <exception> RentIt.AccountExceptions.NoSuchUser </exception>
    /// <exception> RentIt.ProductExceptions.NoSuchProduct </exception>
    let createBuyTransaction (buy:Buy) :Buy =
      try
        let dataQ = ref (convertToDataIn (RentOrBuy.Buy buy))
        dataQ := Persistence.DataIn.createDataIn !dataQ objectName "Type" "B"
        convertFromResultToBuy ((Persistence.Api.create objectName !dataQ).Head)
      with
        | PersistenceExceptions.ReferenceDoesNotExist -> 
          try
            let user = Account.getByUsername buy.item.user
            raise ProductExceptions.NoSuchProduct
          with
            | AccountPersistenceExceptions.NoUserWithSuchName -> raise AccountExceptions.NoSuchUser
            | _ as e -> raise e
        | _ as e -> raise e

    /// <summary>
    /// Create a rent transaction
    /// </summary>
    /// <typeparam> Username of account that creates the transaction </typeparam>
    /// <typeparam> The time of the transaction </typeparam>
    /// <typeparam> The product id </typeparam>
    /// <typeparam> The expire date of the transaction </typeparam>
    /// <returns> The new transaction. Persisted </returns>
    /// <exception> RentIt.AccountExceptions.NoSuchUser </exception>
    /// <exception> RentIt.ProductExceptions.NoSuchProduct </exception>
    let createRentTransaction (rent:Rent) :Rent =
      try
        let dataQ = ref (convertToDataIn (RentOrBuy.Rent rent))
        dataQ := Persistence.DataIn.createDataIn !dataQ objectName "Type" "R"
        convertFromResultToRent ((Persistence.Api.create objectName !dataQ).Head)
      with
        | PersistenceExceptions.ReferenceDoesNotExist -> 
          try
            let user = Account.getByUsername rent.item.user
            raise ProductExceptions.NoSuchProduct
          with
            | AccountPersistenceExceptions.NoUserWithSuchName -> raise AccountExceptions.NoSuchUser
            | _ as e -> raise e
        | _ as e -> raise e

    /// <summary>
    /// Get a transaction by Id
    /// </summary>
    /// <typeparam> The Id to get transaction by </typeparam>
    /// <returns> The transaction </returns>
    /// <exception> RentIt.CreditsExceptions.NoSuchTransaction </exception>
    let getTransactionById (id:int) :RentOrBuy =
      let fieldsQ = Persistence.ReadField.createReadFieldProc [] "" "" Persistence.ReadField.All
      let filtersQ = Persistence.FilterGroup.createSingleFilterGroup [] objectName "Id" (string id)
      let readR = Persistence.Api.read fieldsQ objectName [] filtersQ
      if (readR.Length.Equals 0) then raise CreditsExceptions.NoSuchTransaction
      convertFromResult (readR.Head)

    /// <summary>
    /// Get list of transactions by username
    /// </summary>
    /// <typeparam> The username to get transaction by </typeparam>
    /// <returns> The transactions </returns>
    /// <exception> RentIt.AccountExceptions.NoSuchUser </exception>
    let getTransactionByAccount (username:string) :RentOrBuy List =
      try
        let user = Account.getByUsername username
        let fieldsQ = Persistence.ReadField.createReadFieldProc [] "" "" Persistence.ReadField.All
        let filtersQ = Persistence.FilterGroup.createSingleFilterGroup [] objectName "User_Username" username 
        convertFromResults (Persistence.Api.read fieldsQ objectName [] filtersQ)
      with
        | AccountPersistenceExceptions.NoUserWithSuchName -> raise AccountExceptions.NoSuchUser
        | _ as e -> raise e

    /// <summary>
    /// Get list of transactions by type
    /// </summary>
    /// <typeparam> Whether or not it should be rent or buy. 0 = buy, 1 = rent </typeparam>
    /// <returns> The transactions </returns>
    let getTransactionsByType (isRent:bool) :RentOrBuy List =
      let fieldsQ = Persistence.ReadField.createReadFieldProc [] "" "" Persistence.ReadField.All
      let filtersQ = Persistence.FilterGroup.createSingleFilterGroup [] objectName "Type" (if isRent then "R" else "B") 
      convertFromResults (Persistence.Api.read fieldsQ objectName [] filtersQ)

    /// <summary>
    /// Get list of transactions by type and username
    /// </summary>
    /// <typeparam> Whether or not it should be rent or buy. 0 = buy, 1 = rent </typeparam>
    /// <typeparam> Username of the account to get by </typeparam>
    /// <returns> The transactions </returns>
    /// <exception> RentIt.AccountExceptions.NoSuchUser </exception>
    let getTransactionsByTypeAccount (isRent:bool) (username:string) :RentOrBuy List =
      try
        let user = Account.getByUsername username
        let fieldsQ = Persistence.ReadField.createReadFieldProc [] "" "" Persistence.ReadField.All
        let filtersQ = ref (Persistence.FilterGroup.createSingleFilterGroup [] objectName "Type" (if isRent then "R" else "B") )
        filtersQ := Persistence.FilterGroup.createFilterGroup !filtersQ objectName "User_Username" username 
        convertFromResults (Persistence.Api.read fieldsQ objectName [] !filtersQ)
      with
        | AccountPersistenceExceptions.NoUserWithSuchName -> raise AccountExceptions.NoSuchUser
        | _ as e -> raise e

    /// <summary>
    /// Get list of transactions by username. Only transactions with access now.
    /// </summary>
    /// <typeparam> Username of the account to get by </typeparam>
    /// <returns> The transactions </returns>
    /// <exception> RentIt.AccountExceptions.NoSuchUser </exception>
    let getTransactionByAccountAccess  (username:string) :RentOrBuy List =
      try
        let user = Account.getByUsername username
        let fieldsQ = Persistence.ReadField.createReadFieldProc [] "" "" Persistence.ReadField.All
        let filtersQ = createFiltersFromAccountAccess username

        convertFromResults (Persistence.Api.read fieldsQ objectName [] filtersQ)
      with
        | AccountPersistenceExceptions.NoUserWithSuchName -> raise AccountExceptions.NoSuchUser
        | _ as e -> raise e

    /// <summary>
    /// Get whether or not user has access to product
    /// </summary>
    /// <typeparam> Username of the account to get by </typeparam>
    /// <typeparam> Product id to check </typeparam>
    /// <returns> Whether or not user has access </returns>
    /// <exception> RentIt.AccountExceptions.NoSuchUser </exception>
    /// <exception> RentIt.ProductExceptions.NoSuchProduct </exception>
    let checkAccessToProduct (username:string) (product:int) :bool =
      try
        let user = Account.getByUsername username
        let product = Product.getProductById product
        let fieldsQ = Persistence.ReadField.createReadFieldProc [] "" "" Persistence.ReadField.All
        let filtersQ = ref (createFiltersFromAccountAccess username)
        filtersQ := Persistence.FilterGroup.createFilterGroup !filtersQ objectName "Product_Id" (string product.id)

        if ((Persistence.Api.read fieldsQ objectName [] !filtersQ).Length.Equals 1) then true
        else false
      with
        | AccountPersistenceExceptions.NoUserWithSuchName -> raise AccountExceptions.NoSuchUser
        | _ as e -> raise e