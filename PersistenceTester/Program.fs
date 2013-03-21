// Learn more about F# at http://fsharp.net
// See the 'F# Tutorial' project for more help.

namespace RentIt

module Main = 
    [<EntryPoint>]
    let main argv =
        //Testing Create
        printfn "%A" "---------- Create query ----------"
        let objectName = "User"
        let dataQ = Persistence.DataIn.createDataIn []    objectName "Id" "2"
        let dataQ = Persistence.DataIn.createDataIn dataQ objectName "Username" "tumpe3"
        let dataQ = Persistence.DataIn.createDataIn dataQ objectName "Email" "tumpe3@example.com"
        let dataQ = Persistence.DataIn.createDataIn dataQ objectName "Address" "Address"
        let dataQ = Persistence.DataIn.createDataIn dataQ objectName "Date_of_birth" "2013-10-10"
        let dataQ = Persistence.DataIn.createDataIn dataQ objectName "Password" "pass"
        let dataQ = Persistence.DataIn.createDataIn dataQ objectName "Created_date" "2013-12-12"
        let dataQ = Persistence.DataIn.createDataIn dataQ objectName "Banned" "0"
        let dataQ = Persistence.DataIn.createDataIn dataQ objectName "About_me" "about me"
        let dataQ = Persistence.DataIn.createDataIn dataQ objectName "Type_name" "User"
        let dataQ = Persistence.DataIn.createDataIn dataQ objectName "Balance" "0"
        let dataQ = Persistence.DataIn.createDataIn dataQ objectName "Zipcode" "4425"
        let dataQ = Persistence.DataIn.createDataIn dataQ objectName "Country_Name" "Denmark"
        let createR = Persistence.Api.create objectName dataQ
        printfn "%A" createR

        //Testing Read
        printfn "%A" "---------- Read 1 query ----------"
        let fieldsQ         = Persistence.ReadField.createReadField [] "User" "Username"
        let fieldsQ         = Persistence.ReadField.createReadField fieldsQ "Country" "Name"
        let baseObjectNameQ = "User"
        let joinsQ          = Persistence.ObjectJoin.createObjectJoin [] "User" "Country_Name" "Country" "Name"
        let filtersQ        = Persistence.Filter.createFilter [] "Country" "Name" "=" "Denmark"
        let readR1 = Persistence.Api.read fieldsQ baseObjectNameQ joinsQ filtersQ
        printfn "%A" readR1
        
        //Testing Read again
        printfn "%A" "---------- Read 2 query ----------"
        let fieldsQ         = Persistence.ReadField.createReadFieldProc [] "User" "Date_of_birth" Persistence.ReadField.Max
        let baseObjectNameQ = "User"
        let joinsQ          = []
        let filtersQ        = []
        let readR2 = Persistence.Api.read fieldsQ baseObjectNameQ joinsQ filtersQ
        printfn "%A" readR2

        //Testing Update
        printfn "%A" "---------- Update query ----------"
        let objectName = "User"
        let dataQ      = Persistence.DataIn.createDataIn [] "User" "Address" "Updated address"
        let filtersQ   = Persistence.Filter.createFilter [] "User" "Username" "=" "tumpe2"
        let updateR = Persistence.Api.update objectName filtersQ dataQ
        printfn "%A" updateR

        //Testing Delete
        printfn "%A" "---------- Delete query ----------"
        let objectName = "User"
        let filtersQ   = Persistence.Filter.createFilter [] "User" "Id" "=" "2"
        let deleteR = Persistence.Api.delete objectName filtersQ
        printfn "%A" deleteR

        //Testing transaction
        printfn "%A" "---------- Transaction containing 2 Create and 2 delete ----------"
        let transactionQ = Persistence.Transaction.createTransaction
        let objectName = "Loggable"
        let dataQ = Persistence.DataIn.createDataIn [] objectName "Id" "3"
        let createQ = Persistence.Create.createCreate objectName dataQ
        let transactionQ = Persistence.Transaction.addCreate transactionQ createQ

        let objectName = "User"
        let dataQ = Persistence.DataIn.createDataIn []    objectName "Id" "3"
        let dataQ = Persistence.DataIn.createDataIn dataQ objectName "Username" "tumpe4"
        let dataQ = Persistence.DataIn.createDataIn dataQ objectName "Email" "tumpe4@example.com"
        let dataQ = Persistence.DataIn.createDataIn dataQ objectName "Address" "Address"
        let dataQ = Persistence.DataIn.createDataIn dataQ objectName "Date_of_birth" "2013-10-10"
        let dataQ = Persistence.DataIn.createDataIn dataQ objectName "Password" "pass"
        let dataQ = Persistence.DataIn.createDataIn dataQ objectName "Created_date" "2013-12-12"
        let dataQ = Persistence.DataIn.createDataIn dataQ objectName "Banned" "0"
        let dataQ = Persistence.DataIn.createDataIn dataQ objectName "About_me" "about me"
        let dataQ = Persistence.DataIn.createDataIn dataQ objectName "Type_name" "User"
        let dataQ = Persistence.DataIn.createDataIn dataQ objectName "Balance" "0"
        let dataQ = Persistence.DataIn.createDataIn dataQ objectName "Zipcode" "4425"
        let dataQ = Persistence.DataIn.createDataIn dataQ objectName "Country_Name" "Denmark"
        let createQ2 = Persistence.Create.createCreate objectName dataQ
        let transactionQ = Persistence.Transaction.addCreate transactionQ createQ2

        let objectName = "User"
        let filtersQ   = Persistence.Filter.createFilter [] "User" "Id" "=" "3"
        let deleteQ = Persistence.Delete.createDelete objectName filtersQ
        let transactionQ = Persistence.Transaction.addDelete transactionQ deleteQ

        let objectName = "Loggable"
        let filtersQ   = Persistence.Filter.createFilter [] "Loggable" "Id" "=" "3"
        let deleteQ2 = Persistence.Delete.createDelete objectName filtersQ
        let transactionQ = Persistence.Transaction.addDelete transactionQ deleteQ2

        let transactionR = Persistence.Api.transactionQ transactionQ
        printfn "%A" transactionR

        let s = System.Console.ReadLine()

        0 // return an integer exit code
