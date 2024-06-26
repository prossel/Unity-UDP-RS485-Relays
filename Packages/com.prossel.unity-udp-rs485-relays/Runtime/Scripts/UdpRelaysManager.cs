/*

This script is used to control the relays a Waveshare Modbus RTU Relay 16 CH device through a python script acting as an UDP - Serial gateway.

The `UdpRelays` class is a singleton that sends UDP messages to the device to control the relays.

To use this script, attach it to a GameObject in the scene and set the `ipAddress` and `port` fields to the IP address and port of the device. Default values sould work.

Default IP addrerss 255.255.255.255 is used to broadcast the UDP messages to all devices on the network. The device will respond with its IP address and the script will use it to send the control messages.

The `RelayOn`, `RelayOff`, and `RelayToggle` methods are used to control one (by numeber) or all (with 255) relays on the device.

The `SetRelay` method is used to set the state of a relay (on or off) by number.

The reference of the can be obtained from the `Instance` property of the `UdpRelays` class.

Examples: 
UdpRelays.Instance.RelayOn(1);           // turn on relay CH1
UdpRelays.Instance.RelayOff(2);          // turn off relay CH2
UdpRelays.Instance.RelayToggle(3);       // toggle relay CH3
UdpRelays.Instance.SetRelay(255, false); // turn off all relays

Or use the `UdpRelays` class to control the relays in the scene.

Pierre Rossel - 2024-06-06 - v1.0 - Initial version
Pierre Rossel - 2024-06-11 - v1.1 - Use relay chanel numbers (1-16) instead of relay numbers (0-15) to match the numbers on the device

*/

using System;
using System.Linq;
using System.Net.Sockets;
using UnityEngine;


public class UdpRelaysManager : MonoBehaviour
{
    public string ipAddress = "255.255.255.255";
    public int port = 8080;

    public int listeningPort = 8081;

    static UdpRelaysManager instance;

    public static UdpRelaysManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<UdpRelaysManager>();
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
            Debug.Log("Received " + result.Buffer.Length + " bytes: " + BitConverter.ToString(result.Buffer));

            if (result.Buffer.Length == 0)
            {
                // get the IP address of the device
                ipAddress = result.RemoteEndPoint.Address.ToString();
                Debug.Log("IP Address: " + ipAddress);
            }
        }
    }

    public void RelayOn(int chanel)
    {
        SetRelay(chanel, true);
    }

    public void RelayOff(int chanel)
    {
        SetRelay(chanel, false);
    }

    /** 
    * Toggle the state of a relay by number
    *
    * @param chanel The number of the relay to control (1-16) or 255 for all relays
    */
    public void RelayToggle(int chanel)
    {
        // default parameters values (broadcast address, switch relay, all relays, toggle)
        // deviceAddress = 0x00      # 00: broadcast address
        // command = 0x05            # 05: switch relay
        // registerAddress = 0x00FF  # register address 00FF: all relays
        // commandData = 0x5500      # command data  FF00: switch on, 0000: switch off, 5500: toggle

        byte[] message = { 0x00, 0x05, 0x00, 0x00, 0x55, 0x00 };
        message[3] = (byte)(chanel == 255 ? 255 : chanel - 1);
        SendUdpMessage(message.Concat(crc16(message)).ToArray());
    }

    /** 
    * Set the state of a relay by number
    * 
    * @param chanel The number of the relay to control (1-16) or 255 for all relays
    * @param state The state of the relay (true: on, false: off)
    */
    public void SetRelay(int chanel, bool state)
    {
        // default parameters values (broadcast address, switch relay, all relays, toggle)
        // deviceAddress = 0x00      # 00: broadcast address
        // command = 0x05            # 05: switch relay
        // registerAddress = 0x00FF  # register address 00FF: all relays
        // commandData = 0x5500      # command data  FF00: switch on, 0000: switch off, 5500: toggle

        byte[] message = { 0x00, 0x05, 0x00, 0x00, 0x55, 0x00 };
        message[3] = (byte)(chanel == 255 ? 255 : chanel - 1);
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
