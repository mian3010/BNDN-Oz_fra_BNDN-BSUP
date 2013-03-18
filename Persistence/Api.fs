namespace RentIt.Persistence

open System
open System.Data

    // The Persistence module is used for reading and wrting data.
    // This module works with a SQL database
    module Api =
        // Get stuff from database. Takes a List of Filters
        let Read (readQuery:Types.Read) =
         use reader = Helper.performSql (readQuery.processor readQuery)
         Helper.extractData reader
         (*
        let Insert objectName data = 
         let tupleData = Helper.joinInsertData data
         use reader = Helper.performSql ("INSERT INTO [" + objectName + "] (" + fst tupleData + ") VALUES (" + snd tupleData + ")")
         if reader.RecordsAffected > 0 then true
         else false

        let Update objectName (filters:Filter.Filter List) (data:Map<string,string>) = 
         use reader = Helper.performSql ("UPDATE [" + objectName + "] SET " + Helper.joinData data + " WHERE " + Helper.joinFilters filters)
         if reader.RecordsAffected > 0 then true
         else false

        let Delete objectName (filters:Filter.Filter List) = 
         use reader = Helper.performSql ("DELETE FROM [" + objectName + "] WHERE " + Helper.joinFilters filters)
         if reader.RecordsAffected > 0 then true
         else false

        let InitTransaction a =
         raise (new System.NotImplementedException())

        let ExecTransaction a =
         raise (new System.NotImplementedException())
         *)