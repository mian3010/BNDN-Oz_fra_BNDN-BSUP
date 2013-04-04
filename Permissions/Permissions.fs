namespace RentIt
open RentIt

  module Permissions = 
    
    type Identity =   Auth of string // Username
                    | Unauth         // Unauthenticated users
    
    type Target =     Type of string // Target type
                    | Own            // Own
                    | Any            // Not specific type
    
    let checkUserTypePermission (usertype:string) (permission:string) :bool =
      PermissionsHelper.checkUserTypePermission usertype permission
    
    // Check if a User has Permission to Action
    // Takes UserId and Permission. Both as strings
    // Returns true/false
    let checkUserPermission (id:Identity) (permission:string) (tp:Target) :bool =
      let perm = match tp with
                 | Type x -> permission + "_TYPE_" + x
                 | Own -> permission + "_OWN"
                 | _ -> permission + "_ANY"

      let result =
        match id with
        | Unauth -> PermissionsHelper.checkUserTypePermission "Unauth" (permission + "_ANY")
        | Auth x -> PermissionsHelper.checkUserPermission x (permission + "_ANY")
      
      if not result then
        match id with
        | Unauth -> PermissionsHelper.checkUserTypePermission "Unauth" perm
        | Auth x -> PermissionsHelper.checkUserPermission x perm
      else result

    let checkProductPermission (id:int) (permission:string) : bool =
      // Find Product_has_AllowedAction
      // WHERE Product_Id = id
      // WHERE AllowedAction = permission
      let objectName = "Product_has_AllowedAction"
      let fieldsQ = Persistence.ReadField.createReadFieldProc [] "" "" Persistence.ReadField.All
      let filtersQ = ref (Persistence.Filter.createFilter [] objectName "Product_Id" "=" (string id))
      filtersQ := Persistence.Filter.createFilter !filtersQ objectName "AllowedAction" "=" permission
      let permissions = Persistence.Api.read fieldsQ objectName [] !filtersQ

      // Check if any results
      if not permissions.IsEmpty then true
      else false

    // Assign a Permission to an ActionGroup
    // Takes ActionGroup Name and Permission. Both as strings
    let assignPermissionToActionGroup (name:string) (permission:string) =
      let objectName = "ActionGroup_has_AllowedAction"
      let fieldProcessor = Persistence.Field.Default
      let dataQ = Persistence.DataIn.createDataIn []    objectName "ActionGroup_name" name
      let dataQ = Persistence.DataIn.createDataIn dataQ objectName "AllowedAction_name" permission
      
      Persistence.Api.create objectName dataQ
      
    // Unassign a Permission to an ActionGroup
    // Takes ActionGroup Name and Permission. Both as strings
    let unassignPermissionFromActionGroup (name:string) (permission:string) = 
      let objectName = "ActionGroup_has_AllowedAction"
      let filtersQ = Persistence.Filter.createFilter [] "ActionGroup_has_AllowedAction" "ActionGroup_name" "=" name
      let filtersQ = Persistence.Filter.createFilter filtersQ "ActionGroup_has_AllowedAction" "AllowedAction_name" "=" permission
      
      Persistence.Api.delete objectName filtersQ

    // Add Allowed Action
    // Takes Action Name and Description. Both strings.
    let addAllowedAction (name:string) (desc:string) =
      let objectName = "AllowedAction"
      let dataQ = Persistence.DataIn.createDataIn []    objectName "Name" name
      let dataQ = Persistence.DataIn.createDataIn dataQ objectName "Description" desc
      
      Persistence.Api.create objectName dataQ

    // Delete Allowed Action
    // Takes Allowed Action Name
    let deleteAllowedAction (name:string) =
      let objectName = "AllowedAction"
      let filtersQ = Persistence.Filter.createFilter [] objectName "Name" "=" name

      Persistence.Api.delete objectName filtersQ

    // Add Action Group
    // Takes Name and Description
    let addActionGroup (name:string) (desc:string) =
      let objectName = "ActionGroup"
      let dataQ = Persistence.DataIn.createDataIn []    objectName "Name" name
      let dataQ = Persistence.DataIn.createDataIn dataQ objectName "Description" desc
      
      Persistence.Api.create objectName dataQ

    // Delete Action Group
    // Takes Action Group Name
    let deleteActionGroup (name:string) =
      let objectName = "ActionGroup"
      let filtersQ = Persistence.Filter.createFilter [] objectName "Name" "=" name

      Persistence.Api.delete objectName filtersQ

    // Assign Action Group to User Type
    // Takes User Type Name and Action Group Name
    let assignActionGroupToUserType (userType:string) (actionGroup:string) = 
      let objectName = "UserType_has_ActionGroup"
      let dataQ = Persistence.DataIn.createDataIn []    objectName "UserType_name" userType
      let dataQ = Persistence.DataIn.createDataIn dataQ objectName "ActionGroup_name" actionGroup
      
      Persistence.Api.create objectName dataQ

    // Unassign Action Group to User Type
    // Takes User Type Name and Action Group Name
    let unassignActionGroupToUserType (userType:string) (actionGroup:string) = 
      let objectName = "UserType_has_ActionGroup"
      let filtersQ = Persistence.Filter.createFilter []       objectName "UserType_name" "=" userType
      let filtersQ = Persistence.Filter.createFilter filtersQ objectName "ActionGroup_name" "=" actionGroup

      Persistence.Api.delete objectName filtersQ

    // Add User Type
    // Takes User Type Name
    let addUserType (name:string) = 
      let objectName = "UserType"
      let dataQ = Persistence.DataIn.createDataIn [] objectName "Name" name

      Persistence.Api.create objectName dataQ

    // Delete User Type
    // Takes User Type Name
    // Returns false if a User has User Type
    let deleteUserType (name:string) :bool =
      let objectName = "User"
      let fieldsQ = Persistence.ReadField.createReadField [] objectName "Type_name" 
      let joinsQ = []
      let filtersQ = Persistence.Filter.createFilter [] objectName "Type_name" "=" name
      let readR = Persistence.Api.read fieldsQ objectName joinsQ filtersQ
      
      if not readR.IsEmpty then false
      else
        let objectName = "UserType"
        let filtersQ = Persistence.Filter.createFilter [] objectName "Name" "=" name
        Persistence.Api.delete objectName filtersQ |> ignore
        true //TODO Add check
      