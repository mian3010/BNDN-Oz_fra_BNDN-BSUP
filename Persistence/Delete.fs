namespace RentIt.Persistence
    ///Contains the processors and the factory functions for Delete type
    module Delete =
        ///Default processor
        let Default (delete:Types.Delete) =
            "DELETE FROM ["+delete.objectName+"] OUTPUT DELETED.* WHERE "+Read.joinFilterGroups delete.filters

        //Factory functions
        let createDelete objectName filters =
            ({objectName=objectName;filters=filters;}:Types.Delete)