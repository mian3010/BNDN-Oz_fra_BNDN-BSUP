namespace RentIt

module ProductTypes = 

  type Rating = {
                  rating : int;
                  votes : int;
                }

  type Meta = {
                key : string;
                value : string;
              }

  type Product = {
                   name : string;
                   createDate : System.DateTime;
                   productType : string;
                   owner : string;
                   rating : Rating option;
                   published : bool;
                   id : int;
                   metadata : Map<string, Meta> option;
                   description : string option;
                   rentPrice : int option;
                   buyPrice : int option;
                 }

    type PublishedStatus = 
                          Published
                        | Unpublished
                        | Both