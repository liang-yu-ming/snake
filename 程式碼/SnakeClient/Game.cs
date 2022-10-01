using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text.Json;
using System.Threading;
using System.Windows.Input;

namespace SnakeClient {
	class Game {
		private readonly Network network;
		private List<Player> players = new List<Player>();
		private Tuple<int, int> point = new Tuple<int, int>(-1, -1);
		private readonly ClientPacket clientPacket = new ClientPacket();
		private int isNameSent = 0;
		public bool IsDead { get; private set; }

		public Game(string ip, string name) {
			network = new Network(ip);
			clientPacket.IP = network.LocalIP;
			clientPacket.Name = name;
			new Thread(Recieve) {
				IsBackground = true
			}.Start();
		}

		public void Update() {
			if (network.Connected) {
				int pressKey = -1;
				if ((Keyboard.GetKeyStates(Key.Up) & KeyStates.Down) > 0)
					pressKey = 0;
				else if ((Keyboard.GetKeyStates(Key.Down) & KeyStates.Down) > 0)
					pressKey = 1;
				else if ((Keyboard.GetKeyStates(Key.Left) & KeyStates.Down) > 0)
					pressKey = 2;
				else if ((Keyboard.GetKeyStates(Key.Right) & KeyStates.Down) > 0)
					pressKey = 3;
				if (pressKey != -1) {
					clientPacket.Presskey = pressKey;
					network.Send(JsonSerializer.Serialize(clientPacket));
				}
				if (isNameSent < 3) {
					isNameSent += 1;
					network.Send(JsonSerializer.Serialize(clientPacket));
				}
			}
		}

		public void Stop() {
			network.Close();
		}

		private void Recieve() {
			while (network.Connected) {
				string msg = network?.GetMsg();
				if (msg != null && msg != "") {
					try {
						Console.WriteLine(msg);
						ServerPacket serverPacket = JsonSerializer.Deserialize<ServerPacket>(msg);
						players = serverPacket.Players;
						point = new Tuple<int, int>(serverPacket.Pointx, serverPacket.Pointy);
						IsDead = serverPacket.IsDead;
					}
					catch {
						Console.WriteLine("!!");
					}
				}
				Thread.Sleep(50);
			}
		}

		public int GetNetworkStatus() {
			if (network.IsConnectSuceed) {
				if (network.Connected)
					return 2;
				else
					return 1;
			}
			else {
				return 0;
			}
		}

		public string GetColor() {
			foreach (Player player in players) {
				if (player.Ip == network.LocalIP)
					return player.ColorName;
			}
			return "Waiting...";
		}

		public void Draw(Graphics gfx) {
			gfx.FillRectangle(new SolidBrush(Color.Black), new Rectangle(0, 0, 600, 600));
			if (point.Item1 != -1 && point.Item2 != -1)
				gfx.FillRectangle(new SolidBrush(Color.White), new Rectangle(20 * point.Item2, 20 * point.Item1, 20, 20));
			foreach (Player player in players)
				player.Draw(gfx);
		}
	}
}
