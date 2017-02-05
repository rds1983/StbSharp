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
			this.labelStatus = new System.Windows.Forms.Label();
			this.labelOrdinary = new System.Windows.Forms.Label();
			this.labelStbSharp = new System.Windows.Forms.Label();
			this.labelWrongCount = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
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
			this.pictureBox1.Size = new System.Drawing.Size(707, 458);
			this.pictureBox1.TabIndex = 1;
			this.pictureBox1.TabStop = false;
			// 
			// labelStatus
			// 
			this.labelStatus.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.labelStatus.AutoSize = true;
			this.labelStatus.Location = new System.Drawing.Point(12, 503);
			this.labelStatus.Name = "labelStatus";
			this.labelStatus.Size = new System.Drawing.Size(37, 13);
			this.labelStatus.TabIndex = 2;
			this.labelStatus.Text = "Status";
			// 
			// labelOrdinary
			// 
			this.labelOrdinary.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.labelOrdinary.AutoSize = true;
			this.labelOrdinary.Location = new System.Drawing.Point(12, 523);
			this.labelOrdinary.Name = "labelOrdinary";
			this.labelOrdinary.Size = new System.Drawing.Size(35, 13);
			this.labelOrdinary.TabIndex = 3;
			this.labelOrdinary.Text = "label1";
			// 
			// labelStbSharp
			// 
			this.labelStbSharp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.labelStbSharp.AutoSize = true;
			this.labelStbSharp.Location = new System.Drawing.Point(378, 523);
			this.labelStbSharp.Name = "labelStbSharp";
			this.labelStbSharp.Size = new System.Drawing.Size(35, 13);
			this.labelStbSharp.TabIndex = 4;
			this.labelStbSharp.Text = "label1";
			// 
			// labelWrongCount
			// 
			this.labelWrongCount.AutoSize = true;
			this.labelWrongCount.Location = new System.Drawing.Point(559, 522);
			this.labelWrongCount.Name = "labelWrongCount";
			this.labelWrongCount.Size = new System.Drawing.Size(35, 13);
			this.labelWrongCount.TabIndex = 5;
			this.labelWrongCount.Text = "label1";
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(731, 545);
			this.Controls.Add(this.labelWrongCount);
			this.Controls.Add(this.labelStbSharp);
			this.Controls.Add(this.labelOrdinary);
			this.Controls.Add(this.labelStatus);
			this.Controls.Add(this.pictureBox1);
			this.Controls.Add(this.button1);
			this.Name = "Form1";
			this.Text = "Form1";
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.PictureBox pictureBox1;
		private System.Windows.Forms.Label labelStatus;
		private System.Windows.Forms.Label labelOrdinary;
		private System.Windows.Forms.Label labelStbSharp;
		private System.Windows.Forms.Label labelWrongCount;
	}
}

