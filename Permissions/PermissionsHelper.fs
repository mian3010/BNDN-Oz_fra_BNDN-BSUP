namespace RentIt
open RentIt

  module PermissionsHelper =

    let internal findUserType id:string =
      // Find User Type for User
      let objectName = "User"
      let fieldsQ = Persistence.ReadField.createReadField [] objectName "Type_name"
      let joinsQ = []
      let filtersQ = Persistence.Filter.createFilter [] objectName "Type_name" "=" id
      let user = Persistence.Api.read fieldsQ objectName joinsQ filtersQ

      if user.Length = 0 then ""
      else   
      user.Item(0).Item("Type_name") //TODO Defens

    let internal checkUserTypePermission (usertype:string) (permission:string) :bool=
      // Find Action Groups with desired Action
      let objectName = "ActionGroup_has_AllowedAction"
      let fieldsQ = Persistence.ReadField.createReadField [] objectName "ActionGroup_name"
      let joinsQ = []
      let filtersQ = Persistence.Filter.createFilter [] objectName "AllowedAction_name" "=" permission
      let actionGroups = Persistence.Api.read fieldsQ objectName joinsQ filtersQ

      // Check if any results
      if not actionGroups.IsEmpty then 

        // Does User Type have Action Group?
        let mutable typeActions:List<Map<string,string>> = List.empty
        for actionGroup in actionGroups do
          let ac = actionGroup.Item("ActionGroup_name")
          let objectName = "UserType_has_ActionGroup"
          let fieldsQ = Persistence.ReadField.createReadField [] "UserType_name" usertype
          let joinsQ = []
          let filtersQ = Persistence.Filter.createFilter []       objectName "UserType_name" "=" usertype
          let filtersQ = Persistence.Filter.createFilter filtersQ objectName "ActionGroup_name" "=" ac
          typeActions <- Persistence.Api.read fieldsQ objectName joinsQ filtersQ
             
        // Does user have reference to Allowed Action?
        if typeActions.IsEmpty then false
        else true
      else true

    let internal checkUserPermission (id:string) (permission:string) :bool=
      let userType = findUserType id
      checkUserTypePermission userType permission