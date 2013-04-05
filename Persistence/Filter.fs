namespace RentIt.Persistence
    ///Contains the processors and the factory functions for Filter type
    module Filter =
        //Filter processors supported
        ///Default processor
        let Default (filter:Types.Filter) =
            (filter.field.processor filter.field)+"='"+filter.value+"'"

        let anyBefore (filter:Types.Filter) =
            (filter.field.processor filter.field)+" LIKE '%"+filter.value+"'"

        let anyAfter (filter:Types.Filter) =
            (filter.field.processor filter.field)+" LIKE '"+filter.value+"%'"

        let anyBeforeAndAfter (filter:Types.Filter) =
            (filter.field.processor filter.field)+" LIKE '%"+filter.value+"%'"

        let greaterThanOrEqual (filter:Types.Filter) =
            (filter.field.processor filter.field)+">='"+filter.value+"'"

        let lessThanOrEqual (filter:Types.Filter) =
            (filter.field.processor filter.field)+"<='"+filter.value+"'"

        //Factory functions creating an instance of the type
        let createFilter (list:Types.Filter List) objectName field value =
            list@[({field=(Field.createField [] objectName field).Head;value=value;processor=Default}:Types.Filter)]
        ///Creates an instance with a specific processor
        let createFilterProc (list:Types.Filter List) objectName field value processor =
            list@[({field=(Field.createField [] objectName field).Head;value=value;processor=processor}:Types.Filter)]
        ///Creates an instance with a field type given
        let createFilterField (list:Types.Filter List) field value =
            list@[({field=field;value=value;processor=Default}:Types.Filter)]
        ///Creates an instance with a field type given and a specific processor
        let createFilterFieldProc (list:Types.Filter List) field value processor =
            list@[({field=field;value=value;processor=processor}:Types.Filter)]