using System;
using System.Collections.Generic;
using Microsoft.ML.Runtime.Api;
using System.Text;

namespace Built.ML.Demo1
{
    public class HistoryData
    {
        [Column("0")]
        public int Num1;

        [Column("1")]
        public int Num2;

        [Column("2")]
        public int Num3;

        [Column("3")]
        public int Num4;

        [Column("4")]
        public int Num5;

        [Column("5")]
        public int Num6;

        [Column("6")]
        public int Red1;
    }

    public class HistoryDataPrediction
    {
        [Column("0")]
        public int Num1;

        [Column("1")]
        public int Num2;

        [Column("2")]
        public int Num3;

        [Column("3")]
        public int Num4;

        [Column("4")]
        public int Num5;

        [Column("5")]
        public int Num6;

        [Column("6")]
        public int Red1;
    }
}