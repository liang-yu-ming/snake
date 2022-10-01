
namespace SnakeServer {
    partial class View {
        /// <summary>
        /// 設計工具所需的變數。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清除任何使用中的資源。
        /// </summary>
        /// <param name="disposing">如果應該處置受控資源則為 true，否則為 false。</param>
        protected override void Dispose(bool disposing)
        {
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
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.playerListView = new System.Windows.Forms.ListView();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.IPText = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("新細明體", 20F);
            this.label1.Location = new System.Drawing.Point(625, 290);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(120, 27);
            this.label1.TabIndex = 0;
            this.label1.Text = "玩家清單";
            // 
            // playerListView
            // 
            this.playerListView.AutoArrange = false;
            this.playerListView.Font = new System.Drawing.Font("新細明體", 14F);
            this.playerListView.HideSelection = false;
            this.playerListView.Location = new System.Drawing.Point(630, 320);
            this.playerListView.MultiSelect = false;
            this.playerListView.Name = "playerListView";
            this.playerListView.Scrollable = false;
            this.playerListView.ShowGroups = false;
            this.playerListView.Size = new System.Drawing.Size(300, 200);
            this.playerListView.TabIndex = 0;
            this.playerListView.TabStop = false;
            this.playerListView.UseCompatibleStateImageBehavior = false;
            this.playerListView.View = System.Windows.Forms.View.Details;
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.Color.OrangeRed;
            this.pictureBox1.Location = new System.Drawing.Point(600, 0);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(10, 640);
            this.pictureBox1.TabIndex = 2;
            this.pictureBox1.TabStop = false;
            // 
            // IPText
            // 
            this.IPText.Font = new System.Drawing.Font("新細明體", 20F);
            this.IPText.Location = new System.Drawing.Point(620, 80);
            this.IPText.Name = "IPText";
            this.IPText.Size = new System.Drawing.Size(310, 30);
            this.IPText.TabIndex = 3;
            this.IPText.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // View
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(944, 601);
            this.Controls.Add(this.IPText);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.playerListView);
            this.Controls.Add(this.label1);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(960, 640);
            this.MinimumSize = new System.Drawing.Size(960, 640);
            this.Name = "View";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "SnakeServer";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ListView playerListView;
        private System.Windows.Forms.PictureBox pictureBox1;
		private System.Windows.Forms.Label IPText;
	}
}

