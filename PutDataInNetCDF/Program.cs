using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Research.Science.Data;
using Microsoft.Research.Science.Data.Imperative;

namespace PutDataInNetCDF
{
    class Program
    {
        static void Main(string[] args)
        {
            double missingData = (double)-9999.0;
            DataSet dsInputFlux = DataSet.Open("msds:csv?file=C:\\depot\\FoBAARF\\Data\\FRHes\\fluxData.csv&openMode=readOnly");
            DataSet dsOutput = DataSet.Open("msds:csv?file=C:\\depot\\FoBAARF\\Data\\FRHes.csv&openMode=create");
            dsOutput.IsAutocommitEnabled = false;
            string[] fluxNames = { "yearFlux", "contDayInYearFlux", "dayInYearFlux", "indexFLUXnee", "indexFLUXneeE", "indexFLUXneegf",
                                   "indexFLUXle", "indexFLUXleE", "indexFLUXlegf", "indexFLUXgpp", "indexFLUXgppgf", "indexFLUXre", "indexFLUXregf"};
            int[] fluxInds= {1,2,3,4,5,6,7,8,9,11,12,14,14};
            double[] yearFlux = dsInputFlux.GetData<double[]>(1);
            int size = yearFlux.Length;
            double[] fluxDataOut = new double[size-3];
            int[] fluxRecord = new int[size - 3];
            for (int jj = 0; jj < fluxRecord.Length; jj++) fluxRecord[jj] = jj;
            var aa = dsOutput.Add<int[]>("fluxID", fluxRecord, "fluxID");
            for (int ii = 0; ii < fluxInds.Length; ii++)
            {
                double[] input = dsInputFlux.GetData<double[]>(fluxInds[ii]);
                for (int jj = 3; jj < size; jj++) fluxDataOut[jj - 3] = (double)input[jj];
                aa = dsOutput.Add<double[]>(fluxNames[ii], fluxDataOut, "fluxID");
                aa.MissingValue = missingData;
            }
                
            DataSet dsInputMet = DataSet.Open("msds:csv?file=C:\\depot\\FoBAARF\\Data\\FRHes\\metData2.csv&openMode=readOnly");
            string[] metNames = { "yearMet", "dayInYearMet", "hourInYear", "temp", "par", "vpd","rh", "soilT", "co2", "precip"};
            int[] metInds = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            double[] yearMet = dsInputMet.GetData<double[]>(1);
            size = yearMet.Length;
            double[] metDataOut = new double[size - 3];
            int[] metRecord = new int[size - 3];
            for (int jj = 0; jj < metRecord.Length; jj++) metRecord[jj] = jj;
            var bb = dsOutput.Add<int[]>("metID", metRecord, "metID");
            for (int ii = 0; ii < metInds.Length; ii++)
            {
                double[] input = dsInputMet.GetData<double[]>(metInds[ii]);
                for (int jj = 3; jj < size; jj++) metDataOut[jj - 3] = (double)input[jj];
                bb = dsOutput.Add<double[]>(metNames[ii], metDataOut, "metID");
                bb.MissingValue = missingData;
            }

            DataSet dsInputBio = DataSet.Open("msds:csv?file=C:\\depot\\FoBAARF\\Data\\FRHes\\bioData2.csv&openMode=readOnly");
            string[] bioNames = { "yearBio", "dayInYearBio", "indexBioLAI", "indexBioLAIe", "indexBioLitterF", "indexBioLitterFe","indexBioPhenology",
                                   "indexBioPhenologyE", "indexBioCw", "indexBioCwE", "indexBioCwInc", "indexBioCwIncE", "indexBioSoilTotC", "indexBioSoilTotCe"};
            int[] bioInds = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 11, 12, 13, 14,15 };
            double[] yearBio = dsInputBio.GetData<double[]>(1);
            size = yearBio.Length;
            double[] bioDataOut = new double[size - 1];
            int[] bioRecord = new int[size - 1];
            for (int jj = 0; jj < bioRecord.Length; jj++) bioRecord[jj] = jj;
            var cc = dsOutput.Add<int[]>("bioID", bioRecord, "bioID");
            for (int ii = 0; ii < bioInds.Length; ii++)
            {
                double[] input = dsInputBio.GetData<double[]>(bioInds[ii]);
                for (int jj = 1; jj < size; jj++) bioDataOut[jj - 1] = (double)input[jj];
                cc = dsOutput.Add<double[]>(bioNames[ii], bioDataOut, "bioID");
                cc.MissingValue = missingData;
            }

            dsOutput.Commit();

            /*foreach (string dsVar in varNames)
            {
                data = new double[maxRows, datasetNames.Length];
                for (int jj = 0; jj<datasetNames.Length; jj++)
                {
                    DataSet dsLoaded = DataSet.Open("msds:csv?file=C:\\Users\\mattsmi\\SkyDrive @ Microsoft\\GlobalCarbonDynamics\\MakeCMIP5Plots\\ForrestData\\" + datasetNames[jj] + "&openMode=readOnly");
                    double[] time = dsLoaded.GetData<double[]>("time");
                    if (dsLoaded.Variables.Contains(dsVar))
                    {
                        double[] dataIn = dsLoaded.GetData<double[]>(dsVar);
                        int counter = 0;
                        for (int ii = 0; ii < maxRows; ii++)
                        {
                            if (counter < (time.Length))
                            {
                                if (times[ii] == time[counter])
                                {
                                    data[ii, jj] = dataIn[counter++];
                                }
                                else
                                {
                                    data[ii, jj] = missingNumber;
                                }
                            }
                            else
                            {
                                data[ii, jj] = missingNumber;
                            }
                        }
                    }
                    else
                    {
                        for (int ii = 0; ii < time.Length; ii++) data[ii, jj] = missingNumber;
                    }
                    dsLoaded.Dispose();
                }
                var aa = dsOutput.Add<double[,]>(dsVar,data,dim2D);
                aa.MissingValue = missingNumber;
            }

            double[,] cumLandCDirect = new double[maxRows, datasetNames.Length];
            double[,] cumLandCInDirect = new double[maxRows, datasetNames.Length];

            for (int jj = 0; jj < datasetNames.Length; jj++)
            {
                DataSet dsLoaded = DataSet.Open("msds:csv?file=C:\\Users\\mattsmi\\SkyDrive @ Microsoft\\GlobalCarbonDynamics\\MakeCMIP5Plots\\ForrestData\\" + datasetNames[jj] + "&openMode=readOnly");
                double[] time = dsLoaded.GetData<double[]>("time");
                if (dsLoaded.Variables.Contains("delta_Cl"))
                {
                    double[] dataIn = dsLoaded.GetData<double[]>("delta_Cl");
                    int counter = 0;
                    for (int ii = 0; ii < maxRows; ii++)
                    {
                        if (counter < (time.Length))
                        {
                            if (times[ii] == time[counter])
                            {
                                cumLandCDirect[ii, jj] = (counter == 0) ? dataIn[counter++] : cumLandCDirect[ii - 1,jj] + dataIn[counter++];
                            }
                            else
                            {
                                cumLandCDirect[ii, jj] = missingNumber;
                            }
                        }
                        else
                        {
                            cumLandCDirect[ii, jj] = missingNumber;
                        }
                    }
                }
                else
                {
                    for (int ii = 0; ii < time.Length; ii++) cumLandCDirect[ii, jj] = missingNumber;
                }
            }
            var bb = dsOutput.Add<double[,]>("CumulativeLandC", cumLandCDirect, dim2D);
            bb.MissingValue = missingNumber;

            for (int jj = 0; jj < datasetNames.Length; jj++)
            {
                DataSet dsLoaded = DataSet.Open("msds:csv?file=C:\\Users\\mattsmi\\SkyDrive @ Microsoft\\GlobalCarbonDynamics\\MakeCMIP5Plots\\ForrestData\\" + datasetNames[jj] + "&openMode=readOnly");
                double[] time = dsLoaded.GetData<double[]>("time");
                if ((dsLoaded.Variables.Contains("ffe"))&&(dsLoaded.Variables.Contains("delta_Ca"))&&(dsLoaded.Variables.Contains("delta_Co")))
                {
                    double[] dataffe = dsLoaded.GetData<double[]>("ffe");
                    double[] dataCa = dsLoaded.GetData<double[]>("delta_Ca");
                    double[] dataCo = dsLoaded.GetData<double[]>("delta_Co");
                    int counter = 0;
                    for (int ii = 0; ii < maxRows; ii++)
                    {
                        if (counter < (time.Length))
                        {
                            if (times[ii] == time[counter])
                            {
                                double dl = dataffe[counter] - dataCa[counter] - dataCo[counter];
                                cumLandCInDirect[ii, jj] = (counter == 0) ? dl : cumLandCInDirect[ii - 1, jj] + dl;
                                counter++;
                            }
                            else
                            {
                                cumLandCInDirect[ii, jj] = missingNumber;
                            }
                        }
                        else
                        {
                            cumLandCInDirect[ii, jj] = missingNumber;
                        }
                    }
                }
                else
                {
                    for (int ii = 0; ii < time.Length; ii++) cumLandCInDirect[ii, jj] = missingNumber;
                }
            }
            var cc = dsOutput.Add<double[,]>("CumulativeLandCIndirect", cumLandCInDirect, dim2D);
            cc.MissingValue = missingNumber;*/
        }
    }
}
