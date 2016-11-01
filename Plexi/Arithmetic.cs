using System;
using System.Drawing;

namespace Plexi
{
	public enum Arithmetic
	{
		Sum,
		Difference,
		Multiply,
		Divide,
		Average,
		Min,
		Max,
		Self
	}

	public static partial class MatrixExtensions
	{

		public static Matrix ApplyFunction(this Matrix sourceMatrix, Arithmetic arithmetic, Matrix targetMatrix)
		{
			switch (arithmetic)
			{
				case Arithmetic.Sum:
					return Sum(sourceMatrix, targetMatrix);
				case Arithmetic.Difference:
					return Difference(sourceMatrix, targetMatrix);
				case Arithmetic.Average:
					return Average(sourceMatrix, targetMatrix);
				case Arithmetic.Multiply:
					return Multiply(sourceMatrix, targetMatrix);
				case Arithmetic.Divide:
					return Divide(sourceMatrix, targetMatrix);
				case Arithmetic.Min:
					return Min(sourceMatrix, targetMatrix);
				case Arithmetic.Max:
					return Max(sourceMatrix, targetMatrix);
				default:
					return sourceMatrix;
			}
		}

		private static Matrix Sum(Matrix sourceMatrix, Matrix targetMatrix)
		{
			var returnMatrix = new Matrix(sourceMatrix.X, sourceMatrix.Y);
			for (int x = 0; x < targetMatrix.X; x++)
			{
				for (int y = 0; y < targetMatrix.Y; y++)
				{

					var grayValue1 = targetMatrix[x, y].R;
					var grayValue2 = sourceMatrix[x, y].R;
					var grayValue = Math.Min(Math.Max(grayValue1 + grayValue2, 0), 255);
					// min and max makes sure the value stays within the 0-255 range
					returnMatrix[x, y] = Color.FromArgb(grayValue, grayValue, grayValue);
				}
			}
			return returnMatrix;
		}

		private static Matrix Difference(Matrix sourceMatrix, Matrix targetMatrix)
		{
			var returnMatrix = new Matrix(sourceMatrix.X, sourceMatrix.Y);
			for (int x = 0; x < targetMatrix.X; x++)
			{
				for (int y = 0; y < targetMatrix.Y; y++)
				{

					var grayValue1 = targetMatrix[x, y].R;
					var grayValue2 = sourceMatrix[x, y].R;
					var grayValue = Math.Min(Math.Max(grayValue1 - grayValue2, 0), 255);
					// min and max makes sure the value stays within the 0-255 range
					returnMatrix[x, y] = Color.FromArgb(grayValue, grayValue, grayValue);
				}
			}
			return returnMatrix;
		}

		private static Matrix Average(Matrix sourceMatrix, Matrix targetMatrix)
		{
			var returnMatrix = new Matrix(sourceMatrix.X, sourceMatrix.Y);
			for (int x = 0; x < targetMatrix.X; x++)
			{
				for (int y = 0; y < targetMatrix.Y; y++)
				{

					var grayValue1 = targetMatrix[x, y].R;
					var grayValue2 = sourceMatrix[x, y].R;
					var grayValue = Math.Min(Math.Max((grayValue1 + grayValue2) / 2, 0), 255);
					// min and max makes sure the value stays within the 0-255 range
					returnMatrix[x, y] = Color.FromArgb(grayValue, grayValue, grayValue);
				}
			}
			return returnMatrix;
		}

		private static Matrix Multiply(Matrix sourceMatrix, Matrix targetMatrix)
		{
			var returnMatrix = new Matrix(sourceMatrix.X, sourceMatrix.Y);
			for (int x = 0; x < targetMatrix.X; x++) {
				for (int y = 0; y < targetMatrix.Y; y++) {

					var grayValue1 = targetMatrix[x, y].R;
					var grayValue2 = sourceMatrix[x, y].R;
					var grayValue = Math.Min(Math.Max(grayValue1 * grayValue2, 0), 255);
					// min and max makes sure the value stays within the 0-255 range
					returnMatrix[x, y] = Color.FromArgb(grayValue, grayValue, grayValue);
				}
			}
			return returnMatrix;
		}

		private static Matrix Divide(Matrix sourceMatrix, Matrix targetMatrix)
		{
			var returnMatrix = new Matrix(sourceMatrix.X, sourceMatrix.Y);
			for (int x = 0; x < targetMatrix.X; x++) {
				for (int y = 0; y < targetMatrix.Y; y++) {

					var grayValue1 = targetMatrix[x, y].R;
					var grayValue2 = sourceMatrix[x, y].R;
					var grayValue = Math.Min(Math.Max(grayValue1 / grayValue2, 0), 255);
					// min and max makes sure the value stays within the 0-255 range
					returnMatrix[x, y] = Color.FromArgb(grayValue, grayValue, grayValue);
				}
			}
			return returnMatrix;
		}

		private static Matrix Min(Matrix sourceMatrix, Matrix targetMatrix)
		{
			var returnMatrix = new Matrix(sourceMatrix.X, sourceMatrix.Y);
			for (int x = 0; x < targetMatrix.X; x++) {
				for (int y = 0; y < targetMatrix.Y; y++) {

					var grayValue1 = targetMatrix[x, y].R;
					var grayValue2 = sourceMatrix[x, y].R;
					var grayValue = Math.Min(grayValue1, grayValue2);
					// min and max makes sure the value stays within the 0-255 range
					returnMatrix[x, y] = Color.FromArgb(grayValue, grayValue, grayValue);
				}
			}
			return returnMatrix;
		}

		private static Matrix Max(Matrix sourceMatrix, Matrix targetMatrix)
		{
			var returnMatrix = new Matrix(sourceMatrix.X, sourceMatrix.Y);
			for (int x = 0; x < targetMatrix.X; x++) {
				for (int y = 0; y < targetMatrix.Y; y++) {

					var grayValue1 = targetMatrix[x, y].R;
					var grayValue2 = sourceMatrix[x, y].R;
					var grayValue = Math.Max(grayValue1, grayValue2);
					// min and max makes sure the value stays within the 0-255 range
					returnMatrix[x, y] = Color.FromArgb(grayValue, grayValue, grayValue);
				}
			}
			return returnMatrix;
		}
	}
}
