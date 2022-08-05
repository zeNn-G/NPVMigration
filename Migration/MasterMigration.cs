using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Migration
{
    internal class MasterMigration
    {
        //! PROPERTIES
        private MigrationParams mp;

        //! CONSTRUCTOR
        public MasterMigration(MigrationParams mp)
        {
            this.mp = mp;
        }

        //!======================= METHODS =====================================
        public void ReadParams()
        {


            //! Dictionary for Param - Value
            Dictionary<string, string> paramValueDic = new Dictionary<string, string>();

            using (StreamReader streamReader = new StreamReader(mp.parameterLocation))
            {
                //! Reads the file line by line
                while (streamReader.Peek() >= 0)
                {
                    string line = string.Empty;
                    string[] lineArr;

                    line = streamReader.ReadLine();

                    //! Splits the value Param / Value --- For Example: blocks_z / 18
                    lineArr = line.Split('/');

                    paramValueDic.Add(lineArr[0], lineArr[1]);

                }
            }
            //! Traverses the dictionary and compares the Key and asigns the value to proper Key
            foreach (KeyValuePair<string, string> kvp in paramValueDic)
            {
                //! DEBUG
                //Console.WriteLine("Param is {0}, value is {1}", kvp.Key, kvp.Value);

                if (kvp.Key.Equals("blocks_x"))
                {
                    _ = int.TryParse(kvp.Value, out int valueInt);

                    mp.blocks_x = valueInt;
                }
                else if (kvp.Key.Equals("blocks_z"))
                {
                    _ = int.TryParse(kvp.Value, out int valueInt);

                    mp.blocks_z = valueInt;
                }
                else if (kvp.Key.Equals("blocks_y"))
                {
                    _ = int.TryParse(kvp.Value, out int valueInt);

                    mp.blocks_y = valueInt;
                }
                else if (kvp.Key.Equals("tonnage"))
                {
                    _ = int.TryParse(kvp.Value, out int valueInt);

                    mp.tonnage[0] = valueInt;
                    mp.tonnage[1] = valueInt;
                }
                else if (kvp.Key.Equals("financial_simulations"))
                {
                    _ = int.TryParse(kvp.Value, out int valueInt);

                    mp.financialSims = valueInt;
                }
                else if (kvp.Key.Equals("financial_parameters"))
                {
                    _ = int.TryParse(kvp.Value, out int valueInt);

                    mp.financialParams = valueInt;
                }
                else if (kvp.Key.Equals("grade_simulations"))
                {
                    _ = int.TryParse(kvp.Value, out int valueInt);

                    mp.gradeSims = valueInt;
                }
                else if (kvp.Key.Equals("unitPrice"))
                {
                    mp.unitPrice = kvp.Value;
                }
                else if (kvp.Key.Equals("unitGrade"))
                {
                    mp.unitGrade = kvp.Value;
                }
                else if (kvp.Key.Equals("destinations"))
                {
                    _ = int.TryParse(kvp.Value, out int valueInt);

                    mp.noOfDestinations = valueInt;
                }
                else if (kvp.Key.Equals("mcaf"))
                {
                    _ = float.TryParse(kvp.Value, out float valueFloat);

                    mp.mcaf = valueFloat;
                }
            }
            //! DEBUG
            //Console.WriteLine("y:{0} x:{1} z:{2}\nFinancial_Sim:{3}\nFinancial_Params:{4}\nGrade_Sim:{5}\nTonnage:{6}\nUnitPrice:{7}\nUnitGrade:{8}\nNoOfDestinations:{9}", mp.blocks_y, mp.blocks_x, mp.blocks_z, mp.financialSims, mp.financialParams, mp.gradeSims, mp.tonnage[0], mp.unitPrice, mp.unitGrade, mp.noOfDestinations);
        }
        public void ReadFinancial()
        {
            //! Reads the financial.txt and splits data into pieces and relates them with the financial_parameters and financial_simulations variables and returns the related 2dArray

            float[,] financial = new float[mp.financialSims, mp.financialParams];

            int w = 0;

            //! Reads the file line by line
            using (StreamReader streamReader = new StreamReader(mp.financialLocation))
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

                        //! DEBUG
                        //Console.WriteLine(financial[w, i] = float.Parse(lineArr[i]));
                    }
                    w = w + 1;
                }
            }
            mp.financial = financial;
        }
        public float TranslateUnit(float scale)
        {
            float GradeUnitScale;

            if (mp.GradeUnit == 0)
                GradeUnitScale = mp.Percentage * scale;

            else if (mp.GradeUnit == 1)
                GradeUnitScale = mp.Gram * scale;

            else if (mp.GradeUnit == 2)
                GradeUnitScale = mp.PPM * scale;

            else if (mp.GradeUnit == 3)
                GradeUnitScale = mp.PPB * scale;

            else if (mp.GradeUnit == 4)
                GradeUnitScale = mp.MetricTon * scale;

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
                    UnitPriceCalc = TranslateUnit(mp.Gram);
                    break;

                case 2:
                    UnitPriceCalc = TranslateUnit(1 / mp.Kilogram);
                    break;

                case 3:
                    UnitPriceCalc = TranslateUnit(1 / mp.MetricTon);
                    break;

                case 4:
                    UnitPriceCalc = TranslateUnit(1 / mp.LongTon);
                    break;

                case 5:
                    UnitPriceCalc = TranslateUnit(1 / mp.ShortTon);
                    break;

                case 6:
                    UnitPriceCalc = TranslateUnit(1 / mp.Oz);
                    break;

                case 7:
                    UnitPriceCalc = TranslateUnit(1 / mp.TOz);
                    break;

                case 8:
                    UnitPriceCalc = TranslateUnit(1 / mp.Pound);
                    break;

                default:
                    UnitPriceCalc = TranslateUnit(1);
                    break;
            }

            return UnitPriceCalc;
        }
        public void ReadGradeSimulations()
        {
            int levels_L = 0, row_L = 0, column_L = 0;

            double[,,,] arr4D = new double[mp.blocks_x + 1, mp.blocks_y + 1, mp.blocks_z + 1, mp.financialParams + 1];

            int firstPos = 0, secondPos = 0, thirdPos = 0;

            using (StreamReader streamReader = new StreamReader(mp.gradesimulationLocation))
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
                            Console.Write(arr4D[firstPos, secondPos, thirdPos, i - 3] + " ");
                        }

                        i = i + 1;
                    }
                    mp.numberOfSimulations = i;
                }
            }

            float conversionFactorPrice_L = 1f;
            float conversionFactorGrade_L = 1f;

            #region unitGradeEq
            if (mp.unitGrade.Equals("%"))
            {
                mp.GradeUnit = 0;
            }
            else if (mp.unitGrade.Equals("ppm"))
            {
                mp.GradeUnit = 1;
            }
            else if (mp.unitGrade.Equals("gTon"))
            {
                mp.GradeUnit = 2;
            }
            else if (mp.unitGrade.Equals("kCal/kg"))
            {
                mp.GradeUnit = 4;
            }
            #endregion

            #region conversionFactorPriceEq
            if (mp.unitPrice.Equals("$pg"))
            {
                conversionFactorPrice_L = SetMineParams(1);
            }
            else if (mp.unitPrice.Equals("$pkg"))
            {
                conversionFactorPrice_L = SetMineParams(2);
            }
            else if (mp.unitPrice.Equals("$pTonnage"))
            {
                conversionFactorPrice_L = SetMineParams(3);
            }
            else if (mp.unitPrice.Equals("$pLongTon"))
            {
                conversionFactorPrice_L = SetMineParams(4);
            }
            else if (mp.unitPrice.Equals("$pShortTon"))
            {
                conversionFactorPrice_L = SetMineParams(5);
            }
            else if (mp.unitPrice.Equals("$poz"))
            {
                conversionFactorPrice_L = SetMineParams(6);
            }
            else if (mp.unitPrice.Equals("$ptroyoz"))
            {
                conversionFactorPrice_L = SetMineParams(7);
            }
            else if (mp.unitPrice.Equals("$plb"))
            {
                conversionFactorPrice_L = SetMineParams(8);
            }

            mp.row = row_L;
            mp.levels = levels_L;
            mp.column = column_L;
            mp.conversionFactorGrade = conversionFactorGrade_L;
            mp.conversionFactorPrice = conversionFactorPrice_L;
            #endregion

            mp.data = arr4D;
        }

    }

}
