﻿namespace Game
{
    partial class GameDlg
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GameDlg));
            timerPlayBack = new System.Windows.Forms.Timer(components);
            btnStart = new Button();
            lblGameOver = new Label();
            lblTreffen = new Label();
            lblZeitUebrig = new Label();
            timerZeitUebrig = new System.Windows.Forms.Timer(components);
            lblLandingZone = new Label();
            timerCreate = new System.Windows.Forms.Timer(components);
            timerOneShot = new System.Windows.Forms.Timer(components);
            SuspendLayout();
            // 
            // timerPlayBack
            // 
            timerPlayBack.Interval = 10;
            timerPlayBack.Tick += timerPlayBack_Tick;
            // 
            // btnStart
            // 
            btnStart.BackColor = Color.FromArgb(128, 255, 128);
            btnStart.Font = new Font("Segoe UI", 14.25F, FontStyle.Bold, GraphicsUnit.Point);
            btnStart.Location = new Point(346, 484);
            btnStart.Name = "btnStart";
            btnStart.Size = new Size(125, 50);
            btnStart.TabIndex = 0;
            btnStart.Text = "Start";
            btnStart.UseVisualStyleBackColor = false;
            btnStart.Click += btnStart_Click;
            // 
            // lblGameOver
            // 
            lblGameOver.AutoSize = true;
            lblGameOver.Location = new Point(712, 9);
            lblGameOver.Name = "lblGameOver";
            lblGameOver.Size = new Size(76, 15);
            lblGameOver.TabIndex = 2;
            lblGameOver.Text = "lblGameOver";
            // 
            // lblTreffen
            // 
            lblTreffen.AutoSize = true;
            lblTreffen.Location = new Point(12, 7);
            lblTreffen.Name = "lblTreffen";
            lblTreffen.Size = new Size(56, 15);
            lblTreffen.TabIndex = 5;
            lblTreffen.Text = "lblTreffen";
            // 
            // lblZeitUebrig
            // 
            lblZeitUebrig.AutoSize = true;
            lblZeitUebrig.Location = new Point(12, 22);
            lblZeitUebrig.Name = "lblZeitUebrig";
            lblZeitUebrig.Size = new Size(75, 15);
            lblZeitUebrig.TabIndex = 7;
            lblZeitUebrig.Text = "lblZeitUebrig";
            // 
            // timerZeitUebrig
            // 
            timerZeitUebrig.Interval = 1000;
            timerZeitUebrig.Tick += timerZeitUebrig_Tick;
            // 
            // lblLandingZone
            // 
            lblLandingZone.AutoSize = true;
            lblLandingZone.Location = new Point(12, 37);
            lblLandingZone.Name = "lblLandingZone";
            lblLandingZone.Size = new Size(90, 15);
            lblLandingZone.TabIndex = 8;
            lblLandingZone.Text = "lblLandingZone";
            // 
            // timerCreate
            // 
            timerCreate.Interval = 5000;
            timerCreate.Tick += timerCreate_Tick;
            // 
            // timerOneShot
            // 
            timerOneShot.Interval = 2000;
            timerOneShot.Tick += timerOneShot_Tick;
            // 
            // GameDlg
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.White;
            ClientSize = new Size(800, 600);
            Controls.Add(lblLandingZone);
            Controls.Add(lblZeitUebrig);
            Controls.Add(lblTreffen);
            Controls.Add(lblGameOver);
            Controls.Add(btnStart);
            DoubleBuffered = true;
            FormBorderStyle = FormBorderStyle.SizableToolWindow;
            Icon = (Icon)resources.GetObject("$this.Icon");
            KeyPreview = true;
            Name = "GameDlg";
            Text = "No Food 4 U : Lehrlingswettbewerb Züri Oberland 2023";
            FormClosing += GameDlg_FormClosing;
            Load += GameDlg_Load;
            ResizeEnd += GameDlg_ResizeEnd;
            Paint += GameDlg_Paint;
            KeyDown += GameDlg_KeyDown;
            MouseDown += GameDlg_MouseDown;
            Resize += GameDlg_Resize;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Timer timerPlayBack;
        private Button btnStart;
        private Label lblGameOver;
        private Label lblTreffen;
        private Label lblZeitUebrig;
        private System.Windows.Forms.Timer timerZeitUebrig;
        private Label lblLandingZone;
        private System.Windows.Forms.Timer timerCreate;
        private System.Windows.Forms.Timer timerOneShot;
    }
}