namespace RentIt

module ControlledProduct =
    
    type Invoker =     Auth of AccountTypes.Account   // authenticated invokers
                     | Unauth                    // anonymous invokers

    type Access =       Accepted
                      | Denied         // Reason why it was denied

    exception AccountBanned              // Raised when a banned invoker attempts to perform an action
    exception PermissionDenied
        
    open ProductTypes    
    open AccountTypes
    open Product

    module internal Internal =

            
        type CheckTarget =     Other of   Permissions.Target
                             | Type of    string

        let own = CheckTarget.Other Permissions.Target.Own
        let any = CheckTarget.Other Permissions.Target.Any

        let invokerToId = function
            | Invoker.Auth acc -> Permissions.Auth acc.user
            | Invoker.Unauth   -> Permissions.Unauth

        let checkUser (invoker:Invoker) (permission:string) (target:CheckTarget) =
            match invoker with
            | Invoker.Auth acc when acc.banned  ->  Access.Denied "Invokers account is banned"
            | _                                 ->  let check = Permissions.checkUserPermission (invokerToId invoker) permission
                                                    let hasPermission = match target with
                                                                        | Other x -> check x
                                                                        | Type t  -> check (Permissions.Target.Type t)
                                                    if hasPermission then Access.Accepted
                                                    else Access.Denied

        let checkAllowed (invoker:Invoker) (access:Access) =
            match invoker with
                | Invoker.Auth auth when auth.banned -> raise AccountBanned
                | _                                  -> match access with
                                                        | Access.Denied         -> raise PermissionDenied
                                                        | Access.Accepted       -> ignore; // Return normally in this case
                                                        

    open Internal
    
    ///
    ///
    let make (invoker:Invoker) (userName:string) (name:string) (productType:string) (description:string option) (buyPrice:int option) (rentPrice:int option) =
        let allowed = Internal.checkUser invoker "CREATE" (CheckTarget.Type "Own")
        Internal.checkAllowed invoker allowed |> ignore 
        Product.make userName name productType description buyPrice  rentPrice

    ///
    ///
    let getListOfproductTypes (invoker:Invoker) =
        let allowed = Internal.checkUser invoker "READ" (CheckTarget.Type "Any")
        Internal.checkAllowed invoker allowed |> ignore
        Product.getListOfProductTypes

    ///
    ///
    let getAll (invoker:Invoker) =
        let allowed = Internal.checkUser invoker "READ" (CheckTarget.Type "Any")
        Internal.checkAllowed invoker allowed |> ignore
        Product.getAll
    
    ///
    ///
    let getProductById (invoker:Invoker) (id:int) =
        let product = Product.getProductById(id)
        if(product.owner = (invokerToId invoker)) then
            let allowed = Internal.checkUser invoker "READ" (CheckTarget.Type "Own")
            Internal.checkAllowed invoker allowed |> ignore
            Product.getProductById id
         else
            let allowed = Internal.checkUser invoker "READ" (CheckTarget.Type "Any")
            Internal.checkAllowed invoker allowed |> ignore
            Product.getProductById id

    ///
    ///
    let getProductByName (invoker:Invoker) (pName:string) =
        let allowed = Internal.checkUser invoker "READ" (CheckTarget.Type "Any")
        Internal.checkAllowed invoker allowed |> ignore
        Product.getProductByName pName

    ///
    ///
    let getAllByType (invoker:Invoker) (typeName:string) =
        let allowed = Internal.checkUser invoker "READ" (CheckTarget.Type "Any")
        Internal.checkAllowed invoker allowed |> ignore
        Product.getAllByType typeName

    ///
    ///
    let update (invoker:Invoker) (p:Product) =
        if((invokerToId invoker) = p.owner) then 
            let allowed = Internal.checkUser invoker "EDIT" (CheckTarget.Type "Own")
            Internal.checkAllowed invoker allowed |> ignore
            Product.update p
        else 
            let allowed = Internal.checkUser invoker "EDIT" (CheckTarget.Type "Any")
            Internal.checkAllowed invoker allowed |> ignore
            Product.update p

    ///
    ///
    let publish (invoker:Invoker) (pId:int) (status:bool) =
        let product = Product.getProductById(pId)
        if((invokerToId invoker) = product.owner) then
            let allowed = Internal.checkUser invoker "PUBLISH" (CheckTarget.Type "Own")
            Internal.checkAllowed invoker allowed |> ignore
            Product.publishProduct pId status

        else
            let allowed = Internal.checkUser invoker "PUBLISH" (CheckTarget.Type "Any")
            Internal.checkAllowed invoker allowed |> ignore
            Product.publishProduct pId status
    
    ///
    ///
    let rateProduct (invoker:Invoker) (pId:int) (user:string) (rating:int) =
        let allowed = Internal.checkUser invoker "RATE_PRODUCT" (CheckTarget.Type "Any")
        Internal.checkAllowed invoker allowed |> ignore
        Product.rateProduct pId user rating

    ///
    ///
    let getAllProductsByUser (invoker:Invoker) (user:string) (showPublished:PublishedStatus) =
        if(showPublished = PublishedStatus.Published) then
            let allowed = Internal.checkUser invoker "READ" (CheckTarget.Type "Own")
            Internal.checkAllowed invoker allowed |> ignore
            Product.getAllProductsByUser user showPublished
        else
            let allowed = Internal.checkUser invoker "READ_UNPUBLISHED" (CheckTarget.Type "Own")
            Internal.checkAllowed invoker allowed |> ignore
            Product.getAllProductsByUser user showPublished


                