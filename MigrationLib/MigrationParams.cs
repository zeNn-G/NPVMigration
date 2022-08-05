﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Migration
{
    public class MigrationParams
    {
        //! CONST PROPERTIES
        public float Gram = 1;
        public float Kilogram = 1000;
        public float MetricTon = 1000000;
        public float LongTon = 1016000;

        public float ShortTon = 907187;
        public float Oz = 28.350f; //! /1000
        public float TOz = 31.103f; //! /1000
        public float Pound = 453.592f;//! /1000

        public float PPB = 0.001f; //! gram/ton
        public float PPM = 1; //! gram/ton
        public float Percentage = 10000;

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

        //! Results
        public List<double> results = new List<double>();

        //!Constructor
        public MigrationParams()
        {

        }
    }
}
