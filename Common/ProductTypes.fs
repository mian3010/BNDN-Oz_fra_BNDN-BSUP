namespace RentIt

module ProductTypes = 

  type Rating = {
                  rating : int;
                  votes : int;
                }

  type Product = {
                   name : string;
                   createDate : System.DateTime;
                   productType : string;
                   owner : string;
                   rating : Rating;
                   published : bool;
                   id : int;
                   metadata : Map<string, string>;
                   description : string option;
                   rentPrice : int option;
                   buyPrice : int option;
                 }