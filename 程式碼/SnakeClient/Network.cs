using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace SnakeClient {
    class Network {
        public string LocalIP { get; private set; }
        public bool IsConnectSuceed { get; private set; } = true;
        public bool Connected { get; private set; } = true;

        private readonly Socket socket;
        private readonly string serverIP;
        private string ReceiveMsg;

        public Network(string ip) {
            LocalIP = Dns.GetHostEntry(Dns.GetHostName()).AddressList.ToList().Where(p => p.AddressFamily == AddressFamily.InterNetwork).Last().ToString();
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            serverIP = ip;
            new Thread(Accept) {
                IsBackground = true
            }.Start();
        }

        private void Accept() {
			try {
                socket.Connect(new IPEndPoint(IPAddress.Parse(serverIP), 8888));
            }
            catch {
                Console.WriteLine("failed");
                socket.Close();
                IsConnectSuceed = false;
            }
            try {
                byte[] lenBytes = new byte[4];
                while (true) {
                    socket.Receive(lenBytes);
                    int len = Convert.ToInt32(Encoding.UTF8.GetString(lenBytes));
                    //Console.WriteLine(len);
                    byte[] bytes = new byte[len];
                    socket.Receive(bytes);
                    ReceiveMsg = Encoding.UTF8.GetString(bytes, 0, len);
                }
            }
            catch {
                Console.WriteLine("closed");
                socket.Close();
                Connected = false;
            }
        }

        public void Send(string msg) {
            if (msg == null) {
                throw new NullReferenceException("message不可為Null");
            }
            else {
                try {
                    if (socket != null && socket.Connected)
                        socket.Send(Encoding.UTF8.GetBytes(msg.Length.ToString().PadLeft(4, '0') + msg));
                }
                catch {
                    Console.WriteLine("closed");
                    socket.Close();
                    Connected = false;
                }
            }
        }

        public string GetMsg() {
            string temp = ReceiveMsg;
            ReceiveMsg = "";
            return temp;
        }

        public void Close() {
            socket.Close();
        }
    }
}
