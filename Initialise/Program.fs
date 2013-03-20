namespace RentIt

module Main = 

  let internal createDataInForPermission name description =
      [Persistence.DataIn.createDataIn (Persistence.DataIn.createDataIn [] "AllowedAction" "Name" name) "AllowedAction" "Description" description]

  [<EntryPoint>]
  let main argv = 
    // Create Permissions
    let objectName = "AllowedAction"
    // "Name" "Description"
    let mutable insert:((Persistence.Types.DataIn List) List) = []
    insert <- insert@createDataInForPermission "HAS_CREDITS" "Allows an account to have credits associated"
    insert <- insert@createDataInForPermission "BAN_UNBAN" "Ban accounts"
    insert <- insert@createDataInForPermission "CHANGE_EMAIL" "CHANGE_EMAIL"
    insert <- insert@createDataInForPermission "CHANGE_PASSWORD" "CHANGE_PASSWORD"
    insert <- insert@createDataInForPermission "GIVE_CREDITS" "GIVE_CREDITS"
    insert <- insert@createDataInForPermission "TAKE_CREDITS" "TAKE_CREDITS"
    insert <- insert@createDataInForPermission "READ_AUTH_INFO" "READ_AUTH_INFO"
    insert <- insert@createDataInForPermission "RESET_PASSWORD" "RESET_PASSWORD"
    insert <- insert@createDataInForPermission "CREATE" "CREATE"
    insert <- insert@createDataInForPermission "READ" "READ"

    insert <- insert@createDataInForPermission "CHANGE_EMAIL_OWN" "CHANGE_EMAIL"
    insert <- insert@createDataInForPermission "CHANGE_PASSWORD_OWN" "CHANGE_PASSWORD"
    insert <- insert@createDataInForPermission "READ_AUTH_INFO_OWN" "READ_AUTH_INFO"
    insert <- insert@createDataInForPermission "RESET_PASSWORD_OWN" "RESET_PASSWORD"
    insert <- insert@createDataInForPermission "CREATE_OWN" "CREATE"
    insert <- insert@createDataInForPermission "READ_OWN" "READ"

    insert <- insert@createDataInForPermission "BAN_UNBAN_TYPE_Unauth" "Ban accounts"
    insert <- insert@createDataInForPermission "CHANGE_EMAIL_TYPE_Unauth" "CHANGE_EMAIL"
    insert <- insert@createDataInForPermission "CHANGE_PASSWORD_TYPE_Unauth" "CHANGE_PASSWORD"
    insert <- insert@createDataInForPermission "GIVE_CREDITS_TYPE_Unauth" "GIVE_CREDITS"
    insert <- insert@createDataInForPermission "TAKE_CREDITS_TYPE_Unauth" "TAKE_CREDITS"
    insert <- insert@createDataInForPermission "READ_AUTH_INFO_TYPE_Unauth" "READ_AUTH_INFO"
    insert <- insert@createDataInForPermission "RESET_PASSWORD_TYPE_Unauth" "RESET_PASSWORD"
    insert <- insert@createDataInForPermission "CREATE_TYPE_Unauth" "CREATE"
    insert <- insert@createDataInForPermission "READ_TYPE_Unauth" "READ"

    insert <- insert@createDataInForPermission "BAN_UNBAN_TYPE_" "Ban accounts"
    insert <- insert@createDataInForPermission "CHANGE_EMAIL_TYPE_" "CHANGE_EMAIL"
    insert <- insert@createDataInForPermission "CHANGE_PASSWORD_TYPE_" "CHANGE_PASSWORD"
    insert <- insert@createDataInForPermission "GIVE_CREDITS_TYPE_" "GIVE_CREDITS"
    insert <- insert@createDataInForPermission "TAKE_CREDITS_TYPE_" "TAKE_CREDITS"
    insert <- insert@createDataInForPermission "READ_AUTH_INFO_TYPE_" "READ_AUTH_INFO"
    insert <- insert@createDataInForPermission "RESET_PASSWORD_TYPE_" "RESET_PASSWORD"
    insert <- insert@createDataInForPermission "CREATE_TYPE_" "CREATE"
    insert <- insert@createDataInForPermission "READ_TYPE_" "READ"

    insert <- insert@createDataInForPermission "BAN_UNBAN_TYPE_Customer" "Ban accounts"
    insert <- insert@createDataInForPermission "CHANGE_EMAIL_TYPE_Customer" "CHANGE_EMAIL"
    insert <- insert@createDataInForPermission "CHANGE_PASSWORD_TYPE_Customer" "CHANGE_PASSWORD"
    insert <- insert@createDataInForPermission "GIVE_CREDITS_TYPE_Customer" "GIVE_CREDITS"
    insert <- insert@createDataInForPermission "TAKE_CREDITS_TYPE_Customer" "TAKE_CREDITS"
    insert <- insert@createDataInForPermission "READ_AUTH_INFO_TYPE_Customer" "READ_AUTH_INFO"
    insert <- insert@createDataInForPermission "RESET_PASSWORD_TYPE_Customer" "RESET_PASSWORD"
    insert <- insert@createDataInForPermission "CREATE_TYPE_Customer" "CREATE"
    insert <- insert@createDataInForPermission "READ_TYPE_Customer" "READ"

    insert <- insert@createDataInForPermission "BAN_UNBAN_TYPE_Admin" "Ban accounts"
    insert <- insert@createDataInForPermission "CHANGE_EMAIL_TYPE_Admin" "CHANGE_EMAIL"
    insert <- insert@createDataInForPermission "CHANGE_PASSWORD_TYPE_Admin" "CHANGE_PASSWORD"
    insert <- insert@createDataInForPermission "GIVE_CREDITS_TYPE_Admin" "GIVE_CREDITS"
    insert <- insert@createDataInForPermission "TAKE_CREDITS_TYPE_Admin" "TAKE_CREDITS"
    insert <- insert@createDataInForPermission "READ_AUTH_INFO_TYPE_Admin" "READ_AUTH_INFO"
    insert <- insert@createDataInForPermission "RESET_PASSWORD_TYPE_Admin" "RESET_PASSWORD"
    insert <- insert@createDataInForPermission "CREATE_TYPE_Admin" "CREATE"
    insert <- insert@createDataInForPermission "READ_TYPE_Admin" "READ" 

    insert <- insert@createDataInForPermission "BAN_UNBAN_TYPE_ContentProvider" "Ban accounts"
    insert <- insert@createDataInForPermission "CHANGE_EMAIL_TYPE_ContentProvider" "CHANGE_EMAIL"
    insert <- insert@createDataInForPermission "CHANGE_PASSWORD_TYPE_ContentProvider" "CHANGE_PASSWORD"
    insert <- insert@createDataInForPermission "GIVE_CREDITS_TYPE_ContentProvider" "GIVE_CREDITS"
    insert <- insert@createDataInForPermission "TAKE_CREDITS_TYPE_ContentProvider" "TAKE_CREDITS"
    insert <- insert@createDataInForPermission "READ_AUTH_INFO_TYPE_ContentProvider" "READ_AUTH_INFO"
    insert <- insert@createDataInForPermission "RESET_PASSWORD_TYPE_ContentProvider" "RESET_PASSWORD"
    insert <- insert@createDataInForPermission "CREATE_TYPE_ContentProvider" "CREATE"
    insert <- insert@createDataInForPermission "READ_TYPE_ContentProvider" "READ"

    printfn "%A" ("---------- Create " + objectName + " ----------")
    for i in insert do
      Persistence.Api.create objectName i |> ignore
    
    0