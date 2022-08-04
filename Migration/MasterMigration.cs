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
            string fileName = $"{mp.asmPath}\\texts\\parameter.txt";

            //! Dictionary for Param - Value
            Dictionary<string, string> paramValueDic = new Dictionary<string, string>();

            using (StreamReader streamReader = new StreamReader(fileName))
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

                    mp.blocks_x = valueInt;
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
            Console.WriteLine("y:{0} x:{1} z:{2}\nFinancial_Sim:{3}\nFinancial_Params:{4}\nGrade_Sim:{5}\nTonnage:{6}\nUnitPrice:{7}\nUnitGrade:{8}\nNoOfDestinations:{9}", mp.blocks_y, mp.blocks_x, mp.blocks_z, mp.financialSims, mp.financialParams, mp.gradeSims, mp.tonnage[0], mp.unitPrice, mp.unitGrade, mp.noOfDestinations);
        }

    }
}
