namespace RentIt
open RentIt

  module PermissionsHelper =

    let internal findUserType id:string =
      // Find User Type for User
      let objectName = "User"
      let fieldsQ = Persistence.ReadField.createReadField [] objectName "Type_name"
      let joinsQ = []
      let filtersQ = Persistence.Filter.createFilter [] objectName "Username" "=" id
      let user = Persistence.Api.read fieldsQ objectName joinsQ filtersQ

      if user.Length = 0 then ""
      else   
      user.Item(0).Item("Type_name") //TODO Defens

    let internal checkUserTypePermission (usertype:string) (permission:string) :bool=
      // Find Action Groups with desired Action
      let objectName = "ActionGroup_has_AllowedAction"
      let fieldsQ = Persistence.ReadField.createReadField [] objectName "ActionGroup_name"
      let joinsQ = ref (Persistence.ObjectJoin.createObjectJoin [] "ActionGroup_has_AllowedAction" "ActionGroup_name" "ActionGroup" "Name")
      joinsQ := Persistence.ObjectJoin.createObjectJoin !joinsQ "ActionGroup" "Name" "UserType_has_ActionGroup" "ActionGroup_name"
      let filtersQ = ref (Persistence.Filter.createFilter [] objectName "AllowedAction_name" "=" permission)
      filtersQ := Persistence.Filter.createFilter !filtersQ "UserType_has_ActionGroup" "UserType_name" "=" usertype
      let actionGroups = Persistence.Api.read fieldsQ objectName !joinsQ !filtersQ

      // Check if any results
      if not actionGroups.IsEmpty then true
      else false

    let internal checkUserPermission (id:string) (permission:string) :bool=
      let userType = findUserType id
      checkUserTypePermission userType permission