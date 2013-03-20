namespace RentIt.Permissions
open RentIt

  module PermissionsHelper =

    let internal findUserType id:string =
      // Find User Type for User
      let user = Persistence.Get "User" [{field="id";operator="=";value=id}] 
      if user.Length = 0 then ""
      else   
      userType = user.Item(0).Item("Type_name") //TODO Defens

    let internal checkUserTypePermission (usertype:string) (permission:string) :bool=
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