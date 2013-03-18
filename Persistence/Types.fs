namespace RentIt.Persistence
    module Types =
        type Field = {
         objectName : string;
         field : string;
         processor : Field -> string;
        }
        type Filter = {
         field : Field;
         operator : string;
         value : string;
         processor : Filter -> string;
        }
        type ReadField = {
         field : Field;
         processor : ReadField -> string;
        }
        type DataOut = {
         field : Field;
         value : string;
        }
        type DataIn = {
         field : Field;
         value : string;
        }
        type ObjectJoin = {
         fieldFrom : Field;
         fieldTo : Field;
        }
        type Create = {
         lol : bool;
        }
        type Read = {
         fields : ReadField List;
         baseObjectName : string;
         joins : ObjectJoin List;
         filters : Filter List;
         processor : Read -> string
        }
        type Update = {
         lol : bool;
        }
        type Delete = {
         lol : bool;
        }

    //Field processors supported
    module Field =
        let Default (field:Types.Field) =
            "["+field.objectName+"]."+field.field
        let AllInObject (field:Types.Field) =
            "["+field.objectName+"].*"

    //Filter processors supported
    module Filter =
        let Default (filter:Types.Filter) =
            (filter.field.processor filter.field)+filter.operator+"'"+filter.value+"'"

    //Read processors supported
    module ReadField = 
        let Default (readField:Types.ReadField) = 
            (readField.field.processor readField.field)
        let Max (readField:Types.ReadField) = 
            "MAX("+Default readField+")"
        let All (readField:Types.ReadField) =
            "*"

    //ObjectJoin processors supported
    module ObjectJoin =
        let Default (objectJoin:Types.ObjectJoin) =
            "JOIN "+objectJoin.fieldTo.objectName+" ON "+objectJoin.fieldFrom.processor objectJoin.fieldFrom+"="+objectJoin.fieldTo.processor objectJoin.fieldTo

    //Read processors supported
    module Read =
        let rec internal joinReadFields (fields:Types.ReadField List) =
            match fields with 
                | [] -> ""
                | x::[] -> x.processor x
                | x::xs -> x.processor x+","+joinReadFields xs
        let rec internal joinJoins (fields:Types.ObjectJoin List) =
            match fields with 
                | [] -> ""
                | x::[] -> ObjectJoin.Default x
                | x::xs -> ObjectJoin.Default x+","+joinJoins xs
        let rec internal joinFilters (fields:Types.Filter List) =
            match fields with 
                | [] -> ""
                | x::[] -> x.processor x
                | x::xs -> x.processor x+","+joinFilters xs
        let Default (read:Types.Read) =
            "SELECT "+joinReadFields read.fields+" FROM ["+read.baseObjectName+"] "+joinJoins read.joins+" WHERE "+joinFilters read.filters