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
    /// <exception> RentIt.AccountExceptions.InvalidCredits </exception>
    val purchaseCredits : Account -> int -> bool

    /// <summary>
    /// Buy a product for user
    /// </summary>
    /// <typeparam> Account that buys </typeparam>
    /// <typeparam> Product to buy </typeparam>
    /// <returns> Whether or not buy was successfull </returns>
    /// <exception> RentIt.CreditsExceptions.NotEnoughCredits </exception>
    /// <exception> RentIt.ProductExceptions.NoSuchProduct </exception>
    /// <exception> RentIt.AccountExceptions.NoSuchUser </exception>
    val buyProduct : Account -> Product -> CreditsTypes.Buy option

    /// <summary>
    /// Rent a product for user
    /// </summary>
    /// <typeparam> Account that buys </typeparam>
    /// <typeparam> Product to rent </typeparam>
    /// <returns> Expire date for rent </returns>
    /// <exception> RentIt.CreditsExceptions.NotEnoughCredits </exception>
    /// <exception> RentIt.ProductExceptions.NoSuchProduct </exception>
    /// <exception> RentIt.AccountExceptions.NoSuchUser </exception>
    val rentProduct : Account -> Product -> int -> CreditsTypes.Rent option

    /// <summary>
    /// Get a specific transaction
    /// </summary>
    /// <typeparam> Id of the transaction </typeparam>
    /// <returns> RentOrBuy that corresponds to the transaction </returns>
    /// <exception> RentIt.CreditsException.NoSuchTransaction </exception>
    val getTransaction : int -> RentOrBuy

    /// <summary>
    /// Get a list of transactions for a account
    /// </summary>
    /// <typeparam> The account to get transactions for </typeparam>
    /// <returns> RentOrBuy list that corresponds to the account </returns>
    /// <exception> RentIt.AccountExceptions.NoSuchUser </exception>
    val getTransactions : Account -> RentOrBuy List

    /// <summary>
    /// Get a list of transaction for an account by type
    /// </summary>
    /// <typeparam> The account to get transactions for </typeparam>
    /// <typeparam> Whether or not it should be rent or buy. 0 = buy, 1 = rent </typeparam>
    /// <returns> RentOrBuy list that corresponds to the account and type </returns>
    /// <exception> RentIt.AccountExceptions.NoSuchUser </exception>
    val getTransactionsByType : Account -> bool -> RentOrBuy List

    /// <summary>
    /// Check whether or not an account has access to a product
    /// </summary>
    /// <typeparam> The account to check access for </typeparam>
    /// <typeparam> The product to check access to </typeparam>
    /// <returns> Whether or not access is granted </returns>
    /// <exception> RentIt.AccountExceptions.NoSuchUser </exception>
    /// <exception> RentIt.ProductExceptions.NoSuchProduct </exception>
    val checkAccessToProduct : Account -> Product -> bool