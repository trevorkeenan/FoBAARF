module UpdatePhenology
open FoBAARTypes

let GDD2Func state env site pars gdd2in= 
    let gdd2i = 
        if ((env.dayOfYear>1.0)&&(env.dayOfYear<2.0)) then 
            0.0
        else
            gdd2in
    let gdd2 = min pars.Phen (max (gdd2i+((env.maxt+env.mint)/2.0)) 0.0)
    let LMA = pars.LMA
    let LAI= max 0.001 (state.Cf/LMA)
    (gdd2,LMA,LAI)

let SetDeciduousState state env site pars = 
    let (gdd1i, gdd2i) = 
        if (env.dayOfYear<=pars.GDDstart) then
            (0.0,0.0)
        else
            (state.gdd1, state.gdd2)
    let maxfol= if state.dayOfYear>=200.0 then 0.0 else 1.0
    let multtfi=1.0
    let multt1i=0.0
    let gdd1=gdd1i+0.5*(max (env.maxt+env.mint) 0.0) 
    let (multtla, multtfa, leafout) = 
        if (gdd1>pars.winterEndDay) then
            if maxfol=1.0 then
                if state.leafout=0.0 then
                    (1.0,0.0,env.dayOfYear)
                else
                    (1.0,0.0,state.leafout)
            else
                (0.0,0.0,state.leafout)
        else
            (state.multtl,state.multtf,state.leafout)    
    //let (multtl, maxfol) = if env.dayOfYear>=200.0 then (0.0,0.0) else (multtla,maxfol)
    let multtl = if env.dayOfYear>=200.0 then 0.0 else multtla
    let (multtf,leafin) = // DAY AT WHICH PROCESS OF LEAFOUT - MODEL PREDICTED DATE
        if (((env.dayOfYear>=200.0)&&(env.mavt<pars.leafDropTemp))||((env.dayOfYear>=200.0)&&(state.leafin>0.0))) then//drop leaves
            let leafina = if state.leafin=0.0 then env.dayOfYear+9.0 else state.leafin
            (1.0,leafina)
        else
            (multtfa,state.leafin)  
    let (gdd2,LMA,LAI) = GDD2Func state env site pars gdd2i   
    {state with leafin=leafin;leafout=leafout;multtf=multtf;multtl=multtl;Tadj=1.0;Tadj2=1.0;gdd1=gdd1;gdd2=gdd2;totLAI=LAI}
    
let SetEvergreenState state env site pars = 
    let multtl = 1.0
    let multtf = 1.0
    // Make sure that evergreens have frac labile set to 1 // let P.[14]=1 // not sure
    let gdd1 = state.gdd1+0.5*(max (env.maxt+env.mint) 0.0)   
    let (Tadj,Tadj2) = 
        if (float env.dayOfYear) > pars.g1 then
            ((max 0.0 (min 1.0 (pars.tadj1/env.maxt))), (min 1.0 (max 0.0 (env.maxt/40.0)))) // not sure
        else
            (1.0,1.0)
    let (gdd2,LMA,LAI) = GDD2Func state env site pars state.gdd2
    {state with multtf=multtf;multtl=multtl;Tadj=Tadj;Tadj2=Tadj2;gdd1=gdd1;gdd2=gdd2;totLAI=LAI}

