namespace RentIt.Persistence
    ///Contains the processors and the factory functions for Update type
    module Update =
        //Join DataIn list in an SQL format
        let rec internal joinUpdateData (data:Types.DataIn List) =
            match data with
                | [] -> ""
                | x::[] -> x.processor x
                | x::xs -> x.processor x+","+joinUpdateData xs
        ///Default processor
        let Default (update:Types.Update) =
            "UPDATE ["+update.objectName+"] SET "+joinUpdateData update.data+" OUTPUT INSERTED.* WHERE "+Read.joinFilterGroups update.filters

        //Factory functions
        let createUpdate objectName filters data =
            ({objectName=objectName;filters=filters;data=data;}:Types.Update)