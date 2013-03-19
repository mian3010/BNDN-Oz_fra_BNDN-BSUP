namespace RentIt.Persistence
    ///Contains the processors and the factory functions for Delete type
    module Delete =
        ///Default processor
        let Default (delete:Types.Delete) =
            "DELETE FROM ["+delete.objectName+"] OUTPUT DELETED.* WHERE "+Read.joinFilters delete.filters