using System;
using System.Drawing;

namespace Plexi
{
	public enum Morphology
	{
		Erosion,
		Dilation,
		Opening,
		Closing,
	}

	public static partial class MatrixExtensions
	{
		public static Matrix ApplyFunction(this Matrix sourceMatrix, Morphology morphology, Kernel kernel)
		{
			if (kernel != null) {
				switch (morphology) {
					case Morphology.Erosion:
						return Erosion(sourceMatrix, kernel);
					case Morphology.Dilation:
						return Dilation(sourceMatrix, kernel);
					case Morphology.Opening:
						return Opening(sourceMatrix, kernel);
					case Morphology.Closing:
						return Closing(sourceMatrix, kernel);
				}
			}
			return sourceMatrix;
		}

		private static Matrix Erosion(Matrix sourceMatrix, Kernel kernel)
		{
			var returnMatrix = new Matrix(sourceMatrix.X, sourceMatrix.Y);
			int offsetX = kernel.Center().Item1;
			int offsetY = kernel.Center().Item2;

			for (int imageY = offsetY; imageY < (sourceMatrix.Y - offsetY); imageY++) {
				for (int imageX = offsetX; imageX < (sourceMatrix.X - offsetX); imageX++) {
					// reset values after each iteration
					var grayValue = 0;
					var minValue = 255;

					// for loop goes through the kernel
					for (int y = -offsetY; y <= offsetY; y++) {
						for (int x = -offsetX; x <= offsetX; x++) {
							if (kernel.Matrix[x + offsetX, y + offsetY] == 1) {
								minValue = Math.Min(sourceMatrix[imageX + x, imageY + y].R, minValue);
							}
						}
					}

					grayValue = minValue;

					returnMatrix[imageX, imageY] = Color.FromArgb(grayValue, grayValue, grayValue);
				}
			}
			return returnMatrix;
		}

		private static Matrix Dilation(Matrix sourceMatrix, Kernel kernel) {
			var returnMatrix = new Matrix(sourceMatrix.X, sourceMatrix.Y);
			int offsetX = kernel.Center().Item1;
			int offsetY = kernel.Center().Item2;

			for (int imageY = offsetY; imageY < (sourceMatrix.Y - offsetY); imageY++) {
				for (int imageX = offsetX; imageX < (sourceMatrix.X - offsetX); imageX++) {
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

		private static Matrix Opening(Matrix sourceMatrix, Kernel kernel) {
			return Erosion(Dilation(sourceMatrix, kernel), kernel);
		}

		private static Matrix Closing(Matrix sourceMatrix, Kernel kernel) {
			return Dilation(Erosion(sourceMatrix, kernel), kernel);
		}
	}
}
