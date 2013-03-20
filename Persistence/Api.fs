namespace RentIt.Persistence
open System
open System.Data

    // The Persistence module is used for reading and wrting data.
    // This module works with a SQL database
    module Api =
        ///Execute a create query
        let createQ (createQuery:Types.Create) =
            try
                use reader = Helper.performSql (Create.Default createQuery)
                Helper.extractData reader
            with
                | _ -> raise Types.PersistenceException
        ///Execute a create query taking the contents of it
        let create objectName data =
            let createQuery:Types.Create = {
                objectName=objectName;
                data=data;
            }
            createQ createQuery

        ///Execute a read query
        let readQ (readQuery:Types.Read) =
            try 
                use reader = Helper.performSql (readQuery.processor readQuery)
                Helper.extractData reader
            with
                | _ -> raise Types.PersistenceException
        ///Execute a read query taking the contents of it
        let read fields objectName joins filters =
            let readQuery:Types.Read = {
                fields=fields;
                baseObjectName=objectName;
                joins=joins;
                filters=filters;
                processor=Read.Default;
            }
            readQ readQuery
        ///Execute a read query taking the contents of it with a specific processor
        let readProc fields objectName joins filters processor =
            let readQuery:Types.Read = {
                fields=fields;
                baseObjectName=objectName;
                joins=joins;
                filters=filters;
                processor=processor;
            }
            readQ readQuery

        ///Execute an update query
        let updateQ (updateQuery:Types.Update) =
            try
                use reader = Helper.performSql (Update.Default updateQuery)
                Helper.extractData reader
            with
                | _ -> raise Types.PersistenceException
        ///Execute an update query taking the contents of it
        let update objectName filters dataIn =
            let updateQuery:Types.Update = {
                objectName=objectName;
                filters=filters;
                data=dataIn;
            }
            updateQ updateQuery

        ///Execute a delete query
        let deleteQ (deleteQuery:Types.Delete) =
            try
                use reader = Helper.performSql (Delete.Default deleteQuery)
                Helper.extractData reader
            with
                | _ -> raise Types.PersistenceException
        ///Execute a delete query taking the contents of it
        let delete objectName filters =
            let deleteQuery:Types.Delete = {
                objectName = objectName;
                filters=filters;
            }
            deleteQ deleteQuery