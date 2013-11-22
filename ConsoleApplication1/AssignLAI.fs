module AssignLAI
open FoBAARTypes

// PARTITIONING LIGHT BETWEEN SUN AND SHADE
// HOW MUCH LAI IS IN THE SUN - PPFD SUN AND SHADE LEAVES
let AssignLAI env site (state:stateVector) pars =
    let pi = 3.1416

    // 1 - calculate the solar zenith angle
    // calculations according to Goudriaan & van Laar 1994 p30
    let pidiv = pi / 180.0// pi / 180

    // sine and cosine of latitude
    let sinlat = sin site.lat// lat is in radians 
    let coslat = cos site.lat

    // sine of maximum declination
    let sindec = -1.0*(sin (23.45 * pidiv) * cos (2.0 * pi * (state.dayOfYear + 10.0) / 365.0))
    let cosdec = sqrt (1.0 - sindec * sindec) // equation 3.5 Goudriaan & van Laar
    // terms A & B
    let A = sinlat * sindec
    let B = coslat * cosdec
    let coszen = A + B * cos (2.0 * pi * (state.hourInDay/site.period - (site.subDaily / 2.0)) / (site.subDaily)); // = solar zenith angle (equation 3.2 Goudriaan & van Laar) // What is j here?
//    let coszen = A + B * cos(2.0 *pi * (state.hourInDay - (site.subDaily / 2.0)) / (site.subDaily))   // = solar zenith angle (equation 3.2 Goudriaan & van Laar)
    
    // 2 - calculate the fraction of sunlit leaves
    // Ross-Goudriaan function for G(u) (see Wang et al. 2007 eq 13)
    let xphi1 = 0.5 - 0.633 * site.Xfang - 0.33 * site.Xfang * site.Xfang
    let xphi2 = 0.877 * (1.0 - 2.0 * xphi1)
    let funG = xphi1 + xphi2 * coszen //G-function: Projection of unit leaf area in direction of beam

    let (extKb,AssignLAITemp) = 
        if (coszen > 0.0) then
            let extKbi = funG/coszen
            let imed = (1.0 - exp (-extKbi * state.totLAI)) / extKbi
            if (imed > state.totLAI) then
                (extKbi,state.totLAI)
            elif (imed < 0.0) then
                (extKbi,0.0)
            else
                (extKbi,imed)
        else
            (0.0,100.0)

    //Effective extinction coefficient for diffuse radiation Goudriaan & van Laar Eq 6.6)
    let cozen15 = cos (pidiv * 15.0)
    let cozen45 = cos (pidiv * 45.0)
    let cozen75 = cos (pidiv * 75.0)
    let xk15 = xphi1 / cozen15 + xphi2
    let xk45 = xphi1 / cozen45 + xphi2
    let xk75 = xphi1 / cozen75 + xphi2
    let transd = 0.308 * exp (-xk15 * state.totLAI) + 0.514 * exp (-xk45 * state.totLAI) + 0.178 * exp (-xk75 * state.totLAI)
    let extkd = (-1.0 / state.totLAI) * log transd // might be problem with alog here

    // Goudriaan theory as used in Leuning et al 1995 (Eq Nos from Goudriaan & van Laar, 1994)
    let radsol = env.par
    let sinbet = coszen
    let solext = 1370.0 * (1.0 + 0.033 * cos (2.0 * pi * (state.dayOfYear - 10.0) / 365.0)) * sinbet
    let tmprat = radsol / solext

    let tmpr = 0.847 - 1.61 * sinbet + 1.04 * sinbet * sinbet
    let tmpk = (1.47 - tmpr) / 1.66
    let fdiff = 
        if (tmprat <= 0.22) then
            1.0
        elif ((tmprat > 0.22) && (tmprat <= 0.35)) then 
            1.0 - 6.4 * (tmprat - 0.22) * (tmprat - 0.22)
        elif ((tmprat > 0.35) && (tmprat <= tmpk)) then
            1.47 - 1.66 * tmprat
        elif (tmprat >= tmpk) then
            tmpr
        else
            0.0
    let fbeam = 1.0 - fdiff
    let tauL = [|0.1; 0.425|]               //for NIR
    let rhoL = [|0.1;0.425|]                 // leaf reflectance for vis
    let rhoS = [|0.1; 0.3|]                 // for NIR - later function of soil water content
    let scatT = Array.zeroCreate 2
    let kpr = Array2D.zeroCreate 2 2
    let rhoc = Array2D.zeroCreate 2 2
    let reff = 
        let matrix = Array2D.zeroCreate 2 2
        for nw in 0..1 do //nw:1=VIS, 2=NIR
            scatT.[nw] <- tauL.[nw] + rhoL.[nw]                                      //scattering coeff
            kpr.[nw, 0] <- extKb * sqrt (1.0 - scatT.[nw])                              //modified k beam scattered (6.20)
            kpr.[nw, 1] <- extkd * sqrt (1.0 - scatT.[nw])                              ///modified k diffuse (6.20)
            let rhoch = (1.0 - sqrt (1.0 - scatT.[nw])) / (1.0 + sqrt (1.0 - scatT.[nw]))          //canopy reflection black horizontal leaves (6.19)
            let rhoc15 = 2.0 * xk15 * rhoch / (xk15 + extkd)                               //canopy reflection (6.21) diffuse
            let rhoc45 = 2.0 * xk45 * rhoch / (xk45 + extkd)
            let rhoc75 = 2.0 * xk75 * rhoch / (xk75 + extkd)
            rhoc.[nw, 1] <- 0.308 * rhoc15 + 0.514 * rhoc45 + 0.178 * rhoc75
            rhoc.[nw, 0] <- 2.0 * extKb / (extKb + extkd) * rhoch;                         //canopy reflection (6.21) beam 
            matrix.[nw, 0] <- rhoc.[nw, 0] + (rhoS.[nw] - rhoc.[nw, 0]) * exp (-2.0 * kpr.[nw, 0] * state.totLAI)                    //effective canopy-soil reflection coeff - beam (6.27)
            matrix.[nw, 1] <- rhoc.[nw, 1] + (rhoS.[nw] - rhoc.[nw, 1]) * exp (-2.0 * kpr.[nw, 1] * state.totLAI)                       //effective canopy-soil reflection coeff - diffuse (6.27)
        matrix
    let radabv = [| env.par; 0.0 |]// we only use par input ....        
    let Qabs = Array2D.zeroCreate 2 2

    //for (int nw = 0; nw < 1; nw++)
    //{//nw:1=VIS, 2=NIR
    let nw=0
    let Qd0 = (1.0 - fbeam) * radabv.[nw]                                        //diffuse incident radiation
    let Qb0 = fbeam * radabv.[nw]                                            //beam incident radiation
    Qabs.[nw, 1] <- Qd0 * (kpr.[nw, 1] * (1.0 - reff.[nw, 1]) * exp (-kpr.[nw, 1] * state.totLAI)) + Qb0 * (kpr.[nw, 0] * (1.0 - reff.[nw, 0]) * exp (-kpr.[nw, 0] * state.totLAI) - extKb * (1.0 - scatT.[nw]) * exp (-extKb * state.totLAI))   // total absorbed radiation - shaded leaves, diffuse beam scattered, total absorbed radiation - sunlit leaves          
    Qabs.[nw, 0] <- Qabs.[nw, 1] + extKb * Qb0 * (1.0 - scatT.[nw])                    //  
    //}
    let PPFDsun = Qabs.[0, 0];
    let PPFDshd = Qabs.[0, 1];
    {AssignLAITemp=AssignLAITemp; PPFDsun=PPFDsun; PPFDshd=PPFDshd}

