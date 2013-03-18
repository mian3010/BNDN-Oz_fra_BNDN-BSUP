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
        type SelectField = {
         field : Field;
         processor : SelectField -> string;
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
        type Select = {
         fields : SelectField List;
         baseObjectName : string;
         joins : ObjectJoin List;
         filters : Filter List;
         processor : Select -> string
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

    //Select processors supported
    module SelectField = 
        let Default (selectField:Types.SelectField) = 
            (selectField.field.processor selectField.field)
        let Max (selectField:Types.SelectField) = 
            "MAX("+Default selectField+")"
        let All (selectField:Types.SelectField) =
            "*"

    //ObjectJoin processors supported
    module ObjectJoin =
        let Default (objectJoin:Types.ObjectJoin) =
            "JOIN "+objectJoin.fieldTo.objectName+" ON "+objectJoin.fieldFrom.processor objectJoin.fieldFrom+"="+objectJoin.fieldTo.processor objectJoin.fieldTo

    //Select processors supported
    module Select =
        let rec internal joinSelectFields (fields:Types.SelectField List) =
            match fields with 
                | [] -> ""
                | x::[] -> x.processor x
                | x::xs -> x.processor x+","+joinSelectFields xs
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
        let Default (select:Types.Select) =
            "SELECT "+joinSelectFields select.fields+" FROM ["+select.baseObjectName+"] "+joinJoins select.joins+" WHERE "+joinFilters select.filters