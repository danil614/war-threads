namespace WarThreadsGUI
{
    partial class Form1
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
            panelGame = new Panel();
            pbCannon = new PictureBox();
            panelGame.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pbCannon).BeginInit();
            SuspendLayout();
            // 
            // panelGame
            // 
            panelGame.Controls.Add(pbCannon);
            panelGame.Dock = DockStyle.Fill;
            panelGame.Location = new Point(0, 0);
            panelGame.Name = "panelGame";
            panelGame.Size = new Size(804, 466);
            panelGame.TabIndex = 0;
            // 
            // pbCannon
            // 
            pbCannon.Image = Properties.Resources.cannon;
            pbCannon.InitialImage = Properties.Resources.cannon;
            pbCannon.Location = new Point(385, 426);
            pbCannon.Name = "pbCannon";
            pbCannon.Size = new Size(31, 37);
            pbCannon.SizeMode = PictureBoxSizeMode.Zoom;
            pbCannon.TabIndex = 0;
            pbCannon.TabStop = false;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(804, 466);
            Controls.Add(panelGame);
            Name = "Form1";
            Text = "Form1";
            panelGame.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)pbCannon).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private Panel panelGame;
        private PictureBox pbCannon;
    }
}