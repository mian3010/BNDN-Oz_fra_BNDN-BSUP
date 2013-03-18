namespace RentIt.Persistence
open System
open System.Data
    module Helper = 
    (*
        // joinFilter returns Filter as a string
        let internal joinFilter (filter:Filter.Filter) = filter.field + " " + filter.operator + " '" + filter.value + "'"

        // joinFilters takes a List of Filter and returns as a string
        let rec internal joinFilters (filters:Filter.Filter List) = 
         match filters with 
           | [] -> ""
           | x::[] -> joinFilter x
           | x::xs -> joinFilter x+" AND "+joinFilters xs

        let internal joinDataItem k v =
         k + " = '" + v + "'"

        let internal nextKey map = 
         Map.pick (fun k x -> Some(k)) map

        let rec internal joinAllData (data:Map<string,string>) k =
         match data, k, data.Count with 
         | x, k, 1 -> joinDataItem k x.[k]
         | x, k, _ -> 
          let x2 = x.Remove(k)
          joinDataItem k x.[k]+", "+joinAllData (x2) (nextKey x2)

        let rec internal joinData data =
         joinAllData data (nextKey data)

        let internal joinInsertDataItem (ok,ov) k v =
         (ok+", "+k, ov+", '"+v+"'")

        let rec internal joinInsertData (data:Map<string,string>) = 
         match data.Count with
          | 0 -> ("", "")
          | 1 -> (nextKey data, data.[nextKey data])
          | _ -> 
           let key = nextKey data
           let data2 = data.Remove(key)
           joinInsertDataItem (joinInsertData data2) key data.[key]
           *)
        // Takes a query and performs it on the database
        let internal performSql sql = 
         let cmd = new SqlClient.SqlCommand(sql, Settings.connDb)
         cmd.ExecuteReader()
        
        // Read a row
        let internal extractRow (reader:SqlClient.SqlDataReader) data =
         let rec readData = function
          | 0, d -> d
          | i, (d:Map<string,string>) -> readData ((i-1), (d.Add(reader.GetName(i-1), (reader.GetValue(i-1).ToString()))))
         readData ((reader.FieldCount), data)

        // Read rows
        let internal extractData (reader:SqlClient.SqlDataReader) = 
         [ while reader.Read() do
                      let start:Map<string,string> = Map.empty
                      yield (extractRow reader start)
         ]
