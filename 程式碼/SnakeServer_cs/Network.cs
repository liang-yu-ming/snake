using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace SnakeServer {
	class Network {
		public string LocalIP { get; private set; }
		public Queue<string> WaitingQueue { get; set; } = new Queue<string>();
		public Queue<string> RemoveQueue { get; set; } = new Queue<string>();

		private readonly Socket serverSocket;
		private readonly Dictionary<string, Socket> Client = new Dictionary<string, Socket>();
		private string ReceiveMsg;

		public Network() {
			LocalIP = Dns.GetHostEntry(Dns.GetHostName()).AddressList.ToList().Where(p => p.AddressFamily == AddressFamily.InterNetwork).FirstOrDefault().ToString();
			serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			serverSocket.Bind(new IPEndPoint(IPAddress.Parse(LocalIP), 8888));
			serverSocket.Listen(5);
			new Thread(Accept) {
				IsBackground = true
			}.Start();
		}

		private void Accept() {
			string ip = "";
			try {
				Socket socket = serverSocket.Accept();
				ip = ((IPEndPoint)(socket.RemoteEndPoint)).Address.ToString();
				Client[ip] = socket;
				WaitingQueue.Enqueue(ip);
				new Thread(Accept) {
					IsBackground = true
				}.Start();

				byte[] lenBytes = new byte[4];
				while (true) {
					socket.Receive(lenBytes);
					int len = Convert.ToInt32(Encoding.UTF8.GetString(lenBytes));
					byte[] bytes = new byte[len];
					socket.Receive(bytes);
					ReceiveMsg = Encoding.UTF8.GetString(bytes, 0, len);
					//Console.WriteLine(ReceiveMsg);
				}
			}
			catch {
				if (Client.ContainsKey(ip)) {
					Console.WriteLine(ip + "closed");
					Client[ip].Close();
					Client.Remove(ip);
					RemoveQueue.Enqueue(ip);
				}
			}
		}

		public void Send(string ip, string msg) {
			if (msg == null) {
				throw new NullReferenceException("message不可為Null");
			}
			else {
				try {
					if (Client.ContainsKey(ip) && Client[ip].Connected)
						Client[ip].Send(Encoding.UTF8.GetBytes(msg.Length.ToString().PadLeft(4, '0') + msg));
				}
				catch {
					if (Client.ContainsKey(ip)) {
						Console.WriteLine(ip + "closed");
						Client[ip].Close();
						Client.Remove(ip);
						RemoveQueue.Enqueue(ip);
					}
				}
			}
		}

		public string GetMsg() {
			string temp = ReceiveMsg;
			ReceiveMsg = "";
			return temp;
		}

		public void Close() {
			foreach (KeyValuePair<string, Socket> item in Client)
				item.Value.Close();
			Client.Clear();
			serverSocket.Close();
		}
	}
}
