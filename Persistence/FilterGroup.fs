namespace RentIt.Persistence
  module FilterGroup =
    
    let rec internal joinFilters (filters:Types.Filter List) operator = 
      match filters with 
            | [] -> "1=1"
            | x::[] -> x.processor x
            | x::xs -> x.processor x+ (" " + operator + " ") + (joinFilters xs operator)
    
    let Default (filters:Types.FilterGroup) =
      joinFilters filters.filters "AND"

    let orCondition (filters:Types.FilterGroup) =
      joinFilters filters.filters "OR"

    let createFilterGroup (list:Types.FilterGroup List) objectName field value =
      list@[({filters=[({field=(Field.createField [] objectName field).Head;value=value;processor=Filter.Default}:Types.Filter)];processor=Default;}:Types.FilterGroup)]
    
    let createFilterGroupFilterProc (list:Types.FilterGroup List) objectName field value proc =
      list@[({filters=[({field=(Field.createField [] objectName field).Head;value=value;processor=proc}:Types.Filter)];processor=Default;}:Types.FilterGroup)]

    let createFilterGroupGroupFilterProc (list:Types.FilterGroup List) objectName field value proc =
      list@[({filters=[({field=(Field.createField [] objectName field).Head;value=value;processor=Filter.Default}:Types.Filter)];processor=proc;}:Types.FilterGroup)]

    let createFilterGroupProcProc (list:Types.FilterGroup List) objectName field value fProc gProc =
      list@[({filters=[({field=(Field.createField [] objectName field).Head;value=value;processor=fProc}:Types.Filter)];processor=gProc;}:Types.FilterGroup)]

    let createSingleFilterGroup (list:Types.Filter list) objectName field value =
      let group =
        if (list.Length = 0) then []
        else [({filters=list;processor=Default}:Types.FilterGroup)]
      createFilterGroup group objectName field value

    let createSingleFilterGroupProc (list:Types.Filter list) objectName field value processor =
      createFilterGroup [{filters=list;processor=processor}] objectName field value

    let createFilterGroupFiltersProc (list:Types.FilterGroup List) (filters:Types.Filter List) processor =
      list@[({filters=filters;processor=processor;}:Types.FilterGroup)]

    let createFilterGroupFilters (list:Types.FilterGroup List) (filters:Types.Filter List)=
      list@[({filters=filters;processor=Default;}:Types.FilterGroup)]