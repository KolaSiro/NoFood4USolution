namespace Flugbahn
{
    partial class FlugbahnDlg
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            lblResolution = new Label();
            timerRecording = new System.Windows.Forms.Timer(components);
            timerPlayBack = new System.Windows.Forms.Timer(components);
            lblMaus = new Label();
            lblMausPosition = new Label();
            SuspendLayout();
            // 
            // lblResolution
            // 
            lblResolution.AutoSize = true;
            lblResolution.Location = new Point(12, 42);
            lblResolution.Name = "lblResolution";
            lblResolution.Size = new Size(76, 15);
            lblResolution.TabIndex = 0;
            lblResolution.Text = "lblResolution";
            // 
            // timerRecording
            // 
            timerRecording.Interval = 20;
            timerRecording.Tick += timerRecording_Tick;
            // 
            // timerPlayBack
            // 
            timerPlayBack.Interval = 20;
            timerPlayBack.Tick += timerPlayBack_Tick;
            // 
            // lblMaus
            // 
            lblMaus.AutoSize = true;
            lblMaus.Location = new Point(12, 9);
            lblMaus.Name = "lblMaus";
            lblMaus.Size = new Size(49, 15);
            lblMaus.TabIndex = 1;
            lblMaus.Text = "lblMaus";
            // 
            // lblMausPosition
            // 
            lblMausPosition.AutoSize = true;
            lblMausPosition.Location = new Point(12, 80);
            lblMausPosition.Name = "lblMausPosition";
            lblMausPosition.Size = new Size(92, 15);
            lblMausPosition.TabIndex = 2;
            lblMausPosition.Text = "lblMausPosition";
            // 
            // FlugbahnDlg
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 600);
            Controls.Add(lblMausPosition);
            Controls.Add(lblMaus);
            Controls.Add(lblResolution);
            DoubleBuffered = true;
            FormBorderStyle = FormBorderStyle.SizableToolWindow;
            Name = "FlugbahnDlg";
            Text = "FlugbahnDlg";
            Load += FlugbahnDlg_Load;
            ResizeEnd += FlugbahnDlg_ResizeEnd;
            Paint += FlugbahnDlg_Paint;
            KeyDown += FlugbahnDlg_KeyDown;
            MouseEnter += FlugbahnDlg_MouseEnter;
            MouseMove += FlugbahnDlg_MouseMove;
            Resize += FlugbahnDlg_Resize;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label lblResolution;
        private System.Windows.Forms.Timer timerRecording;
        private System.Windows.Forms.Timer timerPlayBack;
        private Label lblMaus;
        private Label lblMausPosition;
    }
}