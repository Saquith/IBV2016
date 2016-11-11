using System.Drawing;

namespace Plexi {
	public static class BitmapExtensions {
		public static Matrix GetMatrix(this Bitmap bitmap) {
			var image = new Matrix(bitmap.Width, bitmap.Height);
			for (var x = 0; x < bitmap.Width; x++) {
				for (var y = 0; y < bitmap.Height; y++) {
					image[x, y] = bitmap.GetPixel(x, y);
				}
			}
			return image;
		}
	}
}
