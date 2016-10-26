using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Plexi
{
    class Kernel
    {
        public double[,] Matrix;
        public int Divisor;
        public Tuple<int,int> Center;

        protected Tuple<int, int> CalcCenter(double[,] matrix)
        {
            var x = (int)((double)matrix.GetLength(0) - 1) / 2;
            var y = (int)((double)matrix.GetLength(1) - 1) / 2;
            return new Tuple<int, int>(x, y);
        }
    }

    class Average3X3 : Kernel
    {
        public Average3X3()
        {
            Matrix = new double[,]
            {
                { 1, 1, 1 },
                { 1, 1, 1 },
                { 1, 1, 1 }
            };

            Divisor = 9;
            Center = CalcCenter(Matrix);
        }
    }
}