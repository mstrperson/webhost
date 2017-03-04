namespace CommentPublishingFormApplication
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.BackgroundProcessLog = new System.Windows.Forms.TextBox();
            this.StateLabel = new System.Windows.Forms.Label();
            this.WorkingLabel = new System.Windows.Forms.Label();
            this.PendingLabel = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel1.Controls.Add(this.BackgroundProcessLog);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel1.Location = new System.Drawing.Point(429, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(835, 681);
            this.panel1.TabIndex = 0;
            // 
            // BackgroundProcessLog
            // 
            this.BackgroundProcessLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.BackgroundProcessLog.Location = new System.Drawing.Point(0, 0);
            this.BackgroundProcessLog.Multiline = true;
            this.BackgroundProcessLog.Name = "BackgroundProcessLog";
            this.BackgroundProcessLog.ReadOnly = true;
            this.BackgroundProcessLog.Size = new System.Drawing.Size(831, 677);
            this.BackgroundProcessLog.TabIndex = 0;
            // 
            // StateLabel
            // 
            this.StateLabel.BackColor = System.Drawing.Color.Green;
            this.StateLabel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.StateLabel.Font = new System.Drawing.Font("Consolas", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.StateLabel.ForeColor = System.Drawing.Color.White;
            this.StateLabel.Location = new System.Drawing.Point(13, 18);
            this.StateLabel.Name = "StateLabel";
            this.StateLabel.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.StateLabel.Size = new System.Drawing.Size(398, 124);
            this.StateLabel.TabIndex = 1;
            this.StateLabel.Text = "Completed";
            this.StateLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // WorkingLabel
            // 
            this.WorkingLabel.BackColor = System.Drawing.Color.DeepSkyBlue;
            this.WorkingLabel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.WorkingLabel.Font = new System.Drawing.Font("Consolas", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.WorkingLabel.ForeColor = System.Drawing.Color.White;
            this.WorkingLabel.Location = new System.Drawing.Point(12, 142);
            this.WorkingLabel.Name = "WorkingLabel";
            this.WorkingLabel.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.WorkingLabel.Size = new System.Drawing.Size(398, 124);
            this.WorkingLabel.TabIndex = 2;
            this.WorkingLabel.Text = "Working";
            this.WorkingLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // PendingLabel
            // 
            this.PendingLabel.BackColor = System.Drawing.Color.Indigo;
            this.PendingLabel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.PendingLabel.Font = new System.Drawing.Font("Consolas", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.PendingLabel.ForeColor = System.Drawing.Color.White;
            this.PendingLabel.Location = new System.Drawing.Point(12, 266);
            this.PendingLabel.Name = "PendingLabel";
            this.PendingLabel.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.PendingLabel.Size = new System.Drawing.Size(398, 124);
            this.PendingLabel.TabIndex = 3;
            this.PendingLabel.Text = "Pending";
            this.PendingLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1264, 681);
            this.Controls.Add(this.PendingLabel);
            this.Controls.Add(this.WorkingLabel);
            this.Controls.Add(this.StateLabel);
            this.Controls.Add(this.panel1);
            this.Name = "Form1";
            this.Text = "Comment Letter Publishing Service";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TextBox BackgroundProcessLog;
        private System.Windows.Forms.Label StateLabel;
        private System.Windows.Forms.Label WorkingLabel;
        private System.Windows.Forms.Label PendingLabel;
    }
}

