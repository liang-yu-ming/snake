using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text.Json.Serialization;

namespace SnakeServer {
	class Player {
		public string Ip { get; set; }
		public string Name { get; set; }
		public List<Tuple<int, int>> Snake { get; set; }
		public string ColorName { get; set; }
		[JsonIgnore]
		public int Direction { get; private set; } = 0;

		private Color color = new Color();

		public Player() {
			Name = "";
			Ip = "";
			Snake = new List<Tuple<int, int>> {
				new Tuple<int, int>(3, 3),
				new Tuple<int, int>(4, 3),
				new Tuple<int, int>(5, 3)
			};
			ColorName = "";
		}

		public void Move(int dir) {
			// 0:up 1:down 2:left 3:right
			if (dir == 0 && Direction == 1)
				dir = Direction;
			else if (dir == 1 && Direction == 0)
				dir = Direction;
			else if (dir == 2 && Direction == 3)
				dir = Direction;
			else if (dir == 3 && Direction == 2)
				dir = Direction;
			Direction = dir;
			switch (dir) {
				case 0:
					Snake.Insert(0, NewPoint(Snake[0].Item1 - 1, Snake[0].Item2));
					break;
				case 1:
					Snake.Insert(0, NewPoint(Snake[0].Item1 + 1, Snake[0].Item2));
					break;
				case 2:
					Snake.Insert(0, NewPoint(Snake[0].Item1, Snake[0].Item2 - 1));
					break;
				case 3:
					Snake.Insert(0, NewPoint(Snake[0].Item1, Snake[0].Item2 + 1));
					break;
			}
			Snake.RemoveAt(Snake.Count - 1);
		}

		public void Eat() {
			int dir = Cal(Snake.Count - 2, Snake.Count - 1);
			switch (dir) {
				case 0:
					Snake.Add(NewPoint(Snake[Snake.Count - 1].Item1 - 1, Snake[Snake.Count - 1].Item2));
					break;
				case 1:
					Snake.Add(NewPoint(Snake[Snake.Count - 1].Item1 + 1, Snake[Snake.Count - 1].Item2));
					break;
				case 2:
					Snake.Add(NewPoint(Snake[Snake.Count - 1].Item1, Snake[Snake.Count - 1].Item2 - 1));
					break;
				case 3:
					Snake.Add(NewPoint(Snake[Snake.Count - 1].Item1, Snake[Snake.Count - 1].Item2 + 1));
					break;
			}
		}

		private int Cal(int a, int b) {
			var (dx, dy) = (Snake[b].Item1 - Snake[a].Item1, Snake[b].Item2 - Snake[a].Item2);
			if (dx == -1 && dy == 0)
				return 0;
			if (dx == 1 && dy == 0)
				return 1;
			if (dx == 0 && dy == -1)
				return 2;
			if (dx == 0 && dy == 1)
				return 3;
			return 0;
		}

		private Tuple<int, int> NewPoint(int a, int b) {
			if (a < 0)
				a = 29;
			if (a > 29)
				a = 0;
			if (b < 0)
				b = 29;
			if (b > 29)
				b = 0;
			return new Tuple<int, int>(a, b);
		}

		public void Draw(Graphics gfx) {
			color = Color.FromName(ColorName);
			for (int i = 1;i < Snake.Count;i += 1) {
				//gfx.FillRectangle(new SolidBrush(color), new Rectangle(20 * Snake[i].Item2, 20 * Snake[i].Item1, 20, 20));
				gfx.FillPie(new SolidBrush(color), 20 * Snake[i].Item2, 20 * Snake[i].Item1, 20, 20, 0, 360);
			}
			gfx.FillPie(new SolidBrush(color), 20 * Snake[0].Item2, 20 * Snake[0].Item1, 20, 20, 0, 360);
		}
	}
}
