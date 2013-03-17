namespace RentIt
    module Filter =
        type Filter = {
         field : string;
         operator : string;
         value : string;
        }