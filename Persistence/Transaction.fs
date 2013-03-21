namespace RentIt.Persistence
module Transaction =
    ///Joins the queries in a transaction to a string
    let rec internal joinQueries (queries:string List) =
        match queries with
            | [] -> ""
            | x::xs -> x+"; "+joinQueries xs
    ///Default processor
    let Default (transaction:Types.Transaction) =
        "BEGIN TRANSACTION; DECLARE @RETURN BIT; BEGIN TRY "+joinQueries transaction.queries+" SET @RETURN=1; IF @@TRANCOUNT > 0 COMMIT TRANSACTION; END TRY BEGIN CATCH SET @RETURN=0; IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION; END CATCH; SELECT @RETURN"

    //Factory functions creating an instance of the type
    let createTransaction =
        ({queries=[]}:Types.Transaction)

    ///Add a create query to the transaction
    let addCreate (transaction:Types.Transaction) (create:Types.Create) =
        ({queries=transaction.queries@[(Create.Default create)]}:Types.Transaction)
    ///Add a read query to the transaction
    let addRead (transaction:Types.Transaction) (read:Types.Read) =
        ({queries=transaction.queries@[(read.processor read)]}:Types.Transaction)
    ///Add an update query to the transaction
    let addUpdate (transaction:Types.Transaction) (update:Types.Update) =
        ({queries=transaction.queries@[(Update.Default update)]}:Types.Transaction)
    ///Add a delete query to the transaction
    let addDelete (transaction:Types.Transaction) (delete:Types.Delete) =
        ({queries=transaction.queries@[(Delete.Default delete)]}:Types.Transaction)