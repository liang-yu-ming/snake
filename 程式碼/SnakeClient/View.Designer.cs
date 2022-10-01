
namespace SnakeClient {
    partial class View {
        /// <summary>
        /// 設計工具所需的變數。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清除任何使用中的資源。
        /// </summary>
        /// <param name="disposing">如果應該處置受控資源則為 true，否則為 false。</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 設計工具產生的程式碼

        /// <summary>
        /// 此為設計工具支援所需的方法 - 請勿使用程式碼編輯器修改
        /// 這個方法的內容。
        /// </summary>
        private void InitializeComponent() {
            this.startBtn = new System.Windows.Forms.Button();
            this.inputName = new System.Windows.Forms.TextBox();
            this.errorText = new System.Windows.Forms.Label();
            this.infoText = new System.Windows.Forms.Label();
            this.stopBtn = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.nameText = new System.Windows.Forms.Label();
            this.inputIP = new System.Windows.Forms.TextBox();
            this.IPText = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // startBtn
            // 
            this.startBtn.Font = new System.Drawing.Font("Arial", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.startBtn.Location = new System.Drawing.Point(650, 400);
            this.startBtn.Name = "startBtn";
            this.startBtn.Size = new System.Drawing.Size(100, 60);
            this.startBtn.TabIndex = 0;
            this.startBtn.TabStop = false;
            this.startBtn.Text = "Start";
            this.startBtn.UseVisualStyleBackColor = true;
            this.startBtn.Click += new System.EventHandler(this.StartBtn_Click);
            // 
            // inputName
            // 
            this.inputName.Font = new System.Drawing.Font("新細明體", 16F);
            this.inputName.Location = new System.Drawing.Point(730, 280);
            this.inputName.MaxLength = 20;
            this.inputName.Name = "inputName";
            this.inputName.Size = new System.Drawing.Size(150, 33);
            this.inputName.TabIndex = 1;
            // 
            // errorText
            // 
            this.errorText.AutoSize = true;
            this.errorText.Font = new System.Drawing.Font("新細明體", 14F);
            this.errorText.ForeColor = System.Drawing.Color.Red;
            this.errorText.Location = new System.Drawing.Point(730, 316);
            this.errorText.Name = "errorText";
            this.errorText.Size = new System.Drawing.Size(0, 19);
            this.errorText.TabIndex = 2;
            // 
            // infoText
            // 
            this.infoText.Font = new System.Drawing.Font("新細明體", 20F);
            this.infoText.Location = new System.Drawing.Point(620, 80);
            this.infoText.Name = "infoText";
            this.infoText.Size = new System.Drawing.Size(310, 30);
            this.infoText.TabIndex = 3;
            this.infoText.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // stopBtn
            // 
            this.stopBtn.Enabled = false;
            this.stopBtn.Font = new System.Drawing.Font("Arial", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.stopBtn.Location = new System.Drawing.Point(800, 400);
            this.stopBtn.Name = "stopBtn";
            this.stopBtn.Size = new System.Drawing.Size(100, 60);
            this.stopBtn.TabIndex = 0;
            this.stopBtn.TabStop = false;
            this.stopBtn.Text = "Stop";
            this.stopBtn.UseVisualStyleBackColor = true;
            this.stopBtn.Click += new System.EventHandler(this.StopBtn_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.Color.OrangeRed;
            this.pictureBox1.Location = new System.Drawing.Point(600, 0);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(10, 640);
            this.pictureBox1.TabIndex = 5;
            this.pictureBox1.TabStop = false;
            // 
            // nameText
            // 
            this.nameText.AutoSize = true;
            this.nameText.Font = new System.Drawing.Font("Arial", 16F);
            this.nameText.Location = new System.Drawing.Point(654, 281);
            this.nameText.Name = "nameText";
            this.nameText.Size = new System.Drawing.Size(76, 25);
            this.nameText.TabIndex = 0;
            this.nameText.Text = "Name:";
            this.nameText.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // inputIP
            // 
            this.inputIP.Font = new System.Drawing.Font("新細明體", 16F);
            this.inputIP.Location = new System.Drawing.Point(730, 230);
            this.inputIP.MaxLength = 20;
            this.inputIP.Name = "inputIP";
            this.inputIP.Size = new System.Drawing.Size(150, 33);
            this.inputIP.TabIndex = 0;
            // 
            // IPText
            // 
            this.IPText.AutoSize = true;
            this.IPText.Font = new System.Drawing.Font("Arial", 16F);
            this.IPText.Location = new System.Drawing.Point(621, 231);
            this.IPText.Name = "IPText";
            this.IPText.Size = new System.Drawing.Size(109, 25);
            this.IPText.TabIndex = 0;
            this.IPText.Text = "Server IP:";
            this.IPText.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // View
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(944, 601);
            this.Controls.Add(this.IPText);
            this.Controls.Add(this.inputIP);
            this.Controls.Add(this.nameText);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.stopBtn);
            this.Controls.Add(this.infoText);
            this.Controls.Add(this.errorText);
            this.Controls.Add(this.inputName);
            this.Controls.Add(this.startBtn);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(960, 640);
            this.MinimumSize = new System.Drawing.Size(960, 640);
            this.Name = "View";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "SnakeClient";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button startBtn;
        private System.Windows.Forms.TextBox inputName;
        private System.Windows.Forms.Label errorText;
        private System.Windows.Forms.Label infoText;
		private System.Windows.Forms.Button stopBtn;
		private System.Windows.Forms.PictureBox pictureBox1;
		private System.Windows.Forms.Label nameText;
		private System.Windows.Forms.TextBox inputIP;
		private System.Windows.Forms.Label IPText;
	}
}

