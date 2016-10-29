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
        /* the matrix holds the structuring element, 
           the divisor is the sum of all the values 
           in the matrix and the centerpoint is used 
           to calculate the offset.*/

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
            // Basic 3 by 3 averaging kernel

            Matrix = new double[,]
            {
                { 1, 1, 1 },
                { 1, 1, 1 },
                { 1, 1, 1 },
            };

            Divisor = 9; // sum of all values in the matrix
            Center = CalcCenter(Matrix);
        }
    }

    class Average5X5 : Kernel
    {
        public Average5X5()
        {
            Matrix = new double[,]
            {
                { 1, 1, 1, 1, 1 },
                { 1, 1, 1, 1, 1 },
                { 1, 1, 1, 1, 1 },
                { 1, 1, 1, 1, 1 },
                { 1, 1, 1, 1, 1 },
            };

            Divisor = 25;
            Center = CalcCenter(Matrix);
        }
    }

    class Gaussian3X3 : Kernel
    {
        public Gaussian3X3()
        {
            Matrix = new double[,]
            {
                { 1, 2, 1 },
                { 2, 4, 2 },
                { 1, 2, 1 },
            };

            Divisor = 16;
            Center = CalcCenter(Matrix);
        }
    }

    class Gaussian5X5 : Kernel
    {
        public Gaussian5X5()
        {
            Matrix = new double[,]
            {
                {  2,  7,  12,  7,  2 },
                {  7, 31,  52, 31,  7 },
                { 12, 52, 127, 52, 12 },
                {  7, 31,  52, 31,  7 },
                {  2,  7,  12,  7,  2 },
            };

            Divisor = 571;
            Center = CalcCenter(Matrix);
        }
    }
}