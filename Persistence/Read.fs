namespace RentIt.Persistence
    ///Contains the processors and the factory functions for Read type
    module Read =
        ///Joins ReadField list in an SQL format
        let rec internal joinReadFields (fields:Types.ReadField List) =
            match fields with 
                | [] -> ""
                | x::[] -> x.processor x
                | x::xs -> x.processor x+","+joinReadFields xs
        ///Join ObjectJoin list in an SQL format
        let rec internal joinJoins (fields:Types.ObjectJoin List) =
            match fields with 
                | [] -> ""
                | x::[] -> ObjectJoin.Default x
                | x::xs -> ObjectJoin.Default x+" "+joinJoins xs
        ///Join Filter list in an SQL format
        let rec internal joinFilters (fields:Types.Filter List) =
            match fields with 
                | [] -> "1=1"
                | x::[] -> x.processor x
                | x::xs -> x.processor x+" AND "+joinFilters xs
        ///Default processor
        let Default (read:Types.Read) =
            "SELECT "+joinReadFields read.fields+" FROM ["+read.baseObjectName+"] "+joinJoins read.joins+" WHERE "+joinFilters read.filters

        //Factory functions
        let createRead fields objectName joins filters =
            ({fields=fields;baseObjectName=objectName;joins=joins;filters=filters;processor=Default}:Types.Read)