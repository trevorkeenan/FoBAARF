module OutputDataCSV
open FoBAARTypes
open FSharp.Data
open FSharp.Data.Csv
open System.IO

let OutputToCSV (outputs : stateVector list) = 
    (*let dsOutData = new CsvProvider<"C:\\depot\\FoBAARF\\Data\\Outputs.csv", Schema = "float option">()
    let OutRecords= outputs |> List.map (fun s -> s.timeStepsElapsed) |> Seq.toArray
    let OutRecordsOut =dsOutputFile.Add("iterations",OutRecords,"iterations")
    for row in msft.Data do
       printfn "HLOC: (%A, %A, %A, %A)" row.High row.Low row.Open row.Close*)
    let NewOuts = outputs |> List.map (fun s -> String.concat ", " [string s.year; 
    string s.dayOfYear; string s.hourInDay; string s.GDaily; string s.RaDaily; string s.NEEDaily; string s.soilWater; string s.totLAI;
    string s.Clit; string s.Rh1Daily; string s.Rh2Daily; string s.Rh3Daily; string s.RrootDaily; string s.Cr; string s.Ar; string s.Lr])
   // let NewOuts = outputs |> List.map (fun s -> String.concat ", " [string s.timeStepsElapsed; string s.gdd2])
    System.IO.File.WriteAllLines(@".//test.csv", NewOuts);

    0.0



