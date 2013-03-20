namespace RentIt.Persistence
    ///Contains the processors and the factory functions for ObjectJoin type
    module ObjectJoin =
        //ObjectJoin processors supported
        //Default processor
        let Default (objectJoin:Types.ObjectJoin) =
            "JOIN "+objectJoin.fieldTo.objectName+" ON "+objectJoin.fieldFrom.processor objectJoin.fieldFrom+"="+objectJoin.fieldTo.processor objectJoin.fieldTo

        //Factory functions creating an instance of the type
        let createObjectJoin (list:Types.ObjectJoin List) fromObjectName fromField toObjectName toField = 
            list@[({fieldFrom=(Field.createField [] fromObjectName fromField).Head;fieldTo=(Field.createField [] toObjectName toField).Head}:Types.ObjectJoin)]
        ///Creates an instance with a field type given
        let createObjectJoinFields (list:Types.ObjectJoin List) fromField toField = 
            list@[({fieldFrom=fromField;fieldTo=toField}:Types.ObjectJoin)]