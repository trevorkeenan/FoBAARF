module FetchClimateFix
open FoBAARTypes

(*let ReadInClimateData site = 
    let dsClimateData=DataSet.Open("msds:nc?file=C:\\depot\\FoBAARF\\Data\\FRHes.nc&openMode=readOnly")
    let temp =dsClimateData.GetData<double[]>("temp");
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

    let dayInYearMet=dsClimateData.GetData<float[]>("dayInYearMet")
    let mavt= [| for i in 1 .. site.nDay -> 
        let dayInYearInd = nTsPerDay*(i-1)
        if (dayInYearMet.[dayInYearInd]>150.0) then
            Array.sum(mint.[i-10..i])*0.1
        else
            0.0|]

    let precip = dsClimateData.GetData<float[]>("precip"); 
    let precipDaily= [| for i in 1 .. site.nDay  -> 
        let extract = precip.[(i-1)*nTsPerDay..(i*nTsPerDay-1)]
        if ((Array.min extract)< -100.0) then 
            -999.0
        else
            Array.sum(precip.[(i-1)*nTsPerDay..(i*nTsPerDay-1)])|]

    {yearFlux=dsClimateData.GetData<float[]>("yearFlux");
    contDayInYearFlux=dsClimateData.GetData<float[]>("contDayInYearFlux");
    dayInYearFlux=dsClimateData.GetData<float[]>("dayInYearFlux");
    indexFLUXnee=dsClimateData.GetData<float[]>("indexFLUXnee");
    indexFLUXneeE=dsClimateData.GetData<float[]>("indexFLUXneeE");
    indexFLUXneegf=dsClimateData.GetData<float[]>("indexFLUXneegf");
    indexFLUXle=dsClimateData.GetData<float[]>("indexFLUXle"); 
    indexFLUXleE=dsClimateData.GetData<float[]>("indexFLUXleE");
    indexFLUXlegf=dsClimateData.GetData<float[]>("indexFLUXlegf");
    indexFLUXgpp=dsClimateData.GetData<float[]>("indexFLUXgpp");
    indexFLUXgppgf=dsClimateData.GetData<float[]>("indexFLUXgppgf");
    indexFLUXre=dsClimateData.GetData<float[]>("indexFLUXre");
    indexFLUXregf=dsClimateData.GetData<float[]>("indexFLUXregf");
    yearMet=dsClimateData.GetData<float[]>("yearMet");
    dayInYearMet=dayInYearMet;
    hourInYear=dsClimateData.GetData<float[]>("hourInYear");
    temp=temp;
    par=dsClimateData.GetData<float[]>("par");
    vpd=dsClimateData.GetData<float[]>("vpd");
    rh=dsClimateData.GetData<float[]>("rh");
    soilT=dsClimateData.GetData<float[]>("soilT");
    co2=dsClimateData.GetData<float[]>("co2");
    precip=precip;
    yearBio=dsClimateData.GetData<float[]>("yearBio");
    dayInYearBio=dsClimateData.GetData<float[]>("dayInYearBio");
    indexBioLAI=dsClimateData.GetData<float[]>("indexBioLAI");
    indexBioLAIe=dsClimateData.GetData<float[]>("indexBioLAIe");
    indexBioLitterF=dsClimateData.GetData<float[]>("indexBioLitterF");
    indexBioLitterFe=dsClimateData.GetData<float[]>("indexBioLitterFe");
    indexBioPhenology=dsClimateData.GetData<float[]>("indexBioPhenology");
    indexBioPhenologyE=dsClimateData.GetData<float[]>("indexBioPhenologyE");
    indexBioCw=dsClimateData.GetData<float[]>("indexBioCw");
    indexBioCwE=dsClimateData.GetData<float[]>("indexBioCwE");
    indexBioCwInc=dsClimateData.GetData<float[]>("indexBioCwInc");
    indexBioCwIncE=dsClimateData.GetData<float[]>("indexBioCwIncE");
    indexBioSoilTotC=dsClimateData.GetData<float[]>("indexBioSoilTotC");
    indexBioSoilTotCe=dsClimateData.GetData<float[]>("indexBioSoilTotCe");
    maxt=maxt;
    mint=mint;
    mavt=mavt;
    precipDaily=precipDaily}

let FetchClimate site climateData timeStepsElapsed =       
    let dayRec = int((float timeStepsElapsed)/(float site.subDaily))
    {airT=climateData.temp.[timeStepsElapsed];
    Ca=climateData.co2.[timeStepsElapsed];
    VPD=climateData.vpd.[timeStepsElapsed];
    par=climateData.par.[timeStepsElapsed];
    hourInDay=climateData.hourInYear.[timeStepsElapsed];
    precip=climateData.precip.[timeStepsElapsed];
    dayOfYear=climateData.dayInYearMet.[timeStepsElapsed];
    maxt=climateData.maxt.[dayRec];
    mint=climateData.mint.[dayRec];
    mavt=climateData.mavt.[dayRec];
    precipDaily=climateData.precipDaily.[dayRec];
    timeStepsElapsed=dayRec}*)

