using System;
using System.Drawing;
using System.Windows.Forms;
using Plexi;

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

       private void applyButton_Click(object sender, EventArgs e)
        {
            if (InputImage == null) return;                                 // Get out if no input image
            pictureBox2.Image = null;                                       // Clear output image

            if (OutputImage != null) OutputImage.Dispose();                 // Reset output image
			
            Processor p1 = new MultiProcessor(new Processor[]
            {
                new Grayscale(),
                new Threshold(80), 
            });
			var source = p1.Process(InputImage);
	        pictureBox1.Image = source;

			//var image1 = new MorphologyProcessor(Morphology.Erosion, new Average3X3()).Process(source);
			//var image2 = new MorphologyProcessor(Morphology.Dilation, new Average3X3()).Process(source);
			//OutputImage = new ArithmeticProcessor(Arithmetic.Difference, image2).Process(image1);

			var image1 = new MorphologyProcessor(Morphology.Opening, new Average5X5()).Process(source);
			var image2 = new MorphologyProcessor(Morphology.Opening, new Average5X5()).Process(image1);
			OutputImage = new ArithmeticProcessor(Arithmetic.Difference, image2).Process(image1);

			//OutputImage = new ArithmeticProcessor(Arithmetic.Difference, image2).Process(image1);
			//OutputImage = new MorphologyProcessor(Arithmetic.Erosion, new Average3X3()).Process(image2);
			//OutputImage = new MorphologyProcessor(Arithmetic.Dilation, new Average5X5()).Process(image2);
			//Processor p3 = new MultiProcessor(new Processor[] {
			//	new MorphologyProcessor(Arithmetic.Erosion, new Average3X3()),
			//	new MorphologyProcessor(Arithmetic.Dilation, new Average3X3()),
			//	new ArithmeticProcessor(Arithmetic.Difference, ), 
			//});
			//OutputImage = p3.Process(image2);

			pictureBox3.Image = image1;
            pictureBox4.Image = image2;
            pictureBox2.Image = OutputImage;                               // Display output image
        }
        
        private void saveButton_Click(object sender, EventArgs e)
        {
            if (OutputImage == null) return;                                // Get out if no output image
            if (saveImageDialog.ShowDialog() == DialogResult.OK)
                OutputImage.Save(saveImageDialog.FileName);                 // Save the output image
        }
    }
}
