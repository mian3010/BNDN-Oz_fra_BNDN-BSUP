namespace RentIt.Persistence
    ///Contains the processors and the factory functions for Filter type
    module Filter =
        //Filter processors supported
        ///Default processor
        let Default (filter:Types.Filter) =
            (filter.field.processor filter.field)+filter.operator+"'"+filter.value+"'"

        //Factory functions creating an instance of the type
        let createFilter (list:Types.Filter List) objectName field operator value =
            list@[({field=(Field.createField [] objectName field).Head;operator=operator;value=value;processor=Default}:Types.Filter)]
        ///Creates an instance with a specific processor
        let createFilterProc (list:Types.Filter List) objectName field operator value processor =
            list@[({field=(Field.createField [] objectName field).Head;operator=operator;value=value;processor=processor}:Types.Filter)]
        ///Creates an instance with a field type given
        let createFilterField (list:Types.Filter List) field operator value =
            list@[({field=field;operator=operator;value=value;processor=Default}:Types.Filter)]
        ///Creates an instance with a field type given and a specific processor
        let createFilterFieldProc (list:Types.Filter List) field operator value processor =
            list@[({field=field;operator=operator;value=value;processor=processor}:Types.Filter)]