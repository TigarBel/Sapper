namespace Sapper
{
    partial class MainFormSapper
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainFormSapper));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.GameMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.easyGame = new System.Windows.Forms.ToolStripMenuItem();
            this.mediunGame = new System.Windows.Forms.ToolStripMenuItem();
            this.hardGame = new System.Windows.Forms.ToolStripMenuItem();
            this.labelOfTimer = new System.Windows.Forms.Label();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.GameMenu});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(184, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // GameMenu
            // 
            this.GameMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.easyGame,
            this.mediunGame,
            this.hardGame});
            this.GameMenu.Name = "GameMenu";
            this.GameMenu.Size = new System.Drawing.Size(50, 20);
            this.GameMenu.Text = "Game";
            // 
            // easyGame
            // 
            this.easyGame.Name = "easyGame";
            this.easyGame.Size = new System.Drawing.Size(153, 22);
            this.easyGame.Text = "Easy Game";
            this.easyGame.Click += new System.EventHandler(this.easyGame_Click);
            // 
            // mediunGame
            // 
            this.mediunGame.Name = "mediunGame";
            this.mediunGame.Size = new System.Drawing.Size(153, 22);
            this.mediunGame.Text = "Medium Game";
            this.mediunGame.Click += new System.EventHandler(this.mediunGame_Click);
            // 
            // hardGame
            // 
            this.hardGame.Name = "hardGame";
            this.hardGame.Size = new System.Drawing.Size(153, 22);
            this.hardGame.Text = "Hard Game";
            this.hardGame.Click += new System.EventHandler(this.hardGame_Click);
            // 
            // labelOfTimer
            // 
            this.labelOfTimer.AutoSize = true;
            this.labelOfTimer.BackColor = System.Drawing.SystemColors.Control;
            this.labelOfTimer.Location = new System.Drawing.Point(155, 5);
            this.labelOfTimer.Name = "labelOfTimer";
            this.labelOfTimer.Size = new System.Drawing.Size(13, 13);
            this.labelOfTimer.TabIndex = 1;
            this.labelOfTimer.Text = "0";
            // 
            // timer1
            // 
            this.timer1.Interval = 1000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // MainFormSapper
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(184, 214);
            this.Controls.Add(this.labelOfTimer);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximumSize = new System.Drawing.Size(200, 253);
            this.MinimumSize = new System.Drawing.Size(200, 253);
            this.Name = "MainFormSapper";
            this.Text = "Sapper";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem GameMenu;
        private System.Windows.Forms.ToolStripMenuItem easyGame;
        private System.Windows.Forms.ToolStripMenuItem mediunGame;
        private System.Windows.Forms.ToolStripMenuItem hardGame;
        private System.Windows.Forms.Label labelOfTimer;
        private System.Windows.Forms.Timer timer1;
    }
}

