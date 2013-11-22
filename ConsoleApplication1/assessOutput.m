
% this script will...
% read in and plot the test.csv file 

close all

dataFsharp=importdata('./bin/Debug/test.csv');


dataFortran=importdata('../../Fortran/FRHes2_outputDaily_1_400_2_3.csv');

% get the true daily data

indx=dataFsharp(:,3)==24;

indexGPP=4;
indexRa=5;
indexNEE=6;
indexSoilW=7;
indexLAI=8;
indexClit=9;
indexRh1=10;
indexRh2=11;
indexRh3=12;
indexRroot=13;
indexCroot=14;
indexAr=15;
indexLr=16;

indexLAIf=42;
indexClitf=27;
indexRaf=5;
indexRh1f=7;
indexRh2f=8;
indexRh3f=9;
indexRrootf=52;
indexCrootf=25;
indexArf=16;
indexLrf=21;

NEEdaily=dataFsharp(indx,indexNEE);
GPPdaily=dataFsharp(indx,indexGPP);
soilWdaily=dataFsharp(indx,indexSoilW);
LAIdaily=dataFsharp(indx,indexLAI);
ClitDaily=dataFsharp(indx,indexClit);
RaDaily=dataFsharp(indx,indexRa);
Rh1Daily=dataFsharp(indx,indexRh1);
Rh2Daily=dataFsharp(indx,indexRh2);
Rh3Daily=dataFsharp(indx,indexRh3);
RrootDaily=dataFsharp(indx,indexRroot);
CrootDaily=dataFsharp(indx,indexCroot);
ArootDaily=dataFsharp(indx,indexAr);
LrootDaily=dataFsharp(indx,indexLr);

%%

figure;
plot(NEEdaily,'.')
hold on
plot(dataFortran(:,11),'r.')
title('NEE')

figure;
plot(GPPdaily,'.')
hold on
plot(dataFortran(:,3),'r.')
title('GPP')

figure;
plot(RaDaily,'.')
hold on
plot(dataFortran(:,indexRaf),'r.')
title('Ra')

figure;
plot(Rh1Daily,'.')
hold on
plot(dataFortran(:,indexRh1f),'r.')
title('Rh1')

figure;
plot(Rh2Daily,'.')
hold on
plot(dataFortran(:,indexRh2f),'r.')
title('Rh2')

figure;
plot(Rh3Daily,'.')
hold on
plot(dataFortran(:,indexRh3f),'r.')
title('Rh3')

figure;
plot(RrootDaily,'.')
hold on
plot(dataFortran(:,indexRrootf),'r.')
title('Rroot')


figure;
plot(LAIdaily,'.')
hold on
plot(dataFortran(:,indexLAIf),'r.')

figure;
plot(ClitDaily,'.')
hold on
plot(dataFortran(:,indexClitf),'r.')
title('C lit')

figure;
plot(CrootDaily,'.')
hold on
plot(dataFortran(:,indexCrootf),'r.')
title('C Root')

figure;
plot(ArootDaily,'.')
hold on
plot(dataFortran(:,indexArf),'r.')
title('Alloc. Root')

figure;
plot(LrootDaily,'.')
hold on
plot(dataFortran(:,indexLrf),'r.')
title('Litter Root')
