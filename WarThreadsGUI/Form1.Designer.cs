﻿namespace WarThreadsGUI
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
            SuspendLayout();
            // 
            // panelGame
            // 
            panelGame.Dock = DockStyle.Fill;
            panelGame.Location = new Point(0, 0);
            panelGame.Name = "panelGame";
            panelGame.Size = new Size(939, 583);
            panelGame.TabIndex = 2;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(939, 583);
            Controls.Add(panelGame);
            Name = "Form1";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "War Threads";
            ResumeLayout(false);
        }

        #endregion

        private Panel panelGame;
    }
}