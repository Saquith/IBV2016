using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plexi
{
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
        public override Color[,] Process(Color[,] image)
        {
            int w = image.GetLength(0), h = image.GetLength(1);
            var newImage = new Color[h, w];
            for (var x = 0; x < w; x++)
                for (var y = 0; y < h; y++)
                    newImage[y, x] = image[x, h - y - 1];
            return newImage;
        }
    }

    public class RotateRight : Processor
    {
        public override Color[,] Process(Color[,] image)
        {
            int w = image.GetLength(0), h = image.GetLength(1);
            var newImage = new Color[h, w];
            for (var x = 0; x < w; x++)
                for (var y = 0; y < h; y++)
                    newImage[y, x] = image[x, h - y - 1];
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
        public override Color Transform(Color c)
        {
            int t = 128;    // Threshold value

            if (c.R > t) { return Color.White; }
            else return Color.Black;
        }
    }

    public class Average : Processor
    {
        public override Color[,] Process(Color[,] image)
        {
            Kernel kernel = new Average3X3();
            Color[,] newImage = new Color[image.GetLength(0),image.GetLength(1)];

            double gray = 0;

            for (int offsetY = kernel.Center.Item2; offsetY < image.GetLength(1); offsetY++)
            {
                for (int offsetX = kernel.Center.Item1; offsetX < image.GetLength(0); offsetX++)
                {

                    gray = 0;

                    for (int y = -kernel.Center.Item2; y <= kernel.Center.Item2; y++)
                    {
                        for (int x = -kernel.Center.Item1; x <= kernel.Center.Item1; x++)
                        {

                        }
                    }
                }
            }

            return newImage;
        }
    }
}
