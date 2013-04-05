namespace RentIt

module internal CreditsPermissions =

    open PermissionsUtil
    open AccountTypes
    open ProductTypes

    let mayPurchaseCredits (invoker:Invoker) (account:Account) (credits:int) = Access.Denied "Not implemented"
        
    let mayBuyProduct (invoker:Invoker) (account:Account) (product:Product) = Access.Denied "Not implemented"

    let mayRentProduct (invoker:Invoker) (account:Account) (product:Product) (days:int) = Access.Denied "Not implemented"

    let mayGetTransaction (invoker:Invoker) (id:int) = Access.Denied "Not implemented"

    let mayGetTransactions (invoker:Invoker) (account:Account) = Access.Denied "Not implemented"

    let mayGetTransactionsByType (invoker:Invoker) (account:Account) (isRent:bool) = Access.Denied "Not implemented"

    let mayCheckAccessToProduct (invoker:Invoker) (account:Account) (product:Product) = Access.Denied "Not implemented"