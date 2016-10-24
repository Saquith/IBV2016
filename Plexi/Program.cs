using System;
using System.Linq;
using System.Drawing;

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

    public static class Program
    {
        //Available Processor instances.
        private static Processor[] processors = new Processor[]
        {
            new Identity(),
            new Negative(),
            new Greenblind(),
            new Shift(),
            new Rotate()
        };

        private static Processor ProcessorFromName(string name)
        {
            try
            {
                return processors.First(processor => string.Compare(name, processor.ToString(), true) == 0);
            }
            catch
            {
                Console.Error.WriteLine("WARNING: Filter {0} not found!", name);
                return null;
            }
        }

        public static void Main(string[] args)
        {
            //Get the Processor to use from the command line arguments or take the last one.
            Processor processor = null;
            if (args.Length >= 1)
            {
                var ps = args[0].Split(new char[] { '+', ',' }, System.StringSplitOptions.RemoveEmptyEntries).Select(arg => ProcessorFromName(arg)).Where(o => o != null).ToArray();
                if (ps.Length > 0)
                    processor = new MultiProcessor(ps);
            }

            if (processor == null)
                processor = processors.Last();

            //Read, process and write the image.
            var input = Plexi.ReadBitmapFromConsole();
            var output = processor.Process(input);
            Plexi.WriteBitmapToConsole(output);
        }
    }
}