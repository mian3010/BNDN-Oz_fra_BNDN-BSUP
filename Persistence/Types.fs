namespace RentIt.Persistence
    module Types =
        exception PersistenceException
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
         objectName : string;
         data : DataIn List;
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

    module Field =
        //Field processors supported
        let Default (field:Types.Field) =
            "["+field.objectName+"]."+field.field
        let AllInObject (field:Types.Field) =
            "["+field.objectName+"].*"

        //Factory functions
        let createField (list:Types.Field List) objectName field =
            list@[({objectName=objectName;field=field;processor=Default}:Types.Field)]
        let createFieldProc (list:Types.Field List) objectName field processor =
            list@[({objectName=objectName;field=field;processor=processor}:Types.Field)]

    module Filter =
        //Filter processors supported
        let Default (filter:Types.Filter) =
            (filter.field.processor filter.field)+filter.operator+"'"+filter.value+"'"

        //Factory functions
        let createFilter (list:Types.Filter List) objectName field operator value =
            list@[({field=(Field.createField [] objectName field).Head;operator=operator;value=value;processor=Default}:Types.Filter)]
        let createFilterProc (list:Types.Filter List) objectName field operator value processor =
            list@[({field=(Field.createField [] objectName field).Head;operator=operator;value=value;processor=processor}:Types.Filter)]
        let createFilterField (list:Types.Filter List) field operator value =
            list@[({field=field;operator=operator;value=value;processor=Default}:Types.Filter)]
        let createFilterFieldProc (list:Types.Filter List) field operator value processor =
            list@[({field=field;operator=operator;value=value;processor=processor}:Types.Filter)]

    module ReadField = 
        //Read processors supported
        let Default (readField:Types.ReadField) = 
            (readField.field.processor readField.field)
        let Max (readField:Types.ReadField) = 
            "MAX("+Default readField+") AS "+readField.field.objectName+"_"+readField.field.field+"_Max"
        let All (readField:Types.ReadField) =
            "*"

        //Factory functions
        let createReadField (list:Types.ReadField List) objectName field =
            list@[({field=(Field.createField [] objectName field).Head;processor=Default}:Types.ReadField)]
        let createReadFieldProc (list:Types.ReadField List) objectName field processor =
            list@[({field=(Field.createField [] objectName field).Head;processor=processor}:Types.ReadField)]
        let createReadFieldField (list:Types.ReadField List) field =
            list@[({field=field;processor=Default}:Types.ReadField)]
        let createReadFieldFieldProc (list:Types.ReadField List) field processor =
            list@[({field=field;processor=processor}:Types.ReadField)]

    module ObjectJoin =
        //ObjectJoin processors supported
        let Default (objectJoin:Types.ObjectJoin) =
            "JOIN "+objectJoin.fieldTo.objectName+" ON "+objectJoin.fieldFrom.processor objectJoin.fieldFrom+"="+objectJoin.fieldTo.processor objectJoin.fieldTo

        //Factory functions
        let createObjectJoin (list:Types.ObjectJoin List) fromObjectName fromField toObjectName toField = 
            list@[({fieldFrom=(Field.createField [] fromObjectName fromField).Head;fieldTo=(Field.createField [] toObjectName toField).Head}:Types.ObjectJoin)]
        let createObjectJoinFields (list:Types.ObjectJoin List) fromField toField = 
            list@[({fieldFrom=fromField;fieldTo=toField}:Types.ObjectJoin)]

    module DataIn =
        //Factory functions
        let createDataIn (list:Types.DataIn List) objectName field value =
            list@[({field=(Field.createField [] objectName field).Head;value=value;}:Types.DataIn)]
        let createDataInField (list:Types.DataIn List) field value =
            list@[({field=field;value=value;}:Types.DataIn)]

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
                | [] -> "1=1"
                | x::[] -> x.processor x
                | x::xs -> x.processor x+","+joinFilters xs
        let Default (read:Types.Read) =
            "SELECT "+joinReadFields read.fields+" FROM ["+read.baseObjectName+"] "+joinJoins read.joins+" WHERE "+joinFilters read.filters

    module Create =
        let internal joinTuple (ok,ov) k v =
             (ok+", "+k, ov+", '"+v+"'")
        let rec internal extractData (data:Types.DataIn List) =
            match data with 
                | [] -> ("", "")
                | x::[] -> (x.field.processor x.field, "'"+x.value+"'")
                | x::xs -> joinTuple (extractData xs) (x.field.processor x.field) x.value
        let Default (create:Types.Create) =
            let data = extractData create.data
            "INSERT INTO ["+create.objectName+"] ("+fst data+") VALUES ("+snd data+")"