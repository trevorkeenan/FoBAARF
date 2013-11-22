// Learn more about F# at http://fsharp.net
// See the 'F# Tutorial' project for more help.
open FoBAARTypes
open Farquhar
open AssignLAI
open SoilWater
open FetchClimateFix2
open UpdatePhenology
open InitialiseStates
open FoBAARSubDaily
open OutputDataCSV

let DailyFoBAARLoop (stateIn : stateVector) climate site pars = 
    let env = FetchClimate site climate stateIn.timeStepsElapsed

    let state =
        if (env.hourInDay=0.5) then
            let yearday=env.dayOfYear
            let stateDay = // set the initial state
                if ((yearday>=1.0)&&(yearday<2.0)) then
                    let stateA = InitStateYear stateIn pars site // may need a seperate ZeroState function
                    InitStateDay stateA pars site // may need a seperate ZeroState function
                else
                    InitStateDay stateIn pars site // may need a seperate ZeroState function
            if (site.phenology=1) then
                SetDeciduousState stateDay env site pars
            else
                SetEvergreenState stateDay env site pars
        else
            {stateIn with hourInDay = env.hourInDay; dayOfYear = env.dayOfYear}

    let subDailyOutput = foBAARSubDaily env site state pars

    let output =
        if (env.hourInDay=24.0) then
            let GDailyPrevious=subDailyOutput.GDaily
            // calculate the temperature rates to be used for decomposition and respiration
            let TrateDaily=0.5*(exp (pars.litToSOMT*(0.5*(env.maxt+env.mint))))
            let TrateDailyS=0.5*(exp (pars.S1tS2T*(env.meanSoilT))) // fix
            // Calculate the transfer of Litter from the litter pool to the fast soil pool	
            let Dlit = (10.0**pars.litToSom)*subDailyOutput.Clit*TrateDaily
            // Set transfer rate between soil pools
            let D1 = 
                if(site.numSoilPools>=2) then
                    (pars.S1tS2)*subDailyOutput.Rh1Daily //TrateDailyS	
                else
                    0.0
            let D2 = 
                if (site.numSoilPools=3) then
                    (pars.S2tS3)*subDailyOutput.Rh2Daily //TrateDailyS	
                else
                    0.0
            let iD1=subDailyOutput.iD1+D1
            let iD2=subDailyOutput.iD2+D2
            let iDlit=subDailyOutput.iDLit+Dlit

            let fgh = // This is just tester code - delete when debugged
               if env.dayOfYear=200.0 then
                   0.0
                   else
                   0.0

            let nppimed = subDailyOutput.GDaily-subDailyOutput.DrespDaily
            let npp = 
                if(subDailyOutput.GDaily>subDailyOutput.DrespDaily) then
                    nppimed-pars.gppresp*(subDailyOutput.GDaily-subDailyOutput.DrespDaily)
                else
                    nppimed
            let Atolab =
                if ((subDailyOutput.multtf>0.0)&&(site.decidflag>0)) then
                    (1.0-pars.fracLab)*(10.0**pars.litFallR)*subDailyOutput.Cf
                else
                    0.0
            let (Afromlab,Af,npp2) = 
                if(subDailyOutput.multtl>0.0) then
                    let Afromlabi=10.0**(pars.cfromLab)*subDailyOutput.Clab*(float site.decidflag) // simplifying the leaf allocation routine here.
                    let (Afi,npp2ia) = 
                        if (npp>0.0) then
                            let npp2ib = npp-(npp*pars.allocLeaf*subDailyOutput.multtl) // allocate to foliage
                            (((npp2ib*pars.allocLeaf*subDailyOutput.multtl)+Afromlabi),npp2ib)                       
                        else
                            (Afromlabi,0.0)
                    (Afromlabi,Afi,npp2ia)      
                else
                    (0.0,0.0,npp)  

            let (Ar,Aw) = 
                if (npp2>0.0) then
                    let Awi=(1.0-pars.allocRoot)*npp2// allocate to wood
                    (npp2*pars.allocRoot,Awi)// allocate to roots
                else
                    (0.0,0.0)
            let Lf1 = 
                if (subDailyOutput.multtf>0.0) then
                    if (subDailyOutput.totLAI<=0.5) then
                        subDailyOutput.Cf 
                    else
                        (10.0**pars.litFallR)*subDailyOutput.Cf*pars.fracLab*subDailyOutput.multtf
                else
                    0.0
            let fgh = // This is just tester code - delete when debugged
                if subDailyOutput.dayOfYear=150.0 then
                    0.0
                else
                    0.0

            let Lw = (10.0**pars.woodFallR)*subDailyOutput.Cw
            let Lr = (10.0**pars.rootFallR)*subDailyOutput.Cr
            let Cf = max 0.0 (subDailyOutput.Cf + Af - Lf1 - Atolab - state.ralabtoDaily)
            let Cw = max 0.0 (subDailyOutput.Cw + Aw - Lw)
            let Cr = max 0.0 (subDailyOutput.Cr+ Ar - Lr-state.RrootDaily)
            let Clit = max 0.0 (subDailyOutput.Clit + Lf1 + Lr - subDailyOutput.RhLitDaily - Dlit)
            let CsomPools1 = max 0.0 (subDailyOutput.cSOMPool1+ Dlit-D1-subDailyOutput.Rh1Daily+Lw)
            let CsomPools2 = max 0.0 (subDailyOutput.cSOMPool2+D1-D2 - subDailyOutput.Rh2Daily)
            let CsomPools3 = max 0.0 (subDailyOutput.cSOMPool3+D2 - subDailyOutput.Rh3Daily)
            let Clab=max 0.0 (subDailyOutput.Clab+Atolab-Afromlab-state.ralabfromDaily) 
            let Lf2=state.Lf2+Lf1
            let Csom = CsomPools1 + CsomPools2 + CsomPools3
            let (swc,runOff) = soilWaterContent env subDailyOutput pars // including past water content
            {subDailyOutput with Ar=Ar;Lr=Lr;Cf=Cf;Cw=Cw;Cr=Cr;Clit=Clit;cSOMPool1=CsomPools1;cSOMPool2=CsomPools2;cSOMPool3=CsomPools3;Clab=Clab;soilWater=swc;Lf2=Lf2; timeStepsElapsed=stateIn.timeStepsElapsed+1}
        else
            subDailyOutput
    output

[<EntryPoint>]
let main argv = 
    printfn "%A" argv
    let defaultPars = DefineDefaultPars
    let site = SetSiteHes
    let climateData = ReadInClimateData site
    let initState = InitStateSimulation defaultPars site
    let initTime = 1
    let endTime = site.nDay*(int site.subDaily)

    let output = 
        initState |> Seq.unfold (fun state ->
            if (int state.timeStepsElapsed)>=endTime then 
                None 
            else 
                let newState = DailyFoBAARLoop state climateData site defaultPars
                Some(newState,newState)
        )|> Seq.toList

    let ArrayLen = output.Length
    let sol = 
        output|> List.map (fun s -> (s.timeStepsElapsed,s.hourInDay,s.dayOfYear,s.gdd2))
        |> Seq.toArray

    let doneOuts = OutputToCSV output

    0 // return an integer exit code
