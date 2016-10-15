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

        public class Negative : Processor
        {
            public override Color Transform(Color c)
            {
                return Color.FromArgb(255 - c.R, 255 - c.G, 255 - c.B);
            }
        }

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

        public class Grayscale : Processor
        {
            public override Color Transform(Color c)
            {
                int grayVal = (int)((c.R * 0.21) + (c.G * 0.72) + (c.B * 0.07)); // The numbers represent a weight for better grayscale values
                return Color.FromArgb(grayVal, grayVal, grayVal);
            }
        }

        public class Threshold : Processor
        {
            public override Color Transform(Color c)
            {
                int t = 128;    // Threshold value

                if (c.R > t) { return Color.White; }
                else return Color.Black;
            }
        }

        private void applyButton_Click(object sender, EventArgs e)
        {
            if (InputImage == null) return;                                 // Get out if no input image
            pictureBox2.Image = null;                                       // Clear output image

            if (OutputImage != null) OutputImage.Dispose();                 // Reset output image

            //TODO use your own processor(s) here
            Processor processor = new MultiProcessor(new Processor[]
            {
                new Grayscale(), 
                new Threshold()
                //new Negative(),
                //new RotateRight()
            });
            OutputImage = processor.Process(InputImage);                   // Process the image

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
