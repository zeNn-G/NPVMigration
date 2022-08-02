using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

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


        static void Main(string[] args)
        {
            int blocks_z = 0, blocks_x = 0, blocks_y = 0, financial_simulations = 0, financial_parameters = 0, grade_simulations = 0, noOfDestinations = 0;
            float tonnage = 0f;

            string unitPrice = String.Empty;
            string unitGrade = String.Empty;

            readParams(ref blocks_z, ref blocks_x, ref blocks_y, ref financial_parameters, ref financial_simulations, ref grade_simulations, ref tonnage, ref unitPrice, ref unitGrade, ref noOfDestinations);

            int row = 0, column = 0, levels = 0, numberOfSimulations = 0;
            float conversionFactorPrice = 0.0f, conversionFactorGrade = 0.0f;

            readGradeSimulations(blocks_x, blocks_y, blocks_z, financial_parameters, financial_simulations, grade_simulations, unitPrice, unitGrade, ref row, ref column, ref levels, ref conversionFactorPrice, ref conversionFactorGrade, ref numberOfSimulations);
        }
        public float TranslateUnit(float scale)
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

        public float SetMineParams(int saleUnit)
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

        public void ReadFinancial(int financialParams, int financial_sims)
        {

        }

        public static void readParams(ref int blocksZ, ref int blocksX, ref int blocksY, ref int financialParams, ref int financialSims, ref int gradeSims, ref float tonnage, ref string unitPrice, ref string unitGrade, ref int noOfDestinations)
        {
            string fileName = @"C:\Users\trero\Desktop\Staj\C#\Migration\Migration\texts\parameter.txt";

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

                    tonnage = valueInt;
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
                else if (kvp.Key.Equals("oredestinations"))
                {
                    _ = int.TryParse(kvp.Value, out int valueInt);

                    noOfDestinations = valueInt;
                }
                else if (kvp.Key.Equals("mcaf"))
                {
                    _ = int.TryParse(kvp.Value, out int valueInt);

                    mcaf = valueInt;
                }
            }

            //! DEBUG
            //Console.WriteLine("y:{0} x:{1} z:{2}\nFinancial_Sim:{3}\nFinancial_Params:{4}\nGrade_Sim:{5}\nTonnage:{6}\nUnitPrice:{7}\nUnitGrade:{8}\nNoOfDestinations:{9}", blocksY, blocksX, blocksZ, financialSims, financialParams, gradeSims, tonnage, unitPrice, unitGrade, noOfDestinations);
        }

        public double calculateNpv()
        {
            float npv = 0f;

            return npv;
        }

        public static void readGradeSimulations(int blocksX, int blocksY, int blocksZ, int financialParams, int financalSims, int gradeSims, string unitPrice, string unitGrade, ref int row, ref int column, ref int levels, ref float conversionFactorPrice, ref float conversionFactorGrade, ref int numberOfSimulations)
        {
            string fileName = @"C:\Users\trero\Desktop\Staj\C#\Migration\Migration\texts\gradesimulation.txt";

            int levels_L = 0, row_L = 0, column_L = 0;
            int i = 0;

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


                    //! DEBUG
                    //Console.WriteLine(lineArr[0] + " " + lineArr[1] + " " + lineArr[2] + " " + lineArr[3] + " " + lineArr[4] + " " + lineArr[5]);
                    while (i < 6)
                    {
                        if (i == 0)
                        {
                            firstPos = int.Parse(lineArr[i]);
                            if (firstPos > levels_L)
                            {
                                row = firstPos;
                            }
                        }
                        else if (i == 1)
                        {
                            secondPos = int.Parse(lineArr[i]);
                            if (secondPos > row_L)
                            {
                                column = secondPos;
                            }
                        }
                        else if (i == 2)
                        {
                            thirdPos = int.Parse(lineArr[i]);
                            if (thirdPos > column_L)
                            {
                                levels_L = thirdPos;
                            }
                        }
                        else if (i > 2)
                        {
                            if (lineArr[4].Equals('-'))
                            {
                                arr4D[firstPos, secondPos, thirdPos, i - 3] = -1;
                            }
                            else
                            {
                                arr4D[firstPos, secondPos, thirdPos, i - 3] = double.Parse(lineArr[5]);
                            }

                            Console.Write(arr4D[firstPos, secondPos, thirdPos, i - 3] + " ");
                        }

                        i = i + 1;
                    }
                }
            }
            numberOfSimulations = i;

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
                conversionFactorGrade_L = SetMineParams(1);
            }
            else if (unitPrice.Equals("$pkg"))
            {
                conversionFactorGrade_L = SetMineParams(2);
            }
            else if (unitPrice.Equals("$pTonnage"))
            {
                conversionFactorGrade_L = SetMineParams(3);
            }
            else if (unitPrice.Equals("$pLongTon"))
            {
                conversionFactorGrade_L = SetMineParams(4);
            }
            else if (unitPrice.Equals("$pShortTon"))
            {
                conversionFactorGrade_L = SetMineParams(5);
            }
            else if (unitPrice.Equals("$poz"))
            {
                conversionFactorGrade_L = SetMineParams(6);
            }
            else if (unitPrice.Equals("$ptroyoz"))
            {
                conversionFactorGrade_L = SetMineParams(7);
            }
            else if (unitPrice.Equals("$plb"))
            {
                conversionFactorGrade_L = SetMineParams(8);
            }

        }
    }
}
}
