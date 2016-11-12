using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace INFOIBV {
	public partial class ImageListBox : UserControl {
		private Dictionary<string, Bitmap> _imageTitles;
		private PictureBox[] _pictureBoxes;
		private Label[] _textBoxes;

		public ImageListBox() {
			InitializeComponent();

			_pictureBoxes = new[] {
				pictureBox1,
				pictureBox2,
				pictureBox3,
				pictureBox4,
				pictureBox5,
				pictureBox10,
				pictureBox9,
				pictureBox8,
				pictureBox7,
				pictureBox6,
				pictureBox15,
				pictureBox14,
				pictureBox13,
				pictureBox12,
				pictureBox11,
			};
			_textBoxes = new[] {
				label1,
				label2,
				label3,
				label4,
				label5,
				label10,
				label9,
				label8,
				label7,
				label6,
				label15,
				label14,
				label13,
				label12,
				label11,
			};
			ClearList();
		}

		public void ClearList() {
			_imageTitles = new Dictionary<string, Bitmap>();
		}

		public void AddImage(Bitmap bitmap, string title) {
			_imageTitles.Add(title, bitmap);

			RefreshImages();
		}

		private void RefreshImages() {
			foreach (var textBox in _textBoxes) {
				textBox.Text = "[NONE]";
			}
			var i = 0;
			foreach (var kvp in _imageTitles) {
				_textBoxes[i].Text = kvp.Key;
				_pictureBoxes[i].Image = kvp.Value;
				i++;
			}
		}

		public event EventHandler Clicked;
		
		private void Picture_Click(object sender, EventArgs e) {
			Clicked(sender, e);
		}
	}
}
