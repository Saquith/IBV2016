namespace INFOIBV
{
    partial class INFOIBV
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
			this.LoadImageButton = new System.Windows.Forms.Button();
			this.openImageDialog = new System.Windows.Forms.OpenFileDialog();
			this.imageFileName = new System.Windows.Forms.TextBox();
			this._inputBox = new System.Windows.Forms.PictureBox();
			this.applyButton = new System.Windows.Forms.Button();
			this.saveImageDialog = new System.Windows.Forms.SaveFileDialog();
			this.saveButton = new System.Windows.Forms.Button();
			this._outputBox = new System.Windows.Forms.PictureBox();
			this._currentBox = new System.Windows.Forms.PictureBox();
			this._imageListBox = new ImageListBox();
			((System.ComponentModel.ISupportInitialize)(this._inputBox)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this._outputBox)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this._currentBox)).BeginInit();
			this.SuspendLayout();
			// 
			// LoadImageButton
			// 
			this.LoadImageButton.Location = new System.Drawing.Point(12, 12);
			this.LoadImageButton.Name = "LoadImageButton";
			this.LoadImageButton.Size = new System.Drawing.Size(98, 23);
			this.LoadImageButton.TabIndex = 0;
			this.LoadImageButton.Text = "Load image...";
			this.LoadImageButton.UseVisualStyleBackColor = true;
			this.LoadImageButton.Click += new System.EventHandler(this.LoadImageButton_Click);
			// 
			// openImageDialog
			// 
			this.openImageDialog.Filter = "Bitmap files (*.bmp;*.gif;*.jpg;*.png;*.tiff;*.jpeg)|*.bmp;*.gif;*.jpg;*.png;*.ti" +
    "ff;*.jpeg";
			this.openImageDialog.InitialDirectory = "..\\..\\images";
			// 
			// imageFileName
			// 
			this.imageFileName.Location = new System.Drawing.Point(116, 14);
			this.imageFileName.Name = "imageFileName";
			this.imageFileName.ReadOnly = true;
			this.imageFileName.Size = new System.Drawing.Size(316, 20);
			this.imageFileName.TabIndex = 1;
			// 
			// _inputBox
			// 
			this._inputBox.Location = new System.Drawing.Point(32, 41);
			this._inputBox.Name = "_inputBox";
			this._inputBox.Size = new System.Drawing.Size(400, 400);
			this._inputBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
			this._inputBox.TabIndex = 2;
			this._inputBox.TabStop = false;
			// 
			// applyButton
			// 
			this.applyButton.Location = new System.Drawing.Point(478, 12);
			this.applyButton.Name = "applyButton";
			this.applyButton.Size = new System.Drawing.Size(103, 23);
			this.applyButton.TabIndex = 3;
			this.applyButton.Text = "Apply";
			this.applyButton.UseVisualStyleBackColor = true;
			this.applyButton.Click += new System.EventHandler(this.applyButton_Click);
			// 
			// saveImageDialog
			// 
			this.saveImageDialog.Filter = "Bitmap file (*.bmp)|*.bmp";
			this.saveImageDialog.InitialDirectory = "..\\..\\images";
			// 
			// saveButton
			// 
			this.saveButton.Location = new System.Drawing.Point(948, 11);
			this.saveButton.Name = "saveButton";
			this.saveButton.Size = new System.Drawing.Size(95, 23);
			this.saveButton.TabIndex = 4;
			this.saveButton.Text = "Save as BMP...";
			this.saveButton.UseVisualStyleBackColor = true;
			this.saveButton.Click += new System.EventHandler(this.saveButton_Click);
			// 
			// _outputBox
			// 
			this._outputBox.Location = new System.Drawing.Point(1372, 41);
			this._outputBox.Name = "_outputBox";
			this._outputBox.Size = new System.Drawing.Size(400, 400);
			this._outputBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
			this._outputBox.TabIndex = 5;
			this._outputBox.TabStop = false;
			// 
			// _currentBox
			// 
			this._currentBox.Location = new System.Drawing.Point(679, 40);
			this._currentBox.Name = "_currentBox";
			this._currentBox.Size = new System.Drawing.Size(450, 450);
			this._currentBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
			this._currentBox.TabIndex = 8;
			this._currentBox.TabStop = false;
			// 
			// _imageListBox
			// 
			this._imageListBox.Location = new System.Drawing.Point(37, 494);
			this._imageListBox.Name = "_imageListBox";
			this._imageListBox.Size = new System.Drawing.Size(1730, 363);
			this._imageListBox.TabIndex = 9;
			// 
			// INFOIBV
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(1784, 861);
			this.Controls.Add(this._currentBox);
			this.Controls.Add(this._imageListBox);
			this.Controls.Add(this._outputBox);
			this.Controls.Add(this.saveButton);
			this.Controls.Add(this.applyButton);
			this.Controls.Add(this._inputBox);
			this.Controls.Add(this.imageFileName);
			this.Controls.Add(this.LoadImageButton);
			this.Location = new System.Drawing.Point(10, 10);
			this.Name = "INFOIBV";
			this.ShowIcon = false;
			this.Text = "INFOIBV";
			((System.ComponentModel.ISupportInitialize)(this._inputBox)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this._outputBox)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this._currentBox)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button LoadImageButton;
        private System.Windows.Forms.OpenFileDialog openImageDialog;
        private System.Windows.Forms.TextBox imageFileName;
        private System.Windows.Forms.PictureBox _inputBox;
        private System.Windows.Forms.Button applyButton;
        private System.Windows.Forms.SaveFileDialog saveImageDialog;
        private System.Windows.Forms.Button saveButton;
        private System.Windows.Forms.PictureBox _outputBox;
		private System.Windows.Forms.PictureBox _currentBox;
		private ImageListBox _imageListBox;
	}
}

