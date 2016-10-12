using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace Plexi
{
    public static class Plexi
    {
        public static Bitmap ReadBitmapFromConsole()
        {
            return new Bitmap(Console.OpenStandardInput());
        }

        public static Bitmap ReadBitmapFromFile(string filename)
        {
            return new Bitmap(filename);
        }

        public static void WriteBitmapToConsole(Bitmap bitmap)
        {
            bitmap.Save(Console.OpenStandardOutput(), ImageFormat.Png);
        }

        public static void WriteBitmapToFile(Bitmap bitmap, string filename)
        {
            bitmap.Save(filename);
        }
    }

    public abstract class Processor
    {
        public Bitmap Process(Bitmap bitmap)
        {
            //Construct a color grid from the bitmap.
            var image = new Color[bitmap.Width, bitmap.Height];
            for (var x = 0; x < bitmap.Width; x++)
                for (var y = 0; y < bitmap.Height; y++)
                    image[x, y] = bitmap.GetPixel(x, y);

            //Perform processing on the color grid.
            var newImage = Process(image);

            //Construct a new Bitmap from the color grid.
            var newBitmap = new Bitmap(newImage.GetLength(0), newImage.GetLength(1));
            for (var x = 0; x < newBitmap.Width; x++)
                for (var y = 0; y < newBitmap.Height; y++)
                    newBitmap.SetPixel(x, y, newImage[x, y]);

            //Return the new Bitmap.
            return newBitmap;
        }

        public virtual Color[,] Process(Color[,] image)
        {
            //Construct a new color grid and transform each color individually.
            var newImage = new Color[image.GetLength(0), image.GetLength(1)];
            for (var x = 0; x < image.GetLength(0); x++)
                for (var y = 0; y < image.GetLength(1); y++)
                    newImage[x, y] = Transform(image[x, y]);

            return newImage;
        }

        public virtual Color Transform(Color c)
        {
            return c;
        }

        public override string ToString()
        {
            return this.GetType().Name;
        }
    }

    public class MultiProcessor : Processor
    {
        private Processor[] _processors;

        public MultiProcessor(Processor[] processors)
        {
            _processors = processors;
        }

        public override Color[,] Process(Color[,] image)
        {
            foreach (var processor in _processors)
                image = processor.Process(image);
            return image;
        }
    }
}
