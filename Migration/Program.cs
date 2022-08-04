using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Linq;
using System.Reflection;

namespace Migration
{
    internal class Program
    {
        public static CultureInfo ci = new CultureInfo("en-US");

        public static string asmPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        static void Main(string[] args)
        {
            Thread.CurrentThread.CurrentCulture = ci;

            MigrationParams mp = new MigrationParams(0, 0, 0, 0, 0, 0, 0, new float[2], String.Empty, String.Empty, 0, 0, 0, 0, 0f, 0f);

            mp.asmPath = asmPath;
            mp.financialLocation = $"{mp.asmPath}\\texts\\financial.txt";
            mp.parameterLocation = $"{mp.asmPath}\\texts\\parameter.txt";
            mp.gradesimulationLocation = $"{mp.asmPath}\\texts\\gradesimulation.txt";

            MasterMigration master = new MasterMigration(mp);

            master.ReadParams();
            master.ReadFinancial();
            master.ReadGradeSimulations();





            //int blocks_z = 0, blocks_x = 0, blocks_y = 0, financial_simulations = 0, financial_parameters = 0, grade_simulations = 0, noOfDestinations = 0;
            //[] tonnage = new float[2];

            //string unitPrice = String.Empty;
            // unitGrade = String.Empty;

            //[,] financial = ReadFinancial(financial_parameters, grade_simulations);


            // row = 0, column = 0, levels = 0, numberOfSimulations = 0;
            //float conversionFactorPrice = 0.0f, conversionFactorGrade = 0.0f;

            //[,,,] data = readGradeSimulations(blocks_x, blocks_y, blocks_z, financial_parameters, financial_simulations, grade_simulations, unitPrice, unitGrade, ref row, ref column, ref levels, ref conversionFactorPrice, ref conversionFactorGrade, ref numberOfSimulations);

            //calculateRisk(levels, row, column, data, financial, tonnage, conversionFactorPrice, conversionFactorGrade, financial_simulations, numberOfSimulations, noOfDestinations);

            /*
        }
        
     
        public static double[,,,] readGradeSimulations(int blocksX, int blocksY, int blocksZ, int financialParams, int financalSims, int gradeSims, string unitPrice, string unitGrade, ref int row, ref int column, ref int levels, ref float conversionFactorPrice, ref float conversionFactorGrade, ref int numberOfSimulations)
        {
            string fileName = $"{asmPath}\\texts\\gradesimulation.txt";

            int levels_L = 0, row_L = 0, column_L = 0;

            double[,,,] arr4D = new double[blocksX + 1, blocksY + 1, blocksZ + 1, financialParams + 1];

            int firstPos = 0, secondPos = 0, thirdPos = 0;

            using (StreamReader streamReader = new StreamReader(fileName))
            {
                while (streamReader.Peek() >= 0)
                {
                    int i = 0;
                    string line = string.Empty;
                    line = streamReader.ReadLine();
                    string[] lineArr = line.Split(',');
                    double value = 0;


                    //! DEBUG
                    //Console.WriteLine(lineArr[0] + " " + lineArr[1] + " " + lineArr[2] + " " + lineArr[3] + " " + lineArr[4] + " " + lineArr[5]);
                    for (int x = 0; x < lineArr.Length; x++)
                    {
                        if (i == 0)
                        {
                            firstPos = int.Parse(lineArr[x]);
                            if (firstPos > levels_L)
                            {
                                row_L = firstPos;
                            }
                        }
                        else if (i == 1)
                        {
                            secondPos = int.Parse(lineArr[x]);
                            if (secondPos > row_L)
                            {
                                column_L = secondPos;
                            }
                        }
                        else if (i == 2)
                        {
                            thirdPos = int.Parse(lineArr[x]);
                            if (thirdPos > column_L)
                            {
                                levels_L = thirdPos;
                            }
                        }
                        else if (i > 2)
                        {
                            if (lineArr[4].Equals("-"))
                            {
                                arr4D[firstPos, secondPos, thirdPos, i - 2] = -1; //"-1;
                            }
                            else
                            {
                                _ = double.TryParse(lineArr[x], out value);
                                arr4D[firstPos, secondPos, thirdPos, i - 3] = value;
                            }
                            //! DEBUG
                            //Console.Write(arr4D[firstPos, secondPos, thirdPos, i - 3] + " ");
                        }

                        i = i + 1;
                    }
                    numberOfSimulations = i;
                }
            }

            float conversionFactorPrice_L = 1f;
            float conversionFactorGrade_L = 1f;

            #region unitGradeEq
            if (unitGrade.Equals("%"))
            {
                GradeUnit = 0;
            }
            else if (unitGrade.Equals("ppm"))
            {
                GradeUnit = 1;
            }
            else if (unitGrade.Equals("gTon"))
            {
                GradeUnit = 2;
            }
            else if (unitGrade.Equals("kCal/kg"))
            {
                GradeUnit = 4;
            }
            #endregion

            #region conversionFactorPriceEq
            if (unitPrice.Equals("$pg"))
            {
                conversionFactorPrice_L = SetMineParams(1);
            }
            else if (unitPrice.Equals("$pkg"))
            {
                conversionFactorPrice_L = SetMineParams(2);
            }
            else if (unitPrice.Equals("$pTonnage"))
            {
                conversionFactorPrice_L = SetMineParams(3);
            }
            else if (unitPrice.Equals("$pLongTon"))
            {
                conversionFactorPrice_L = SetMineParams(4);
            }
            else if (unitPrice.Equals("$pShortTon"))
            {
                conversionFactorPrice_L = SetMineParams(5);
            }
            else if (unitPrice.Equals("$poz"))
            {
                conversionFactorPrice_L = SetMineParams(6);
            }
            else if (unitPrice.Equals("$ptroyoz"))
            {
                conversionFactorPrice_L = SetMineParams(7);
            }
            else if (unitPrice.Equals("$plb"))
            {
                conversionFactorPrice_L = SetMineParams(8);
            }

            row = row_L;
            levels = levels_L;
            column = column_L;
            conversionFactorGrade = conversionFactorGrade_L;
            conversionFactorPrice = conversionFactorPrice_L;
            #endregion

            return arr4D;
        }
        public static double calculateNpv(double[,,,] data, float[,] financial, int z, int x, int y, int k, int w, ref float[] tonnage, float conversionFactorPrice, float conversionFactorGrade, int noOfDestinations)
        {
            float npv = 0f;
            //float[,] financial = new float[financialParams, financial_sims];
            // double[,,,] arr4D = new double[blocksX + 1, blocksY + 1, blocksZ + 1, financialParams + 1];
            int period = (int)data[x, y, z, 0];

            //data = int.TryParse(data,out period);
            int destination = (int)data[x, y, z, 1];
            int dest = destination;
            destination = destination + 1; //to adjust destinations to start from 11
            float price = financial[w, 0];

            int recoveryIndexCheck = (destination - 2) * 2 + 2;
            float recovery;

            if (recoveryIndexCheck < 0)
            {
                recovery = financial[w, 6];
            }
            else
            {
                recovery = financial[w, recoveryIndexCheck];
            }

            int indexCheck = ((destination - 2) * 2 + 1);

            float processingCost;

            if (indexCheck < 0)
            {
                processingCost = 0;
            }
            else
            {
                processingCost = financial[w, indexCheck];
            }

            int start = 0;
            start = noOfDestinations * 2 + 2;
            float miningCost = financial[w, start + dest - 3]; //! Decremeneted 3 bcs of 0 to 3 conversion
            double grade = data[x, y, z, k];
            float discountRate = financial[w, noOfDestinations * 2 + 1];

            double mCost = miningCost + (double)(z - 1) * mcaf;


            if (grade == -100)
            {//air block
                npv = 0;
            }
            else if (dest == -1)
            {  //not extracted
                npv = 0;
            }
            else if (destination == 1)
            {   //waste
                npv = -(float)mCost * tonnage[0];   //mining cost * tonnage
                Math.Pow((1.0 + discountRate / 100.0), period); //discount rate
            }
            else
            {
                npv = (float)((float)((price * recovery * tonnage[1] * grade * conversionFactorPrice * conversionFactorGrade / 100.0) - (processingCost + mCost) * tonnage[0]) / Math.Pow((1.0 + discountRate / 100.0), period));//Di�er Maliyetlerde eklenmeli.
            }
            
           // if (data[z][x][y][1]==2) {
               // npv= (financial[w][0]*financial[w][2]*tonnage*data[z][x][y][k]*conversionFactorPrice*conversionFactorGrade/100-(financial[w][1]+financial[w][6])*tonnage)/powf((1+financial[w][5]/100),data[z][x][y][0]);
            }
           // if (data[z][x][y][1]==3) {
               // npv= (financial[w][0]*financial[w][4]*tonnage*data[z][x][y][k]*conversionFactorPrice*conversionFactorGrade/100-(financial[w][3]+financial[w][6])*tonnage)/powf((1+financial[w][5]/100),data[z][x][y][0]);
           // }
           // return npv;
        }
        public static void calculateRisk(int levels, int row, int column, double[,,,] data, float[,] financial, float[] tonnage, float conversionFactorPrice, float conversionFactorGrade, int financialSims, int numberOfSims, int noOfDestinations)
        {
            Console.WriteLine("======! Started Risk Calculation !======");
            string fileName = $"{asmPath}\\texts\\output.txt";

            int x, y, z, k;
            double npv = 0;

            int w;

            using (FileStream fileStream = File.Create(fileName))
            {
                for (w = 0; w < financialSims; w++)
                {
                    for (k = 2; k < (numberOfSims - 3); k++)
                    {
                        npv = 0f;
                        for (z = 1; z <= levels; z++)
                        {
                            for (y = 1; y <= column; y++)
                            {
                                for (x = 1; x <= row; x++)
                                {
                                    npv += calculateNpv(data, financial, z, x, y, k, w, ref tonnage, conversionFactorPrice, conversionFactorGrade, noOfDestinations);
                                }
                            }
                        }
                        Console.WriteLine(npv);
                    }
                }
            }
        }
        */
        }
    }
}