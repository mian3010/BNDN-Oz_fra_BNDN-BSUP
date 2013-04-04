namespace RentIt
open ProductTypes

module CreditsTypes = 

  type Transaction = {
    id : int;
    purchased : System.DateTime;
    paid : int;
    product : Product;
  }

  type RentOrBuy = Rent | Buy

  type Rent = {
    item : Transaction;
    expires : System.DateTime
  }

  type Buy = {
    item : Transaction;
  }