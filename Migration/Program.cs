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
        public static string asmPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        static void Main(string[] args)
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");

            MigrationParams mp = new MigrationParams();
            SetParams(mp);

            MasterMigration master = new MasterMigration(mp);
            Methods(master);

            //! DEBUG
            DebugOutputConsole(mp);

        }
        public static void SetParams(MigrationParams mp)
        {
            mp.blocks_z = 0;
            mp.blocks_x = 0;
            mp.blocks_y = 0;
            mp.financialSims = 0;
            mp.financialParams = 0;
            mp.gradeSims = 0;
            mp.noOfDestinations = 0;
            mp.tonnage = new float[2];
            mp.unitPrice = String.Empty;
            mp.unitGrade = String.Empty;
            mp.row = 0;
            mp.column = 0;
            mp.levels = 0;
            mp.numberOfSimulations = 0;
            mp.conversionFactorPrice = 0f;
            mp.conversionFactorGrade = 0f;
            mp.asmPath = asmPath;
            mp.financialLocation = $"{mp.asmPath}\\texts\\financial.txt";
            mp.parameterLocation = $"{mp.asmPath}\\texts\\parameter.txt";
            mp.gradesimulationLocation = $"{mp.asmPath}\\texts\\gradesimulation.txt";
        }
        public static void Methods(MasterMigration master)
        {
            master.ReadParams();
            master.ReadFinancial();
            master.ReadGradeSimulations();
            master.CalculateRisk();
        }
        public static void DebugOutputConsole(MigrationParams mp)
        {
            foreach (double item in mp.results)
            {
                Console.WriteLine(item);
            }
        }
    }
}