using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using NativeWebSocket;


public class WebSocketRelays : MonoBehaviour
{
    WebSocket websocket;

    // Start is called before the first frame update
    async void Start()
    {
        Debug.Log("Connecting to websocket server...");

        // websocket = new WebSocket("ws://echo.websocket.org");
        websocket = new WebSocket("ws://192.168.1.103:8765");

        websocket.OnOpen += () =>
        {
            Debug.Log("Connection open!");
        };

        websocket.OnError += (e) =>
        {
            Debug.Log("Error! " + e);
        };

        websocket.OnClose += (e) =>
        {
            Debug.Log("Connection closed!");
        };

        websocket.OnMessage += (bytes) =>
        {
            // Reading a plain text message
            var message = System.Text.Encoding.UTF8.GetString(bytes);
            Debug.Log("Received OnMessage! (" + bytes.Length + " bytes) " + message);
        };

        // Keep sending messages at every 3s
        InvokeRepeating("SendWebSocketMessage", 0.0f, 3f);

        await websocket.Connect();

    }

    // Update is called once per frame
    void Update()
    {
#if !UNITY_WEBGL || UNITY_EDITOR
        websocket.DispatchMessageQueue();
#endif

    }
    async void SendWebSocketMessage()
    {
        
        if (websocket.State == WebSocketState.Open)
        {
            Debug.Log("Sending message to websocket server...");

            // Sending bytes 
            // 000500005500f34b toggle all relays on all devices
            await websocket.Send(new byte[] { 0x00, 0x05, 0x00, 0x00, 0x55, 0x00, 0xf3, 0x4b });
        }
    }

    private async void OnApplicationQuit()
    {
        if (websocket != null && websocket.State == WebSocketState.Open) {
            await websocket.Close();
        }
    }
}
