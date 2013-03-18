// Learn more about F# at http://fsharp.net
// See the 'F# Tutorial' project for more help.

namespace RentIt

module Main = 
    [<EntryPoint>]
    let main argv = 
        let fieldsQ:Persistence.Types.SelectField List = [
            {
                field={
                        objectName="User";
                        field="";
                        processor=Persistence.Field.AllInObject
                };
                processor=Persistence.SelectField.Default
            }
        ]
        let baseObjectNameQ = "User"
        let joinsQ = []
        let filtersQ:Persistence.Types.Filter List = [
            {
                field={
                        objectName="User";
                        field="Username";
                        processor=Persistence.Field.Default
                };
                operator="=";
                value="tumpe";
                processor=Persistence.Filter.Default;
            }
        ]
        let processorQ = Persistence.Select.Default
        let selectQ:Persistence.Types.Select = {
            fields=fieldsQ;
            baseObjectName=baseObjectNameQ;
            joins=joinsQ;
            filters=filtersQ;
            processor=processorQ;
        }
        let result = Persistence.Api.Read selectQ
        printfn "%A" result
        let s = System.Console.ReadLine()
(*
        let data:Map<string,string> = Map.empty
        let data = data.Add("Address", "New address2")
        let data = data.Add("Banned", "1")
        let result2 = Persistence.Update "user" [{field="Username";operator="=";value="tumpe"}] data
        printfn "%A" result2

        let mutable data:Map<string,string> = Map.empty
        data <- data.Add("Id", "1")
        data <- data.Add("Username", "tumpe2")
        data <- data.Add("Email", "tumpe2@example.com")
        data <- data.Add("Address", "Address")
        data <- data.Add("Date_of_birth", "2013-10-10")
        data <- data.Add("Password", "pass")
        data <- data.Add("Created_date", "2013-10-10")
        data <- data.Add("Banned", "0")
        data <- data.Add("About_me", "about me")
        data <- data.Add("Type_name", "User")
        data <- data.Add("Balance", "0")
        data <- data.Add("Zipcode", "2340")
        data <- data.Add("Country_Name", "Denmark")
        let result3 = Persistence.Insert "user" data
        printfn "%A" result3

        let result4 = Persistence.Delete "user" [{field="Username";operator="=";value="tumpe2"}]
        printfn "%A" result
        *)
        0 // return an integer exit code
