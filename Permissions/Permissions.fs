namespace RentIt

  module Permissions = 
    
    (*type Identity =   Auth of string // username
                    | Unauth         // unauthenticated users

    type Permission =    CREATE_CUSTOMER
                       | CREATE_CONTENT_PROVIDER
                       | CREATE_ADMIN
                      // Full read permissions to own account or to accounts of a specific type. Includes "overview" operation.
                       | READ_OWN
                       | READ_CUSTOMER
                       | READ_CONTENT_PROVIDER
                       | READ_ADMIN
                      // Read the last time of authentication of own account or for account of a specific type.
                       | READ_OWN_AUTH_INFO
                       | READ_AUTH_INFO
                      // Ban/unban accounts of a specific type
                       | BAN_UNBAN_CUSTOMER
                       | BAN_UNBAN_CONTENT_PROVIDER
                       | BAN_UNBAN_ADMIN
                      // Give/take credits from a customer account
                       | GIVE_OWN_CREDITS // Without this permission a customer cannot buy more credits
                       | TAKE_OWN_CREDITS // Without this permission a customer cannot do any product purchase
                       | GIVE_CREDITS
                       | TAKE_CREDITS
                      // Change various specific fields (email / password) of own account or for account of a specific type.
                       | CHANGE_OWN_PASSWORD
                       | CHANGE_CUSTOMER_PASSWORD
                       | CHANGE_CONTENT_PROVIDER_PASSWORD
                       | CHANGE_ADMIN_PASSWORD
                       | RESET_OWN_PASSWORD
                       | RESET_CUSTOMER_PASSWORD
                       | RESET_CONTENT_PROVIDER_PASSWORD
                       | RESET_ADMIN_PASSWORD
                       | CHANGE_OWN_EMAIL
                       | CHANGE_CUSTOMER_EMAIL
                       | CHANGE_CONTENT_PROVIDER_EMAIL
                       | CHANGE_ADMIN_EMAIL
                      // Change all non-specific fields (except immutable data, such as Created) of own account or for account of a specific type.
                       | EDIT_OWN
                       | EDIT_CUSTOMER
                       | EDIT_CONTENT_PROVIDER
                       | EDIT_ADMIN*)

    let checkPermissions (id:string) (permission:string) :bool =
      raise (new System.NotImplementedException())

    let assignPermissionToUser (id:string) (permission:string) =
      raise (new System.NotImplementedException())

    let unassignPermissionFromUser (id:string) (permission:string) = 
      raise (new System.NotImplementedException())