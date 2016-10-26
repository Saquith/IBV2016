using System;
using System.Linq;
using System.Drawing;

namespace Plexi
{
    public static class Program
    {
        //Available Processor instances.
        private static Processor[] processors = new Processor[]
        {
            new Identity(),
            new Negative(),
            new Greenblind(),
            new Shift(),
            new Rotate(),
            new RotateRight(), 
            new Grayscale(), 
            new Threshold(), 
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