namespace RentIt.Test

module ProductTest = 
  open Xunit
  open FsUnit.Xunit
  open RentIt

  [<Fact>]
  let ``check user type permission should false``() =
    let allowed = Permissions.checkUserTypePermission "Admin" "not a permission found in db"
    allowed |> should equal false