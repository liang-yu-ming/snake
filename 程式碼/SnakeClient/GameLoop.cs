using System;
using System.Drawing;
using System.Threading.Tasks;

namespace SnakeClient {
	class GameLoop {
		public bool Running { get; private set; } = false;
		
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
				await Task.Delay(100);
			}
		}

		public void Stop() {
			Running = false;
			_game?.Stop();
			_game = null;
		}

		public void Draw(Graphics gfx) {
			if (_game != null)
				_game.Draw(gfx);
		}

		public int GetNetworkStatus() {
			if (_game == null)
				return -1;
			else
				return _game.GetNetworkStatus();
		}

		public bool IsDead() {
			if (_game == null)
				return false;
			else
				return _game.IsDead;
		}

		public string GetColor() {
			return _game.GetColor();
		}
	}
}
