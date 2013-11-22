module Farquhar
open FoBAARTypes

let PhotoSynth env state pars light = 

    // adjust Vcmax for soil moisture content
    let Vcmaxl = pars.Vcmax*(state.soilWater/pars.swhC) //CHECK WITH TREVOR WHAT GOES ON HERE
   
    // the following parameters are set in 'InitialiseStates.fs'. Remove from here?
    let Xfang=0.5
    let EaVcmax= 76459.0
    let EdVcmax=220121.0
    let EaJmax= 44370.0
    let EdJmax= 213908.0
    let SJmax= 710.0

    let Gammast25 = 42.22// values taken from Bernacchi et al. 2001
    let EaGammast = 37830.0// ""
    let Kc = 404.9// ""
    let Ko = 278400.0// ""
    let EaKo = 36380.0// ""
    let EaKc = 79430.0// ""
    let pabs = 0.85//* (DIM) fPAR effectively absorbed by PSII */
    let ppe = 2.6///* (mol/mol) photons absorbed by PSII per e- transported */
    let oi = 207305.0// umoles/mol (ppm)
    let gs0 = 0.001
    let Rgas = 8.3144     // the universal gas constant J/âˆ«K/mol

    let T_leaf = env.airT
    let Ci = env.Ca * 0.8

    /// Bernacchi et al. (2001) temperature dependence of Farquhar et al. 1980 Eq. 38
    let Gammast = Gammast25 * exp ((EaGammast / (Rgas * 298.1)) * (1.0 - 298.1 / (T_leaf + 273.1)))
    let Kct = Kc * exp ((EaKc / (Rgas * 298.1)) * (1.0 - 298.1 / (T_leaf + 273.1)))
    let Kot = Ko * exp ((EaKo / (Rgas * 298.1)) * (1.0 - 298.1 / (T_leaf + 273.1)))

    /// Vcmax temperature dependence
    let A = exp ((EaVcmax * (T_leaf + 273.1 - 298.1)) / (298.1 * Rgas * (T_leaf + 273.1)))
    let B = 1.0 + exp ((SJmax * 298.1 - EdVcmax) / (Rgas * 298.1))
    let C = 1.0 + exp ((SJmax * (273.1 + T_leaf) - EdVcmax) / (Rgas * (273.1 + T_leaf)))

    let Vcmaxt = Vcmaxl * A * B / C

    /// Jmax temperature dependence
    // De Pury and Farquhar et al. 1997   Plant, Cell and Env. 20:537-557
    let A2 = exp ((EaJmax * (T_leaf + 273.1 - 298.1)) / (298.1 * Rgas * (T_leaf + 273.1)))
    let B2 = 1.0 + exp ((SJmax * 298.1 - EdJmax) / (Rgas * 298.1))
    let C2 = 1.0 + exp ((SJmax * (273.1 + T_leaf) - EdJmax) / (Rgas * (273.1 + T_leaf)))

    // calculate Jmax = f(Vmax), reference:
    // Wullschleger, S.D., 1993.  Biochemical limitations to carbon assimilation
    // in C3 plants - A retrospective analysis of the A/Ci curves from
    // 109 species. Journal of Experimental Botany, 44:907-920.	
    let Jmaxt = 2.1 * Vcmaxl * A2 * B2 / C2

    // temperature response of mitochondrial respiration
    let Rdt = pars.RdOrig*0.01 * (pars.Q10Rd**((T_leaf - 25.0) / 10.0))

        // calculate J = f(Jmax, ppfd), reference:
        // de Pury and Farquhar 1997 Plant Cell and Env.
    let IradEf = (light * pabs) / ppe // need to check this
    let discriminante = sqrt ((IradEf + Jmaxt) * (IradEf + Jmaxt) - (4.0 * pars.tetaph * IradEf * Jmaxt))
    let J1 = ((IradEf + Jmaxt) + discriminante) / (2.0 * pars.tetaph)
    let J2 = ((IradEf + Jmaxt) - discriminante) / (2.0 * pars.tetaph)
    let Jk = min J1 J2
    let sol = 
        if (Vcmaxt <= 0.0)||(Jmaxt <= 0.0) then
            {An=0.0; gs=gs0; PSynth=0.0; Rdt=0.0; Gc=0.0}
        else
            let GammaA = (Gammast + (Kct * (1.0 + (oi / Kot))) * Rdt / Vcmaxt) / (1.0 - (Rdt / Vcmaxt))
            let Gamma = if ((GammaA < Gammast)||(GammaA > env.Ca)) then Gammast else GammaA

            // Analytical solution for ci. This is the ci which satisfies supply and demand
            //functions simultaneously
            //calculate X using Luening Ball Berry model (should scale for soil moisture?)
            let  X = pars.g1 / ((env.Ca - Gamma) * (1.0 + env.VPD / pars.gSD0))

            //calculate solution for ci when Rubisco activity limits A
            let Gma = Vcmaxt
            let Bta = Kct * (1.0 + oi / Kot)

            //calculate coefficients for quadratic equation for ci
            let b2 = gs0 + X * (Gma - Rdt)
            let b1 = (1.0 - env.Ca * X) * (Gma - Rdt) + gs0 * (Bta - env.Ca) - X * (Gma * Gammast + Bta * Rdt)
            let b0 = -(1.0 - env.Ca * X) * (Gma * Gammast + Bta * Rdt) - gs0 * Bta * env.Ca
            let bx = b1 * b1 - 4.0 * b2 * b0

            //calculate larger root of quadratic
            let ciquad1A = if (bx > 0.0) then (-b1 + sqrt bx) / (2.0 * b2) else 0.0
            let (Aquad1,ciquad1) = 
                if ((ciquad1A < 0.0) || (bx < 0.0)) then
                    (0.0, Ci)
                else
                    (Gma * (ciquad1A - Gamma) / (ciquad1A + Bta),ciquad1A)

            // calculate +ve root for ci when RuBP regeneration limits A
            let Gma = Jk / 4.0
            let Bta = 2.0 * Gamma

            //calculate coefficients for quadratic equation for ci		
            let b2 = gs0 + X * (Gma - Rdt)
            let b1 = (1.0 - env.Ca * X) * (Gma - Rdt) + gs0 * (Bta - env.Ca) - X * (Gma * Gammast + Bta * Rdt)
            let b0 = -(1.0 - env.Ca * X) * (Gma * Gammast + Bta * Rdt) - gs0 * Bta * env.Ca
            let bx = b1 * b1 - 4.0 * b2 * b0

            //calculate larger root of quadratic
            let ciquad2A = if (bx > 0.0) then (-b1 + sqrt bx) / (2.0 * b2) else 0.0
            let (Aquad2,ciquad2) = 
                if ((ciquad2A < 0.0) || (bx < 0.0)) then
                    (0.0,Ci)
                else
                   (Gma * (ciquad2A - Gamma) / (ciquad2A + Bta),ciquad2A)

            //choose smaller of Ac, Aq
            let An1 = (min Aquad1 Aquad2) - Rdt

            //calculate new values for gsc, cs (BBL model) Leuning R. 1995, Plant,Cell and Environ. 18:339-355 and L. et al. 18:1183-1200
            let gs = if (An1 > 0.0) then gs0 + ((pars.g1 * (An1) * 1.6) / ((env.Ca - Gamma) * (1.0 + (env.VPD / pars.gSD0)))) else gs0
            let Gc = (gs * env.VPD) * 20.0 * 10.0 / 1000.0

            // Gc is the ET rate mol/m^2 leaf area per day
            // there are 20g water per mol 
            // multiply by the density of water 1cm^3/g
            // multipy by 100cm and divide by 10^6cm^3
            let (Ci, An) = 
                if ((gs - gs0) > 0.0) then
                    let CiImed = env.Ca - ((((An1) * 1.6) / (gs - gs0))) 
                    if (CiImed < Gamma) then
                        (Gamma, (((env.Ca - Gamma) * (gs - gs0)) / 1.6) + Rdt)
                    else
                        (CiImed,An1)
                else
                    (Gamma,(((env.Ca - Gamma) * (gs - gs0)) / 1.6))
            let PSynth = An
            {An=An; gs=gs; PSynth=PSynth; Rdt=Rdt; Gc=Gc}
    sol

