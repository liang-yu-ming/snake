using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;

namespace SnakeServer {
	class GameLoop {
		public bool Running { get; private set; }
		
		private Game _game;

		public void Load(Game game) {
			_game = game;
		}

		public async void Start() {
			if (_game == null)
				throw new ArgumentException("Game not loaded!");

			Running = true;

			while (Running) {
				_game.Update();
				await Task.Delay(200);
			}
		}

		public void Stop() {
			Running = false;
			_game?.Stop();
		}

		public List<(string, string)> GetPlayerList() {
			return _game.GetPlayerList();
		}

		public void Draw(Graphics gfx) {
			_game.Draw(gfx);
		}
	}
}
