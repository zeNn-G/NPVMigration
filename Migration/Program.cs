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
        public static double mcaf;
        public static float Gram = 1;
        public static float Kilogram = 1000;
        public static float MetricTon = 1000000;
        public static float LongTon = 1016000;

        public static float ShortTon = 907187;
        public static float Oz = 28.350f; //! /1000
        public static float TOz = 31.103f; //! /1000
        public static float Pound = 453.592f;//! /1000

        public static float PPB = 0.001f; //! gram/ton
        public static float PPM = 1; //! gram/ton
        public static float Percentage = 10000;
        public static int GradeUnit;

        public static CultureInfo ci = new CultureInfo("en-US");

        public static string asmPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        static void Main(string[] args)
        {
            Thread.CurrentThread.CurrentCulture = ci;
            // Thread.CurrentThread.CurrentCulture.NumberFormat = ci.NumberFormat;


            int blocks_z = 0, blocks_x = 0, blocks_y = 0, financial_simulations = 0, financial_parameters = 0, grade_simulations = 0, noOfDestinations = 0;
            float[] tonnage = new float[2];

            string unitPrice = String.Empty;
            string unitGrade = String.Empty;

            readParams(ref blocks_z, ref blocks_x, ref blocks_y, ref financial_parameters, ref financial_simulations, ref grade_simulations, ref tonnage, ref unitPrice, ref unitGrade, ref noOfDestinations);

            float[,] financial = ReadFinancial(financial_parameters, grade_simulations);


            int row = 0, column = 0, levels = 0, numberOfSimulations = 0;
            float conversionFactorPrice = 0.0f, conversionFactorGrade = 0.0f;

            double[,,,] data = readGradeSimulations(blocks_x, blocks_y, blocks_z, financial_parameters, financial_simulations, grade_simulations, unitPrice, unitGrade, ref row, ref column, ref levels, ref conversionFactorPrice, ref conversionFactorGrade, ref numberOfSimulations);

            calculateRisk(levels, row, column, data, financial, tonnage, conversionFactorPrice, conversionFactorGrade, financial_simulations, numberOfSimulations, noOfDestinations);

        }
        public static float TranslateUnit(float scale)
        {
            float GradeUnitScale;

            if (GradeUnit == 0)
                GradeUnitScale = Percentage * scale;

            else if (GradeUnit == 1)
                GradeUnitScale = Gram * scale;

            else if (GradeUnit == 2)
                GradeUnitScale = PPM * scale;

            else if (GradeUnit == 3)
                GradeUnitScale = PPB * scale;

            else if (GradeUnit == 4)
                GradeUnitScale = scale * MetricTon;

            else
                GradeUnitScale = 1000 * scale;

            return GradeUnitScale;
        }
        public static float SetMineParams(int saleUnit)
        {
            float UnitPriceCalc = 0;

            switch (saleUnit)
            {
                case 1:
                    UnitPriceCalc = TranslateUnit(Gram);
                    break;

                case 2:
                    UnitPriceCalc = TranslateUnit(1 / Kilogram);
                    break;

                case 3:
                    UnitPriceCalc = TranslateUnit(1 / MetricTon);
                    break;

                case 4:
                    UnitPriceCalc = TranslateUnit(1 / LongTon);
                    break;

                case 5:
                    UnitPriceCalc = TranslateUnit(1 / ShortTon);
                    break;

                case 6:
                    UnitPriceCalc = TranslateUnit(1 / Oz);
                    break;

                case 7:
                    UnitPriceCalc = TranslateUnit(1 / TOz);
                    break;

                case 8:
                    UnitPriceCalc = TranslateUnit(1 / Pound);
                    break;

                default:
                    UnitPriceCalc = TranslateUnit(1);
                    break;
            }

            return UnitPriceCalc;
        }
        public static float[,] ReadFinancial(int financialParams, int financial_sims)
        {

            string fileName = $"{asmPath}\\texts\\financial.txt";

            float[,] financial = new float[financial_sims, financialParams];

            int w = 0;

            using (StreamReader streamReader = new StreamReader(fileName))
            {
                while (streamReader.Peek() >= 0)
                {
                    string line = string.Empty;
                    string[] lineArr;

                    line = streamReader.ReadLine();
                    lineArr = line.Split(',');

                    for (int i = 0; i < lineArr.Length; i++)
                    {
                        financial[w, i] = float.Parse(lineArr[i]);
                        //Console.WriteLine(financial[w, i] = float.Parse(lineArr[i]));
                    }
                    w = w + 1;
                }
            }

            //string lines = string.Empty;
            //int w = 0;

            //while (!string.IsNullOrEmpty(lines = sr.ReadLine()))
            //{
            //    string[] rows = lines.Split(',');
            //    for (int i = 0; i < rows.Length; i++)
            //    {
            //        financial[w, i] = float.Parse(rows[i]);
            //        //Console.Write(financial[w, i] = int.Parse(rows[i]));
            //    }

            //    w = w + 1;
            //}

            return financial;
        }
        public static void readParams(ref int blocksZ, ref int blocksX, ref int blocksY, ref int financialParams, ref int financialSims, ref int gradeSims, ref float[] tonnage, ref string unitPrice, ref string unitGrade, ref int noOfDestinations)
        {
            string fileName = $"{asmPath}\\texts\\parameter.txt";


            Dictionary<string, string> paramValueDic = new Dictionary<string, string>(); //! Dictionary for Param - Value

            using (StreamReader streamReader = new StreamReader(fileName))
            {

                while (streamReader.Peek() >= 0) //! Reads the file line by line
                {
                    string line = string.Empty;
                    string[] lineArr;

                    line = streamReader.ReadLine();

                    lineArr = line.Split('/'); //! Splits the value Param / Value --- For Example: blocks_z / 18

                    paramValueDic.Add(lineArr[0], lineArr[1]);

                }
            }

            foreach (KeyValuePair<string, string> kvp in paramValueDic) //! Traverses the dictionary and compares the Key and asigns the value to proper Key
            {
                //! DEBUG
                //Console.WriteLine("Param is {0}, value is {1}", kvp.Key, kvp.Value);

                if (kvp.Key.Equals("blocks_x"))
                {
                    _ = int.TryParse(kvp.Value, out int valueInt);

                    blocksX = valueInt;
                }
                else if (kvp.Key.Equals("blocks_z"))
                {
                    _ = int.TryParse(kvp.Value, out int valueInt);

                    blocksZ = valueInt;
                }
                else if (kvp.Key.Equals("blocks_y"))
                {
                    _ = int.TryParse(kvp.Value, out int valueInt);

                    blocksY = valueInt;
                }
                else if (kvp.Key.Equals("tonnage"))
                {
                    _ = int.TryParse(kvp.Value, out int valueInt);

                    tonnage[0] = valueInt;
                    tonnage[1] = valueInt;
                }
                else if (kvp.Key.Equals("financial_simulations"))
                {
                    _ = int.TryParse(kvp.Value, out int valueInt);

                    financialSims = valueInt;
                }
                else if (kvp.Key.Equals("financial_parameters"))
                {
                    _ = int.TryParse(kvp.Value, out int valueInt);

                    financialParams = valueInt;
                }
                else if (kvp.Key.Equals("grade_simulations"))
                {
                    _ = int.TryParse(kvp.Value, out int valueInt);

                    gradeSims = valueInt;
                }
                else if (kvp.Key.Equals("unitPrice"))
                {
                    unitPrice = kvp.Value;
                }
                else if (kvp.Key.Equals("unitGrade"))
                {
                    unitGrade = kvp.Value;
                }
                else if (kvp.Key.Equals("destinations"))
                {
                    _ = int.TryParse(kvp.Value, out int valueInt);

                    noOfDestinations = valueInt;
                }
                else if (kvp.Key.Equals("mcaf"))
                {
                    _ = float.TryParse(kvp.Value, out float floatFl);

                    mcaf = floatFl;
                }
            }

            //! DEBUG
            Console.WriteLine("y:{0} x:{1} z:{2}\nFinancial_Sim:{3}\nFinancial_Params:{4}\nGrade_Sim:{5}\nTonnage:{6}\nUnitPrice:{7}\nUnitGrade:{8}\nNoOfDestinations:{9}", blocksY, blocksX, blocksZ, financialSims, financialParams, gradeSims, tonnage[0], unitPrice, unitGrade, noOfDestinations);
        }
        public static double[,,,] readGradeSimulations(int blocksX, int blocksY, int blocksZ, int financialParams, int financalSims, int gradeSims, string unitPrice, string unitGrade, ref int row, ref int column, ref int levels, ref float conversionFactorPrice, ref float conversionFactorGrade, ref int numberOfSimulations)
        {
            //string fileName = $"{asmPath}\\texts\\gradesimulation.txt";
            string fileName = @"C:\Users\trero\Desktop\Staj\C#\Migration\Migration\texts\gradesimulation.txt";


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

            int recoveryContrl = (destination - 2) * 2 + 2;
            float recovery;

            if (recoveryContrl < 0)
            {
                recovery = financial[w, 6];
            }
            else
            {
                recovery = financial[w, recoveryContrl];
            }
            int exp = ((destination - 2) * 2 + 1);

            float processingCost;

            if (exp < 0)
            {
                processingCost = 0;
            }
            else
            {
                processingCost = financial[w, exp];
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
            /*
            if (data[z][x][y][1]==2) {
                npv= (financial[w][0]*financial[w][2]*tonnage*data[z][x][y][k]*conversionFactorPrice*conversionFactorGrade/100-(financial[w][1]+financial[w][6])*tonnage)/powf((1+financial[w][5]/100),data[z][x][y][0]);
            }
            if (data[z][x][y][1]==3) {
                npv= (financial[w][0]*financial[w][4]*tonnage*data[z][x][y][k]*conversionFactorPrice*conversionFactorGrade/100-(financial[w][3]+financial[w][6])*tonnage)/powf((1+financial[w][5]/100),data[z][x][y][0]);
            }*/
            return npv;
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

    }
}