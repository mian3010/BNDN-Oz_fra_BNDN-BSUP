namespace RentIt

module PermissionsUtil =

     // The different kinds of users which may wish to attempt actions
    type Invoker =    Auth of AccountTypes.Account  // authenticated invokers
                    | Unauth                        // anonymous invokers

    type Access =     Accepted
                    | Denied of string              // Reason why it was denied

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
                                                else Access.Denied ("The system has not granted invoker's account the permission "+permission+", or invoker is not allowed to perform its action on the given target")