namespace RentIt

module internal CreditsPermissions =

    open PermissionsUtil
    open AccountTypes
    open ProductTypes
    open AccountExceptions

    let internal checkAccountAndProductPermission accPermission prodPermission accountType product =
      let invokerHas = Permissions.checkUserTypePermission accountType accPermission
      let productHas = checkProduct product prodPermission
      if (invokerHas.Equals true && productHas.Equals Access.Accepted) then Access.Accepted
      else Access.Denied "No access"

    let mayPurchaseCredits (invoker:Invoker) (account:Account) (credits:int) = check invoker "HAS_CREDITS" (CheckTarget.Type account.accType)
        
    let mayBuyProduct (invoker:Invoker) (account:Account) (product:Product) = checkAccountAndProductPermission "HAS_CREDITS" "BUYABLE" account.accType product

    let mayRentProduct (invoker:Invoker) (account:Account) (product:Product) (days:int) = checkAccountAndProductPermission "HAS_CREDITS" "RENTABLE" account.accType product

    let mayGetTransaction (invoker:Invoker) (id:int) = Access.Accepted //TODO Quick fix

    let mayGetTransactions (invoker:Invoker) (account:Account) = Access.Accepted //TODO Quick fix

    let mayGetTransactionsByType (invoker:Invoker) (account:Account) (isRent:bool) = Access.Accepted //TODO Quick fix

    let mayCheckAccessToProduct (invoker:Invoker) (account:Account) (product:Product) = Access.Accepted //TODO Quick fix

    