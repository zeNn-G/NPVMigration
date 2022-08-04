using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Migration
{
    internal class MigrationParams
    {
        //! CONST PROPERTIES
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

        //! PROPERTIES
        public int blocks_z;
        public int blocks_x;
        public int blocks_y;
        public int financialSims;
        public int financialParams;
        public int gradeSims;
        public int noOfDestinations;

        public float[] tonnage;

        public string unitPrice;
        public string unitGrade;

        public int row;
        public int column;
        public int levels;
        public int numberOfSimulations;

        public float conversionFactorPrice;
        public float conversionFactorGrade;

        public float[,] financial;
        public double[,,,] data;

        //! LOCATION
        public string financialLocation;
        public string parameterLocation;
        public string gradesimulationLocation;
        public string asmPath;

        public double mcaf;
        public int GradeUnit;


        //!Constructor
        public MigrationParams(int blocks_z, int blocks_x, int blocks_y, int financialSims, int financialParams, int gradeSims, int noOfDestinations, float[] tonnage, string unitPrice, string unitGrade, int row, int column, int levels, int numberOfSimulations, float conversionFactorPrice, float conversionFactorGrade)
        {
            this.blocks_z = blocks_z;
            this.blocks_x = blocks_x;
            this.blocks_y = blocks_y;
            this.financialSims = financialSims;
            this.financialParams = financialParams;
            this.gradeSims = gradeSims;
            this.noOfDestinations = noOfDestinations;
            this.tonnage = tonnage;
            this.unitPrice = unitPrice;
            this.unitGrade = unitGrade;
            this.row = row;
            this.column = column;
            this.levels = levels;
            this.numberOfSimulations = numberOfSimulations;
            this.conversionFactorPrice = conversionFactorPrice;
            this.conversionFactorGrade = conversionFactorGrade;
        }
    }
}
