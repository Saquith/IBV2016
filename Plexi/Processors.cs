using System;
using System.Drawing;
using System.Runtime.Remoting.Messaging;

namespace Plexi {
	public class WaterShedProcessor : Processor {
		private int _threshold;

		public WaterShedProcessor(int threshold) {
			_threshold = threshold;
		}

		public override Matrix Process(Matrix source) {
			var returnMatrix = new Matrix(source.X, source.Y);
			while (_threshold < 255) {
				for (int x = 0; x < source.X; x++) {
					for (int y = 0; y < source.Y; y++) {
						var neighbourCount = 0;
						for (int offsetX = -1; offsetX + x >= 0 && offsetX + x < source.X; offsetX++) {
							for (int offsetY = -1; offsetY + y >= 0 && offsetY + y < source.Y; offsetY++) {
								if (source[x + offsetX, y + offsetY].R != 0 && source[x + offsetX, y + offsetY].R < _threshold) {
									neighbourCount++;
								}
							}
						}
						if (neighbourCount > 1) {
							source[x, y] = Color.FromArgb(0, 0, 0);
						} else {
							source[x, y] = Color.FromArgb(_threshold, _threshold, _threshold);
						}
					}
				}
				_threshold++;
			}
			return returnMatrix;
		}
	}

	public class ArithmeticProcessor : Processor {
		private Arithmetic _arithmetic;
		private Matrix _target;

		public ArithmeticProcessor(Arithmetic arithmetic, Matrix target)
		{
			_arithmetic = arithmetic;
			_target = target;
		}

		public ArithmeticProcessor(Arithmetic arithmetic, Bitmap target)
		{
			_arithmetic = arithmetic;
			_target = target.GetMatrix();
		}

		public override Matrix Process(Matrix source)
		{
			return source.ApplyFunction(_arithmetic, _target);
		}
	}

	public class DistanceTransformProcessor : Processor {
		public override Matrix Process(Matrix source) {
			source = PassTop(source);
			return PassBottom(source);
		}

		private Matrix PassTop(Matrix sourceMatrix) {
			var kernel = new DistanceTransformTop();
			int offsetX = kernel.Center().Item1;
			int offsetY = kernel.Center().Item2;

			for (int imageY = offsetY; imageY < (sourceMatrix.Y - offsetY); imageY++) {
				for (int imageX = offsetX; imageX < (sourceMatrix.X - offsetX); imageX++) {
					sourceMatrix[imageX, imageY] = GetMinValue(sourceMatrix, kernel, imageX, imageY, offsetX, offsetY);
				}
			}
			return sourceMatrix;
		}

		private Matrix PassBottom(Matrix sourceMatrix) {
			var kernel = new DistanceTransformBottom();
			int offsetX = kernel.Center().Item1;
			int offsetY = kernel.Center().Item2;

			for (int imageY = (sourceMatrix.Y - offsetY - 1); imageY > offsetY; imageY--) {
				for (int imageX = (sourceMatrix.X - offsetX - 1); imageX > offsetX; imageX--) {
					sourceMatrix[imageX, imageY] = GetMinValue(sourceMatrix, kernel, imageX, imageY, offsetX, offsetY);
				}
			}
			return sourceMatrix;
		}

		private Color GetMinValue(Matrix sourceMatrix, Kernel kernel, int imageX, int imageY, int offsetX, int offsetY) {
			// reset values after each iteration
			var grayValue = 0;

			var minValue = int.MaxValue;

			// for loop goes through the kernel
			for (int y = -offsetY; y <= offsetY; y++) {
				for (int x = -offsetX; x <= offsetX; x++) {
					var currentValue = kernel.Matrix[x + offsetX, y + offsetY];
					if (currentValue.Equals(-1)) {
						continue;
					}
					minValue = Math.Min(sourceMatrix[imageX + x, imageY + y].R + (int)currentValue, minValue);
				}
			}
			return Color.FromArgb(minValue, minValue, minValue);
		}
	}

	public class MorphologyProcessor : Processor {
		private Morphology _morphology;
		private Kernel _kernel;

		public MorphologyProcessor(Morphology morphology, Kernel kernel) {
			_morphology = morphology;
			_kernel = kernel;
		}

		public override Matrix Process(Matrix source) {
			return source.ApplyFunction(_morphology, _kernel);
		}
	}

	public class ReconstructionProcessor : Processor {
		private Matrix _mask;
		private Kernel _kernel;
		private int _erosionCount, _dilationCount;

		public ReconstructionProcessor(Bitmap image, Kernel kernel, int erosions = 5, int dilations = 5) {
			_mask = image.GetMatrix();
			_kernel = kernel;
			_erosionCount = erosions;
			_dilationCount = dilations;
		}

		public override Matrix Process(Matrix source) {
			for (int i = 0; i < _erosionCount; i++) {
				source = source.ApplyFunction(Morphology.Erosion, _kernel);
			}
			for (int i = 0; i < _dilationCount; i++) {
				source = DilationWithMask(source, _mask, _kernel);
			}
			return source;
		}

		private Matrix DilationWithMask(Matrix sourceMatrix, Matrix mask, Kernel kernel) {
			var returnMatrix = new Matrix(sourceMatrix.X, sourceMatrix.Y);
			int offsetX = kernel.Center().Item1;
			int offsetY = kernel.Center().Item2;

			for (int imageY = offsetY; imageY < (sourceMatrix.Y - offsetY); imageY++) {
				for (int imageX = offsetX; imageX < (sourceMatrix.X - offsetX); imageX++) {
					if (mask[imageX, imageY].R <= 0) {
						continue;
					}

					// reset values after each iteration
					var grayValue = 0;
					var maxValue = 0;

					// for loop goes through the kernel
					for (int y = -offsetY; y <= offsetY; y++) {
						for (int x = -offsetX; x <= offsetX; x++) {
							if (kernel.Matrix[x + offsetX, y + offsetY].Equals(1)) {
								maxValue = Math.Max(sourceMatrix[imageX + x, imageY + y].R, maxValue);
							}
						}
					}
					grayValue = maxValue;

					returnMatrix[imageX, imageY] = Color.FromArgb(grayValue, grayValue, grayValue);
				}
			}
			return returnMatrix;
		}
	}

    public class Windowing : Processor
    {
        private int _max = 0;
        private int _min = 255;
        private int _mul;

        public override Matrix Process(Matrix sourceMatrix)
        {

            for (int imageY = 0; imageY < sourceMatrix.Y; imageY++)
            {
                for (int imageX = 0; imageX < sourceMatrix.X; imageX++)
                {
                    var grayValue = sourceMatrix[imageX, imageY].R;
                
                    _max = Math.Max(_max, grayValue);
                    _min = Math.Min(_min, grayValue);
                }
            }

            _mul = 255/(_max - _min);

            var returnMatrix = new Matrix(sourceMatrix.X, sourceMatrix.Y);
            for (int imageY = 0; imageY < sourceMatrix.Y; imageY++)
            {
                for (int imageX = 0; imageX < sourceMatrix.X; imageX++)
                {
                    var newColorVal = (sourceMatrix[imageX, imageY].R - _min) * _mul;
                    returnMatrix[imageX, imageY] = Color.FromArgb(newColorVal, newColorVal, newColorVal);
                }
            }
            return returnMatrix;
        }
    }

    //public class LabelWindowing : Processor
    //{
    //    private int _max = 0;
    //    private int _min = 255;
    //    private int _mul;

    //    public override Matrix Process(Matrix sourceMatrix)
    //    {
    //        int grayValue = -1;

    //        for (int imageY = 0; imageY < sourceMatrix.Y; imageY++)
    //        {
    //            for (int imageX = 0; imageX < sourceMatrix.X; imageX++)
    //            {
    //                if (sourceMatrix[imageX, imageY].R != 0)
    //                {
    //                    if (grayValue == -1)
    //                    {
    //                        _min = sourceMatrix[imageX, imageY].R;
    //                    }

    //                    grayValue = sourceMatrix[imageX, imageY].R;
    //                    _max = Math.Max(_max, grayValue);
    //                    _min = Math.Min(_min, grayValue);
    //                }
                        
    //            }
    //        }

    //        _mul = 255 / (_max - _min);

    //        var returnMatrix = new Matrix(sourceMatrix.X, sourceMatrix.Y);
    //        for (int imageY = 0; imageY < sourceMatrix.Y; imageY++)
    //        {
    //            for (int imageX = 0; imageX < sourceMatrix.X; imageX++)
    //            {
    //                var newColorVal = Math.Max((sourceMatrix[imageX, imageY].R - _min), 0) * _mul;
    //                returnMatrix[imageX, imageY] = Color.FromArgb(newColorVal, newColorVal, newColorVal);
    //            }
    //        }
    //        return returnMatrix;
    //    }
    //}

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
