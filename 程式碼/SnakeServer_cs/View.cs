using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace SnakeServer {
    public partial class View : Form {
		readonly Timer graphicsTimer, updateTimer;
		GameLoop gameLoop;
		List<(string, string)> playerList = new List<(string, string)>();

		public View() {
			InitializeComponent();
			InitListView();

			this.Load += View_Load;
			this.FormClosed += View_Closed;

			Paint += View_Paint;

			graphicsTimer = new Timer {
				Enabled = true,
				Interval = 200
			};
			graphicsTimer.Tick += GraphicsTimer_Tick;

			updateTimer = new Timer {
				Enabled = true,
				Interval = 1000
			};
			updateTimer.Tick += UpdateTimer_Tick;
		}

		private void View_Load(object sender, EventArgs e) {
			Game game = new Game();
			gameLoop = new GameLoop();
			gameLoop.Load(game);
			gameLoop.Start();
			IPText.Text = "Server IP: " + game.GetLocalIP();
		}

		private void View_Closed(object sender, EventArgs e) {
			gameLoop.Stop();
		}

		private void View_Paint(object sender, PaintEventArgs e) {
			// Draw game graphics
			gameLoop.Draw(e.Graphics);
			this.ActiveControl = null;
		}

		private void GraphicsTimer_Tick(object sender, EventArgs e) {
			// Refresh graphics
			Invalidate();
		}

		private void UpdateTimer_Tick(object sender, EventArgs e) {
			playerListView.BeginUpdate();
			playerListView.Columns[0].Width = 150;
			playerListView.Columns[1].Width = 150;
			List<(string, string)> tempList = gameLoop.GetPlayerList();
			if (!playerList.Equals(tempList)) {
				playerList = tempList;
				playerListView.Items.Clear();
				foreach ((string s, string colorName) in playerList) {
					var item = new ListViewItem(s);
					item.SubItems.Add(colorName);
					playerListView.Items.Add(item);
				}
			}
			playerListView.EndUpdate();
		}

		private void InitListView() {
			playerListView.Columns.Add("Name", 150);
			playerListView.Columns.Add("Color", 150);
		}
	}
}
