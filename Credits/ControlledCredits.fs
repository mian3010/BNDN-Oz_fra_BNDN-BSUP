namespace RentIt
  open PermissionsUtil
  open ProductTypes
  open AccountTypes
  open CreditsTypes

  module ControlledCredits = 

    /// <summary>
    /// Purchase more credits, adding a permissions check
    /// </summary>
    /// <typeparam> Invoker of call </typeparam>
    /// <typeparam> Account to add credits to </typeparam>
    /// <typeparam> Amount to buy </typeparam>
    /// <returns> Whether or not add was successfull </returns>
    /// <exception> RentIt.AccountExceptions.NoSuchUser </exception>
    /// <exception> RentIt.AccountExceptions.InvalidCredits </exception>
    /// <exception> RentIt.PermissionsExceptions.PermissionDenied </exception>
    let purchaseCredits (invoker:Invoker) (account:Account) (credits:int) :bool =
      raise (new System.NotImplementedException())

    /// <summary>
    /// Buy a product for user, adding a permissions check
    /// </summary>
    /// <typeparam> Invoker of call </typeparam>
    /// <typeparam> Account that buys </typeparam>
    /// <typeparam> Product to buy </typeparam>
    /// <returns> Whether or not buy was successfull </returns>
    /// <exception> RentIt.CreditsExceptions.NotEnoughCredits </exception>
    /// <exception> RentIt.ProductExceptions.NoSuchProduct </exception>
    /// <exception> RentIt.AccountExceptions.NoSuchUser </exception>
    /// <exception> RentIt.PermissionsExceptions.PermissionDenied </exception>
    let buyProduct (invoker:Invoker) (account:Account) (product:Product) :bool =
      raise (new System.NotImplementedException())

    /// <summary>
    /// Rent a product for user, adding a permissions check
    /// </summary>
    /// <typeparam> Invoker of call </typeparam>
    /// <typeparam> Account that buys </typeparam>
    /// <typeparam> Product to rent </typeparam>
    /// <returns> Expire date for rent </returns>
    /// <exception> RentIt.CreditsExceptions.NotEnoughCredits </exception>
    /// <exception> RentIt.ProductExceptions.NoSuchProduct </exception>
    /// <exception> RentIt.AccountExceptions.NoSuchUser </exception>
    /// <exception> RentIt.PermissionsExceptions.PermissionDenied </exception>
    let rentProduct (invoker:Invoker) (account:Account) (product:Product) (days:int) :System.DateTime =
      raise (new System.NotImplementedException())

    /// <summary>
    /// Get a specific transaction, adding a permissions check
    /// </summary>
    /// <typeparam> Invoker of call </typeparam>
    /// <typeparam> Id of the transaction </typeparam>
    /// <returns> RentOrBuy that corresponds to the transaction </returns>
    /// <exception> RentIt.CreditsException.NoSuchTransaction </exception>
    /// <exception> RentIt.PermissionsExceptions.PermissionDenied </exception>
    let getTransaction (invoker:Invoker) (id:int) :RentOrBuy =
      raise (new System.NotImplementedException())

    /// <summary>
    /// Get a list of transactions for a account, adding a permissions check
    /// </summary>
    /// <typeparam> Invoker of call </typeparam>
    /// <typeparam> The account to get transactions for </typeparam>
    /// <returns> RentOrBuy list that corresponds to the account </returns>
    /// <exception> RentIt.AccountExceptions.NoSuchUser </exception>
    /// <exception> RentIt.PermissionsExceptions.PermissionDenied </exception>
    let getTransactions (invoker:Invoker) (account:Account) :RentOrBuy List =
      raise (new System.NotImplementedException())

    /// <summary>
    /// Get a list of transaction for an account by type, adding a permissions check
    /// </summary>
    /// <typeparam> Invoker of call </typeparam>
    /// <typeparam> The account to get transactions for </typeparam>
    /// <typeparam> Whether or not it should be rent or buy. 0 = buy, 1 = rent </typeparam>
    /// <returns> RentOrBuy list that corresponds to the account and type </returns>
    /// <exception> RentIt.AccountExceptions.NoSuchUser </exception>
    /// <exception> RentIt.PermissionsExceptions.PermissionDenied </exception>
    let getTransactionsByType (invoker:Invoker) (account:Account) (isRent:bool) :RentOrBuy List =
      raise (new System.NotImplementedException())

    /// <summary>
    /// Check whether or not an account has access to a product, adding a permissions check
    /// </summary>
    /// <typeparam> Invoker of call </typeparam>
    /// <typeparam> The account to check access for </typeparam>
    /// <typeparam> The product to check access to </typeparam>
    /// <returns> Whether or not access is granted </returns>
    /// <exception> RentIt.AccountExceptions.NoSuchUser </exception>
    /// <exception> RentIt.ProductExceptions.NoSuchProduct </exception>
    /// <exception> RentIt.PermissionsExceptions.PermissionDenied </exception>
    let checkAccessToProduct (invoker:Invoker) (account:Account) (product:Product) :bool =
      raise (new System.NotImplementedException())