module OutputToSDS
open FoBAARTypes

(*let OutputToSDS (outputs : stateVector list) = 
    let dsOutputFile=DataSet.Open("msds:nc?file=C:\\depot\\FoBAARF\\Data\\Outputs.nc&openMode=create")
    let OutRecords= outputs |> List.map (fun s -> s.timeStepsElapsed) |> Seq.toArray
    let OutRecordsOut =dsOutputFile.Add("iterations",OutRecords,"iterations")
    let OutGDD= outputs |> List.map (fun s -> s.gdd2) |> Seq.toArray
    let OutGDDOut =dsOutputFile.Add("GDD",OutGDD,"iterations")
    let OutYD= outputs |> List.map (fun s -> s.dayOfYear) |> Seq.toArray
    let OutYDOut =dsOutputFile.Add("YD",OutYD,"iterations")
    let OutCF= outputs |> List.map (fun s -> s.Cf) |> Seq.toArray
    let OutCFOut =dsOutputFile.Add("Cf",OutCF,"iterations")
    let OutCR= outputs |> List.map (fun s -> s.Cr) |> Seq.toArray
    let OutCROut =dsOutputFile.Add("Cr",OutCR,"iterations")
    let OutCW= outputs |> List.map (fun s -> s.Cw) |> Seq.toArray
    let OutCWOut =dsOutputFile.Add("Cw",OutCW,"iterations")
    let OutCLit= outputs |> List.map (fun s -> s.Clit) |> Seq.toArray
    let OutCLitOut =dsOutputFile.Add("CLit",OutCLit,"iterations")
    let OutGPP= outputs |> List.map (fun s -> s.iGPP) |> Seq.toArray
    let OutGPPOut =dsOutputFile.Add("iGPP",OutGPP,"iterations")
    0.0*)

    // Print the prices in the HLOC format

