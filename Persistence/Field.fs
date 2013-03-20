namespace RentIt.Persistence
    ///Contains the processors and the factory functions for Field type
    module Field =
        //Field processors supported
        ///Default processor
        let Default (field:Types.Field) =
            "["+field.objectName+"]."+field.field
        ///For select query. Select all in object
        let AllInObject (field:Types.Field) =
            "["+field.objectName+"].*"

        ///Factory functions creating an instance of the type
        let createField (list:Types.Field List) objectName field =
            list@[({objectName=objectName;field=field;processor=Default}:Types.Field)]
        ///Creates an instance with a specific processor
        let createFieldProc (list:Types.Field List) objectName field processor =
            list@[({objectName=objectName;field=field;processor=processor}:Types.Field)]