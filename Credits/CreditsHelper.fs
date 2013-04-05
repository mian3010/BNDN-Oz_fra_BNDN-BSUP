namespace RentIt
open System
open RentIt.CreditsTypes
    module CreditsHelper =
      let createBuy id username purchased paid product = 
        {
          item=({
                id=id;
                user=username;
                purchased=purchased;
                paid=paid;
                product=product;
          })
        }
      let createRent id username purchased paid product expires = 
        {
          item=({
                id=id;
                user=username;
                purchased=purchased;
                paid=paid;
                product=product;
          });
          expires=expires;
        }