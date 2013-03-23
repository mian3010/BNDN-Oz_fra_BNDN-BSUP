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


  [<EntryPoint>]
  let main argv = 
    
    //Truncate data
    Persistence.Api.delete "User" [] |> ignore
    Persistence.Api.delete "ActionGroup_has_AllowedAction" [] |> ignore
    Persistence.Api.delete "AllowedAction" [] |> ignore
    Persistence.Api.delete "UserType_has_ActionGroup" [] |> ignore
    Persistence.Api.delete "ActionGroup" [] |> ignore
    Persistence.Api.delete "User" [] |> ignore
    Persistence.Api.delete "UserType" [] |> ignore
    Persistence.Api.delete "Country" [] |> ignore
    Persistence.Api.delete "Loggable" [] |> ignore

    // Create AllowedAction
    // "Name" "Description"
    let mutable insert:((Persistence.Types.DataIn List) List) = []
    insert <- insert@createDataInForAllowedAction "HAS_CREDITS" "Allows an account to have credits associated"
    

    insert <- insert@createDataInForAllowedAction "BAN_UNBAN_ANY" "Ban/unban accounts"
    insert <- insert@createDataInForAllowedAction "CHANGE_EMAIL_ANY" "CHANGE_EMAIL"
    insert <- insert@createDataInForAllowedAction "CHANGE_PASSWORD_ANY" "CHANGE_PASSWORD"
    insert <- insert@createDataInForAllowedAction "GIVE_CREDITS_ANY" "GIVE_CREDITS to an account which has credits"
    insert <- insert@createDataInForAllowedAction "TAKE_CREDITS_ANY" "TAKE_CREDITS from an account which has credits"
    insert <- insert@createDataInForAllowedAction "READ_AUTH_INFO_ANY" "Read last time of authentication"
    insert <- insert@createDataInForAllowedAction "RESET_PASSWORD_ANY" "RESET_PASSWORD"
    insert <- insert@createDataInForAllowedAction "READ_ANY" "READ account information"
    insert <- insert@createDataInForAllowedAction "EDIT_ANY" "EDIT additional account information"
    insert <- insert@createDataInForAllowedAction "CREATE_ANY" "CREATE account"

    insert <- insert@createDataInForAllowedAction "BAN_UNBAN_OWN" "Ban/unban accounts"
    insert <- insert@createDataInForAllowedAction "CHANGE_EMAIL_OWN" "CHANGE_EMAIL"
    insert <- insert@createDataInForAllowedAction "CHANGE_PASSWORD_OWN" "CHANGE_PASSWORD"
    insert <- insert@createDataInForAllowedAction "GIVE_CREDITS_OWN" "GIVE_CREDITS to an account which has credits"
    insert <- insert@createDataInForAllowedAction "TAKE_CREDITS_OWN" "TAKE_CREDITS from an account which has credits"
    insert <- insert@createDataInForAllowedAction "READ_AUTH_INFO_OWN" "Read last time of authentication"
    insert <- insert@createDataInForAllowedAction "RESET_PASSWORD_OWN" "RESET_PASSWORD"
    insert <- insert@createDataInForAllowedAction "READ_OWN" "READ account information"
    insert <- insert@createDataInForAllowedAction "EDIT_OWN" "EDIT additional account information"

    insert <- insert@createDataInForAllowedAction "BAN_UNBAN_TYPE_ADMIN" "Ban/unban accounts"
    insert <- insert@createDataInForAllowedAction "CHANGE_EMAIL_TYPE_ADMIN" "CHANGE_EMAIL"
    insert <- insert@createDataInForAllowedAction "CHANGE_PASSWORD_TYPE_ADMIN" "CHANGE_PASSWORD"
    insert <- insert@createDataInForAllowedAction "READ_AUTH_INFO_TYPE_ADMIN" "Read last time of authentication"
    insert <- insert@createDataInForAllowedAction "RESET_PASSWORD_TYPE_ADMIN" "RESET_PASSWORD"
    insert <- insert@createDataInForAllowedAction "READ_TYPE_ADMIN" "READ account information"
    insert <- insert@createDataInForAllowedAction "EDIT_TYPE_ADMIN" "EDIT additional account information"
    insert <- insert@createDataInForAllowedAction "CREATE_TYPE_ADMIN" "CREATE account"

    insert <- insert@createDataInForAllowedAction "BAN_UNBAN_TYPE_CONTENTPROVIDER" "Ban/unban accounts"
    insert <- insert@createDataInForAllowedAction "CHANGE_EMAIL_TYPE_CONTENTPROVIDER" "CHANGE_EMAIL"
    insert <- insert@createDataInForAllowedAction "CHANGE_PASSWORD_TYPE_CONTENTPROVIDER" "CHANGE_PASSWORD"
    insert <- insert@createDataInForAllowedAction "READ_AUTH_INFO_TYPE_CONTENTPROVIDER" "Read last time of authentication"
    insert <- insert@createDataInForAllowedAction "RESET_PASSWORD_TYPE_CONTENTPROVIDER" "RESET_PASSWORD"
    insert <- insert@createDataInForAllowedAction "READ_TYPE_CONTENTPROVIDER" "READ account information"
    insert <- insert@createDataInForAllowedAction "EDIT_TYPE_CONTENTPROVIDER" "EDIT additional account information"
    insert <- insert@createDataInForAllowedAction "CREATE_TYPE_CONTENTPROVIDER" "CREATE account"

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
    insert <- insert@createDataInForActionGroup "ContentProvider" "Content provider"

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

    insert <- insert@createDataInForActionGroupHasAllowedAction "Admin" "BAN_UNBAN_TYPE_Admin"
    insert <- insert@createDataInForActionGroupHasAllowedAction "Admin" "CHANGE_EMAIL_TYPE_Admin"
    insert <- insert@createDataInForActionGroupHasAllowedAction "Admin" "CHANGE_PASSWORD_TYPE_Admin"
    insert <- insert@createDataInForActionGroupHasAllowedAction "Admin" "READ_AUTH_INFO_TYPE_Admin"
    insert <- insert@createDataInForActionGroupHasAllowedAction "Admin" "RESET_PASSWORD_TYPE_Admin"
    insert <- insert@createDataInForActionGroupHasAllowedAction "Admin" "CREATE_TYPE_Admin"
    insert <- insert@createDataInForActionGroupHasAllowedAction "Admin" "READ_TYPE_Admin"

    insert <- insert@createDataInForActionGroupHasAllowedAction "Admin" "BAN_UNBAN_TYPE_ContentProvider"
    insert <- insert@createDataInForActionGroupHasAllowedAction "Admin" "CHANGE_EMAIL_TYPE_ContentProvider"
    insert <- insert@createDataInForActionGroupHasAllowedAction "Admin" "CHANGE_PASSWORD_TYPE_ContentProvider"
    insert <- insert@createDataInForActionGroupHasAllowedAction "Admin" "READ_AUTH_INFO_TYPE_ContentProvider"
    insert <- insert@createDataInForActionGroupHasAllowedAction "Admin" "RESET_PASSWORD_TYPE_ContentProvider"
    insert <- insert@createDataInForActionGroupHasAllowedAction "Admin" "CREATE_TYPE_ContentProvider"
    insert <- insert@createDataInForActionGroupHasAllowedAction "Admin" "READ_TYPE_ContentProvider"

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

    insert <- insert@createDataInForActionGroupHasAllowedAction "Unauth" "READ_TYPE_ContentProvider"

    insert <- insert@createDataInForActionGroupHasAllowedAction "Unauth" "READ_TYPE_Customer"
    
    insert <- insert@createDataInForActionGroupHasAllowedAction "Unauth" "READ_OWN"

    insert <- insert@createDataInForActionGroupHasAllowedAction "Customer" "READ_ANY"
    
    insert <- insert@createDataInForActionGroupHasAllowedAction "Customer" "READ_TYPE_Admin"

    insert <- insert@createDataInForActionGroupHasAllowedAction "Customer" "READ_TYPE_ContentProvider"

    insert <- insert@createDataInForActionGroupHasAllowedAction "Customer" "READ_TYPE_Customer"

    insert <- insert@createDataInForActionGroupHasAllowedAction "Customer" "CHANGE_EMAIL_OWN"
    insert <- insert@createDataInForActionGroupHasAllowedAction "Customer" "CHANGE_PASSWORD_OWN"
    insert <- insert@createDataInForActionGroupHasAllowedAction "Customer" "GIVE_CREDITS_OWN"
    insert <- insert@createDataInForActionGroupHasAllowedAction "Customer" "TAKE_CREDITS_OWN"
    insert <- insert@createDataInForActionGroupHasAllowedAction "Customer" "READ_AUTH_INFO_OWN"
    insert <- insert@createDataInForActionGroupHasAllowedAction "Customer" "RESET_PASSWORD_OWN"
    insert <- insert@createDataInForActionGroupHasAllowedAction "Customer" "READ_OWN"

    insert <- insert@createDataInForActionGroupHasAllowedAction "ContentProvider" "READ_ANY"
    
    insert <- insert@createDataInForActionGroupHasAllowedAction "ContentProvider" "READ_TYPE_Admin"

    insert <- insert@createDataInForActionGroupHasAllowedAction "ContentProvider" "READ_TYPE_ContentProvider"

    insert <- insert@createDataInForActionGroupHasAllowedAction "ContentProvider" "READ_TYPE_Customer"

    insert <- insert@createDataInForActionGroupHasAllowedAction "ContentProvider" "CHANGE_EMAIL_OWN"
    insert <- insert@createDataInForActionGroupHasAllowedAction "ContentProvider" "CHANGE_PASSWORD_OWN"
    insert <- insert@createDataInForActionGroupHasAllowedAction "ContentProvider" "READ_AUTH_INFO_OWN"
    insert <- insert@createDataInForActionGroupHasAllowedAction "ContentProvider" "RESET_PASSWORD_OWN"
    insert <- insert@createDataInForActionGroupHasAllowedAction "ContentProvider" "READ_OWN"

    printfn "%A" ("---------- Create " + "ActionGroup_has_AllowedAction" + " ----------")
    for i in insert do
      Persistence.Api.create "ActionGroup_has_AllowedAction" i |> ignore

    // Create UserType
    // "Name"
    let mutable insert:((Persistence.Types.DataIn List) List) = []
    insert <- insert@createDataInForUserType "Admin"
    insert <- insert@createDataInForUserType "Unauth"
    insert <- insert@createDataInForUserType "Customer"
    insert <- insert@createDataInForUserType "ContentProvider"

    printfn "%A" ("---------- Create " + "UserType" + " ----------")
    for i in insert do
      Persistence.Api.create "UserType" i |> ignore

    // Create UserType_has_ActionGroup
    // "UserType_Name" "ActionGroup_name
    let mutable insert:((Persistence.Types.DataIn List) List) = []
    insert <- insert@createDataInForUserTypeHasActionGroup "Admin" "Admin"
    insert <- insert@createDataInForUserTypeHasActionGroup "Unauth" "Unauth"
    insert <- insert@createDataInForUserTypeHasActionGroup "Customer" "Customer"
    insert <- insert@createDataInForUserTypeHasActionGroup "ContentProvider" "ContentProvider"
    
    printfn "%A" ("---------- Create " + "UserType_has_ActionGroup" + " ----------")
    for i in insert do
      Persistence.Api.create "UserType_has_ActionGroup" i |> ignore

    // Create Country
    // "Name"
    let mutable insert:((Persistence.Types.DataIn List) List) = []
    insert <- insert@createDataInForCountry "Denmark"
    insert <- insert@createDataInForCountry "Singapore"
    insert <- insert@createDataInForCountry "USA"
    
    printfn "%A" ("---------- Create " + "Country" + " ----------")
    for i in insert do
      Persistence.Api.create "Country" i |> ignore

    // Create Loggable
    // "Id"
    let mutable insert:((Persistence.Types.DataIn List) List) = []
    insert <- insert@createDataInForLoggable "1"
    insert <- insert@createDataInForLoggable "2"

    printfn "%A" ("---------- Create " + "Loggable" + " ----------")
    for i in insert do
      Persistence.Api.create "Loggable" i |> ignore

    0