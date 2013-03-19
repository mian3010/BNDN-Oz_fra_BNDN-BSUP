namespace RentIt

  module Permissions = 
    
    type Identity =   Auth of string // username
                    | Unauth         // unauthenticated users
    
    let internal findUserType id:string =
      // Find User Type for User
      let user = Persistence.Get "User" [{field="id";operator="=";value=id}] 
      if user.Length = 0 then ""
      else   
      userType = user.Item(0).Item("Type_name") //TODO Defens

    let checkUserTypePermission (usertype:string) (permission:string) :bool=
    // Find Action Groups with desired Action
      let actionGroups = Persistence.Get "ActionGroup_has_AllowedAction" [{field="AllowedAction_name";operator="=";value=permission}]

      // Check if any results
      if not actionGroups.IsEmpty then 

        // Does User Type have Action Group?
        let mutable typeActions:List<Map<string,string>> = List.empty
        for actionGroup in actionGroups do
          let ac = actionGroup.Item("Name")
          typeActions <- Persistence.Get "UserType_has_ActionGroup" [{field="UserType_name";operator="=";value=usertype}; {field="ActionGroup_name";operator="=";value=ac}]
             
        // Does user have reference to Allowed Action?
        if typeActions.IsEmpty then false
        else true
      else true

    let internal checkUserPermission (id:string) (permission:string) :bool=
      let userType = findUserType id
      checkUserTypePermission userType permission
    
    // Check if a User has Permission to Action
    // Takes UserId and Permission. Both as strings
    // Returns true/false
    let CheckUserPermission (id:Identity) (permission:string) :bool =
      match id with
      | Unauth -> checkUserTypePermission "Unauth" permission
      | Auth x -> checkUserPermission x permission

    // Assign a Permission to an ActionGroup
    // Takes ActionGroup Name and Permission. Both as strings
    let AssignPermissionToActionGroup (name:string) (permission:string) =
      let mutable data:Map<string,string> = Map.empty
      data <- data.Add("ActionGroup_name", name)
      data <- data.Add("AllowedAction_name", permission)

      Persistence.Insert "ActionGroup_has_AllowedAction" data

    // Unassign a Permission to an ActionGroup
    // Takes ActionGroup Name and Permission. Both as strings
    let UnassignPermissionFromActionGroup (name:string) (permission:string) = 
      Persistence.Delete "ActionGroup_has_AllowedAction" [{field="ActionGroup_name";operator="=";value=name}; {field="AllowedAction_name";operator="=";value=permission}]

    // Add Allowed Action
    // Takes Action Name and Description. Both strings.
    let AddAllowedAction (name:string) (desc:string) =
      let mutable data:Map<string,string> = Map.empty
      data <- data.Add("Name", name)
      data <- data.Add("Description", desc)
      
      Persistence.Insert "AllowedAction" data

    // Delete Allowed Action
    // Takes Allowed Action Name
    let DeleteAllowedAction (name:string) =
      Persistence.Delete "AllowedAction" [{field="Name";operator="=";value=name}]

    // Add Action Group
    // Takes Name and Description
    let AddActionGroup (name:string) (desc:string) =
      let mutable data:Map<string,string> = Map.empty
      data <- data.Add("Name", name)
      data <- data.Add("Description", desc)
      
      Persistence.Insert "ActionGroup" data

    // Delete Action Group
    // Takes Action Group Name
    let DeleteActionGroup (name:string) =
      Persistence.Delete "ActionGroup" [{field="Name";operator="=";value=name}]

    // Assign Action Group to User Type
    // Takes User Type Name and Action Group Name
    let AssignActionGroupToUserType (userType:string) (actionGroup:string) = 
      let mutable data:Map<string,string> = Map.empty
      data <- data.Add("UserType_name", userType)
      data <- data.Add("ActionGroup_name", actionGroup)
      
      Persistence.Insert "UserType_has_ActionGroup" data

    // Unassign Action Group to User Type
    // Takes User Type Name and Action Group Name
    let UnassignActionGroupToUserType (userType:string) (actionGroup:string) = 
      Persistence.Delete "UserType_has_ActionGroup" [{field="UserType_name";operator="=";value=userType}; {field="ActionGroup_name";operator="=";value=actionGroup}]

    // Add User Type
    // Takes User Type Name
    let AddUserType (name:string) = 
      let mutable data:Map<string,string> = Map.empty
      data <- data.Add("Name", name)
      
      Persistence.Insert "UserType" data

    // Delete User Type
    // Takes User Type Name
    // Returns false if a User has User Type
    let DeleteUserType (name:string) :bool =
      // Check if in use
      let result = Persistence.Get "User" [{field="Type_name";operator="=";value=name}]
      if not result.IsEmpty then false
      else
        // Delete
        Persistence.Delete "UserType" [{field="Name";operator="=";value=name}]