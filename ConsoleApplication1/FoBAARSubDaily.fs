module FoBAARSubDaily
// This is for a namespace
open FoBAARTypes
open Farquhar
open AssignLAI

let foBAARSubDaily env site state pars= 
    let LightOuts = 
        let outM1 = 
            if((state.totLAI>0.001)&&(env.par>0.5)) then
                let LAIsub = AssignLAI env site state pars
                let LAISun = LAIsub.AssignLAITemp
                let dfAparDaily=LAIsub.PPFDsun+LAIsub.PPFDshd

                let psynthSub = PhotoSynth env state pars LAIsub.PPFDsun
                let GA = psynthSub.An*(0.0432396*site.period)*state.Tadj*state.gdd2/pars.Phen
                let GsubDailyA = (GA*LAISun)
                let GcSubDailyA = (psynthSub.Gc*LAISun)
                let DrespA=psynthSub.Rdt*0.0432396*site.period
                let GDaily2=GA
                let GcDaily2=psynthSub.Gc

                let psynthSubShade = PhotoSynth env state pars LAIsub.PPFDshd //need to change something here for shade leaves
                let GB = psynthSubShade.An*(0.0432396*site.period)*state.Tadj*state.gdd2/pars.Phen
                let GsubDailyB = GsubDailyA+ (GB*(state.totLAI-LAISun))
                let GcSubDailyB = GcSubDailyA+ (psynthSubShade.Gc*(state.totLAI-LAISun))
                let DrespB=DrespA+psynthSubShade.Rdt*0.0432396*site.period

                let ETsubDaily=GcSubDailyB

                [|GsubDailyB;GcSubDailyB;DrespB;ETsubDaily;GDaily2;GcDaily2|]
            else
                Array.zeroCreate 6
        outM1

    let fgh = // This is just tester code - delete when debugged
                if env.dayOfYear=180.0 then
                    0.0
                    else
                    0.0

    let TrateRoot=0.5*(exp (pars.RootRespT*env.soilT))
    let Rroot=(10.0**pars.rootResp)*state.Cr*TrateRoot*site.period
    let Trate=0.5*(exp (pars.litRespT*env.airT))
    let RhLit=(10.0**pars.litResp)*state.Clit*Trate*site.period
    let ralabfrom=0.0
    let ralabto=0.0
    // calculate autotrophic respiration
    let Ra = LightOuts.[2]+ralabfrom+ralabto+Rroot+pars.gppresp*(LightOuts.[0]-LightOuts.[2])
    // calculate heterotrophic respiration
    let TrateS=0.5*(exp (pars.SOMrespT*env.meanSoilT)) // this should be mean daily soil temperature
    let Rh1=(10.0**pars.rh1)*state.cSOMPool1*TrateS*state.Tadj2*site.period
    let Rh2=(10.0**pars.rh2)*state.cSOMPool2*TrateS*state.Tadj2*site.period
    let Rh3=(10.0**pars.rh3)*state.cSOMPool3*TrateS*site.period  

    // calculate the net ecosystem exchange by summing respiration and assimilation
    let NEE = Ra+RhLit+Rh1+Rh2+Rh3-LightOuts.[0]
    {state with GDaily2=state.GDaily2+LightOuts.[4];
                GcDaily2=state.GcDaily2+LightOuts.[5];
                ralabfromDaily=state.ralabfromDaily+ralabfrom;
                ralabtoDaily=state.ralabtoDaily+ralabto;
                GDaily=state.GDaily+LightOuts.[0];
                GcDaily=state.GcDaily+LightOuts.[1];
                DrespDaily=state.DrespDaily+LightOuts.[2];
                ETDaily=state.ETDaily+LightOuts.[3];
                RrootDaily=state.RrootDaily+Rroot;
                RhLitDaily=state.RhLitDaily+RhLit;
                RaDaily=state.RaDaily+Ra; 
                Rh1Daily=state.Rh1Daily+Rh1; 
                Rh2Daily=state.Rh2Daily+Rh2; 
                Rh3Daily=state.Rh3Daily+Rh3; 
                NEEDaily=state.NEEDaily+NEE;
                radDaily=state.radDaily+env.par;
                precipDaily=state.precipDaily+env.precip;
                iNEE=state.iNEE+NEE;
                iGPP = state.iGPP + LightOuts.[0];
                iRa = state.iRa + Ra;
                iDresp=state.iDresp+LightOuts.[2];
                iRh = state.iRh + Rh1 + Rh2+Rh3+RhLit;
                iRh1=state.iRh1+Rh1;
                iRh2=state.iRh2+Rh2;
                iRh3=state.iRh3+Rh3;
                iRhLit=state.iRhLit+RhLit;
                iRoot=state.iRoot+Rroot;
                RsoilModDaily=state.RsoilModDaily+Rh1+Rh2+Rh3+RhLit+Rroot;
                iRsoil = state.iRsoil + Rh1+Rh2+Rh3+RhLit+Rroot;
                timeStepsElapsed = state.timeStepsElapsed+1}

