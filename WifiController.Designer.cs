namespace FeederDemoCS
{
    partial class WifiController
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
            this.sepAxes = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // sepAxes
            // 
            this.sepAxes.AutoSize = true;
            this.sepAxes.Location = new System.Drawing.Point(13, 13);
            this.sepAxes.Name = "sepAxes";
            this.sepAxes.Size = new System.Drawing.Size(129, 17);
            this.sepAxes.TabIndex = 0;
            this.sepAxes.Text = "Separate throttle axes";
            this.sepAxes.UseVisualStyleBackColor = true;
            this.sepAxes.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // WifiController
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(573, 374);
            this.Controls.Add(this.sepAxes);
            this.Name = "WifiController";
            this.Text = "WifiController";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox sepAxes;
    }
}