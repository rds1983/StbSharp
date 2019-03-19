namespace StbSharp.OggPlayer
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
			this._buttonOpen = new System.Windows.Forms.Button();
			this._buttonPlay = new System.Windows.Forms.Button();
			this._buttonStop = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// _buttonOpen
			// 
			this._buttonOpen.Location = new System.Drawing.Point(13, 13);
			this._buttonOpen.Name = "_buttonOpen";
			this._buttonOpen.Size = new System.Drawing.Size(75, 23);
			this._buttonOpen.TabIndex = 0;
			this._buttonOpen.Text = "Open...";
			this._buttonOpen.UseVisualStyleBackColor = true;
			this._buttonOpen.Click += new System.EventHandler(this._buttonOpen_Click);
			// 
			// _buttonPlay
			// 
			this._buttonPlay.Location = new System.Drawing.Point(95, 12);
			this._buttonPlay.Name = "_buttonPlay";
			this._buttonPlay.Size = new System.Drawing.Size(75, 23);
			this._buttonPlay.TabIndex = 1;
			this._buttonPlay.Text = "Play";
			this._buttonPlay.UseVisualStyleBackColor = true;
			this._buttonPlay.Click += new System.EventHandler(this._buttonPlay_Click);
			// 
			// _buttonStop
			// 
			this._buttonStop.Location = new System.Drawing.Point(177, 11);
			this._buttonStop.Name = "_buttonStop";
			this._buttonStop.Size = new System.Drawing.Size(75, 23);
			this._buttonStop.TabIndex = 2;
			this._buttonStop.Text = "Stop";
			this._buttonStop.UseVisualStyleBackColor = true;
			this._buttonStop.Click += new System.EventHandler(this._buttonStop_Click);
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(526, 44);
			this.Controls.Add(this._buttonStop);
			this.Controls.Add(this._buttonPlay);
			this.Controls.Add(this._buttonOpen);
			this.Name = "Form1";
			this.Text = "StbSharp OggPlayer";
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button _buttonOpen;
		private System.Windows.Forms.Button _buttonPlay;
		private System.Windows.Forms.Button _buttonStop;
	}
}

