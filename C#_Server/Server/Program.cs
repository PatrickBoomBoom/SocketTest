using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class MyTestServer
    {
        static Socket listen;
        static Dictionary<Socket, ClientState> clients = new Dictionary<Socket, ClientState>();

        //开启服务器
        static void Main(string[] args)
        {
            Console.WriteLine("Main Start !!! ");
            //监听用socket实例
            listen = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            //绑定本机 ip 以及 端口
            IPAddress ipAdr = IPAddress.Parse("192.168.1.115");
            IPEndPoint ipEnd = new IPEndPoint(ipAdr, 8888);
            listen.Bind(ipEnd);

            listen.Listen(0);

            Console.WriteLine("服务器启动 !!! ");
            listen.BeginAccept(AcceptCallBack, listen);

            Console.ReadKey();
        }

        //接收回调
        static void AcceptCallBack(IAsyncResult ar)
        {
            try
            {
                //分配state 加入clients列表
                Console.WriteLine("Accepted");
                Socket listen = (Socket)ar.AsyncState;
                Socket client = listen.EndAccept(ar);
                ClientState state = new ClientState
                {
                    socket = client
                };
                clients.Add(client, state);
                client.BeginReceive(state.readBuff, 0, 1024, 0, ReceiveCallback, state);
            }
            catch (SocketException ex)
            {
                Console.WriteLine(ex);
            }

        }

        static void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                ClientState state = (ClientState)ar.AsyncState;
                Socket client = state.socket;
                int count = client.EndReceive(ar);
                if (count == 0)
                {
                    //暂时 为0就断开
                    client.Close();
                    clients.Remove(client);
                    Console.WriteLine("socket close");
                    return;
                }

                string recvStr = System.Text.Encoding.Default.GetString(state.readBuff, 0, count);
                Console.WriteLine("recv : " + recvStr);

                byte[] sendBytes = System.Text.Encoding.Default.GetBytes("echo" + recvStr);
                client.Send(sendBytes);
                client.BeginReceive(state.readBuff, 0, 1024, 0, ReceiveCallback, state);
            }
            catch (SocketException ex)
            {
                Console.WriteLine("Socket Receive Fail " + ex.ToString());
            }
        }
    }

    class ClientState
    {
        public Socket socket;
        public byte[] readBuff = new byte[1024];
    }
}
