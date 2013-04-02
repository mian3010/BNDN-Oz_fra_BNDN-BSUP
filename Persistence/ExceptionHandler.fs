namespace RentIt.Persistence
open System.Data
open System
open RentIt.PersistenceExceptions
  module ExceptionHandler =
    let handleException (e:Exception) =
      match e with 
        | :? SqlClient.SqlException as e -> match e.Number with
                                              | 547 -> raise ReferenceDoesNotExist
                                              | 2627 -> raise AlreadyExists
                                              | _ -> raise PersistenceException
        | _ -> raise PersistenceException