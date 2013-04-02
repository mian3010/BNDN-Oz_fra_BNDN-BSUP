namespace RentIt.Persistence
    ///Contains the processors and the factory functions for ReadField type
    module ReadField = 
        //Read processors supported
        ///Default processor
        let Default (readField:Types.ReadField) = 
            (readField.field.processor readField.field)
        ///Read the max value of a field
        let Max (readField:Types.ReadField) = 
            "MAX("+Default readField+") AS "+readField.field.objectName+"_"+readField.field.field+"_Max"
        ///Read the average value of a field
        let Avg (readField:Types.ReadField) = 
            "AVG("+Default readField+") AS "+readField.field.objectName+"_"+readField.field.field+"_Avg"
        ///Read the number of rows
        let Num (readField:Types.ReadField) = 
            "NUM("+Default readField+") AS "+readField.field.objectName+"_"+readField.field.field+"_Num"
        ///For select only. Select everything in query
        let All (readField:Types.ReadField) =
            "*"
        
        //Factory functions creating an instance of the type
        let createReadField (list:Types.ReadField List) objectName field =
            list@[({field = (Field.createField [] objectName field).Head; processor = Default}:Types.ReadField)]
        ///Creates an instance with a specific processor
        let createReadFieldProc (list:Types.ReadField List) objectName field processor =
            list@[({field=(Field.createField [] objectName field).Head;processor=processor}:Types.ReadField)]
        ///Creates an instance with a field type given
        let createReadFieldField (list:Types.ReadField List) field =
            list@[({field=field;processor=Default}:Types.ReadField)]
        ///Creates an instance with a field type given and a specific processor
        let createReadFieldFieldProc (list:Types.ReadField List) field processor =
            list@[({field=field;processor=processor}:Types.ReadField)]