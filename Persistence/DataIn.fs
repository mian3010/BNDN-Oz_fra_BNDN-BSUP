namespace RentIt.Persistence
    ///Contains the processors and the factory functions for DataIn type
    module DataIn =
        //DataIn processors supported
        ///Default processor
        let Default (dataIn:Types.DataIn) =
            (dataIn.field.processor dataIn.field)+"='"+dataIn.value+"'"
        ///Insert NULL processor
        let Null (dataIn:Types.DataIn) =
            (dataIn.field.processor dataIn.field)+"=NULL"

        //Factory functions creating an instance of the type
        let createDataIn (list:Types.DataIn List) objectName field value =
            list@[({field=(Field.createField [] objectName field).Head;value=value;processor=Default;}:Types.DataIn)]
        //Factory functions creating an instance of the type
        let createDataInProc (list:Types.DataIn List) objectName field value processor =
            list@[({field=(Field.createField [] objectName field).Head;value=value;processor=processor;}:Types.DataIn)]
        ///Creates an instance with a field type given
        let createDataInField (list:Types.DataIn List) field value =
            list@[({field=field;value=value;processor=Default;}:Types.DataIn)]