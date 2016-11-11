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
	        var image = bitmap.GetMatrix();

            //Perform processing on the color grid.
            var newImage = Process(image);

            //Construct a new Bitmap from the color grid.
            var newBitmap = new Bitmap(newImage.X, newImage.Y);
            for (var x = 0; x < newBitmap.Width; x++)
                for (var y = 0; y < newBitmap.Height; y++)
                    newBitmap.SetPixel(x, y, newImage[x, y]);

            //Return the new Bitmap.
            return newBitmap;
        }

	    public virtual Matrix Process(Matrix source)
        {
            //Construct a new color grid and transform each color individually.
            var newImage = new Matrix(source.X, source.Y);
            for (var x = 0; x < source.X; x++)
                for (var y = 0; y < source.Y; y++)
                    newImage[x, y] = Transform(source[x, y]);

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

        public override Matrix Process(Matrix image)
        {
            foreach (var processor in _processors)
                image = processor.Process(image);
            return image;
        }
    }
}
