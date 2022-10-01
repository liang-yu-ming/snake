using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Windows.Forms;

namespace SnakeClient {
	public partial class View : Form {
		readonly Timer graphicsTimer, updateTimer;
		GameLoop gameLoop;

		public View() {
			InitializeComponent();

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
			gameLoop = new GameLoop();
			inputIP.Text = Dns.GetHostEntry(Dns.GetHostName()).AddressList.ToList().Where(p => p.AddressFamily == AddressFamily.InterNetwork).Last().ToString();
		}

		private void View_Closed(object sender, EventArgs e) {
			gameLoop.Stop();
		}

		private void View_Paint(object sender, PaintEventArgs e) {
			// Draw game graphics
			gameLoop.Draw(e.Graphics);
		}

		private void GraphicsTimer_Tick(object sender, EventArgs e) {
			// Refresh graphics
			Invalidate();
		}

		private void UpdateTimer_Tick(object sender, EventArgs e) {
			int temp;
			if ((temp = gameLoop.GetNetworkStatus()) != -1) {
				switch (temp) {
					case 0:
						infoText.Text = "Wrong IP!";
						GameLoop_Stop();
						break;
					case 1:
						infoText.Text = "Server Closed";
						GameLoop_Stop();
						break;
					case 2:
						infoText.Text = gameLoop.GetColor();
						break;
				}
			}
			if (gameLoop.IsDead()) {
				infoText.Text = "You Lose!!";
				GameLoop_Stop();
			}
		}

		private void GameLoop_Stop() {
			gameLoop.Stop();
			inputIP.Enabled = true;
			inputName.Enabled = true;
			startBtn.Enabled = true;
			stopBtn.Enabled = false;
		}

		private void StartBtn_Click(object sender, EventArgs e) {
			infoText.Text = "";
			if (inputIP.Text != "" && inputName.Text != "") {
				Game game = new Game(inputIP.Text, inputName.Text);
				gameLoop.Load(game);
				gameLoop.Start();
				errorText.Text = "";
				inputIP.Enabled = false;
				inputName.Enabled = false;
				startBtn.Enabled = false;
				stopBtn.Enabled = true;
			}
			else if (inputIP.Text == "") {
				errorText.Text = "IP cannot be empty!!";
			}
			else if (inputName.Text == "") {
				errorText.Text = "Name cannot be empty!!";
			}
		}

		private void StopBtn_Click(object sender, EventArgs e) {
			GameLoop_Stop();
		}
	}
}
