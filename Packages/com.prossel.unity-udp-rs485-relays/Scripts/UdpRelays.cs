/*

This script is used to control the relays a Waveshare Modbus RTU Relay 16 CH device through a python script acting as an UDP - Serial gateway.

The `UdpRelays` class is a singleton that sends UDP messages to the device to control the relays.

To use this script, attach it to a GameObject in the scene and set the `ipAddress` and `port` fields to the IP address and port of the device. Default values sould work.

Default IP addrerss 255.255.255.255 is used to broadcast the UDP messages to all devices on the network. The device will respond with its IP address and the script will use it to send the control messages.

The `RelayOn`, `RelayOff`, and `RelayToggle` methods are used to control one (by numeber) or all (with 255) relays on the device.

The `SetRelay` method is used to set the state of a relay (on or off) by number.

The reference of the can be obtained from the `Instance` property of the `UdpRelays` class.

Examples: 
UdpRelays.Instance.RelayOn(1);           // turn on relay 1
UdpRelays.Instance.RelayOff(2);          // turn off relay 2
UdpRelays.Instance.RelayToggle(3);       // toggle relay 3
UdpRelays.Instance.SetRelay(255, false); // turn off all relays

Pierre Rossel - 2024-06-06 - v1.0 - Initial version

*/

using System;
using System.Linq;
using System.Net.Sockets;
using Unity.VisualScripting;
using UnityEngine;


public class UdpRelays : MonoBehaviour
{
    public string ipAddress = "255.255.255.255";
    public int port = 8080;

    public int listeningPort = 8081;

    static UdpRelays instance;

    public static UdpRelays Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<UdpRelays>();
            }
            return instance;
        }
    }

    UdpClient client;

    void OnEnable()
    {
        client = new UdpClient(listeningPort);
        Listen();

        // Send an empty message to tigger an empty response with the 
        // IP address of the device
        byte[] message = { };
        SendUdpMessage(message);
    }

    void OnDisable()
    {
        client.Close();
    }

    async void Listen()
    {
        UdpReceiveResult result;
        while (true)
        {
            try
            {
                result = await client.ReceiveAsync();
            }
            catch (ObjectDisposedException)
            {
                break;
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }

            // Debug.Log("Received " + result.Buffer.Length + " bytes: " + Encoding.UTF8.GetString(result.Buffer));
            Debug.Log("Received " + result.Buffer.Length + " bytes: " + result.Buffer.ToHexString());

            if (result.Buffer.Length == 0)
            {
                // get the IP address of the device
                ipAddress = result.RemoteEndPoint.Address.ToString();
                Debug.Log("IP Address: " + ipAddress);
            }
        }
    }

    public void RelayOn(int num)
    {
        SetRelay(num, true);
    }

    public void RelayOff(int num)
    {
        SetRelay(num, false);
    }

    public void RelayToggle(int num)
    {
        // default parameters values (broadcast address, switch relay, all relays, toggle)
        // deviceAddress = 0x00      # 00: broadcast address
        // command = 0x05            # 05: switch relay
        // registerAddress = 0x00FF  # register address 00FF: all relays
        // commandData = 0x5500      # command data  FF00: switch on, 0000: switch off, 5500: toggle

        byte[] message = { 0x00, 0x05, 0x00, 0x00, 0x55, 0x00 };
        message[3] = (byte)num;
        SendUdpMessage(message.Concat(crc16(message)).ToArray());
    }

    public void SetRelay(int num, bool state)
    {
        // default parameters values (broadcast address, switch relay, all relays, toggle)
        // deviceAddress = 0x00      # 00: broadcast address
        // command = 0x05            # 05: switch relay
        // registerAddress = 0x00FF  # register address 00FF: all relays
        // commandData = 0x5500      # command data  FF00: switch on, 0000: switch off, 5500: toggle

        byte[] message = { 0x00, 0x05, 0x00, 0x00, 0x55, 0x00 };
        message[3] = (byte)num;
        message[4] = (byte)(state ? 0xFF : 0x00);
        SendUdpMessage(message.Concat(crc16(message)).ToArray());
    }

    void SendUdpMessage(byte[] message)
    {
        // Send the message to the server
        client.Send(message, message.Length, ipAddress, port);
    }

    byte[] crc16(byte[] data)
    {
        ushort crc = 0xFFFF;
        foreach (byte b in data)
        {
            crc ^= b;
            for (int i = 0; i < 8; i++)
            {
                if ((crc & 0x0001) != 0)
                {
                    crc = (ushort)((crc >> 1) ^ 0xA001);
                }
                else
                {
                    crc = (ushort)(crc >> 1);
                }
            }
        }
        return new byte[] { (byte)(crc & 0xFF), (byte)(crc >> 8) };
    }

}
