
namespace Muse.LiveFeed
{
    partial class frmDevices
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
            this.panButtons = new System.Windows.Forms.Panel();
            this.lisDevices = new System.Windows.Forms.ListBox();
            this.panDevices = new System.Windows.Forms.Panel();
            this.panDevices.SuspendLayout();
            this.SuspendLayout();
            // 
            // panButtons
            // 
            this.panButtons.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panButtons.Location = new System.Drawing.Point(0, 382);
            this.panButtons.Name = "panButtons";
            this.panButtons.Size = new System.Drawing.Size(434, 74);
            this.panButtons.TabIndex = 0;
            // 
            // lisDevices
            // 
            this.lisDevices.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lisDevices.FormattingEnabled = true;
            this.lisDevices.Location = new System.Drawing.Point(8, 8);
            this.lisDevices.Name = "lisDevices";
            this.lisDevices.Size = new System.Drawing.Size(418, 366);
            this.lisDevices.TabIndex = 1;
            // 
            // panDevices
            // 
            this.panDevices.Controls.Add(this.lisDevices);
            this.panDevices.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panDevices.Location = new System.Drawing.Point(0, 0);
            this.panDevices.Name = "panDevices";
            this.panDevices.Padding = new System.Windows.Forms.Padding(8);
            this.panDevices.Size = new System.Drawing.Size(434, 382);
            this.panDevices.TabIndex = 2;
            // 
            // frmDevices
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(434, 456);
            this.Controls.Add(this.panDevices);
            this.Controls.Add(this.panButtons);
            this.MaximizeBox = false;
            this.Name = "frmDevices";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Devices";
            this.panDevices.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panButtons;
        private System.Windows.Forms.ListBox lisDevices;
        private System.Windows.Forms.Panel panDevices;
    }
}