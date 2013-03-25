namespace RentIt

module ProductTypes = 

  type Product = {
                    name : string;
                    createDate : System.DateTime;
                    productType : string;
                    owner : string;
                    description : string option;
                    rentPrice : int option;
                    buyPrice : int option;
                 }