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
			};
			_textBoxes = new[] {
				label1,
				label2,
				label3,
				label4,
				label5,
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

		private void pictureBox1_Click(object sender, EventArgs e) {
			Clicked(sender, e);
		}

		private void pictureBox2_Click(object sender, EventArgs e) {
			Clicked(sender, e);
		}

		private void pictureBox3_Click(object sender, EventArgs e) {
			Clicked(sender, e);
		}

		private void pictureBox4_Click(object sender, EventArgs e) {
			Clicked(sender, e);
		}

		private void pictureBox5_Click(object sender, EventArgs e) {
			Clicked(sender, e);
		}
	}
}
