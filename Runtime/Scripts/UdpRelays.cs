/*

This script is used to control the relays a Waveshare Modbus RTU Relay 16 CH device through a python script acting as an UDP - Serial gateway.

Add this script to any GameObject in the scene and call the methods to control the relays. 

The methods can be called from other scripts, from the Unity Event System and from animation events.

The `RelayOn`, `RelayOff`, and `RelayToggle` methods are used to control one (by chanel number) or all (with 255) relays on the device.

There must be an instance of the `UdpRelaysManager` class in the scene to handle the UDP communication. You can use the `UdpRelaysManager` prefab provided in the package.

Pierre Rossel - 2024-06-06 - v1.0 - Initial version
Pierre Rossel - 2024-06-11 - v1.1 - Split the `UdpRelays` class from the `UdpRelaysManager` class

*/

using UnityEngine;


public class UdpRelays : MonoBehaviour
{
    /**
    * Turn on a relay by number.
    * 
    * @param num The number of the relay to turn on (1-16).
    */
    public void RelayOn(int num)
    {
        UdpRelaysManager.Instance.SetRelay(num, true);
    }

    /**
    * Turn off a relay by number.
    * 
    * @param num The number of the relay to turn off (1-16).
    */
    public void RelayOff(int num)
    {
        UdpRelaysManager.Instance.SetRelay(num, false);
    }

    /**
    * Toggle a relay by number.
    * 
    * @param num The number of the relay to toggle (1-16).
    */
    public void RelayToggle(int num)
    {
        UdpRelaysManager.Instance.RelayToggle(num);
    }

    /**
    * Set the state of a relay by number.
    * 
    * @param num The number of the relay to set (1-16).
    * @param state The state of the relay (true = on, false = off).
    */
    public void SetRelay(int num, bool state)
    {
        UdpRelaysManager.Instance.SetRelay(num, state);
    }
}
