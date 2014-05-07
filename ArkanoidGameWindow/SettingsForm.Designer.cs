namespace ArkanoidGameWindow
{
    partial class SettingsForm
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
            this.cbFullscreen = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.rbLowDetails = new System.Windows.Forms.RadioButton();
            this.rbHighDetails = new System.Windows.Forms.RadioButton();
            this.rbVeryHighDetails = new System.Windows.Forms.RadioButton();
            this.btnStartGame = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // cbFullscreen
            // 
            this.cbFullscreen.AutoSize = true;
            this.cbFullscreen.Location = new System.Drawing.Point(180, 19);
            this.cbFullscreen.Name = "cbFullscreen";
            this.cbFullscreen.Size = new System.Drawing.Size(74, 17);
            this.cbFullscreen.TabIndex = 0;
            this.cbFullscreen.Text = "Fullscreen";
            this.cbFullscreen.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.rbVeryHighDetails);
            this.groupBox1.Controls.Add(this.cbFullscreen);
            this.groupBox1.Controls.Add(this.rbHighDetails);
            this.groupBox1.Controls.Add(this.rbLowDetails);
            this.groupBox1.Location = new System.Drawing.Point(12, 13);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(260, 116);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Graphic details";
            // 
            // rbLowDetails
            // 
            this.rbLowDetails.AutoSize = true;
            this.rbLowDetails.Location = new System.Drawing.Point(16, 21);
            this.rbLowDetails.Name = "rbLowDetails";
            this.rbLowDetails.Size = new System.Drawing.Size(45, 17);
            this.rbLowDetails.TabIndex = 0;
            this.rbLowDetails.TabStop = true;
            this.rbLowDetails.Text = "Low";
            this.rbLowDetails.UseVisualStyleBackColor = true;
            // 
            // rbMediumDetails
            // 
            this.rbHighDetails.AutoSize = true;
            this.rbHighDetails.Checked = true;
            this.rbHighDetails.Location = new System.Drawing.Point(16, 53);
            this.rbHighDetails.Name = "rbMediumDetails";
            this.rbHighDetails.Size = new System.Drawing.Size(47, 17);
            this.rbHighDetails.TabIndex = 1;
            this.rbHighDetails.TabStop = true;
            this.rbHighDetails.Text = "High";
            this.rbHighDetails.UseVisualStyleBackColor = true;
            // 
            // rbVeryHighDetails
            // 
            this.rbVeryHighDetails.AutoSize = true;
            this.rbVeryHighDetails.Location = new System.Drawing.Point(16, 83);
            this.rbVeryHighDetails.Name = "rbVeryHighDetails";
            this.rbVeryHighDetails.Size = new System.Drawing.Size(69, 17);
            this.rbVeryHighDetails.TabIndex = 2;
            this.rbVeryHighDetails.TabStop = true;
            this.rbVeryHighDetails.Text = "Very high";
            this.rbVeryHighDetails.UseVisualStyleBackColor = true;
            // 
            // btnStartGame
            // 
            this.btnStartGame.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnStartGame.Location = new System.Drawing.Point(105, 135);
            this.btnStartGame.Name = "btnStartGame";
            this.btnStartGame.Size = new System.Drawing.Size(75, 23);
            this.btnStartGame.TabIndex = 2;
            this.btnStartGame.Text = "Start game";
            this.btnStartGame.UseVisualStyleBackColor = true;
            // 
            // SettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 164);
            this.Controls.Add(this.btnStartGame);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "SettingsForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Arkanoid Settings";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SettingsForm_FormClosing);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.CheckBox cbFullscreen;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton rbVeryHighDetails;
        private System.Windows.Forms.RadioButton rbHighDetails;
        private System.Windows.Forms.RadioButton rbLowDetails;
        private System.Windows.Forms.Button btnStartGame;
    }
}