namespace RentIt

module AuthExceptions = 
  exception AuthenticationFailed
  exception TokenExpired              // Raised if a token is expired and was not supposed to be so
  exception IllegalToken of string    // Raised if a token is malformed

    

module ControlledProductExceptions =
    exception Conflict

module GeneralExceptions =
  exception ArgumentException of string
  exception AccountBanned              // Raised when a banned invoker attempts to perform an action
  exception PermissionDenied

module ProductExceptions =
  exception NoSuchProduct
  exception NoSuchUser
  exception ProductNotPublished
  exception ArgumentException of string
  exception ProductAlreadyExists
  exception NoSuchProductType
  exception UpdateNotAllowed of string
  exception OutdatedData      // If a call to 'update' cannot succeed, because the changes conflict with more recent changes
  exception TooLargeData      // If an product could not be persisted, because its fields were too large
  exception MimeTypeNotAllowed
  exception NoSuchMedia       // The product or thumbnail was not found

module PersistenceExceptions =
  exception PersistenceException
  exception ReferenceDoesNotExist
  exception AlreadyExists

module AccountExceptions = 
  exception UserAlreadyExists // If one tries to create/persist an account whose username already is taken
  exception UnknownAccType    // If no account type exists which matches the specified account type string
  exception NoSuchUser        // If one tries to retrieve/update an account, but no account is found for the passed identifier
  exception OutdatedData      // If a call to 'update' cannot succeed, because the changes conflict with more recent changes
  exception BrokenInvariant   // If a function is invoked on an account whose invariants have been broken
  exception TooLargeData      // If an account could not be persisted, because its fields were too large

module AccountPersistenceExceptions =
  exception NoUserWithSuchName
  exception UsernameAlreadyInUse
  exception NewerVersionExist
  exception IllegalAccountVersion
  exception NoSuchAccountType
  exception NoSuchCountry

module CreditsExceptions =
  exception NotEnoughCredits
  exception NoSuchTransaction
  exception UnexpectedType
  exception TooLargeData      // If credits could not be bought, because the account cannot have any more credits
  exception InvalidCredits

module PermissionExceptions =
  exception AccountBanned              // Raised when a banned invoker attempts to perform an action
  exception PermissionDenied of string // Raised when an invoker attempts to perform an inaccessible action
