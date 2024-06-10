# Demo RS485 Relays

Unity demo project to show how to control relays through an UDP gateway to RS485 relay box.

The [Murmuration-UDPSerial-gateway](https://github.com/prossel/Murmuration-UDPSerial-gateway) must be running on the local netword (or on the same machine),

Unity project based on [Unity-Template-QuestXRHandsURP](https://github.com/prossel/Unity-Template-QuestXRHandsURP)

## Getting started

* Copy the Assets/Murmuration folder in your project (including meta files)
* Add an Empty GameObject to your scene "UDP Relays"
* Add the UDP Relays component
* From your scripts, call the functions to manipulate the relays

### Examples

```c#
UdpRelays.Instance.RelayOn(1);           // turn on relay 1
UdpRelays.Instance.RelayOff(2);          // turn off relay 2
UdpRelays.Instance.RelayToggle(3);       // toggle relay 3
UdpRelays.Instance.SetRelay(255, false); // turn off all relays
```

## Run the demo

### DemoToggleRelays

* Using Package Manager, find XR Interaction Toolkit, Samples and import the Hands interaction demo
* Open the scene `Assets/Murmuration/Scenes/DemoToggleRelays.unity`
* Play in Unity Editor and click the cubes to toggle the relays
* Build and run in the headset to toggle the relays in VR
