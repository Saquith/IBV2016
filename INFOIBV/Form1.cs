using System;
using System.Drawing;
using System.Windows.Forms;
using Plexi;
using Plexi = Plexi.Plexi;

namespace INFOIBV
{
    public partial class INFOIBV : Form
    {
        private Bitmap InputImage;
        private Bitmap OutputImage;

        public INFOIBV()
        {
            InitializeComponent();
        }

        private void LoadImageButton_Click(object sender, EventArgs e)
        {
           if (openImageDialog.ShowDialog() == DialogResult.OK)             // Open File Dialog
            {
                string file = openImageDialog.FileName;                     // Get the file name
                imageFileName.Text = file;                                  // Show file name
                if (InputImage != null) InputImage.Dispose();               // Reset image
                InputImage = new Bitmap(file);                              // Create new Bitmap from file
                if (InputImage.Size.Height <= 0 || InputImage.Size.Width <= 0 ||
                    InputImage.Size.Height > 512 || InputImage.Size.Width > 512) // Dimension check
                    MessageBox.Show("Error in image dimensions (have to be > 0 and <= 512)");
                else
                    pictureBox1.Image = InputImage;                         // Display input image
            }
        }

		private void applyButton_Click(object sender, EventArgs e) {
			if (InputImage == null) return; // Get out if no input image
			pictureBox2.Image = null; // Clear output image

			if (OutputImage != null) OutputImage.Dispose(); // Reset output image

			// Preparation step for white top hat
			Processor ppre = new MultiProcessor(new Processor[] {
				new Grayscale(),
				new MorphologyProcessor(Morphology.Opening, new Average3X3()),
			});
			var pre = ppre.Process(InputImage);

			// White top hat
			Processor pdark = new MultiProcessor(new Processor[] {
				new Grayscale(),
				new ArithmeticProcessor(Arithmetic.Difference, InputImage),
				new Threshold(38),
			});
			var darkImage = pdark.Process(pre);

			// Thresholded image
			Processor ppre2 = new MultiProcessor(new Processor[] {
				new Grayscale(),
				new Threshold(77),
			});

			var lightImage = ppre2.Process(InputImage);

			// Combine white top hat & thresholded images, additional processing steps for connecting objects
			Processor pcombine = new MultiProcessor(new Processor[] {
				new ArithmeticProcessor(Arithmetic.Sum, darkImage),
				new MorphologyProcessor(Morphology.Opening, new Average3X3()),
				new MorphologyProcessor(Morphology.Closing, new Average3X3()),
				new MorphologyProcessor(Morphology.Dilation, new Average3X3()),
				new MorphologyProcessor(Morphology.Dilation, new Average3X3()),
				new MorphologyProcessor(Morphology.Erosion, new Average3X3()),
				new MorphologyProcessor(Morphology.Erosion, new Average3X3()),
			});
			var fullimage = pcombine.Process(lightImage);

			// Remove edge objects
			Processor p2 = new MultiProcessor(new Processor[] {
				new ReconstructionProcessor(fullimage, new Average3X3(), 0, 204),
				new ArithmeticProcessor(Arithmetic.Difference, fullimage),
			});

			var edge = CreateEdgeMatrix(InputImage, 5).ToBitmap();
			var edgeRemoved = p2.Process(edge);

			Processor p4 = new MultiProcessor(new Processor[] {
			});
			OutputImage = p4.Process(edgeRemoved);

			pictureBox1.Image = lightImage;
			pictureBox3.Image = fullimage;
			pictureBox4.Image = edgeRemoved;
			pictureBox2.Image = OutputImage; // Display output image
		}

		private Matrix CreateEdgeMatrix(Bitmap image1, int width) {
			var returnMatrix = new Matrix(image1.Width, image1.Height);
			for (int x = 0; x < width; x++) {
				for (int y = 0; y < image1.Width; y++) {
					returnMatrix[y, x] = Color.FromArgb(255, 255, 255);
					returnMatrix[y, image1.Height - x - 1] = Color.FromArgb(255, 255, 255);
				}
				for (int y = 0; y < image1.Height; y++) {
					returnMatrix[x, y] = Color.FromArgb(255, 255, 255);
					returnMatrix[image1.Width - x - 1, y] = Color.FromArgb(255, 255, 255);
				}
			}
			return returnMatrix;
		}

	    private void saveButton_Click(object sender, EventArgs e)
        {
            if (OutputImage == null) return;                                // Get out if no output image
            if (saveImageDialog.ShowDialog() == DialogResult.OK)
                OutputImage.Save(saveImageDialog.FileName);                 // Save the output image
        }
    }
}
