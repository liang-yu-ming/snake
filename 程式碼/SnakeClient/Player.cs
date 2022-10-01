using System;
using System.Collections.Generic;
using System.Drawing;

namespace SnakeClient {
    class Player {
        public string Ip { get; set; }
        public string Name { get; set; }
        public List<Tuple<int, int>> Snake { get; set; }
        public string ColorName { get; set; }
        private Color color = new Color();

        public Player() {
            Name = "";
            Ip = "";
            Snake = new List<Tuple<int, int>>();
            ColorName = "";
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
