using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using UnityEngine.InputSystem;

// This is working but it's not the best way to do it

public class UdpRelaysWithAsyncCallbackBkp : MonoBehaviour
{
    public string ipAddress = "255.255.255.255";
    public int port = 8080;

    public int listeningPort = 8081;

    UdpClient client;
    private IPEndPoint remoteEndPoint;
    private IPEndPoint listenEndPoint;


    private InputAction clickAction;

    private object obj = null;
    private System.AsyncCallback AC;

    void OnEnable()
    {
        listenEndPoint = new IPEndPoint(IPAddress.Any, listeningPort);
        client = new UdpClient();
        client.Client.Bind(listenEndPoint);
        remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);

        AC = new System.AsyncCallback(ReceivedUDPPacket);
        client.BeginReceive(AC, obj);
        Debug.Log("UDP - Start Receiving..");

        // Create a new InputAction for left click
        clickAction = new InputAction(type: InputActionType.Button, binding: "<Mouse>/leftButton");
        clickAction.performed += ctx => OnClick();
        clickAction.Enable();
    }

    void OnDisable()
    {
        client.Close();
        clickAction.Disable();
    }

    // Start is called before the first frame update
    void Start()
    {
        // a messsage is composed of a 8 bytes
        // example: 000500005500f34b toggle all relays on all devices

        // array of bytes
        // byte[] message = {0x00, 0x05, 0x00, 0x00, 0x55, 0x00, 0xf3, 0x4b};
        byte[] message = { };

        SendUdpMessage(message);
    }

    // Update is called once per frame
    void Update()
    {
        if (client.Available > 0)
        {
            Debug.Log("client.available: " + client.Available);
            byte[] data = client.Receive(ref remoteEndPoint);
            string message = Encoding.UTF8.GetString(data);
            Debug.Log("Received: " + message);
        }

    }

    void ReceivedUDPPacket(System.IAsyncResult result)
    {
        Debug.Log("ReceivedUDPPacket");
        
        //stopwatch.Start();
        byte[] receivedBytes;
        receivedBytes = client.EndReceive(result, ref listenEndPoint);

        client.BeginReceive(AC, obj);

        //stopwatch.Stop();
        //Debug.Log(stopwatch.ElapsedTicks);
        //stopwatch.Reset();
    } // ReceiveCallBack

    void OnClick()
    {
        // array of bytes
        byte[] message = { 0x00, 0x05, 0x00, 0x00, 0x55, 0x00, 0xf3, 0x4b };

        SendUdpMessage(message);

        Debug.Log("click");

        UdpClient client2 = new UdpClient();
        //IPEndPoint ip = new IPEndPoint(IPAddress.Parse("localhost"), 8081); // Replace with your IP and port
        byte[] helloMessage = Encoding.UTF8.GetBytes("hello"); // Convert string to byte array
        // client2.Send(helloMessage, helloMessage.Length, ip);
        client2.Send(helloMessage, helloMessage.Length, "localhost", listeningPort);
        client2.Close();
    }

    void SendUdpMessage(byte[] message)
    {
        // Send the message to the server
        client.Send(message, message.Length, ipAddress, port);
    }
}
