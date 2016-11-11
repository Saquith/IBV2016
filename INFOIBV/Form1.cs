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

			Processor p1 = new MultiProcessor(new Processor[] {
				new Grayscale(),
				new Threshold(60),
				new MorphologyProcessor(Morphology.Opening, new Average5X5()),
				new MorphologyProcessor(Morphology.Erosion, new Average5X5()),
			});

			var image1 = p1.Process(InputImage);

			Processor p2 = new MultiProcessor(new Processor[] {
				new ReconstructionProcessor(image1, new Average9X9(), 0, 51),
			});

			var edge = CreateEdgeMatrix(image1, 5).ToBitmap();
			var image2 = p2.Process(edge);

			Processor p3 = new MultiProcessor(new Processor[] {
				new ArithmeticProcessor(Arithmetic.Difference, image1), 
			});
			var image3 = p3.Process(image2);

			OutputImage = image3;

			//var image3 = new LabelProcessor().Process(image1);
			//OutputImage = image3;

			pictureBox1.Image = image2;
			pictureBox3.Image = image1;
			pictureBox4.Image = edge;
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
