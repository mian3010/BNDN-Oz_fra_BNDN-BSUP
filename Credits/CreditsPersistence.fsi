namespace RentIt
  open ProductTypes
  open AccountTypes
  open CreditsTypes

  module CreditsPersistence =

    /// <summary>
    /// Update credits
    /// </summary>
    /// <typeparam> Username of account to update </typeparam>
    /// <typeparam> Amount to modify by </typeparam>
    /// <returns> Whether or not modify was successfull </returns>
    /// <exception> RentIt.AccountExceptions.NoSuchUser </exception>
    /// <exception> RentIt.AccountExceptions.InvalidCredits </exception>
    val updateCredits : string -> int -> bool

    /// <summary>
    /// Create a buy transaction
    /// </summary>
    /// <typeparam> Username of account that creates the transaction </typeparam>
    /// <typeparam> The time of the transaction </typeparam>
    /// <typeparam> The product id </typeparam>
    /// <returns> The new transaction. Persisted </returns>
    /// <exception> RentIt.AccountExceptions.NoSuchUser </exception>
    /// <exception> RentIt.ProductExceptions.NoSuchProduct </exception>
    val createBuyTransaction : string -> System.DateTime -> int -> RentOrBuy

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
    val createRentTransaction : string -> System.DateTime -> int -> System.DateTime -> RentOrBuy

    /// <summary>
    /// Get a transaction by Id
    /// </summary>
    /// <typeparam> The Id to get transaction by </typeparam>
    /// <returns> The transaction </returns>
    /// <exception> RentIt.CreditsExceptions.NoSuchTransaction </exception>
    val getTransactionById : int -> RentOrBuy

    /// <summary>
    /// Get list of transactions by username
    /// </summary>
    /// <typeparam> The username to get transaction by </typeparam>
    /// <returns> The transactions </returns>
    /// <exception> RentIt.AccountExceptions.NoSuchUser </exception>
    val getTransactionByAccount : string -> RentOrBuy List

    /// <summary>
    /// Get list of transactions by type
    /// </summary>
    /// <typeparam> Whether or not it should be rent or buy. 0 = buy, 1 = rent </typeparam>
    /// <returns> The transactions </returns>
    val getTransactionsByType : bool -> RentOrBuy List

    /// <summary>
    /// Get list of transactions by type and username
    /// </summary>
    /// <typeparam> Whether or not it should be rent or buy. 0 = buy, 1 = rent </typeparam>
    /// <typeparam> Username of the account to get by </typeparam>
    /// <returns> The transactions </returns>
    val getTransactionsByTypeAccount : bool -> string -> RentOrBuy List

    /// <summary>
    /// Get list of transactions by username. Only transactions with access now.
    /// </summary>
    /// <typeparam> Username of the account to get by </typeparam>
    /// <returns> The transactions </returns>
    val getTransactionByAccountAccess : string -> RentOrBuy List