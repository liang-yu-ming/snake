using System.Collections.Generic;

namespace SnakeClient {
    class ServerPacket {
        public string IP { get; set; }
        public bool IsDead { get; set; }
        public List<Player> Players { get; set; } = new List<Player>();

        public int Pointx { get; set; }
        public int Pointy { get; set; }
    }
}
