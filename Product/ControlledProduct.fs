namespace RentIt
open PermissionsUtil
open ControlledProductExceptions
open Controlled

module ControlledProduct =

        
    open ProductTypes    
    open AccountTypes
    open Product

    module internal Internal =


        let checkAllowed (invoker:Invoker) (access:Access) =
            match invoker with
                | Invoker.Auth auth when auth.banned -> raise AccountBanned
                | _                                  -> match access with
                                                        | Access.Denied string         -> raise PermissionDenied
                                                        | Access.Accepted       -> ignore; // Return normally in this case
                                                        

    open Internal
    
    ///
    ///
    let make (invoker:Invoker) (userName:string) (name:string) (productType:string) (description:string option) (buyPrice:int option) (rentPrice:int option) =
        let allowed = PermissionsUtil.check invoker "CREATE" (CheckTarget.Type "Own")
        Internal.checkAllowed invoker allowed |> ignore 
        Product.make userName name productType description buyPrice  rentPrice

    ///
    ///
    let getListOfproductTypes (invoker:Invoker) =
        let allowed = PermissionsUtil.check invoker "READ" (CheckTarget.Type "Any")
        Internal.checkAllowed invoker allowed |> ignore
        Product.getListOfProductTypes

    ///
    ///
    let getAll (invoker:Invoker)(status:PublishedStatus) =
        if(status = PublishedStatus.Published) then
            let allowed = PermissionsUtil.check invoker "READ" (CheckTarget.Type "Any")
            Internal.checkAllowed invoker allowed |> ignore
            Product.getAllProducts status
        else
            let allowed = PermissionsUtil.check invoker "READ_UNPUBLISHED" (CheckTarget.Type "Any")
            Internal.checkAllowed invoker allowed |> ignore
            Product.getAllProducts status    
    
    ///
    ///
    let getProductById (invoker:Invoker) (id:int) =
        let product = Product.getProductById(id)
        if(product.owner = (string (invokerToId invoker))) then
            let allowed = PermissionsUtil.check invoker "READ" (CheckTarget.Type "Own")
            Internal.checkAllowed invoker allowed |> ignore
            Product.getProductById id
         else
            let allowed = PermissionsUtil.check invoker "READ" (CheckTarget.Type "Any")
            Internal.checkAllowed invoker allowed |> ignore
            Product.getProductById id

    ///
    ///
    let getProductByName (invoker:Invoker) (pName:string) =
        let allowed = PermissionsUtil.check invoker "READ" (CheckTarget.Type "Any")
        Internal.checkAllowed invoker allowed |> ignore
        Product.getProductByName pName

    ///
    ///
    let getProductByType (invoker:Invoker) (pType:string) (status:PublishedStatus) =
        if(status = PublishedStatus.Published) then
            let allowed = PermissionsUtil.check invoker "READ" (CheckTarget.Type "Any")
            Internal.checkAllowed invoker allowed |> ignore
            Product.getAllProductsByType pType status
        else
            let allowed = PermissionsUtil.check invoker "READ_UNPUBLISHED" (CheckTarget.Type "Any")
            Internal.checkAllowed invoker allowed |> ignore
            Product.getAllProductsByType pType status

    ///
    ///
    let update (invoker:Invoker) (p:Product) =
        if((string (invokerToId invoker)) = p.owner) then 
            let allowed = PermissionsUtil.check invoker "EDIT" (CheckTarget.Type "Own")
            Internal.checkAllowed invoker allowed |> ignore
            Product.update p
        else 
            let allowed = PermissionsUtil.check invoker "EDIT" (CheckTarget.Type "Any")
            Internal.checkAllowed invoker allowed |> ignore
            Product.update p

    ///
    ///
    let publish (invoker:Invoker) (pId:int) (status:bool) =
        let product = Product.getProductById(pId)
        if((string (invokerToId invoker)) = product.owner) then
            let allowed = PermissionsUtil.check invoker "PUBLISH" (CheckTarget.Type "Own")
            Internal.checkAllowed invoker allowed |> ignore
            if(Product.hasMedia (uint32 pId)) then
                Product.publishProduct pId status
            else
                raise Conflict 

        else
            let allowed = PermissionsUtil.check invoker "PUBLISH" (CheckTarget.Type "Any")
            Internal.checkAllowed invoker allowed |> ignore
            if(Product.hasMedia (uint32 pId)) then
                Product.publishProduct pId status
            else
                raise Conflict
    
    ///
    ///
    let rateProduct (invoker:Invoker) (pId:int) (user:string) (rating:int) =
        let allowed = PermissionsUtil.check invoker "RATE_PRODUCT" (CheckTarget.Type "Any")
        Internal.checkAllowed invoker allowed |> ignore
        Product.rateProduct pId user rating

    ///
    ///
    let getAllProductsByUser (invoker:Invoker) (user:string) (status:PublishedStatus) =
        if(status = PublishedStatus.Published) then
            let allowed = PermissionsUtil.check invoker "READ" (CheckTarget.Type "Own")
            Internal.checkAllowed invoker allowed |> ignore
            Product.getAllProductsByUser user status
        else
            let allowed = PermissionsUtil.check invoker "READ_UNPUBLISHED" (CheckTarget.Type "Own")
            Internal.checkAllowed invoker allowed |> ignore
            Product.getAllProductsByUser user status

    ///
    ///
    let getProductByUserAndTitle (invoker:Invoker) (user:string) (pTitle:string) (status:PublishedStatus) =
        if(status = PublishedStatus.Published) then
            let allowed = PermissionsUtil.check invoker "READ" (CheckTarget.Type "Own")
            Internal.checkAllowed invoker allowed |> ignore
            Product.getAllProductsByUserAndTitle user pTitle status
        else
            let allowed = PermissionsUtil.check invoker "READ_UNPUBLISHED" (CheckTarget.Type "Own")
            Internal.checkAllowed invoker allowed |> ignore
            Product.getAllProductsByUserAndTitle user pTitle status

    ///
    ///
    let persistMediaThumbnail (invoker:Invoker) (id:uint32) (mime:string) (stream:System.IO.Stream) =
        let p = Product.getProductById (int id)
        if((string (invokerToId invoker)) = p.owner) then
            let allowed = PermissionsUtil.check invoker "UPLOAD_THUMBNAIL" (CheckTarget.Type "Own")
            Internal.checkAllowed invoker allowed |> ignore
            Product.persistMediaThumbnail id mime stream
        else
            let allowed = PermissionsUtil.check invoker "UPLOAD_THUMBNAIL" (CheckTarget.Type "Any")
            Internal.checkAllowed invoker allowed |> ignore
            Product.persistMediaThumbnail id mime stream

    ///
    ///
    let persitMedia (invoker:Invoker) (id:uint32) (mime:string) (stream:System.IO.Stream) =
        let p = Product.getProductById (int id)
        if((string (invokerToId invoker)) = p.owner) then
            let allowed = PermissionsUtil.check invoker "UPLOAD_MEDIA" (CheckTarget.Type "Own")
            Internal.checkAllowed invoker allowed |> ignore
            Product.persistMedia id mime stream
        else
            let allowed = PermissionsUtil.check invoker "UPLOAD_MEDIA" (CheckTarget.Type "Any")
            Internal.checkAllowed invoker allowed |> ignore
            Product.persistMedia id mime stream
