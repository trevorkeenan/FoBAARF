module SoilWater
open FoBAARTypes
                    
let soilWaterContent env state pars = 
    // a simple bucket model for soil water content.
    let swcTemp = max (state.soilWater + env.precipDaily - state.ETDaily) 0.0

    //if (swcTemp < 0) swcTemp = 0;

    //alculate runoff
    let runoffTemp = if (swcTemp > pars.swhC) then (swcTemp - pars.swhC) else 0.0
    let swcTemp = swcTemp - runoffTemp

    // calculate drainage
    let drainageTemp = pars.drainage * swcTemp
    let swcTemp = swcTemp - drainageTemp

    (swcTemp,drainageTemp)


