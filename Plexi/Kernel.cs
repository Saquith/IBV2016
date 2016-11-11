using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Plexi
{
    public class Kernel
    {
        /* the matrix holds the structuring element, 
           the divisor is the sum of all the values 
           in the matrix and the centerpoint is used 
           to calculate the offset.*/

        public double[,] Matrix;

        public Tuple<int, int> Center()
        {
            var x = (int)((double)Matrix.GetLength(0) - 1) / 2;
            var y = (int)((double)Matrix.GetLength(1) - 1) / 2;
            return new Tuple<int, int>(x, y);
        }

        public virtual double Divisor()
        {
            return Math.Max(Matrix.Cast<double>().Sum(), 1); // the total sum of the matrix is used to preserve luminance
        }
    }

    public class Average3X3 : Kernel
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
        }
    }

	public class DistanceTransformTop : Kernel {
		public DistanceTransformTop() {
			Matrix = new double[,]
			{
				{ 2, 1, 2 },
				{ 1, 0, -1 },
				{ -1, -1, -1 },
			};
		}
	}

	public class DistanceTransformBottom : Kernel {
		public DistanceTransformBottom() {
			Matrix = new double[,]
			{
				{ -1, -1, -1 },
				{ -1, 0, 1 },
				{ 2, 1, 2 },
			};
		}
	}

	public class Average5X5 : Kernel
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
        }
	}

	public class Average7X7 : Kernel {
		public Average7X7() {
			Matrix = new double[,]
			{
				{ 1, 1, 1, 1, 1, 1, 1 },
				{ 1, 1, 1, 1, 1, 1, 1 },
				{ 1, 1, 1, 1, 1, 1, 1 },
				{ 1, 1, 1, 1, 1, 1, 1 },
				{ 1, 1, 1, 1, 1, 1, 1 },
				{ 1, 1, 1, 1, 1, 1, 1 },
				{ 1, 1, 1, 1, 1, 1, 1 },
			};
		}
	}

	public class Average9X9 : Kernel {
		public Average9X9() {
			Matrix = new double[,]
			{
				{ 1, 1, 1, 1, 1, 1, 1, 1, 1 },
				{ 1, 1, 1, 1, 1, 1, 1, 1, 1 },
				{ 1, 1, 1, 1, 1, 1, 1, 1, 1 },
				{ 1, 1, 1, 1, 1, 1, 1, 1, 1 },
				{ 1, 1, 1, 1, 1, 1, 1, 1, 1 },
				{ 1, 1, 1, 1, 1, 1, 1, 1, 1 },
				{ 1, 1, 1, 1, 1, 1, 1, 1, 1 },
				{ 1, 1, 1, 1, 1, 1, 1, 1, 1 },
				{ 1, 1, 1, 1, 1, 1, 1, 1, 1 },
			};
		}
	}

	public class Gaussian3X3 : Kernel
    {
        public Gaussian3X3()
        {
            Matrix = new double[,]
            {
                { 1, 2, 1 },
                { 2, 4, 2 },
                { 1, 2, 1 },
            };
        }
    }

    public class Gaussian5X5 : Kernel
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
        }
    }

    public class GaussianAlt5X5 : Kernel
    {
        public GaussianAlt5X5()
        {
            Matrix = new double[,]
            {
                { 2,  4,  5,  4,  2 },
                { 4,  9, 12,  9,  4 },
                { 5, 12, 15, 12,  5 },
                { 4,  9, 12,  9,  4 },
                { 2,  4,  5,  4,  2 },
            };
        }
    }

    public class Smooth3X3 : Kernel
    {
        public Smooth3X3()
        {
            Matrix = new double[,]
            {
                { 1, 1, 1 },
                { 1, 0, 1 },
                { 1, 1, 1 },
            };
        }
    }

    public class LowSmooth3X3 : Kernel
    {
        public LowSmooth3X3()
        {
            Matrix = new double[,]
            {
                { 1, 0, 1 },
                { 0, 0, 0 },
                { 1, 0, 1 },
            };
        }
    }

    public class Sharp3X3 : Kernel
    {
        public Sharp3X3()
        {
            Matrix = new double[,]
            {
                { -1, -1, -1 },
                { -1,  9, -1 },
                { -1, -1, -1 },
            };
        }
    }

    public class HighPass3X3 : Kernel
    {
        public HighPass3X3()
        {
            Matrix = new double[,]
            {
                { -1, -1, -1 },
                { -1,  8, -1 },
                { -1, -1, -1 },
            };
        }

        public override double Divisor()
        {
            return 9;
        }
    }

    public class NoiseReduction3X3 : Kernel
    {
        public NoiseReduction3X3()
        {
            Matrix = new double[,]
            {
                { 1, 1, 1 },
                { 1, 0, 1 },
                { 1, 1, 1 },
            };
        }
    }
    public class SobelHorizontal : Kernel
    {
        public SobelHorizontal()
        {
            Matrix = new double[,]
            {
                { -1, 0, 1 },
                { -2, 0, 2 },
                { -1, 0, 1 },
            };
        }
    }

    public class SobelVertical : Kernel
    {
        public SobelVertical()
        {
            Matrix = new double[,]
            {
                { -1, -2, -1 },
                {  0,  0,  0 },
                {  1,  2,  1 },
            };
        }
    }
}