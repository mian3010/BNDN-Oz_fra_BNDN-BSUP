namespace RentIt
  open ProductTypes
  open AccountTypes
  open CreditsTypes

  module Credits = 
    /// <summary>
    /// Purchase more credits
    /// </summary>
    /// <typeparam> Account to add credits to </typeparam>
    /// <typeparam> Amount to buy </typeparam>
    /// <returns> Whether or not add was successfull </returns>
    /// <exception> RentIt.AccountExceptions.NoSuchUser </exception>
    /// <exception> RentIt.CreditsExceptions.InvalidCredits </exception>
    let purchaseCredits (account:Account) (credits:int) :bool =
      if (credits < 1) then raise CreditsExceptions.InvalidCredits
      CreditsPersistence.updateCredits account.user credits

    /// <summary>
    /// Buy a product for user
    /// </summary>
    /// <typeparam> Account that buys </typeparam>
    /// <typeparam> Product to buy </typeparam>
    /// <returns> Whether or not buy was successfull </returns>
    /// <exception> RentIt.CreditsExceptions.NotEnoughCredits </exception>
    /// <exception> RentIt.ProductExceptions.NoSuchProduct </exception>
    /// <exception> RentIt.AccountExceptions.NoSuchUser </exception>
    let buyProduct (account:Account) (product:Product) :CreditsTypes.Buy option =
      if (CreditsPersistence.updateCredits account.user -(product.buyPrice.Value)) then
        Some (CreditsPersistence.createBuyTransaction (CreditsHelper.createBuy -1 account.user System.DateTime.Now product.buyPrice.Value product))
      else None

    /// <summary>
    /// Rent a product for user
    /// </summary>
    /// <typeparam> Account that buys </typeparam>
    /// <typeparam> Product to rent </typeparam>
    /// <returns> Expire date for rent </returns>
    /// <exception> RentIt.CreditsExceptions.NotEnoughCredits </exception>
    /// <exception> RentIt.ProductExceptions.NoSuchProduct </exception>
    /// <exception> RentIt.AccountExceptions.NoSuchUser </exception>
    let rentProduct (account:Account) (product:Product) (days:int) :CreditsTypes.Rent option =
      if (CreditsPersistence.updateCredits account.user -((product.rentPrice.Value*days))) then
        let expire = System.DateTime.Now.AddDays (float days)
        Some (CreditsPersistence.createRentTransaction (CreditsHelper.createRent -1 account.user System.DateTime.Now (product.rentPrice.Value*days) product expire))
      else None

    /// <summary>
    /// Get a specific transaction
    /// </summary>
    /// <typeparam> Id of the transaction </typeparam>
    /// <returns> RentOrBuy that corresponds to the transaction </returns>
    /// <exception> RentIt.CreditsException.NoSuchTransaction </exception>
    let getTransaction (id:int) :RentOrBuy =
      if (id < 1) then raise CreditsExceptions.NoSuchTransaction
      CreditsPersistence.getTransactionById id

    /// <summary>
    /// Get a list of transactions for a account
    /// </summary>
    /// <typeparam> The account to get transactions for </typeparam>
    /// <returns> RentOrBuy list that corresponds to the account </returns>
    /// <exception> RentIt.AccountExceptions.NoSuchUser </exception>
    let getTransactions (account:Account) :RentOrBuy List =
      CreditsPersistence.getTransactionByAccount account.user

    /// <summary>
    /// Get a list of transaction for an account by type
    /// </summary>
    /// <typeparam> The account to get transactions for </typeparam>
    /// <typeparam> Whether or not it should be rent or buy. 0 = buy, 1 = rent </typeparam>
    /// <returns> RentOrBuy list that corresponds to the account and type </returns>
    /// <exception> RentIt.AccountExceptions.NoSuchUser </exception>
    let getTransactionsByType (account:Account) (isRent:bool) :RentOrBuy List =
      CreditsPersistence.getTransactionsByTypeAccount isRent account.user

    /// <summary>
    /// Check whether or not an account has access to a product
    /// </summary>
    /// <typeparam> The account to check access for </typeparam>
    /// <typeparam> The product to check access to </typeparam>
    /// <returns> Whether or not access is granted </returns>
    /// <exception> RentIt.AccountExceptions.NoSuchUser </exception>
    /// <exception> RentIt.ProductExceptions.NoSuchProduct </exception>
    let checkAccessToProduct (account:Account) (product:Product) :bool =
      CreditsPersistence.checkAccessToProduct account.user product.id