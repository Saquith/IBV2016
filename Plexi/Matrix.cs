using System.Drawing;

namespace Plexi {
	public class Matrix {
		private Color[,] _matrix;

		public int X => _matrix.GetLength(0);
		public int Y => _matrix.GetLength(1);

		public Matrix(int rows, int columns) {
			_matrix = new Color[rows, columns];
		}

		public Color this[int a, int b] {
			get { return _matrix[a, b]; }
			set { _matrix[a, b] = value; }
		}

		public Bitmap ToBitmap() {
			var newBitmap = new Bitmap(X, Y);
			for (var x = 0; x < newBitmap.Width; x++)
				for (var y = 0; y < newBitmap.Height; y++)
					newBitmap.SetPixel(x, y, this[x, y]);

			//Return the new Bitmap.
			return newBitmap;
		}
	}
}
