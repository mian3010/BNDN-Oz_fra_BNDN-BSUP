// Learn more about F# at http://fsharp.net
// See the 'F# Tutorial' project for more help.

namespace RentIt

module Main = 
    [<EntryPoint>]
    let main argv = 
        let result = Persistence.Get "user" [{field="Username";operator="=";value="tumpe"}]
        let rec outString (inResult:Map<string,string> List) = 
         match inResult with
          | [] -> ""
          | x::xs -> (Map.find "Username" x) + "\n\r" + outString xs
        printfn "%A" (outString result)
        0 // return an integer exit code
