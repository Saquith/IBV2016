﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Plexi;
using Plexi = Plexi.Plexi;

namespace INFOIBV
{
    public partial class INFOIBVForm : Form
    {
        private Bitmap InputImage;
        private Bitmap OutputImage;
        private Dictionary<string, Bitmap> _queuedImages;

        public INFOIBVForm()
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
                    _inputBox.Image = InputImage;                         // Display input image
            }
        }

        private void applyButton_Click(object sender, EventArgs e)
        {
            if (InputImage == null) return; // Get out if no input image
            _outputBox.Image = null; // Clear output image
            _queuedImages = new Dictionary<string, Bitmap>();
            _imageListBox.ClearList();

            if (OutputImage != null) OutputImage.Dispose(); // Reset output image
            InputImage = new Grayscale().Process(InputImage); // Grayscale the InputImage

            // White top hat
            Processor pdark = new MultiProcessor(new Processor[] {
                new MorphologyProcessor(Morphology.Opening, new Average3X3()),
                new ArithmeticProcessor(Arithmetic.Difference, InputImage),
                new Threshold(38),
            });
            var darkImage = pdark.Process(InputImage);
            AddImageToPreview(darkImage, "White tophat thresholded image");

            // Combine white top hat & thresholded images
            Processor pcombine = new MultiProcessor(new Processor[] {
                new Threshold(77),
                new ArithmeticProcessor(Arithmetic.Sum, darkImage),
            });
            var fullImage = pcombine.Process(InputImage);
            AddImageToPreview(fullImage, "Combined thresholded image");

            // Morphology
            Processor pmorph = new MultiProcessor(new Processor[]
            {
                new MorphologyProcessor(Morphology.Closing, new Average5X5()),
            });
            var morphedImage = pmorph.Process(fullImage);
            AddImageToPreview(morphedImage, "Closing to reduce holes");

            // Reconstruction
            Processor prec = new MultiProcessor(new Processor[]
            {
                new ReconstructionProcessor(morphedImage, new Average3X3(), 3, 15),
                new Negative(),
                new ArithmeticProcessor(Arithmetic.Difference, morphedImage),
            });
            var reconstructedImage = prec.Process(morphedImage);
            AddImageToPreview(reconstructedImage, "Reconstruction to remove small structures");

            // Remove edge objects
            Processor edgeRemoveProcessor = new MultiProcessor(new Processor[] {
                new ReconstructionProcessor(reconstructedImage, new Average3X3(), 0, 204),
                new ArithmeticProcessor(Arithmetic.Difference, reconstructedImage),
            });

            var edge = CreateEdgeMatrix(InputImage, 5).ToBitmap();
            var edgeRemoved = edgeRemoveProcessor.Process(edge);
            AddImageToPreview(edgeRemoved, "Edge objects removed");

            //var distanceTransformed = new DistanceTransformProcessor().Process(edgeRemoved);
            //AddImageToPreview(distanceTransformed, "Distance transform");
            // No longer needed as WaterShed is off the board for now

            // Create three sets of filters, use the previous (more specific) one to filter the new one (more generic)
            Processor processor = new MultiProcessor(new Processor[] {
                new FilterObjects(0.80f, 80, 1),
            });
            var filteredA = processor.Process(edgeRemoved);

            processor = new MultiProcessor(new Processor[] {
                new FilterObjects(0.70f, 120, 1),
            });
            var filteredB = processor.Process(edgeRemoved);

            processor = new MultiProcessor(new Processor[] {
                new FilterObjects(0.60f, 160, 1),
            });
            var filteredC = processor.Process(edgeRemoved);

            // Remove what's already been filtered before & filter C before B as B still contains A
            // FilteredC - FilteredB
            filteredC = new ArithmeticProcessor(Arithmetic.Difference, filteredC).Process(filteredB);
            // FilteredB - FilteredA
            filteredB = new ArithmeticProcessor(Arithmetic.Difference, filteredB).Process(filteredA);
            AddImageToPreview(filteredA, "Filtered strict");
            AddImageToPreview(filteredB, "Filtered intermediate");
            AddImageToPreview(filteredC, "Filtered lenient");

            // Labelling, mark from white > gray for better visuals
            var labelledA = new Labeling(225).Process(filteredA);
            AddImageToPreview(labelledA, "Labelling A");
            var labelledB = new Labeling(125).Process(filteredB);
            AddImageToPreview(labelledB, "Labelling B");
            var labelledC = new Labeling(25).Process(filteredC);
            AddImageToPreview(labelledC, "Labelling C");

            // Compile Output image from ABC
            var fullImageProcessor = new MultiProcessor(new Processor[] {
                new ArithmeticProcessor(Arithmetic.Sum, labelledA),
                new ArithmeticProcessor(Arithmetic.Sum, labelledB),
                new WindowingProcessor(),
            });
            OutputImage = fullImageProcessor.Process(labelledC);
            AddImageToPreview(OutputImage, "Combined result");

            _inputBox.Image = InputImage;
            _outputBox.Image = OutputImage;

            ViewImages();
        }

        private void ViewImages()
        {
            foreach (var kvp in _queuedImages)
            {
                _imageListBox.AddImage(kvp.Value, kvp.Key);
            }
            _currentBox.Image = _queuedImages.Values.FirstOrDefault();
            _imageListBox.Clicked += this.SetCurrentImage;
        }

        private void SetCurrentImage(object sender, EventArgs e)
        {
            if ((sender as PictureBox).Image != null)
            {
                _currentBox.Image = (sender as PictureBox).Image;
            }
        }

        private void AddImageToPreview(Bitmap bitmap, string s)
        {
            _queuedImages.Add(s, bitmap);
        }

        private Matrix CreateEdgeMatrix(Bitmap image1, int width)
        {
            var returnMatrix = new Matrix(image1.Width, image1.Height);
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < image1.Width; y++)
                {
                    returnMatrix[y, x] = Color.FromArgb(255, 255, 255);
                    returnMatrix[y, image1.Height - x - 1] = Color.FromArgb(255, 255, 255);
                }
                for (int y = 0; y < image1.Height; y++)
                {
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

        private void _outputBox_Click(object sender, EventArgs e)
        {
            if ((sender as PictureBox).Image != null)
            {
                _currentBox.Image = (sender as PictureBox).Image;
            }
        }

        private void _inputBox_Click(object sender, EventArgs e)
        {
            if ((sender as PictureBox).Image != null)
            {
                _currentBox.Image = (sender as PictureBox).Image;
            }
        }
    }
}
