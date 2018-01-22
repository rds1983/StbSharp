namespace StbSharp.WinForms.Test
{
	partial class Form1
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
			this.button1 = new System.Windows.Forms.Button();
			this.pictureBox1 = new System.Windows.Forms.PictureBox();
			this.buttonSave = new System.Windows.Forms.Button();
			this._numericWidth = new System.Windows.Forms.NumericUpDown();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this._numericHeight = new System.Windows.Forms.NumericUpDown();
			this.buttonResize = new System.Windows.Forms.Button();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this._numericWidth)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this._numericHeight)).BeginInit();
			this.SuspendLayout();
			// 
			// button1
			// 
			this.button1.Location = new System.Drawing.Point(13, 13);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(75, 23);
			this.button1.TabIndex = 0;
			this.button1.Text = "Load...";
			this.button1.UseVisualStyleBackColor = true;
			this.button1.Click += new System.EventHandler(this.button1_Click);
			// 
			// pictureBox1
			// 
			this.pictureBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.pictureBox1.Location = new System.Drawing.Point(12, 42);
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Size = new System.Drawing.Size(707, 491);
			this.pictureBox1.TabIndex = 1;
			this.pictureBox1.TabStop = false;
			// 
			// buttonSave
			// 
			this.buttonSave.Location = new System.Drawing.Point(94, 13);
			this.buttonSave.Name = "buttonSave";
			this.buttonSave.Size = new System.Drawing.Size(75, 23);
			this.buttonSave.TabIndex = 6;
			this.buttonSave.Text = "Save...";
			this.buttonSave.UseVisualStyleBackColor = true;
			this.buttonSave.Click += new System.EventHandler(this.buttonSave_Click);
			// 
			// _numericWidth
			// 
			this._numericWidth.Location = new System.Drawing.Point(343, 16);
			this._numericWidth.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
			this._numericWidth.Name = "_numericWidth";
			this._numericWidth.Size = new System.Drawing.Size(120, 20);
			this._numericWidth.TabIndex = 7;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(299, 18);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(38, 13);
			this.label1.TabIndex = 8;
			this.label1.Text = "Width:";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(469, 18);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(41, 13);
			this.label2.TabIndex = 10;
			this.label2.Text = "Height:";
			// 
			// _numericHeight
			// 
			this._numericHeight.Location = new System.Drawing.Point(513, 16);
			this._numericHeight.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
			this._numericHeight.Name = "_numericHeight";
			this._numericHeight.Size = new System.Drawing.Size(120, 20);
			this._numericHeight.TabIndex = 9;
			// 
			// buttonResize
			// 
			this.buttonResize.Location = new System.Drawing.Point(639, 13);
			this.buttonResize.Name = "buttonResize";
			this.buttonResize.Size = new System.Drawing.Size(75, 23);
			this.buttonResize.TabIndex = 11;
			this.buttonResize.Text = "Resize";
			this.buttonResize.UseVisualStyleBackColor = true;
			this.buttonResize.Click += new System.EventHandler(this.buttonResize_Click);
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(731, 545);
			this.Controls.Add(this.buttonResize);
			this.Controls.Add(this.label2);
			this.Controls.Add(this._numericHeight);
			this.Controls.Add(this.label1);
			this.Controls.Add(this._numericWidth);
			this.Controls.Add(this.buttonSave);
			this.Controls.Add(this.pictureBox1);
			this.Controls.Add(this.button1);
			this.Name = "Form1";
			this.Text = "Form1";
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this._numericWidth)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this._numericHeight)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.PictureBox pictureBox1;
		private System.Windows.Forms.Button buttonSave;
		private System.Windows.Forms.NumericUpDown _numericWidth;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.NumericUpDown _numericHeight;
		private System.Windows.Forms.Button buttonResize;
	}
}

