module FetchClimateFix2
open FSharp.Data
open FSharp.Data.Csv
open FoBAARTypes

let ReadInClimateData site = 
//    let dsClimateData = new CsvProvider<"C:\\depot\\FoBAARF\\Data\\FRHes.csv", Schema = "float,float,float,float,float,float,float,float,float,float,float,float,float,float,float,float,float,float,float,float,float,float,float,float,float,float,float,float,float,float,float,float,float,float,float,float,float,float,float,float">()
    let dsClimateData = new CsvProvider<"..//Data//FRHes2.csv", Schema = "float,float,float,float,float,float,float,float,float,float,float,float,float,float,float,float,float,float,float,float,float,float,float,float,float,float,float,float,float,float,float,float,float,float,float,float,float,float,float,float">()
    let temp = dsClimateData.Data |> Seq.map (fun r -> float r.temp) |> Seq.toArray
    let soilT = dsClimateData.Data |> Seq.map (fun r -> float r.soilT) |> Seq.toArray
    let maxtdummy=Array.zeroCreate temp.Length
    let nTsPerDay = int site.subDaily
    let maxt= [| for i in 1 .. site.nDay ->
        let extract = temp.[(i-1)*nTsPerDay..(i*nTsPerDay-1)]
        if ((Array.min extract)< -100.0) then 
            -999.0
        else
            Array.max(temp.[(i-1)*nTsPerDay..(i*nTsPerDay-1)])|]
    let mint= [| for i in 1 .. site.nDay  -> 
        let extract = temp.[(i-1)*nTsPerDay..(i*nTsPerDay-1)]
        if ((Array.min extract)< -100.0) then 
            -999.0
        else
            Array.min(temp.[(i-1)*nTsPerDay..(i*nTsPerDay-1)])|]
    let meanSoilT= [| for i in 1 .. site.nDay  -> 
        let extract = soilT.[(i-1)*nTsPerDay..(i*nTsPerDay-1)]
        if ((Array.average extract)< -100.0) then 
            -999.0
        else
            Array.average(soilT.[(i-1)*nTsPerDay..(i*nTsPerDay-1)])|]
    let dayInYearMet=dsClimateData.Data |> Seq.map (fun r -> float r.dayInYearMet) |> Seq.toArray
    let mavt= [| for i in 1 .. site.nDay -> 
        let dayInYearInd = nTsPerDay*(i-1)
        if (dayInYearMet.[dayInYearInd]>150.0) then
            Array.sum(mint.[i-10..i])*0.1
        else
            0.0|]

    let precip=dsClimateData.Data |> Seq.map (fun r -> float r.precip) |> Seq.toArray
    let precipDaily= [| for i in 1 .. site.nDay  -> 
        let extract = precip.[(i-1)*nTsPerDay..(i*nTsPerDay-1)]
        if ((Array.min extract)< -100.0) then 
            -999.0
        else
            Array.sum(precip.[(i-1)*nTsPerDay..(i*nTsPerDay-1)])|]

    {yearFlux=dsClimateData.Data |> Seq.map (fun r -> float r.yearFlux) |> Seq.toArray;
    contDayInYearFlux=dsClimateData.Data |> Seq.map (fun r -> float r.contDayInYearFlux) |> Seq.toArray;
    dayInYearFlux=dsClimateData.Data |> Seq.map (fun r -> float r.dayInYearFlux) |> Seq.toArray;
    indexFLUXnee=dsClimateData.Data |> Seq.map (fun r -> float r.PindexFLUXnee) |> Seq.toArray;
    indexFLUXneeE=dsClimateData.Data |> Seq.map (fun r -> float r.PindexFLUXneeE) |> Seq.toArray;
    indexFLUXneegf=dsClimateData.Data |> Seq.map (fun r -> float r.PindexFLUXneegf) |> Seq.toArray;
    indexFLUXle=dsClimateData.Data |> Seq.map (fun r -> float r.PindexFLUXle) |> Seq.toArray; 
    indexFLUXleE=dsClimateData.Data |> Seq.map (fun r -> float r.PindexFLUXleE) |> Seq.toArray;
    indexFLUXlegf=dsClimateData.Data |> Seq.map (fun r -> float r.PindexFLUXlegf) |> Seq.toArray;
    indexFLUXgpp=dsClimateData.Data |> Seq.map (fun r -> float r.PindexFLUXgpp) |> Seq.toArray;
    indexFLUXgppgf=dsClimateData.Data |> Seq.map (fun r -> float r.PindexFLUXgppgf) |> Seq.toArray;
    indexFLUXre=dsClimateData.Data |> Seq.map (fun r -> float r.PindexFLUXre) |> Seq.toArray;
    indexFLUXregf=dsClimateData.Data |> Seq.map (fun r -> float r.PindexFLUXregf) |> Seq.toArray;
    yearMet=dsClimateData.Data |> Seq.map (fun r -> float r.yearMet) |> Seq.toArray;
    dayInYearMet=dayInYearMet;
    hourInYear=dsClimateData.Data |> Seq.map (fun r -> float r.hourInYear) |> Seq.toArray;
    temp=temp;
    soilT=soilT;
    par=dsClimateData.Data |> Seq.map (fun r -> float r.par) |> Seq.toArray;
    vpd=dsClimateData.Data |> Seq.map (fun r -> float r.vpd) |> Seq.toArray;
    rh=dsClimateData.Data |> Seq.map (fun r -> float r.rh) |> Seq.toArray;
    co2=dsClimateData.Data |> Seq.map (fun r -> float r.co2) |> Seq.toArray;
    precip=precip;
    yearBio=dsClimateData.Data |> Seq.map (fun r -> float r.yearBio) |> Seq.toArray;
    dayInYearBio=dsClimateData.Data |> Seq.map (fun r -> float r.dayInYearBio) |> Seq.toArray;
    indexBioLAI=dsClimateData.Data |> Seq.map (fun r -> float r.PindexBioLAI) |> Seq.toArray;
    indexBioLAIe=dsClimateData.Data |> Seq.map (fun r -> float r.PindexBioLAIe) |> Seq.toArray;
    indexBioLitterF=dsClimateData.Data |> Seq.map (fun r -> float r.PindexBioLitterF) |> Seq.toArray;
    indexBioLitterFe=dsClimateData.Data |> Seq.map (fun r -> float r.PindexBioLitterFe) |> Seq.toArray;
    indexBioPhenology=dsClimateData.Data |> Seq.map (fun r -> float r.PindexBioPhenology) |> Seq.toArray;
    indexBioPhenologyE=dsClimateData.Data |> Seq.map (fun r -> float r.PindexBioPhenologyE) |> Seq.toArray;
    indexBioCw=dsClimateData.Data |> Seq.map (fun r -> float r.PindexBioCw) |> Seq.toArray;
    indexBioCwE=dsClimateData.Data |> Seq.map (fun r -> float r.PindexBioCwE) |> Seq.toArray;
    indexBioCwInc=dsClimateData.Data |> Seq.map (fun r -> float r.PindexBioCwInc) |> Seq.toArray;
    indexBioCwIncE=dsClimateData.Data |> Seq.map (fun r -> float r.PindexBioCwIncE) |> Seq.toArray;
    indexBioSoilTotC=dsClimateData.Data |> Seq.map (fun r -> float r.PindexBioSoilTotC) |> Seq.toArray;
    indexBioSoilTotCe=dsClimateData.Data |> Seq.map (fun r -> float r.PindexBioSoilTotCe) |> Seq.toArray;
    maxt=maxt;
    mint=mint;
    mavt=mavt;
    meanSoilT=meanSoilT;
    precipDaily=precipDaily}

let FetchClimate site climateData timeStepsElapsed =       
    let dayRec = int((float timeStepsElapsed)/(float site.subDaily))
    {airT=climateData.temp.[timeStepsElapsed];
    soilT=climateData.soilT.[timeStepsElapsed];     // tjk bug-fix, was omitted (Nov 20 '13)
    Ca=climateData.co2.[timeStepsElapsed];
    VPD=climateData.vpd.[timeStepsElapsed];
    par=climateData.par.[timeStepsElapsed];
    hourInDay=climateData.hourInYear.[timeStepsElapsed];
    precip=climateData.precip.[timeStepsElapsed];
    dayOfYear=climateData.dayInYearMet.[timeStepsElapsed];
    maxt=climateData.maxt.[dayRec];
    mint=climateData.mint.[dayRec];
    meanSoilT=climateData.meanSoilT.[dayRec];
    mavt=climateData.mavt.[dayRec];
    precipDaily=climateData.precipDaily.[dayRec];
    timeStepsElapsed=dayRec}

