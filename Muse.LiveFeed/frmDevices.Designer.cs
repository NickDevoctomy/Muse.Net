
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
            this.butCancel = new System.Windows.Forms.Button();
            this.butOk = new System.Windows.Forms.Button();
            this.panButtons.SuspendLayout();
            this.panDevices.SuspendLayout();
            this.SuspendLayout();
            // 
            // panButtons
            // 
            this.panButtons.Controls.Add(this.butOk);
            this.panButtons.Controls.Add(this.butCancel);
            this.panButtons.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panButtons.Location = new System.Drawing.Point(0, 403);
            this.panButtons.Name = "panButtons";
            this.panButtons.Size = new System.Drawing.Size(434, 53);
            this.panButtons.TabIndex = 0;
            // 
            // lisDevices
            // 
            this.lisDevices.DisplayMember = "Name";
            this.lisDevices.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lisDevices.FormattingEnabled = true;
            this.lisDevices.Location = new System.Drawing.Point(8, 8);
            this.lisDevices.Name = "lisDevices";
            this.lisDevices.Size = new System.Drawing.Size(418, 387);
            this.lisDevices.TabIndex = 1;
            // 
            // panDevices
            // 
            this.panDevices.Controls.Add(this.lisDevices);
            this.panDevices.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panDevices.Location = new System.Drawing.Point(0, 0);
            this.panDevices.Name = "panDevices";
            this.panDevices.Padding = new System.Windows.Forms.Padding(8);
            this.panDevices.Size = new System.Drawing.Size(434, 403);
            this.panDevices.TabIndex = 2;
            // 
            // butCancel
            // 
            this.butCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.butCancel.Location = new System.Drawing.Point(328, 6);
            this.butCancel.Name = "butCancel";
            this.butCancel.Size = new System.Drawing.Size(94, 35);
            this.butCancel.TabIndex = 0;
            this.butCancel.Text = "Cancel";
            this.butCancel.UseVisualStyleBackColor = true;
            this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
            // 
            // butOk
            // 
            this.butOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.butOk.Location = new System.Drawing.Point(228, 6);
            this.butOk.Name = "butOk";
            this.butOk.Size = new System.Drawing.Size(94, 35);
            this.butOk.TabIndex = 1;
            this.butOk.Text = "OK";
            this.butOk.UseVisualStyleBackColor = true;
            this.butOk.Click += new System.EventHandler(this.butOk_Click);
            // 
            // frmDevices
            // 
            this.AcceptButton = this.butCancel;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.butOk;
            this.ClientSize = new System.Drawing.Size(434, 456);
            this.Controls.Add(this.panDevices);
            this.Controls.Add(this.panButtons);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "frmDevices";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Devices";
            this.panButtons.ResumeLayout(false);
            this.panDevices.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panButtons;
        private System.Windows.Forms.ListBox lisDevices;
        private System.Windows.Forms.Panel panDevices;
        private System.Windows.Forms.Button butOk;
        private System.Windows.Forms.Button butCancel;
    }
}