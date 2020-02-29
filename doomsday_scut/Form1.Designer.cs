namespace doomsday_scut
{
    partial class Form1
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.new_game = new System.Windows.Forms.Button();
            this.help_about = new System.Windows.Forms.Button();
            this.exit = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // new_game
            // 
            this.new_game.Font = new System.Drawing.Font("微软雅黑", 16.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.new_game.Location = new System.Drawing.Point(250, 214);
            this.new_game.Name = "new_game";
            this.new_game.Size = new System.Drawing.Size(271, 72);
            this.new_game.TabIndex = 0;
            this.new_game.Text = "Start New Game";
            this.new_game.UseVisualStyleBackColor = true;
            this.new_game.Click += new System.EventHandler(this.new_game_Click);
            // 
            // help_about
            // 
            this.help_about.Font = new System.Drawing.Font("微软雅黑", 16.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.help_about.Location = new System.Drawing.Point(250, 317);
            this.help_about.Name = "help_about";
            this.help_about.Size = new System.Drawing.Size(271, 72);
            this.help_about.TabIndex = 1;
            this.help_about.Text = "About";
            this.help_about.UseVisualStyleBackColor = true;
            this.help_about.Click += new System.EventHandler(this.help_about_Click);
            // 
            // exit
            // 
            this.exit.Font = new System.Drawing.Font("微软雅黑", 16.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.exit.Location = new System.Drawing.Point(250, 420);
            this.exit.Name = "exit";
            this.exit.Size = new System.Drawing.Size(271, 72);
            this.exit.TabIndex = 2;
            this.exit.Text = "Exit";
            this.exit.UseVisualStyleBackColor = true;
            this.exit.Click += new System.EventHandler(this.exit_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(770, 540);
            this.Controls.Add(this.exit);
            this.Controls.Add(this.help_about);
            this.Controls.Add(this.new_game);
            this.Name = "Form1";
            this.Text = "Doomsday in SCUT v0.1";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button new_game;
        private System.Windows.Forms.Button help_about;
        private System.Windows.Forms.Button exit;
    }
}

