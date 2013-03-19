namespace RentIt.Persistence

open System
open System.Data

    // The Persistence module is used for reading and wrting data.
    // This module works with a SQL database
    module Api =
        //Create functions
        let createQ (createQuery:Types.Create) =
            try
                use reader = Helper.performSql (Create.Default createQuery)
                Helper.extractData reader
            with
                | _ -> raise Types.PersistenceException

        let create objectName data =
            let createQuery:Types.Create = {
                objectName=objectName;
                data=data;
            }
            createQ createQuery

        //Read functions
        let readQ (readQuery:Types.Read) =
            try 
                use reader = Helper.performSql (readQuery.processor readQuery)
                Helper.extractData reader
            with
                | _ -> raise Types.PersistenceException

        let read fields objectName joins filters =
            let readQuery:Types.Read = {
                fields=fields;
                baseObjectName=objectName;
                joins=joins;
                filters=filters;
                processor=Read.Default;
            }
            readQ readQuery

        let readProc fields objectName joins filters processor =
            let selectQ:Types.Read = {
                fields=fields;
                baseObjectName=objectName;
                joins=joins;
                filters=filters;
                processor=processor;
            }
            readQ selectQ

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