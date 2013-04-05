namespace RentIt.Test

module TestAssignPermissionToProduct = 
  open Xunit
  open FsUnit.Xunit
  open RentIt

  [<Fact>]
  let ``Test assign permission to product should work``() =
    let p1 = Helper.createTestProduct "test as p"
    let permission = "test as p"
    try
      Permissions.addAllowedAction permission "test permission" |> ignore
      Permissions.assignPermissionToProduct p1.id permission |> ignore
      let allowed = Permissions.checkProductPermission p1.id permission
      allowed |> should equal true
    finally
      Permissions.unassignPermissionToProduct p1.id permission
      Helper.removeTestProduct "test as p"
      Permissions.deleteAllowedAction permission |> ignore