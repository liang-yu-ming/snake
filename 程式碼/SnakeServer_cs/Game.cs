using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text.Json;
using System.Threading;

namespace SnakeServer {
	class Game {
		private bool running = true;
		private readonly Network network = new Network();
		private readonly List<Player> players = new List<Player>();
		private readonly Dictionary<string, int> playerActions = new Dictionary<string, int>();
		private Tuple<int, int> point = new Tuple<int, int>(15, 15);
		private readonly Random random = new Random();
		private readonly Queue<string> colorNames = new Queue<string>();
		private readonly List<string> deadPlayers = new List<string>();

		public Game() {
			colorNames.Enqueue("Red");
			colorNames.Enqueue("Blue");
			colorNames.Enqueue("Green");
			colorNames.Enqueue("Yellow");
			colorNames.Enqueue("Purple");
			new Thread(RecvAction).Start();
			new Thread(SendAll).Start();
			new Thread(ManagePlayer) {
				IsBackground = true
			}.Start();
			new Thread(CheckPointEaten) {
				IsBackground = true
			}.Start();
		}

		public void Update() {
			for (int i = 0;i < players.Count;i += 1) {
				if (playerActions.ContainsKey(players[i].Ip))
					players[i].Move(playerActions[players[i].Ip]);
				else
					players[i].Move(players[i].Direction);
			}
			playerActions.Clear();
			for (int i = 0;i < players.Count;i += 1) {
				for (int j = 0;j < players.Count;j += 1) {
					if (i == j)
						continue;
					if (players[i].Snake.Contains(players[j].Snake[0]))
						deadPlayers.Add(players[j].Ip);
				}
			}
		}

		public void Stop() {
			running = false;
			network.Close();
			players.Clear();
		}

		private void RecvAction() {
			while (running) {
				string msg = network?.GetMsg();
				if (msg != null && msg != "") {
					try {
						ClientPacket clientPacket = JsonSerializer.Deserialize<ClientPacket>(msg);
						Player player = FindPlayer(clientPacket.IP);
						if (player != null)
							player.Name = clientPacket.Name;
						if (clientPacket.Presskey != -1)
							playerActions[clientPacket.IP] = clientPacket.Presskey;
					}
					catch {
						Console.WriteLine("##");
					}
				}
				Thread.Sleep(50);
			}
		}

		private void SendAll() {
			while (running) {
				for (int i = 0;i < players.Count;i += 1) {
					ServerPacket serverPacket = new ServerPacket {
						IP = players[i].Ip,
						IsDead = deadPlayers.Contains(players[i].Ip),
						Players = players,
						Pointx = point.Item1,
						Pointy = point.Item2
					};
					network.Send(players[i].Ip, JsonSerializer.Serialize(serverPacket));
				}
				Thread.Sleep(50);
			}
		}

		private Player FindPlayer(string ip) {
			foreach (Player player in players) {
				if (player.Ip == ip)
					return player;
			}
			return null;
		}

		private void ManagePlayer() {
			while (running) {
				while (network.WaitingQueue.Count > 0) {
					Player player = new Player {
						Ip = network.WaitingQueue.Dequeue(),
						ColorName = colorNames.Dequeue()
					};
					players.Add(player);
				}
				while (network.RemoveQueue.Count > 0) {
					string temp = network.RemoveQueue.Dequeue();
					Player player = FindPlayer(temp);
					colorNames.Enqueue(player.ColorName);
					players.Remove(player);
					deadPlayers.Remove(player.Ip);
				}
				Thread.Sleep(200);
			}
		}

		private void CheckPointEaten() {
			while (running) {
				bool check = false;
				foreach (Player player in players) {
					if (point.Equals(player.Snake[0])) {
						player.Eat();
						check = true;
					}
				}
				if (check) {
					point = new Tuple<int, int>(-1, -1);
					while (true) {
						int x = random.Next(0, 30);
						int y = random.Next(0, 30);
						Tuple<int, int> newPoint = new Tuple<int, int>(x, y);
						bool isSpace = true;
						foreach (Player player in players) {
							if (player.Snake.Contains(newPoint)) {
								isSpace = false;
								break;
							}
						}
						if (isSpace) {
							point = newPoint;
							break;
						}
					}
				}
				Thread.Sleep(50);
			}
		}

		public List<(string, string)> GetPlayerList() {
			List<(string, string)> list = new List<(string, string)>();
			foreach (Player player in players)
				list.Add((player.Name, player.ColorName));
			return list;
		}

		public string GetLocalIP() {
			return network.LocalIP;
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
