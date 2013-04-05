namespace RentIt

module Main = 

  let internal createDataInForAllowedAction name description =
      [Persistence.DataIn.createDataIn (Persistence.DataIn.createDataIn [] "AllowedAction" "Name" name) "AllowedAction" "Description" description]


  let internal createDataInForActionGroupHasAllowedAction group action =
      [Persistence.DataIn.createDataIn (Persistence.DataIn.createDataIn [] "ActionGroup_has_AllowedAction" "ActionGroup_name" group) "ActionGroup_has_AllowedAction" "AllowedAction_name" action]

  let internal createDataInForActionGroup group description =
      [Persistence.DataIn.createDataIn (Persistence.DataIn.createDataIn [] "ActionGroup" "Name" group) "ActionGroup" "Description" description]

  let internal createDataInForUserType userType =
      [Persistence.DataIn.createDataIn [] "UserType" "Name" userType]

  let internal createDataInForUserTypeHasActionGroup usertype group =
      [Persistence.DataIn.createDataIn (Persistence.DataIn.createDataIn [] "UserType_has_ActionGroup" "UserType_name" usertype) "UserType_has_ActionGroup" "ActionGroup_name" group]

  let internal createDataInForCountry name =
      [Persistence.DataIn.createDataIn [] "Country" "Name" name]

  let internal createDataInForLoggable id =
      [Persistence.DataIn.createDataIn [] "Loggable" "Id" id]

  let internal createDataInForProductType name =
      [Persistence.DataIn.createDataIn [] "ProductType" "Name" name]

  let internal createDataInForMimeType pType mType =
      [Persistence.DataIn.createDataIn (Persistence.DataIn.createDataIn [] "MimeType" "ProductType_Name" pType) "MimeType" "Type" mType]

  [<EntryPoint>]
  let main argv = 
    
    //Truncate data
    Persistence.Api.delete "Transaction" [] |> ignore
    Persistence.Api.delete "ProductRating" [] |> ignore
    Persistence.Api.delete "ActionGroup_has_AllowedAction" [] |> ignore
    Persistence.Api.delete "UserType_has_ActionGroup" [] |> ignore
    Persistence.Api.delete "Product_has_AllowedAction" [] |> ignore
    Persistence.Api.delete "AllowedAction" [] |> ignore
    Persistence.Api.delete "ActionGroup" [] |> ignore
    Persistence.Api.delete "Log" [] |> ignore
    Persistence.Api.delete "LogEntryType" [] |> ignore
    Persistence.Api.delete "MetaData" [] |> ignore
    Persistence.Api.delete "MetaDataType" [] |> ignore
    Persistence.Api.delete "Product" [] |> ignore
    Persistence.Api.delete "User" [] |> ignore
    Persistence.Api.delete "UserType" [] |> ignore
    Persistence.Api.delete "Country" [] |> ignore
    Persistence.Api.delete "ProductType" [] |> ignore
    Persistence.Api.delete "Loggable" [] |> ignore
    Persistence.Api.delete "MimeType" [] |> ignore
    (Persistence.Helper.performSql "DBCC CHECKIDENT('Loggable', RESEED, 1)").Close() |> ignore

    // Create AllowedAction
    // "Name" "Description"
    let mutable insert:((Persistence.Types.DataIn List) List) = []
    insert <- insert@createDataInForAllowedAction "HAS_CREDITS" "Allows an account to have credits associated"
    insert <- insert@createDataInForAllowedAction "BUYABLE" "Allows a Product to bought"
    insert <- insert@createDataInForAllowedAction "RENTABLE" "Allows a Product to be rented"
    insert <- insert@createDataInForAllowedAction "CREATE_PRODUCT_ANY" "Allows an account to create products"
    insert <- insert@createDataInForAllowedAction "RATE_PRODUCT_ANY" "Allows an account to rate products"

    insert <- insert@createDataInForAllowedAction "BAN_UNBAN_ANY" "Ban/unban accounts"
    insert <- insert@createDataInForAllowedAction "CHANGE_EMAIL_ANY" "CHANGE_EMAIL"
    insert <- insert@createDataInForAllowedAction "CHANGE_PASSWORD_ANY" "CHANGE_PASSWORD"
    insert <- insert@createDataInForAllowedAction "GIVE_CREDITS_ANY" "GIVE_CREDITS to an account which has credits"
    insert <- insert@createDataInForAllowedAction "TAKE_CREDITS_ANY" "TAKE_CREDITS from an account which has credits"
    insert <- insert@createDataInForAllowedAction "READ_AUTH_INFO_ANY" "Read last time of authentication"
    insert <- insert@createDataInForAllowedAction "RESET_PASSWORD_ANY" "RESET_PASSWORD"
    insert <- insert@createDataInForAllowedAction "CREATE_ANY" "CREATE any account type"
    insert <- insert@createDataInForAllowedAction "READ_ANY" "READ account information"
    insert <- insert@createDataInForAllowedAction "EDIT_ANY" "EDIT additional account information"
    insert <- insert@createDataInForAllowedAction "READ_PRODUCT_ANY" "Read any published product"
    insert <- insert@createDataInForAllowedAction "READ_UNPUBLISHED_PRODUCT_ANY" "Read any unpublished product"
    insert <- insert@createDataInForAllowedAction "EDIT_PRODUCT_ANY" "Edit any product"
    insert <- insert@createDataInForAllowedAction "EDIT_UNPUBLISHED_PRODUCT_ANY" "Edit any unpublished product"
    insert <- insert@createDataInForAllowedAction "PUBLISH_PRODUCT_ANY" "Publish/unpublish any product"
    insert <- insert@createDataInForAllowedAction "UPLOAD_MEDIA_ANY" "Upload media for any product"
    insert <- insert@createDataInForAllowedAction "UPLOAD_THUMBNAIL_ANY" "Upload thumbnail for any product"

    insert <- insert@createDataInForAllowedAction "BAN_UNBAN_OWN" "Ban/unban accounts"
    insert <- insert@createDataInForAllowedAction "CHANGE_EMAIL_OWN" "CHANGE_EMAIL"
    insert <- insert@createDataInForAllowedAction "CHANGE_PASSWORD_OWN" "CHANGE_PASSWORD"
    insert <- insert@createDataInForAllowedAction "GIVE_CREDITS_OWN" "GIVE_CREDITS to an account which has credits"
    insert <- insert@createDataInForAllowedAction "TAKE_CREDITS_OWN" "TAKE_CREDITS from an account which has credits"
    insert <- insert@createDataInForAllowedAction "READ_AUTH_INFO_OWN" "Read last time of authentication"
    insert <- insert@createDataInForAllowedAction "RESET_PASSWORD_OWN" "RESET_PASSWORD"
    insert <- insert@createDataInForAllowedAction "READ_OWN" "READ account information"
    insert <- insert@createDataInForAllowedAction "EDIT_OWN" "EDIT additional account information"
    insert <- insert@createDataInForAllowedAction "READ_PRODUCT_OWN" "Read OWN published product"
    insert <- insert@createDataInForAllowedAction "READ_UNPUBLISHED_PRODUCT_OWN" "Read OWN unpublished product"
    insert <- insert@createDataInForAllowedAction "EDIT_PRODUCT_OWN" "Edit OWN product"
    insert <- insert@createDataInForAllowedAction "EDIT_UNPUBLISHED_PRODUCT_OWN" "Edit OWN unpublished product"
    insert <- insert@createDataInForAllowedAction "PUBLISH_PRODUCT_OWN" "Publish/unpublish OWN product"
    insert <- insert@createDataInForAllowedAction "UPLOAD_MEDIA_OWN" "Upload media for OWN product"
    insert <- insert@createDataInForAllowedAction "UPLOAD_THUMBNAIL_OWN" "Upload thumbnail for OWN product"

    insert <- insert@createDataInForAllowedAction "BAN_UNBAN_TYPE_ADMIN" "Ban/unban accounts"
    insert <- insert@createDataInForAllowedAction "CHANGE_EMAIL_TYPE_ADMIN" "CHANGE_EMAIL"
    insert <- insert@createDataInForAllowedAction "CHANGE_PASSWORD_TYPE_ADMIN" "CHANGE_PASSWORD"
    insert <- insert@createDataInForAllowedAction "READ_AUTH_INFO_TYPE_ADMIN" "Read last time of authentication"
    insert <- insert@createDataInForAllowedAction "RESET_PASSWORD_TYPE_ADMIN" "RESET_PASSWORD"
    insert <- insert@createDataInForAllowedAction "READ_TYPE_ADMIN" "READ account information"
    insert <- insert@createDataInForAllowedAction "EDIT_TYPE_ADMIN" "EDIT additional account information"
    insert <- insert@createDataInForAllowedAction "CREATE_TYPE_ADMIN" "CREATE account"

    insert <- insert@createDataInForAllowedAction "BAN_UNBAN_TYPE_Content Provider" "Ban/unban accounts"
    insert <- insert@createDataInForAllowedAction "CHANGE_EMAIL_TYPE_Content Provider" "CHANGE_EMAIL"
    insert <- insert@createDataInForAllowedAction "CHANGE_PASSWORD_TYPE_Content Provider" "CHANGE_PASSWORD"
    insert <- insert@createDataInForAllowedAction "READ_AUTH_INFO_TYPE_Content Provider" "Read last time of authentication"
    insert <- insert@createDataInForAllowedAction "RESET_PASSWORD_TYPE_Content Provider" "RESET_PASSWORD"
    insert <- insert@createDataInForAllowedAction "READ_TYPE_Content Provider" "READ account information"
    insert <- insert@createDataInForAllowedAction "EDIT_TYPE_Content Provider" "EDIT additional account information"
    insert <- insert@createDataInForAllowedAction "CREATE_TYPE_Content Provider" "CREATE account"

    insert <- insert@createDataInForAllowedAction "BAN_UNBAN_TYPE_CUSTOMER" "Ban/unban accounts"
    insert <- insert@createDataInForAllowedAction "CHANGE_EMAIL_TYPE_CUSTOMER" "CHANGE_EMAIL"
    insert <- insert@createDataInForAllowedAction "CHANGE_PASSWORD_TYPE_CUSTOMER" "CHANGE_PASSWORD"
    insert <- insert@createDataInForAllowedAction "GIVE_CREDITS_TYPE_CUSTOMER" "GIVE_CREDITS to an account which has credits"
    insert <- insert@createDataInForAllowedAction "TAKE_CREDITS_TYPE_CUSTOMER" "TAKE_CREDITS from an account which has credits"
    insert <- insert@createDataInForAllowedAction "READ_AUTH_INFO_TYPE_CUSTOMER" "Read last time of authentication"
    insert <- insert@createDataInForAllowedAction "RESET_PASSWORD_TYPE_CUSTOMER" "RESET_PASSWORD"
    insert <- insert@createDataInForAllowedAction "READ_TYPE_CUSTOMER" "READ account information"
    insert <- insert@createDataInForAllowedAction "EDIT_TYPE_CUSTOMER" "EDIT additional account information"
    insert <- insert@createDataInForAllowedAction "CREATE_TYPE_CUSTOMER" "CREATE account"

    printfn "%A" ("---------- Create " + "AllowedAction" + " ----------")
    for i in insert do
      Persistence.Api.create "AllowedAction" i |> ignore
    
    // Create ActionGroup
    // "Name" "Description"
    let mutable insert:((Persistence.Types.DataIn List) List) = []
    insert <- insert@createDataInForActionGroup "Admin" "Admin"
    insert <- insert@createDataInForActionGroup "Unauth" "Unauthenticated user"
    insert <- insert@createDataInForActionGroup "Customer" "Customer"
    insert <- insert@createDataInForActionGroup "Content Provider" "Content provider"

    printfn "%A" ("---------- Create " + "ActionGroup" + " ----------")
    for i in insert do
      Persistence.Api.create "ActionGroup" i |> ignore
    
    // Create ActionGroup_has_AllowedAction
    // "ActionGroup_name" "AllowedAction_name"
    let mutable insert:((Persistence.Types.DataIn List) List) = []
    insert <- insert@createDataInForActionGroupHasAllowedAction "Admin" "BAN_UNBAN_ANY"
    insert <- insert@createDataInForActionGroupHasAllowedAction "Admin" "CHANGE_EMAIL_ANY"
    insert <- insert@createDataInForActionGroupHasAllowedAction "Admin" "CHANGE_PASSWORD_ANY"
    insert <- insert@createDataInForActionGroupHasAllowedAction "Admin" "GIVE_CREDITS_ANY"
    insert <- insert@createDataInForActionGroupHasAllowedAction "Admin" "TAKE_CREDITS_ANY"
    insert <- insert@createDataInForActionGroupHasAllowedAction "Admin" "READ_AUTH_INFO_ANY"
    insert <- insert@createDataInForActionGroupHasAllowedAction "Admin" "RESET_PASSWORD_ANY"
    insert <- insert@createDataInForActionGroupHasAllowedAction "Admin" "CREATE_ANY"
    insert <- insert@createDataInForActionGroupHasAllowedAction "Admin" "READ_ANY"
    insert <- insert@createDataInForActionGroupHasAllowedAction "Admin" "READ_PRODUCT_ANY"
    insert <- insert@createDataInForActionGroupHasAllowedAction "Admin" "READ_UNPUBLISHED_PRODUCT_ANY"
    insert <- insert@createDataInForActionGroupHasAllowedAction "Admin" "EDIT_PRODUCT_ANY"
    insert <- insert@createDataInForActionGroupHasAllowedAction "Admin" "EDIT_UNPUBLISHED_PRODUCT_ANY"
    insert <- insert@createDataInForActionGroupHasAllowedAction "Admin" "PUBLISH_PRODUCT_ANY"
    insert <- insert@createDataInForActionGroupHasAllowedAction "Admin" "UPLOAD_MEDIA_ANY"
    insert <- insert@createDataInForActionGroupHasAllowedAction "Admin" "UPLOAD_THUMBNAIL_ANY"

    insert <- insert@createDataInForActionGroupHasAllowedAction "Admin" "BAN_UNBAN_TYPE_Admin"
    insert <- insert@createDataInForActionGroupHasAllowedAction "Admin" "CHANGE_EMAIL_TYPE_Admin"
    insert <- insert@createDataInForActionGroupHasAllowedAction "Admin" "CHANGE_PASSWORD_TYPE_Admin"
    insert <- insert@createDataInForActionGroupHasAllowedAction "Admin" "READ_AUTH_INFO_TYPE_Admin"
    insert <- insert@createDataInForActionGroupHasAllowedAction "Admin" "RESET_PASSWORD_TYPE_Admin"
    insert <- insert@createDataInForActionGroupHasAllowedAction "Admin" "CREATE_TYPE_Admin"
    insert <- insert@createDataInForActionGroupHasAllowedAction "Admin" "READ_TYPE_Admin"

    insert <- insert@createDataInForActionGroupHasAllowedAction "Admin" "BAN_UNBAN_TYPE_Content Provider"
    insert <- insert@createDataInForActionGroupHasAllowedAction "Admin" "CHANGE_EMAIL_TYPE_Content Provider"
    insert <- insert@createDataInForActionGroupHasAllowedAction "Admin" "CHANGE_PASSWORD_TYPE_Content Provider"
    insert <- insert@createDataInForActionGroupHasAllowedAction "Admin" "READ_AUTH_INFO_TYPE_Content Provider"
    insert <- insert@createDataInForActionGroupHasAllowedAction "Admin" "RESET_PASSWORD_TYPE_Content Provider"
    insert <- insert@createDataInForActionGroupHasAllowedAction "Admin" "CREATE_TYPE_Content Provider"
    insert <- insert@createDataInForActionGroupHasAllowedAction "Admin" "READ_TYPE_Content Provider"

    insert <- insert@createDataInForActionGroupHasAllowedAction "Admin" "BAN_UNBAN_TYPE_Customer"
    insert <- insert@createDataInForActionGroupHasAllowedAction "Admin" "CHANGE_EMAIL_TYPE_Customer"
    insert <- insert@createDataInForActionGroupHasAllowedAction "Admin" "CHANGE_PASSWORD_TYPE_Customer"
    insert <- insert@createDataInForActionGroupHasAllowedAction "Admin" "GIVE_CREDITS_TYPE_Customer"
    insert <- insert@createDataInForActionGroupHasAllowedAction "Admin" "TAKE_CREDITS_TYPE_Customer"
    insert <- insert@createDataInForActionGroupHasAllowedAction "Admin" "READ_AUTH_INFO_TYPE_Customer"
    insert <- insert@createDataInForActionGroupHasAllowedAction "Admin" "RESET_PASSWORD_TYPE_Customer"
    insert <- insert@createDataInForActionGroupHasAllowedAction "Admin" "CREATE_TYPE_Customer"
    insert <- insert@createDataInForActionGroupHasAllowedAction "Admin" "READ_TYPE_Customer"

    insert <- insert@createDataInForActionGroupHasAllowedAction "Admin" "CHANGE_EMAIL_OWN"
    insert <- insert@createDataInForActionGroupHasAllowedAction "Admin" "CHANGE_PASSWORD_OWN"
    insert <- insert@createDataInForActionGroupHasAllowedAction "Admin" "READ_AUTH_INFO_OWN"
    insert <- insert@createDataInForActionGroupHasAllowedAction "Admin" "RESET_PASSWORD_OWN"
    insert <- insert@createDataInForActionGroupHasAllowedAction "Admin" "READ_OWN"

    insert <- insert@createDataInForActionGroupHasAllowedAction "Unauth" "READ_TYPE_Content Provider"

    insert <- insert@createDataInForActionGroupHasAllowedAction "Unauth" "READ_TYPE_Customer"

    //insert <- insert@createDataInForActionGroupHasAllowedAction "Unauth" "READ_ANY" //For testing
    insert <- insert@createDataInForActionGroupHasAllowedAction "Unauth" "READ_PRODUCT_ANY"
    
    insert <- insert@createDataInForActionGroupHasAllowedAction "Unauth" "CREATE_TYPE_Customer"

    insert <- insert@createDataInForActionGroupHasAllowedAction "Customer" "HAS_CREDITS"
    insert <- insert@createDataInForActionGroupHasAllowedAction "Customer" "RATE_PRODUCT_ANY"
    insert <- insert@createDataInForActionGroupHasAllowedAction "Customer" "READ_ANY"
    insert <- insert@createDataInForActionGroupHasAllowedAction "Customer" "READ_PRODUCT_ANY"
    
    insert <- insert@createDataInForActionGroupHasAllowedAction "Customer" "READ_TYPE_Admin"

    insert <- insert@createDataInForActionGroupHasAllowedAction "Customer" "READ_TYPE_Content Provider"

    insert <- insert@createDataInForActionGroupHasAllowedAction "Customer" "READ_TYPE_Customer"

    insert <- insert@createDataInForActionGroupHasAllowedAction "Customer" "CHANGE_EMAIL_OWN"
    insert <- insert@createDataInForActionGroupHasAllowedAction "Customer" "CHANGE_PASSWORD_OWN"
    insert <- insert@createDataInForActionGroupHasAllowedAction "Customer" "GIVE_CREDITS_OWN"
    insert <- insert@createDataInForActionGroupHasAllowedAction "Customer" "TAKE_CREDITS_OWN"
    insert <- insert@createDataInForActionGroupHasAllowedAction "Customer" "READ_AUTH_INFO_OWN"
    insert <- insert@createDataInForActionGroupHasAllowedAction "Customer" "RESET_PASSWORD_OWN"
    insert <- insert@createDataInForActionGroupHasAllowedAction "Customer" "READ_OWN"

    insert <- insert@createDataInForActionGroupHasAllowedAction "Content Provider" "READ_ANY"
    insert <- insert@createDataInForActionGroupHasAllowedAction "Content Provider" "CREATE_PRODUCT_ANY"
    insert <- insert@createDataInForActionGroupHasAllowedAction "Content Provider" "READ_PRODUCT_ANY"
    
    insert <- insert@createDataInForActionGroupHasAllowedAction "Content Provider" "READ_TYPE_Admin"

    insert <- insert@createDataInForActionGroupHasAllowedAction "Content Provider" "READ_TYPE_Content Provider"

    insert <- insert@createDataInForActionGroupHasAllowedAction "Content Provider" "READ_TYPE_Customer"

    insert <- insert@createDataInForActionGroupHasAllowedAction "Content Provider" "CHANGE_EMAIL_OWN"
    insert <- insert@createDataInForActionGroupHasAllowedAction "Content Provider" "CHANGE_PASSWORD_OWN"
    insert <- insert@createDataInForActionGroupHasAllowedAction "Content Provider" "READ_AUTH_INFO_OWN"
    insert <- insert@createDataInForActionGroupHasAllowedAction "Content Provider" "RESET_PASSWORD_OWN"
    insert <- insert@createDataInForActionGroupHasAllowedAction "Content Provider" "READ_OWN"
    insert <- insert@createDataInForActionGroupHasAllowedAction "Content Provider" "READ_UNPUBLISHED_PRODUCT_OWN"
    insert <- insert@createDataInForActionGroupHasAllowedAction "Content Provider" "EDIT_PRODUCT_OWN"
    insert <- insert@createDataInForActionGroupHasAllowedAction "Content Provider" "EDIT_UNPUBLISHED_PRODUCT_OWN"
    insert <- insert@createDataInForActionGroupHasAllowedAction "Content Provider" "PUBLISH_PRODUCT_OWN"
    insert <- insert@createDataInForActionGroupHasAllowedAction "Content Provider" "UPLOAD_MEDIA_OWN"
    insert <- insert@createDataInForActionGroupHasAllowedAction "Content Provider" "UPLOAD_THUMBNAIL_OWN"

    printfn "%A" ("---------- Create " + "ActionGroup_has_AllowedAction" + " ----------")
    for i in insert do
      Persistence.Api.create "ActionGroup_has_AllowedAction" i |> ignore

    // Create UserType
    // "Name"
    let mutable insert:((Persistence.Types.DataIn List) List) = []
    insert <- insert@createDataInForUserType "Admin"
    insert <- insert@createDataInForUserType "Unauth"
    insert <- insert@createDataInForUserType "Customer"
    insert <- insert@createDataInForUserType "Content Provider"

    printfn "%A" ("---------- Create " + "UserType" + " ----------")
    for i in insert do
      Persistence.Api.create "UserType" i |> ignore

    // Create UserType_has_ActionGroup
    // "UserType_Name" "ActionGroup_name
    let mutable insert:((Persistence.Types.DataIn List) List) = []
    insert <- insert@createDataInForUserTypeHasActionGroup "Admin" "Admin"
    insert <- insert@createDataInForUserTypeHasActionGroup "Unauth" "Unauth"
    insert <- insert@createDataInForUserTypeHasActionGroup "Customer" "Customer"
    insert <- insert@createDataInForUserTypeHasActionGroup "Content Provider" "Content Provider"
    
    printfn "%A" ("---------- Create " + "UserType_has_ActionGroup" + " ----------")
    for i in insert do
      Persistence.Api.create "UserType_has_ActionGroup" i |> ignore

    // Create Country
    // "Name"
    let mutable insert:((Persistence.Types.DataIn List) List) = []
    use sr = new System.IO.StreamReader("../.././country_list.csv")
    while not sr.EndOfStream do
      insert <- insert@createDataInForCountry (sr.ReadLine().Trim())

    insert <- insert@createDataInForCountry "Over the rainbow"

    printfn "%A" ("---------- Create " + "Country" + " ----------")
    for i in insert do
      Persistence.Api.create "Country" i |> ignore

    // User
    printfn "%A" ("---------- Create " + "User" + " ----------")
    let info = {
                  name = Some "Lynette";
                  address = ({
                               address = Some "Somewhere";
                               postal = Some 7738;
                               country = Some "Over the rainbow";
                            }:AccountTypes.Address);
                  birth = Some System.DateTime.Now;
                  about = None;
                  credits = Some 42;
               }:AccountTypes.ExtraAccInfo
    let user = Account.make "Admin" "Lynette" "lynette@smu" "Awesome" info
    Account.persist user
    let info = {
                  name = Some "Claus";
                  address = ({
                               address = Some "Somewhere";
                               postal = Some 7738;
                               country = Some "Over the rainbow";
                            }:AccountTypes.Address);
                  birth = Some System.DateTime.Now;
                  about = None;
                  credits = Some 42;
               }:AccountTypes.ExtraAccInfo
    let user = Account.make "Content Provider" "Claus" "asd@smu" "Claus" info
    Account.persist user

    // Product types
    let mutable insert:((Persistence.Types.DataIn List) List) = []
    insert <- insert@createDataInForProductType "ebook"
    insert <- insert@createDataInForProductType "audio"
    insert <- insert@createDataInForProductType "music"
    insert <- insert@createDataInForProductType "film"
    insert <- insert@createDataInForProductType "series"

    printfn "%A" ("---------- Create " + "ProductType" + " ----------")
    for i in insert do
      Persistence.Api.create "ProductType" i |> ignore

    // Mime types
    let mutable insert:((Persistence.Types.DataIn List) List) = []
    insert <- insert@createDataInForMimeType "ebook" "application/pdf"

    insert <- insert@createDataInForMimeType "audio" "audio/ogg"
    insert <- insert@createDataInForMimeType "audio" "audio/mpeg"
    insert <- insert@createDataInForMimeType "audio" "audio/mp4"
    insert <- insert@createDataInForMimeType "audio" "audio/mid"
    insert <- insert@createDataInForMimeType "audio" "audio/wav"
    insert <- insert@createDataInForMimeType "audio" "audio/x-wav"
    insert <- insert@createDataInForMimeType "audio" "audio/x-aiff"
    insert <- insert@createDataInForMimeType "audio" "audio/x-ms-wma"

    insert <- insert@createDataInForMimeType "music" "music/ogg"
    insert <- insert@createDataInForMimeType "music" "music/mpeg"
    insert <- insert@createDataInForMimeType "music" "music/mp4"
    insert <- insert@createDataInForMimeType "music" "music/mid"
    insert <- insert@createDataInForMimeType "music" "music/wav"
    insert <- insert@createDataInForMimeType "music" "music/x-wav"
    insert <- insert@createDataInForMimeType "music" "music/x-aiff"
    insert <- insert@createDataInForMimeType "music" "music/x-ms-wma"

    insert <- insert@createDataInForMimeType "film" "video/ogg"
    insert <- insert@createDataInForMimeType "film" "video/mp4"
    insert <- insert@createDataInForMimeType "film" "video/webm"
    insert <- insert@createDataInForMimeType "film" "video/H264"
    insert <- insert@createDataInForMimeType "film" "video/x-ms-wmv"
    
    insert <- insert@createDataInForMimeType "series" "video/ogg"
    insert <- insert@createDataInForMimeType "series" "video/mp4"
    insert <- insert@createDataInForMimeType "series" "video/webm"
    insert <- insert@createDataInForMimeType "series" "video/H264"
    insert <- insert@createDataInForMimeType "series" "video/x-ms-wmv"

    printfn "%A" ("---------- Create " + " MimeType" + " ----------")
    for i in insert do
      Persistence.Api.create "MimeType" i |> ignore


    0