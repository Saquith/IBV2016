using System;
using System.Drawing;

namespace Plexi
{
	public class ArithmeticProcessor : Processor {
		private Function _function;
		private Matrix _target;

		public ArithmeticProcessor(Function function, Matrix target)
		{
			_function = function;
			_target = target;
		}

		public ArithmeticProcessor(Function function, Bitmap target)
		{
			_function = function;
			_target = GetMatrix(target);
		}

		public override Matrix Process(Matrix source)
		{
			return source.ApplyFunction(_function, _target);
		}
	}

	public class Negative : Processor
    {
        public override Color Transform(Color c)
        {
            return Color.FromArgb(255 - c.R, 255 - c.G, 255 - c.B);
        }
    }

    public class Identity : Processor
    {
        public override Color Transform(Color c)
        {
            return c;
        }
    }

    public class Greenblind : Processor
    {
        public override Color Transform(Color c)
        {
            return Color.FromArgb(c.R, 0, c.B);
        }
    }

    public class Shift : Processor
    {
        public override Color Transform(Color c)
        {
            return Color.FromArgb(c.B, c.R, c.G);
        }
    }

    public class Rotate : Processor
    {
        public override Matrix Process(Matrix source)
        {
            int w = source.X, h = source.Y;
            var newImage = new Matrix(h, w);
            for (var x = 0; x < w; x++)
                for (var y = 0; y < h; y++)
                    newImage[y, x] = source[x, h - y - 1];
            return newImage;
        }
    }

    public class RotateRight : Processor
    {
        public override Matrix Process(Matrix source)
        {
            int w = source.X, h = source.Y;
			var newImage = new Matrix(h, w);
			for (var x = 0; x < w; x++)
                for (var y = 0; y < h; y++)
                    newImage[y, x] = source[x, h - y - 1];
            return newImage;
        }
    }

    public class Grayscale : Processor
    {
        public override Color Transform(Color c)
        {
            int grayVal = (int)((c.R * 0.21) + (c.G * 0.72) + (c.B * 0.07)); // The numbers represent a weight for better grayscale values (HDTV Luma coding)
            return Color.FromArgb(grayVal, grayVal, grayVal);
        }
    }

    public class Threshold : Processor
    {
        int t;  // Threshold value
        public Threshold(int tVal)
        {
            t = tVal;
        }
        public override Color Transform(Color c)
        {
            

            if (c.R > t) { return Color.White; }
            else return Color.Black;
        }
    }

    public class Filter : Processor
    {
        private Kernel kernel;

        public Filter(Kernel k)
        {
            kernel = k;
        }
        public override Matrix Process(Matrix source)
        {
            
            Matrix newImage = new Matrix(source.X,source.Y);

            int grayValue = 0;
            double pixelSum = 0;
            int offsetX = kernel.Center().Item1;
            int offsetY = kernel.Center().Item2;

            for (int imageY = offsetY; imageY < (source.Y - offsetY); imageY++)
            {
                for (int imageX = offsetX; imageX < (source.X - offsetX); imageX++)
                {
                    // reset values after each iteration
                    grayValue = 0;
                    pixelSum = 0;


                    // for loop goes through the kernel
                    for (int y = -offsetY; y <= offsetY; y++)
                    {
                        for (int x = -offsetY; x <= offsetX; x++)
                        {
                            // The sum of the pixelvalues multiplied by the kernel values
                            pixelSum += (source[(imageX + x), (imageY + y)].R) * (kernel.Matrix[x + offsetX, y + offsetY]);
                        }
                    }

                    grayValue = Math.Min(Math.Max((int)pixelSum/(int)kernel.Divisor(),0),255); // min and max makes sure the value stays within the 0-255 range
                    newImage[imageX, imageY] = Color.FromArgb(grayValue, grayValue, grayValue);
                }
            }

            return newImage;
        }
    }
}
