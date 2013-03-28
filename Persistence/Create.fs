namespace RentIt.Persistence
    ///Contains the processors and the factory functions for Create type
    module Create =
        ///Join a tuple with a key and value in SQL format
        let internal joinTuple (ok,ov) k v =
             (ok+", "+k, ov+", '"+v+"'")
        ///Extract the data from an sql reader. From results of a query
        let rec internal extractData (data:Types.DataIn List) =
            match data with 
                | [] -> ("", "")
                | x::[] -> (x.field.processor x.field, "'"+x.value+"'")
                | x::xs -> joinTuple (extractData xs) (x.field.processor x.field) x.value
        ///Default proessor
        let Default (create:Types.Create) =
          if not create.data.IsEmpty then
            let data = extractData create.data
            "INSERT INTO ["+create.objectName+"] ("+fst data+") OUTPUT INSERTED.* VALUES ("+snd data+")"
          else 
            "INSERT ["+create.objectName+"] OUTPUT INSERTED.* DEFAULT VALUES"

        //Factory functions
        let createCreate objectName data =
            ({objectName=objectName;data=data;}:Types.Create)