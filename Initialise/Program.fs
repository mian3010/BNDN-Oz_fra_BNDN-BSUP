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
      [Persistence.DataIn.createDataIn (Persistence.DataIn.createDataIn [] "UserType_has_ActionGroup" "UserType_name" usertype) "UserType_has_ActionGroup" "ActionGroupe_name" group]


  [<EntryPoint>]
  let main argv = 
    // Create AllowedAction
    // "Name" "Description"
    let mutable insert:((Persistence.Types.DataIn List) List) = []
    insert <- insert@createDataInForAllowedAction "HAS_CREDITS" "Allows an account to have credits associated"
    insert <- insert@createDataInForAllowedAction "BAN_UNBAN" "Ban accounts"
    insert <- insert@createDataInForAllowedAction "CHANGE_EMAIL" "CHANGE_EMAIL"
    insert <- insert@createDataInForAllowedAction "CHANGE_PASSWORD" "CHANGE_PASSWORD"
    insert <- insert@createDataInForAllowedAction "GIVE_CREDITS" "GIVE_CREDITS"
    insert <- insert@createDataInForAllowedAction "TAKE_CREDITS" "TAKE_CREDITS"
    insert <- insert@createDataInForAllowedAction "READ_AUTH_INFO" "READ_AUTH_INFO"
    insert <- insert@createDataInForAllowedAction "RESET_PASSWORD" "RESET_PASSWORD"
    insert <- insert@createDataInForAllowedAction "CREATE" "CREATE"
    insert <- insert@createDataInForAllowedAction "READ" "READ"

    insert <- insert@createDataInForAllowedAction "CHANGE_EMAIL_OWN" "CHANGE_EMAIL"
    insert <- insert@createDataInForAllowedAction "CHANGE_PASSWORD_OWN" "CHANGE_PASSWORD"
    insert <- insert@createDataInForAllowedAction "READ_AUTH_INFO_OWN" "READ_AUTH_INFO"
    insert <- insert@createDataInForAllowedAction "RESET_PASSWORD_OWN" "RESET_PASSWORD"
    insert <- insert@createDataInForAllowedAction "CREATE_OWN" "CREATE"
    insert <- insert@createDataInForAllowedAction "READ_OWN" "READ"

    insert <- insert@createDataInForAllowedAction "BAN_UNBAN_TYPE_Unauth" "Ban accounts"
    insert <- insert@createDataInForAllowedAction "CHANGE_EMAIL_TYPE_Unauth" "CHANGE_EMAIL"
    insert <- insert@createDataInForAllowedAction "CHANGE_PASSWORD_TYPE_Unauth" "CHANGE_PASSWORD"
    insert <- insert@createDataInForAllowedAction "GIVE_CREDITS_TYPE_Unauth" "GIVE_CREDITS"
    insert <- insert@createDataInForAllowedAction "TAKE_CREDITS_TYPE_Unauth" "TAKE_CREDITS"
    insert <- insert@createDataInForAllowedAction "READ_AUTH_INFO_TYPE_Unauth" "READ_AUTH_INFO"
    insert <- insert@createDataInForAllowedAction "RESET_PASSWORD_TYPE_Unauth" "RESET_PASSWORD"
    insert <- insert@createDataInForAllowedAction "CREATE_TYPE_Unauth" "CREATE"
    insert <- insert@createDataInForAllowedAction "READ_TYPE_Unauth" "READ"

    insert <- insert@createDataInForAllowedAction "BAN_UNBAN_TYPE_OWN" "Ban accounts"
    insert <- insert@createDataInForAllowedAction "CHANGE_EMAIL_TYPE_OWN" "CHANGE_EMAIL"
    insert <- insert@createDataInForAllowedAction "CHANGE_PASSWORD_TYPE_OWN" "CHANGE_PASSWORD"
    insert <- insert@createDataInForAllowedAction "GIVE_CREDITS_TYPE_OWN" "GIVE_CREDITS"
    insert <- insert@createDataInForAllowedAction "TAKE_CREDITS_TYPE_OWN" "TAKE_CREDITS"
    insert <- insert@createDataInForAllowedAction "READ_AUTH_INFO_TYPE_OWN" "READ_AUTH_INFO"
    insert <- insert@createDataInForAllowedAction "RESET_PASSWORD_TYPE_OWN" "RESET_PASSWORD"
    insert <- insert@createDataInForAllowedAction "CREATE_TYPE_OWN" "CREATE"
    insert <- insert@createDataInForAllowedAction "READ_TYPE_OWN" "READ"

    insert <- insert@createDataInForAllowedAction "BAN_UNBAN_TYPE_Customer" "Ban accounts"
    insert <- insert@createDataInForAllowedAction "CHANGE_EMAIL_TYPE_Customer" "CHANGE_EMAIL"
    insert <- insert@createDataInForAllowedAction "CHANGE_PASSWORD_TYPE_Customer" "CHANGE_PASSWORD"
    insert <- insert@createDataInForAllowedAction "GIVE_CREDITS_TYPE_Customer" "GIVE_CREDITS"
    insert <- insert@createDataInForAllowedAction "TAKE_CREDITS_TYPE_Customer" "TAKE_CREDITS"
    insert <- insert@createDataInForAllowedAction "READ_AUTH_INFO_TYPE_Customer" "READ_AUTH_INFO"
    insert <- insert@createDataInForAllowedAction "RESET_PASSWORD_TYPE_Customer" "RESET_PASSWORD"
    insert <- insert@createDataInForAllowedAction "CREATE_TYPE_Customer" "CREATE"
    insert <- insert@createDataInForAllowedAction "READ_TYPE_Customer" "READ"

    insert <- insert@createDataInForAllowedAction "BAN_UNBAN_TYPE_Admin" "Ban accounts"
    insert <- insert@createDataInForAllowedAction "CHANGE_EMAIL_TYPE_Admin" "CHANGE_EMAIL"
    insert <- insert@createDataInForAllowedAction "CHANGE_PASSWORD_TYPE_Admin" "CHANGE_PASSWORD"
    insert <- insert@createDataInForAllowedAction "GIVE_CREDITS_TYPE_Admin" "GIVE_CREDITS"
    insert <- insert@createDataInForAllowedAction "TAKE_CREDITS_TYPE_Admin" "TAKE_CREDITS"
    insert <- insert@createDataInForAllowedAction "READ_AUTH_INFO_TYPE_Admin" "READ_AUTH_INFO"
    insert <- insert@createDataInForAllowedAction "RESET_PASSWORD_TYPE_Admin" "RESET_PASSWORD"
    insert <- insert@createDataInForAllowedAction "CREATE_TYPE_Admin" "CREATE"
    insert <- insert@createDataInForAllowedAction "READ_TYPE_Admin" "READ" 

    insert <- insert@createDataInForAllowedAction "BAN_UNBAN_TYPE_ContentProvider" "Ban accounts"
    insert <- insert@createDataInForAllowedAction "CHANGE_EMAIL_TYPE_ContentProvider" "CHANGE_EMAIL"
    insert <- insert@createDataInForAllowedAction "CHANGE_PASSWORD_TYPE_ContentProvider" "CHANGE_PASSWORD"
    insert <- insert@createDataInForAllowedAction "GIVE_CREDITS_TYPE_ContentProvider" "GIVE_CREDITS"
    insert <- insert@createDataInForAllowedAction "TAKE_CREDITS_TYPE_ContentProvider" "TAKE_CREDITS"
    insert <- insert@createDataInForAllowedAction "READ_AUTH_INFO_TYPE_ContentProvider" "READ_AUTH_INFO"
    insert <- insert@createDataInForAllowedAction "RESET_PASSWORD_TYPE_ContentProvider" "RESET_PASSWORD"
    insert <- insert@createDataInForAllowedAction "CREATE_TYPE_ContentProvider" "CREATE"
    insert <- insert@createDataInForAllowedAction "READ_TYPE_ContentProvider" "READ"

    printfn "%A" ("---------- Create " + "AllowedAction" + " ----------")
    for i in insert do
      Persistence.Api.create "AllowedAction" i |> ignore
    
    // Create ActionGroup
    // "Name" "Description"
    let mutable insert:((Persistence.Types.DataIn List) List) = []
    insert <- insert@createDataInForActionGroupHasAllowedAction "Admin" "Admin"
    insert <- insert@createDataInForActionGroupHasAllowedAction "Unauth" "Unauthenticated user"
    insert <- insert@createDataInForActionGroupHasAllowedAction "Customer" "Customer"
    insert <- insert@createDataInForActionGroupHasAllowedAction "ContentProvider" "Content provider"

    printfn "%A" ("---------- Create " + "ActionGroup" + " ----------")
    for i in insert do
      Persistence.Api.create "ActionGroup" i |> ignore
    
    // Create ActionGroup_has_AllowedAction
    // "ActionGroup_name" "AllowedAction_name"
    let mutable insert:((Persistence.Types.DataIn List) List) = []
    insert <- insert@createDataInForActionGroupHasAllowedAction "Admin" "BAN_UNBAN"
    insert <- insert@createDataInForActionGroupHasAllowedAction "Admin" "CHANGE_EMAIL"
    insert <- insert@createDataInForActionGroupHasAllowedAction "Admin" "CHANGE_PASSWORD"
    insert <- insert@createDataInForActionGroupHasAllowedAction "Admin" "GIVE_CREDITS"
    insert <- insert@createDataInForActionGroupHasAllowedAction "Admin" "TAKE_CREDITS"
    insert <- insert@createDataInForActionGroupHasAllowedAction "Admin" "READ_AUTH_INFO"
    insert <- insert@createDataInForActionGroupHasAllowedAction "Admin" "RESET_PASSWORD"
    insert <- insert@createDataInForActionGroupHasAllowedAction "Admin" "CREATE"
    insert <- insert@createDataInForActionGroupHasAllowedAction "Admin" "READ"

    insert <- insert@createDataInForActionGroupHasAllowedAction "Admin" "BAN_UNBAN_TYPE_Admin"
    insert <- insert@createDataInForActionGroupHasAllowedAction "Admin" "CHANGE_EMAIL_TYPE_Admin"
    insert <- insert@createDataInForActionGroupHasAllowedAction "Admin" "CHANGE_PASSWORD_TYPE_Admin"
    insert <- insert@createDataInForActionGroupHasAllowedAction "Admin" "GIVE_CREDITS_TYPE_Admin"
    insert <- insert@createDataInForActionGroupHasAllowedAction "Admin" "TAKE_CREDITS_TYPE_Admin"
    insert <- insert@createDataInForActionGroupHasAllowedAction "Admin" "READ_AUTH_INFO_TYPE_Admin"
    insert <- insert@createDataInForActionGroupHasAllowedAction "Admin" "RESET_PASSWORD_TYPE_Admin"
    insert <- insert@createDataInForActionGroupHasAllowedAction "Admin" "CREATE_TYPE_Admin"
    insert <- insert@createDataInForActionGroupHasAllowedAction "Admin" "READ_TYPE_Admin"

    insert <- insert@createDataInForActionGroupHasAllowedAction "Admin" "BAN_UNBAN_TYPE_ContentProvider"
    insert <- insert@createDataInForActionGroupHasAllowedAction "Admin" "CHANGE_EMAIL_TYPE_ContentProvider"
    insert <- insert@createDataInForActionGroupHasAllowedAction "Admin" "CHANGE_PASSWORD_TYPE_ContentProvider"
    insert <- insert@createDataInForActionGroupHasAllowedAction "Admin" "GIVE_CREDITS_TYPE_ContentProvider"
    insert <- insert@createDataInForActionGroupHasAllowedAction "Admin" "TAKE_CREDITS_TYPE_ContentProvider"
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

    insert <- insert@createDataInForActionGroupHasAllowedAction "Admin" "CREATE_TYPE_Unauth"
    insert <- insert@createDataInForActionGroupHasAllowedAction "Admin" "READ_TYPE_Unauth"
    
    insert <- insert@createDataInForActionGroupHasAllowedAction "Admin" "BAN_UNBAN_OWN"
    insert <- insert@createDataInForActionGroupHasAllowedAction "Admin" "CHANGE_EMAIL_OWN"
    insert <- insert@createDataInForActionGroupHasAllowedAction "Admin" "CHANGE_PASSWORD_OWN"
    insert <- insert@createDataInForActionGroupHasAllowedAction "Admin" "GIVE_CREDITS_OWN"
    insert <- insert@createDataInForActionGroupHasAllowedAction "Admin" "TAKE_CREDITS_OWN"
    insert <- insert@createDataInForActionGroupHasAllowedAction "Admin" "READ_AUTH_INFO_OWN"
    insert <- insert@createDataInForActionGroupHasAllowedAction "Admin" "RESET_PASSWORD_OWN"
    insert <- insert@createDataInForActionGroupHasAllowedAction "Admin" "CREATE_OWN"
    insert <- insert@createDataInForActionGroupHasAllowedAction "Admin" "READ_OWN"

    insert <- insert@createDataInForActionGroupHasAllowedAction "Unauth" "READ_AUTH_INFO"
    insert <- insert@createDataInForActionGroupHasAllowedAction "Unauth" "READ"

    insert <- insert@createDataInForActionGroupHasAllowedAction "Unauth" "READ_AUTH_INFO_TYPE_ContentProvider"
    insert <- insert@createDataInForActionGroupHasAllowedAction "Unauth" "READ_TYPE_ContentProvider"

    insert <- insert@createDataInForActionGroupHasAllowedAction "Unauth" "READ_AUTH_INFO_TYPE_Customer"
    insert <- insert@createDataInForActionGroupHasAllowedAction "Unauth" "READ_TYPE_Customer"

    insert <- insert@createDataInForActionGroupHasAllowedAction "Unauth" "READ_TYPE_Unauth"
    
    insert <- insert@createDataInForActionGroupHasAllowedAction "Unauth" "READ_AUTH_INFO_OWN"
    insert <- insert@createDataInForActionGroupHasAllowedAction "Unauth" "READ_OWN"

    insert <- insert@createDataInForActionGroupHasAllowedAction "Customer" "GIVE_CREDITS"
    insert <- insert@createDataInForActionGroupHasAllowedAction "Customer" "READ_AUTH_INFO"
    insert <- insert@createDataInForActionGroupHasAllowedAction "Customer" "CREATE"
    insert <- insert@createDataInForActionGroupHasAllowedAction "Customer" "READ"
    insert <- insert@createDataInForActionGroupHasAllowedAction "Customer" "HAS_CREDIT"

    insert <- insert@createDataInForActionGroupHasAllowedAction "Customer" "READ_AUTH_INFO_TYPE_Admin"
    insert <- insert@createDataInForActionGroupHasAllowedAction "Customer" "RESET_PASSWORD_TYPE_Admin"
    insert <- insert@createDataInForActionGroupHasAllowedAction "Customer" "CREATE_TYPE_Admin"
    insert <- insert@createDataInForActionGroupHasAllowedAction "Customer" "READ_TYPE_Admin"

    insert <- insert@createDataInForActionGroupHasAllowedAction "Customer" "GIVE_CREDITS_TYPE_ContentProvider"
    insert <- insert@createDataInForActionGroupHasAllowedAction "Customer" "READ_AUTH_INFO_TYPE_ContentProvider"
    insert <- insert@createDataInForActionGroupHasAllowedAction "Customer" "CREATE_TYPE_ContentProvider"
    insert <- insert@createDataInForActionGroupHasAllowedAction "Customer" "READ_TYPE_ContentProvider"

    insert <- insert@createDataInForActionGroupHasAllowedAction "Customer" "GIVE_CREDITS_TYPE_Customer"
    insert <- insert@createDataInForActionGroupHasAllowedAction "Customer" "READ_AUTH_INFO_TYPE_Customer"
    insert <- insert@createDataInForActionGroupHasAllowedAction "Customer" "CREATE_TYPE_Customer"
    insert <- insert@createDataInForActionGroupHasAllowedAction "Customer" "READ_TYPE_Customer"

    insert <- insert@createDataInForActionGroupHasAllowedAction "Customer" "CREATE_TYPE_Unauth"
    insert <- insert@createDataInForActionGroupHasAllowedAction "Customer" "READ_TYPE_Unauth"
    
    insert <- insert@createDataInForActionGroupHasAllowedAction "Customer" "CHANGE_EMAIL_OWN"
    insert <- insert@createDataInForActionGroupHasAllowedAction "Customer" "CHANGE_PASSWORD_OWN"
    insert <- insert@createDataInForActionGroupHasAllowedAction "Customer" "GIVE_CREDITS_OWN"
    insert <- insert@createDataInForActionGroupHasAllowedAction "Customer" "TAKE_CREDITS_OWN"
    insert <- insert@createDataInForActionGroupHasAllowedAction "Customer" "READ_AUTH_INFO_OWN"
    insert <- insert@createDataInForActionGroupHasAllowedAction "Customer" "RESET_PASSWORD_OWN"
    insert <- insert@createDataInForActionGroupHasAllowedAction "Customer" "CREATE_OWN"
    insert <- insert@createDataInForActionGroupHasAllowedAction "Customer" "READ_OWN"

    insert <- insert@createDataInForActionGroupHasAllowedAction "ContentProvider" "GIVE_CREDITS"
    insert <- insert@createDataInForActionGroupHasAllowedAction "ContentProvider" "TAKE_CREDITS"
    insert <- insert@createDataInForActionGroupHasAllowedAction "ContentProvider" "READ_AUTH_INFO"
    insert <- insert@createDataInForActionGroupHasAllowedAction "ContentProvider" "CREATE"
    insert <- insert@createDataInForActionGroupHasAllowedAction "ContentProvider" "READ"
    insert <- insert@createDataInForActionGroupHasAllowedAction "ContentProvider" "HAS_CREDIT"

    insert <- insert@createDataInForActionGroupHasAllowedAction "ContentProvider" "READ_AUTH_INFO_TYPE_Admin"
    insert <- insert@createDataInForActionGroupHasAllowedAction "ContentProvider" "CREATE_TYPE_Admin"
    insert <- insert@createDataInForActionGroupHasAllowedAction "ContentProvider" "READ_TYPE_Admin"

    insert <- insert@createDataInForActionGroupHasAllowedAction "ContentProvider" "GIVE_CREDITS_TYPE_ContentProvider"
    insert <- insert@createDataInForActionGroupHasAllowedAction "ContentProvider" "TAKE_CREDITS_TYPE_ContentProvider"
    insert <- insert@createDataInForActionGroupHasAllowedAction "ContentProvider" "READ_AUTH_INFO_TYPE_ContentProvider"
    insert <- insert@createDataInForActionGroupHasAllowedAction "ContentProvider" "CREATE_TYPE_ContentProvider"
    insert <- insert@createDataInForActionGroupHasAllowedAction "ContentProvider" "READ_TYPE_ContentProvider"

    insert <- insert@createDataInForActionGroupHasAllowedAction "ContentProvider" "GIVE_CREDITS_TYPE_Customer"
    insert <- insert@createDataInForActionGroupHasAllowedAction "ContentProvider" "TAKE_CREDITS_TYPE_Customer"
    insert <- insert@createDataInForActionGroupHasAllowedAction "ContentProvider" "READ_AUTH_INFO_TYPE_Customer"
    insert <- insert@createDataInForActionGroupHasAllowedAction "ContentProvider" "CREATE_TYPE_Customer"
    insert <- insert@createDataInForActionGroupHasAllowedAction "ContentProvider" "READ_TYPE_Customer"

    insert <- insert@createDataInForActionGroupHasAllowedAction "ContentProvider" "CREATE_TYPE_Unauth"
    insert <- insert@createDataInForActionGroupHasAllowedAction "ContentProvider" "READ_TYPE_Unauth"
    
    insert <- insert@createDataInForActionGroupHasAllowedAction "ContentProvider" "CHANGE_EMAIL_OWN"
    insert <- insert@createDataInForActionGroupHasAllowedAction "ContentProvider" "CHANGE_PASSWORD_OWN"
    insert <- insert@createDataInForActionGroupHasAllowedAction "ContentProvider" "GIVE_CREDITS_OWN"
    insert <- insert@createDataInForActionGroupHasAllowedAction "ContentProvider" "TAKE_CREDITS_OWN"
    insert <- insert@createDataInForActionGroupHasAllowedAction "ContentProvider" "READ_AUTH_INFO_OWN"
    insert <- insert@createDataInForActionGroupHasAllowedAction "ContentProvider" "RESET_PASSWORD_OWN"
    insert <- insert@createDataInForActionGroupHasAllowedAction "ContentProvider" "CREATE_OWN"
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

    0