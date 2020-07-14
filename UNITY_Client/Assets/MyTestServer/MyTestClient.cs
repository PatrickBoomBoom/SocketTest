using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.UI;

public class MyTestClient : MonoBehaviour
{
    public InputField inputF;
    public Text text;
    Socket socket;

    byte[] readBuff = new byte[1024];
    string recvStr = "";
    List<string> msgs = new List<string>();
    private void Update()
    {
        if (msgs.Count > 0)
        {
            for(int i = 0; i < msgs.Count; i++)
            {
                text.text += msgs[i] + "\r\n";
            }
            msgs.Clear();
        }
    }
    public void Connection()
    {
        socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        socket.BeginConnect("192.168.1.115", 8888, ConnectCallback, socket);
    }

    void ConnectCallback(IAsyncResult ar)
    {
        Debug.Log("ConnectCallback");
        try
        {
            Socket socket = (Socket)ar.AsyncState;
            socket.EndConnect(ar);
            Debug.Log("Socket Connect Success ... ");
            
            socket.BeginReceive(readBuff, 0, 1024, SocketFlags.None, ReceiveCallBack, socket);
        }
        catch (SocketException ex)
        {
            Debug.LogError(ex);
        }
    }

    void ReceiveCallBack(IAsyncResult ar)
    {
        try
        {
            Socket socket = (Socket)ar.AsyncState;
            int count = socket.EndReceive(ar);

            recvStr = System.Text.Encoding.Default.GetString(readBuff, 0, count);

            msgs.Add(recvStr);

            socket.BeginReceive(readBuff, 0, 1024, SocketFlags.None, ReceiveCallBack, socket);
        }
        catch (SocketException ex)
        {
            Debug.LogError(ex);
        }
    }

    public void Send()
    {
        Debug.Log("Send : " + inputF.text);
        byte[] b = System.Text.Encoding.Default.GetBytes(inputF.text);
        socket.Send(b);
    }
}
