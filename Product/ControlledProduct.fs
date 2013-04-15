namespace RentIt
open PermissionsUtil
open ControlledProductExceptions
open GeneralExceptions

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
    
  /// <summary>
  /// Creater
  ///</summary>
  /// <typeparam>Invoker</typeparam>
  /// <typeparam> userName </typeparam>
  /// <typeparam> Name </typeparam>
  /// <typeparam> ProductType </typeparam>
  /// <typeparam> Description </typeparam>
  /// <typeparam> BuyPrice </typeparam>
  /// <typeparam> RentPrice </typeparam>
  /// <exception> RentIt.Product.NoSuchUser </exception>
  /// <exception> RentIt.Product.ArgumentException </exception>
  /// <exception> RentIt.ControlledProduct.AccessDenied </excepption>
  /// <exception> RentIt.ControlledProduct.AccountBanned </excpetion>
    let make (invoker:Invoker) (userName:string) (name:string) (productType:string) (description:string option) (buyPrice:int option) (rentPrice:int option) =
        let allowed = PermissionsUtil.check invoker "CREATE" (CheckTarget.Type "Own")
        Internal.checkAllowed invoker allowed |> ignore 
        Product.make userName name productType description buyPrice  rentPrice

  
  /// <exception> RentIt.ControlledProduct.AccessDenied </excepption>
  /// <exception> RentIt.ControlledProduct.AccountBanned </excpetion>
    let getListOfproductTypes (invoker:Invoker) =
        let allowed = PermissionsUtil.check invoker "READ" (CheckTarget.Type "Any")
        Internal.checkAllowed invoker allowed |> ignore
        Product.getListOfProductTypes

  
  /// <exception> RentIt.ControlledProduct.AccessDenied </excepption>
  /// <exception> RentIt.ControlledProduct.AccountBanned </excpetion>
    let getAll (invoker:Invoker)(status:PublishedStatus) =
        if(status = PublishedStatus.Published) then
            let allowed = PermissionsUtil.check invoker "READ" (CheckTarget.Type "Any")
            Internal.checkAllowed invoker allowed |> ignore
            Product.getAllProducts status
        else
            let allowed = PermissionsUtil.check invoker "READ_UNPUBLISHED" (CheckTarget.Type "Any")
            Internal.checkAllowed invoker allowed |> ignore
            Product.getAllProducts status    
    
  /// <summary>
  /// Get prodcut by product id
  /// </summary>
  /// <typeparam> Invoker </typeparam>
  /// <typeparam> Id </typeparam>
  /// <returns> Product </returns>
  /// <exception> RentIt.Product.NoSuchProduct </exception>
  /// <exception> RentIt.Product.ArgumentException </exception>
  /// <exception> RentIt.ControlledProduct.AccessDenied </excepption>
  /// <exception> RentIt.ControlledProduct.AccountBanned </excpetion>
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

  /// <summary>
  /// Get products by product name
  /// </summary>
  /// <typeparam> Invoker </typeparam>
  /// <typeparam> Product name </typeparam>
  /// <returns> List of products </returns>
  /// <exception> RentIt.Product.NoSuchProduct </exception>
  /// <exception> RentIt.Product.ArgumentException </exception>
  /// <exception> RentIt.ControlledProduct.AccessDenied </excepption>
  /// <exception> RentIt.ControlledProduct.AccountBanned </excpetion>
    let getProductByName (invoker:Invoker) (pName:string) =
        let allowed = PermissionsUtil.check invoker "READ" (CheckTarget.Type "Any")
        Internal.checkAllowed invoker allowed |> ignore
        Product.getProductByName pName

  
  /// <exception> RentIt.ControlledProduct.AccessDenied </excepption>
  /// <exception> RentIt.ControlledProduct.AccountBanned </excpetion>
    let getProductByType (invoker:Invoker) (pType:string) (status:PublishedStatus) =
        if(status = PublishedStatus.Published) then
            let allowed = PermissionsUtil.check invoker "READ" (CheckTarget.Type "Any")
            Internal.checkAllowed invoker allowed |> ignore
            Product.getAllProductsByType pType status
        else
            let allowed = PermissionsUtil.check invoker "READ_UNPUBLISHED" (CheckTarget.Type "Any")
            Internal.checkAllowed invoker allowed |> ignore
            Product.getAllProductsByType pType status

  /// <summary>
  /// Update existing product
  /// </summary>
  // <typeparam> Product name </typeparam>
  /// <exception> RentIt.Product.NoSuchProduct </exception>
  /// <exception> RentIt.Product.UpdateNotAllowed </exception>
  /// <exception> RentIt.Product.ArgumentException </exception> 
  /// <exception> RentIt.ControlledProduct.AccessDenied </excepption>
  /// <exception> RentIt.ControlledProduct.AccountBanned </excpetion>
    let update (invoker:Invoker) (p:Product) =
        if((string (invokerToId invoker)) = p.owner) then 
            let allowed = PermissionsUtil.check invoker "EDIT" (CheckTarget.Type "Own")
            Internal.checkAllowed invoker allowed |> ignore
            Product.update p
        else 
            let allowed = PermissionsUtil.check invoker "EDIT" (CheckTarget.Type "Any")
            Internal.checkAllowed invoker allowed |> ignore
            Product.update p
  
  /// <summary>
  /// Change Published-flag on Product
  ///</summary>
  /// <typeparam> Product id </typeparam>
  /// <typeparam> Boolean </typeparam>
  /// <exception> NoSuchProduct </exception>
  /// <exception> RentIt.ControlledProduct.AccessDenied </excepption>
  /// <exception> RentIt.ControlledProduct.AccountBanned </excpetion>
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
    
  /// <summary>
  /// Rate Product
  ///</summary>
  /// <typeparam> Product id </typeparam>
  /// <typeparam> Rating </typeparam>
  /// <exception> NoSuchProduct </exception>
  /// <exception> RentIt.ControlledProduct.AccessDenied </excepption>
  /// <exception> RentIt.ControlledProduct.AccountBanned </excpetion>
    let rateProduct (invoker:Invoker) (pId:int) (user:string) (rating:int) =
        let allowed = PermissionsUtil.check invoker "RATE_PRODUCT" (CheckTarget.Type "Any")
        Internal.checkAllowed invoker allowed |> ignore
        Product.rateProduct pId user rating

  
  /// <exception> RentIt.ControlledProduct.AccessDenied </excepption>
  /// <exception> RentIt.ControlledProduct.AccountBanned </excpetion>
    let getAllProductsByUser (invoker:Invoker) (user:string) (status:PublishedStatus) =
        if(status = PublishedStatus.Published) then
            let allowed = PermissionsUtil.check invoker "READ" (CheckTarget.Type "Own")
            Internal.checkAllowed invoker allowed |> ignore
            Product.getAllProductsByUser user status
        else
            let allowed = PermissionsUtil.check invoker "READ_UNPUBLISHED" (CheckTarget.Type "Own")
            Internal.checkAllowed invoker allowed |> ignore
            Product.getAllProductsByUser user status

  
  /// <exception> RentIt.ControlledProduct.AccessDenied </excepption>
  /// <exception> RentIt.ControlledProduct.AccountBanned </excpetion>
    let getProductByUserAndTitle (invoker:Invoker) (user:string) (pTitle:string) (status:PublishedStatus) =
        if(status = PublishedStatus.Published) then
            let allowed = PermissionsUtil.check invoker "READ" (CheckTarget.Type "Own")
            Internal.checkAllowed invoker allowed |> ignore
            Product.getAllProductsByUserAndTitle user pTitle status
        else
            let allowed = PermissionsUtil.check invoker "READ_UNPUBLISHED" (CheckTarget.Type "Own")
            Internal.checkAllowed invoker allowed |> ignore
            Product.getAllProductsByUserAndTitle user pTitle status

  
  /// <exception> RentIt.ControlledProduct.AccessDenied </excepption>
  /// <exception> RentIt.ControlledProduct.AccountBanned </excpetion>
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

  
  /// <exception> RentIt.ControlledProduct.AccessDenied </excepption>
  /// <exception> RentIt.ControlledProduct.AccountBanned </excpetion>
    let persistMedia (invoker:Invoker) (id:uint32) (mime:string) (stream:System.IO.Stream) =
        let p = Product.getProductById (int id)
        if((string (invokerToId invoker)) = p.owner) then
            let allowed = PermissionsUtil.check invoker "UPLOAD_MEDIA" (CheckTarget.Type "Own")
            Internal.checkAllowed invoker allowed |> ignore
            Product.persistMedia id mime stream
        else
            let allowed = PermissionsUtil.check invoker "UPLOAD_MEDIA" (CheckTarget.Type "Any")
            Internal.checkAllowed invoker allowed |> ignore
            Product.persistMedia id mime stream

  
  /// <exception> RentIt.ControlledProduct.AccessDenied </excepption>
  /// <exception> RentIt.ControlledProduct.AccountBanned </excpetion>
    let searchProducts (invoker:Invoker) (search:string) =
        let allowed = PermissionsUtil.check invoker "READ" (CheckTarget.Type "Any")
        Internal.checkAllowed invoker allowed |> ignore
        Product.searchProducts search
    
    
  /// <exception> RentIt.ControlledProduct.AccessDenied </excepption>
  /// <exception> RentIt.ControlledProduct.AccountBanned </excpetion>   
//    let getMedia (invoker:Invoker) (id:uint32) =
//        let p = Product.getProductById (int id)
//        let acc = Account.getByUsername (string (invokerToId invoker))
//        if((string (invokerToId invoker)) = p.owner) then
//            let allowed = PermissionsUtil.check invoker "READ" (CheckTarget.Type "Own")
//            Internal.checkAllowed invoker allowed |> ignore   
//            if (ControlledCredits.checkAccessToProduct invoker acc p) then
//                Product.getMedia id
//            else
//                raise PermissionDenied
//        else    
//            let allowed = PermissionsUtil.check invoker "READ" (CheckTarget.Type "Any")       
//            Internal.checkAllowed invoker allowed |> ignore
//            if (ControlledCredits.checkAccessToProduct invoker acc p) then
//                Product.getMedia id
//            else
//                raise PermissionDenied
    
  /// <exception> RentIt.ControlledProduct.AccessDenied </excepption>
  /// <exception> RentIt.ControlledProduct.AccountBanned </excpetion>        
    let getMediaThumbnail (invoker:Invoker) (id:uint32) =
        let allowed = PermissionsUtil.check invoker "READ" (CheckTarget.Type "Any")
        Internal.checkAllowed invoker allowed |> ignore
        Product.getMediaThumbnail id 
