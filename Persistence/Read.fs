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
        let rec internal joinJoins (joins:Types.ObjectJoin List) =
            match joins with 
                | [] -> ""
                | x::[] -> ObjectJoin.Default x
                | x::xs -> ObjectJoin.Default x+" "+joinJoins xs
        ///Join Filter list in an SQL format
        let rec internal joinFilterGroups (filters:Types.FilterGroup List) =
            match filters with 
                | [] -> "1=1"
                | x::[] -> "("+ (x.processor x) + ")"
                | x::xs when x.joiner.IsSome -> "("+ (x.processor x) + ") " + x.joiner.Value + " "+joinFilterGroups xs
                | x::xs -> "("+ (x.processor x) + ") " + FilterGroup.defaultJoiner + " "+joinFilterGroups xs
        ///Default processor
        let Default (read:Types.Read) =
            "SELECT "+joinReadFields read.fields+" FROM ["+read.baseObjectName+"] "+joinJoins read.joins+" WHERE "+joinFilterGroups read.filters

        //Factory functions
        let createRead fields objectName joins filters =
            ({fields=fields;baseObjectName=objectName;joins=joins;filters=filters;processor=Default}:Types.Read)