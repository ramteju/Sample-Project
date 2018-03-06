namespace Mol2Image
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
            this.inPath = new System.Windows.Forms.TextBox();
            this.s8500Btn = new System.Windows.Forms.Button();
            this.s9000Btn = new System.Windows.Forms.Button();
            this.statusLabel = new System.Windows.Forms.Label();
            this.backgroundWorker = new System.ComponentModel.BackgroundWorker();
            this.outPath = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // inPath
            // 
            this.inPath.Location = new System.Drawing.Point(84, 12);
            this.inPath.Name = "inPath";
            this.inPath.Size = new System.Drawing.Size(454, 25);
            this.inPath.TabIndex = 0;
            this.inPath.Text = "\\\\192.168.45.38\\share\\Annotation PDFs Creation_2016\\New folder\\OrgRef8500.xml";
            // 
            // s8500Btn
            // 
            this.s8500Btn.Location = new System.Drawing.Point(460, 86);
            this.s8500Btn.Name = "s8500Btn";
            this.s8500Btn.Size = new System.Drawing.Size(75, 23);
            this.s8500Btn.TabIndex = 2;
            this.s8500Btn.Text = "8500";
            this.s8500Btn.UseVisualStyleBackColor = true;
            this.s8500Btn.Click += new System.EventHandler(this.s8500Btn_Click);
            // 
            // s9000Btn
            // 
            this.s9000Btn.Location = new System.Drawing.Point(379, 86);
            this.s9000Btn.Name = "s9000Btn";
            this.s9000Btn.Size = new System.Drawing.Size(75, 23);
            this.s9000Btn.TabIndex = 3;
            this.s9000Btn.Text = "9000";
            this.s9000Btn.UseVisualStyleBackColor = true;
            this.s9000Btn.Click += new System.EventHandler(this.s9000Btn_Click);
            // 
            // statusLabel
            // 
            this.statusLabel.Location = new System.Drawing.Point(9, 89);
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(364, 20);
            this.statusLabel.TabIndex = 4;
            this.statusLabel.Text = "statusLabel";
            // 
            // backgroundWorker
            // 
            this.backgroundWorker.WorkerReportsProgress = true;
            this.backgroundWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker_DoWork);
            this.backgroundWorker.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.backgroundWorker_ProgressChanged);
            this.backgroundWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker_RunWorkerCompleted);
            // 
            // outPath
            // 
            this.outPath.Location = new System.Drawing.Point(84, 47);
            this.outPath.Name = "outPath";
            this.outPath.Size = new System.Drawing.Size(454, 25);
            this.outPath.TabIndex = 5;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(9, 17);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(69, 20);
            this.label1.TabIndex = 6;
            this.label1.Text = "XML Path";
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(9, 50);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(69, 20);
            this.label2.TabIndex = 7;
            this.label2.Text = "OUT Dir";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(550, 121);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.outPath);
            this.Controls.Add(this.statusLabel);
            this.Controls.Add(this.s9000Btn);
            this.Controls.Add(this.s8500Btn);
            this.Controls.Add(this.inPath);
            this.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Mol2Image";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox inPath;
        private System.Windows.Forms.Button s8500Btn;
        private System.Windows.Forms.Button s9000Btn;
        private System.Windows.Forms.Label statusLabel;
        private System.ComponentModel.BackgroundWorker backgroundWorker;
        private System.Windows.Forms.TextBox outPath;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
    }
}

