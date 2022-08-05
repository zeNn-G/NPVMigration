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
            master.CalculateRisk();

            //! DEBUG
            foreach (double item in mp.results)
            {
                Console.WriteLine(item);
            }
        }
    }
}