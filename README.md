# Plexi

Simple image processing framework. Tested on Windows (Visual Studio) and OSX (Xamarin Studio).

- Plexi: Console application with pipes.
- INFOIBX: GUI application.

## Documentation

The framework ([Plexi.cs](https://github.com/mrexodia/Plexi/blob/master/Plexi/Plexi.cs)) allows you to easily define image [processors](https://github.com/mrexodia/Plexi/blob/master/Plexi/Plexi.cs#L30) that can be applied to a [System.Drawing.Bitmap](https://msdn.microsoft.com/en-us/library/system.drawing.bitmap(v=vs.110).aspx) class. There are currently two methods in the `Processor` class that you can override: `Transform` and `Process`.

### Transform

The `Transform` method will be called for every color in your input image, the `Negative` processor will return the negative for every color in your image and after running this processor you will have the negative image. This is useful for simple processors that don't require contextual information to function.

```c#
public class Negative : Processor
{
    public override Color Transform(Color c)
    {
        return Color.FromArgb(255 - c.R, 255 - c.G, 255 - c.B);
    }
}
```

### Process

The `Process` method will be called with the image as a two-dimensional array (width * height) of colors. This processor will rotate the input image 90 degrees to the right. You have to return a new `Color[,]` object (to support resizing operations).

```c#
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
```

### MultiProcessor

To combine multiple `Processor` instances there is a class called [MultiProcessor](https://github.com/mrexodia/Plexi/blob/master/Plexi/Plexi.cs#L75). It takes an array of `Processor` instances and simply executes them in order.

```c#
Processor processor = new MultiProcessor(new Processor[]
{
    new Negative(),
    new RotateRight()
});

var InputImage = Plexi.ReadBitmapFromFile("images/lena_color.jpg");
var OutputImage = processor.Process(InputImage);
Plexi.WriteBitmapToFile(OutputImage, "output.png");
```

## INFOIBV (GUI)

Add your processors to the pipeline by adding your own processors to the `MultiProcessor` in  [Form1.cs line 66](https://github.com/mrexodia/Plexi/blob/master/INFOIBV/Form1.cs#L66).

## Plexi (command line)

The Plexi project ([Program.cs](https://github.com/mrexodia/Plexi/blob/master/Plexi/Program.cs)) is a simple command line interface that works with pipes (for Linux/OS X users). It works on images of arbitrary sizes.

The following command will execute a pipeline of the `Negative` and `Rotate` processors (defined in [Program.cs line 57](https://github.com/mrexodia/Plexi/blob/master/Plexi/Program.cs#L57)) on `images/lena_color.jpg` and save the resulting image as `output.png`.

```bash
mono bin/Debug/Plexi.exe Negative,Rotate < images/lena_color.jpg > output.png
```

The [test](https://github.com/mrexodia/Plexi/blob/master/Plexi/test) script for OS X allows you to quickly execute a pipeline on an image and view the results (this does the same as the command above).

```bash
./test images/lena_color.jpg Negative,Rotate
```
