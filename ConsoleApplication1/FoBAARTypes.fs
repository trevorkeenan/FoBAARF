module FoBAARTypes

/// Contains the parameters of the model
type inferredParams = {
                        litToSom:float
                        gppresp:float
                        allocLeaf:float
                        allocRoot:float
                        litFallR:float
                        woodFallR:float
                        rootFallR:float
                        litResp:float
                        rh1:float
                        litRespT:float
                        litToSOMT:float
                        winterEndDay:float
                        leafDropTemp:float
                        fracLab:float
                        cfromLab:float
                        swhC:float
                        drainage:float
                        rootResp:float
                        Cr:float
                        Cw:float
                        Clit:float
                        Csom1:float
                        Clab:float
                        LMA:float
                        GDDstart:float // shoukd be int?
                        rh2:float
                        Phen:float
                        SOMrespT:float
                        RootRespT:float
                        Vcmax:float
                        gSD0:float
                        tetaph:float
                        tadj1:float
                        g1:float
                        S1tS2:float
                        RdOrig:float
                        Q10Rd:float
                        SoilR1:float
                        SoilR2:float
                        SoilR3:float
                        S1tS2T:float
                        Csom2:float
                        Csom3:float
                        rh3:float
                        S2tS3:float}                    

/// Contains a state of the system at a given time
type site = {
                stDay:int
                stDayCal:int
                nDayCal:int
                nDay:int
                nYears:int
                subDaily:float
                period:float
                siteName: string
                lat:float
                phenology: int
                numSoilPools:int
                decidflag:int
                Nit:float
                LMA:float
                Xfang:float  
                EaVcmax:float
                EdVcmax:float
                EaJmax:float
                EdJmax:float
                SJmax:float 
                }


type stateVector  = {
                    year:float
                    dayOfYear:float
                    timeStepsElapsed:int
                    hourInDay:float
                    soilWater:float    
                    Cf:float
                    totLAI:float
                    Tadj:float // Accounts for seasonal variation in VcMax
                    Tadj2:float // Additional seasonality to respiration - coupled to photosynthesis during the season.
                    cSOMPool1:float
                    cSOMPool2:float
                    cSOMPool3:float
                    G:float
                    iGPP:float
                    iNEE:float
                    iRa:float
                    iDresp:float
                    iRoot:float
                    iRh:float
                    iRhLit:float
                    iRh1:float
                    iRh2:float
                    iRh3:float
                    iD1:float
                    iD2:float
                    iDLit:float
                    iRsoil:float
                    iLw:float
                    atolab:float
                    afromlab:float
                    Lf2:float
                    ca:float
                    leafin:float // could be int
                    leafout:float // coulf be int
                    gdd:float
                    GDaily:float
                    GDaily2:float
                    GcDaily:float
                    GcDaily2:float
                    ETDaily:float
                    DrespDaily:float
                    RaDaily:float
                    RhLitDaily:float
                    Rh1Daily:float
                    Rh2Daily:float
                    Rh3Daily:float
                    RrootDaily:float
                    RsoilModDaily:float
                    ralabtoDaily:float
                    ralabfromDaily:float
                    radDaily:float
                    fAparDaily:float
                    Clit:float                 
                    multtf:float
                    multtl:float
                    Clab:float
                    Cw:float
                    Cr:float
                    gdd1:float
                    gdd2:float
                    NEEDaily:float
                    precipDaily:float
                    Ar:float
                    Lr:float
                    }

// Contains the entire climate time series data
type climateTimeSeries = {yearFlux:float[]
                          contDayInYearFlux:float[]
                          dayInYearFlux:float[]
                          indexFLUXnee:float[]
                          indexFLUXneeE:float[]
                          indexFLUXneegf:float[]
                          indexFLUXle:float[] 
                          indexFLUXleE:float[]
                          indexFLUXlegf:float[]
                          indexFLUXgpp:float[]
                          indexFLUXgppgf:float[]
                          indexFLUXre:float[]
                          indexFLUXregf:float[]
                          yearMet:float[]
                          dayInYearMet:float[]
                          hourInYear:float[]
                          temp:float[]
                          soilT:float[]
                          par:float[]
                          vpd:float[]
                          rh:float[]
                          co2:float[]
                          precip:float[]
                          yearBio:float[]
                          dayInYearBio:float[]
                          indexBioLAI:float[]
                          indexBioLAIe:float[]
                          indexBioLitterF:float[]
                          indexBioLitterFe:float[]
                          indexBioPhenology:float[]
                          indexBioPhenologyE:float[]
                          indexBioCw:float[]
                          indexBioCwE:float[]
                          indexBioCwInc:float[]
                          indexBioCwIncE:float[]
                          indexBioSoilTotC:float[]
                          indexBioSoilTotCe:float[]
                          maxt:float[];
                          mint:float[];
                          meanSoilT:float[];
                          mavt:float[];
                          precipDaily:float[]}

type outSubDailyOuts = {                                                    // canopy fluxes 
                            GSubDaily:float// Canopy photosynthesis per hour
                            GcSubDaily:float// Canopy conductance per hour
                            Dresp:float// Canopy dark respiration per hour

                            // non-canopy terms
                            Rroot:float// Root respiration
                            RhLit:float// Litter respiration
                            Ra:float// Autotrophic respiration
                            Rh1:float
                            Rh2:float
                            Rh3:float// Heterotrophic respiration from three soil carbon pools
                            NEE:float// Could be calculated externally as it is a function of the above terms
                        }
type pSynthOuts = {
                    An:float
                    gs:float
                    PSynth:float
                    Rdt:float
                    Gc:float
                    }

type LAIOuts = {
                AssignLAITemp:float
                PPFDsun:float
                PPFDshd:float
                }

type simPars = {
                dummy:int
                }

type climateSlice = {
                 dayOfYear:float
                 timeStepsElapsed:int
                 hourInDay:float
                 airT:float
                 soilT:float    // tjk debug addition Nov 20 '13
                 Ca:float
                 VPD:float
                 par:float
                 precip:float
                 maxt:float
                 mint:float
                 mavt:float
                 meanSoilT:float
                 precipDaily:float
                 }

