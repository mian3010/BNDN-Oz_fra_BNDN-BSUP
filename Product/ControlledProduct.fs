namespace RentIt

module ControlledProduct =
    
    type Invoker =    Auth of AccountTypes.Account   // authenticated invokers
                        | Unauth                    // anonymous invokers

    type Access =     Accepted
                        | Denied         // Reason why it was denied

    exception AccountBanned              // Raised when a banned invoker attempts to perform an action
    exception PermissionDenied
        
        
    open AccountTypes
    open Product
    open Permissions

    module internal Internal =

            
        type CheckTarget =   Other of   Permissions.Target
                             | Type of    string

        let own = CheckTarget.Other Permissions.Target.Own
        let any = CheckTarget.Other Permissions.Target.Any

        let invokerToId = function
            | Invoker.Auth acc -> Permissions.Auth acc.user
            | Invoker.Unauth   -> Permissions.Unauth

        let check (invoker:Invoker) (permission:string) (target:CheckTarget) =
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
    
    let make (invoker:Invoker) (userName:string) (name:string) (productType:string) (description:string option) (buyPrice:int option) (rentPrice:int option) =
        let allowed = Internal.check invoker "CREATE" invoker "OWN"
            Internal.checkAllowed invoker allowd |> ignore 
            Product.make userName name productType description buyPrice  rentPrice
