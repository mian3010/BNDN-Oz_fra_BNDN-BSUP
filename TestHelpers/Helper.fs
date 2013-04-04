namespace RentIt.Test
open RentIt.ProductPersistence
open RentIt.AccountPersistence
open RentIt

  module Helper =
    //Get testproduct
    let getTestProduct test =
      {
        name = "TESTPRODUCT_"+test;
        createDate = (System.DateTime.Parse "2012-10-10 02:04:00");
        productType = "TESTTYPE_"+test;
        owner = "TESTUSER_"+test;
        rating = None;
        published = true;
        id = -1;
        thumbnailPath = None;
        metadata = None;
        description = Some "Description";
        rentPrice = Some 20;
        buyPrice = Some 100;
      }:ProductTypes.Product

    //Create test product type
    let createTestType test = 
      createProductType ("TESTTYPE_"+test) |> ignore
      "TESTTYPE_"+test
    //Remove test product type
    let removeTestType test =
      let filtersQ = Persistence.Filter.createFilter [] "ProductType" "Name" "=" ("TESTTYPE_"+test)
      Persistence.Api.delete "ProductType" filtersQ |> ignore
    //Get product type
    let getProductType test =
      let filtersQ = Persistence.Filter.createFilter [] "ProductType" "Name" "=" ("TESTTYPE_"+test)
      let readQ = Persistence.ReadField.createReadField [] "ProductType" "Name"
      (Persistence.Api.read readQ "ProductType" [] filtersQ).Head.["Name"]

    //Create test account
    let createTestUser test =
      let acc:AccountTypes.Account = {
        user = "TESTUSER_"+test;
        email = "test@example.com";
        password = Account.Password.create "Testpass"
        created = (System.DateTime.Parse "2012-10-10 02:04:00");
        banned = false;
        info = ({
                name = Some "Testuser";
                address = ({
                            address = Some "Testaddress";
                            postal = Some 1123;
                            country = Some "Denmark"
                }:AccountTypes.Address);
                birth = Some (System.DateTime.Parse "2012-10-10 02:04:00");
                about = Some "Testing";
                credits = Some 0;
        }:AccountTypes.ExtraAccInfo);
        accType = "Customer";
        version = (uint32 0);
      }
      createUser acc |> ignore
      "TESTUSER_"+test

    //Remove ratings
    let removeUserRatings test =
      let filtersQ = Persistence.Filter.createFilter [] "ProductRating" "User_Username" "=" ("TESTUSER_"+test)
      Persistence.Api.delete "ProductRating" filtersQ |> ignore

    let removeProductRatings test =
      try
        let prod = getProductByName ("TESTPRODUCT_"+test)
        let filtersQ = Persistence.Filter.createFilter [] "ProductRating" "Product_Id" "=" (string prod.Head.id)
        Persistence.Api.delete "ProductRating" filtersQ |> ignore
      with
        | _ -> ()

    //Remove test account
    let removeTestUser test =
      removeUserRatings test
      let filtersQ = Persistence.Filter.createFilter [] "User" "Username" "=" ("TESTUSER_"+test)
      Persistence.Api.delete "User" filtersQ |> ignore

    //Create test product
    let createTestProduct test =
      let testType = createTestType test
      let testUser = createTestUser test
      let prod = getTestProduct test
      createProduct prod

    //Remove test product
    let removeTestProduct test =
      removeProductRatings test
      let filtersQ = Persistence.Filter.createFilter [] "Product" "Name" "=" ("TESTPRODUCT_"+test)
      Persistence.Api.delete "Product" filtersQ |> ignore
      let removeType = removeTestType test
      let removeUser = removeTestUser test
      ()

    