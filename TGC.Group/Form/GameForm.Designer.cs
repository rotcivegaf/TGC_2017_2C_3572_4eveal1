namespace TGC.Group.Form
{
    partial class GameForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GameForm));
            this.panel3D = new System.Windows.Forms.Panel();
            this.button_mode = new System.Windows.Forms.Button();
            this.button_quit = new System.Windows.Forms.Button();
            this.button_play = new System.Windows.Forms.Button();
            this.inventarioForm = new System.Windows.Forms.Panel();
            this.panel3D.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel3D
            // 
            this.panel3D.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.panel3D.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("panel3D.BackgroundImage")));
            this.panel3D.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.panel3D.Controls.Add(this.inventarioForm);
            this.panel3D.Controls.Add(this.button_mode);
            this.panel3D.Controls.Add(this.button_quit);
            this.panel3D.Controls.Add(this.button_play);
            this.panel3D.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3D.Location = new System.Drawing.Point(0, 0);
            this.panel3D.Name = "panel3D";
            this.panel3D.Size = new System.Drawing.Size(784, 561);
            this.panel3D.TabIndex = 0;
            this.panel3D.Paint += new System.Windows.Forms.PaintEventHandler(this.panel3D_Paint);
            // 
            // button_mode
            // 
            this.button_mode.BackColor = System.Drawing.SystemColors.Control;
            this.button_mode.Location = new System.Drawing.Point(12, 468);
            this.button_mode.Name = "button_mode";
            this.button_mode.Size = new System.Drawing.Size(137, 23);
            this.button_mode.TabIndex = 4;
            this.button_mode.Text = "Mode";
            this.button_mode.UseVisualStyleBackColor = false;
            // 
            // button_quit
            // 
            this.button_quit.BackColor = System.Drawing.SystemColors.Control;
            this.button_quit.Location = new System.Drawing.Point(12, 526);
            this.button_quit.Name = "button_quit";
            this.button_quit.Size = new System.Drawing.Size(137, 23);
            this.button_quit.TabIndex = 3;
            this.button_quit.Text = "Quit";
            this.button_quit.UseVisualStyleBackColor = false;
            this.button_quit.Click += new System.EventHandler(this.button_quit_Click);
            // 
            // button_play
            // 
            this.button_play.BackColor = System.Drawing.SystemColors.Control;
            this.button_play.Location = new System.Drawing.Point(12, 497);
            this.button_play.Name = "button_play";
            this.button_play.Size = new System.Drawing.Size(137, 23);
            this.button_play.TabIndex = 2;
            this.button_play.Text = "Play";
            this.button_play.UseVisualStyleBackColor = false;
            this.button_play.Click += new System.EventHandler(this.button_play_Click);
            // 
            // inventarioForm
            // 
            this.inventarioForm.Location = new System.Drawing.Point(159, 144);
            this.inventarioForm.Name = "inventarioForm";
            this.inventarioForm.Size = new System.Drawing.Size(483, 270);
            this.inventarioForm.TabIndex = 5;
            this.inventarioForm.Paint += new System.Windows.Forms.PaintEventHandler(this.inventarioForm_Paint);
            // 
            // GameForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 561);
            this.Controls.Add(this.panel3D);
            this.Name = "GameForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Form";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.GameForm_FormClosing);
            this.Load += new System.EventHandler(this.GameForm_Load);
            this.panel3D.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel3D;
        public System.Windows.Forms.Button button_play;
        public System.Windows.Forms.Button button_quit;
        public System.Windows.Forms.Button button_mode;
        public System.Windows.Forms.Panel inventarioForm;
    }
}

