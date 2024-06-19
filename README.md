# Unity UDP RS485 Relays

Control relays with Unity through an UDP gateway and a RS485 relay box.

The [Murmuration-UDPSerial-gateway](https://github.com/prossel/Murmuration-UDPSerial-gateway) must be running on the local netword (or on the same machine),

Unity project based on [Unity-Template-QuestXRHandsURP](https://github.com/prossel/Unity-Template-QuestXRHandsURP)

## Installation

This repository is compatible with UPM (Unity Package Manager) for an easy install and update.

* In Unity, open Window > Package Manager
* Press the + button, choose "Add package from git URL..."
* Enter https://github.com/prossel/Unity-UDP-RS485-Relays.git#upm and press Add

## Getting started

* Add "UDP Relays Manager" prefab to your scene
* Add the *UDP Relays* component to any script to control the relays from UnityEvent's, animation events or your scripts
* Or from your scripts, directly call the manager's functions to manipulate the relays

### Examples

```C#

# Direct call the manager functions (esasier from code)
UdpRelaysManager.Instance.RelayOn(1);           // turn on relay 1
UdpRelaysManager.Instance.RelayOff(2);          // turn off relay 2
UdpRelaysManager.Instance.RelayToggle(3);       // toggle relay 3
UdpRelaysManager.Instance.SetRelay(255, false); // turn off all relays

# Or call functions from UDP Relays component on the same object
UdpRelays relays = GetComponent<UdpRelays>();
relays.RelayOn(1);           // turn on relay 1
relays.RelayOff(2);          // turn off relay 2
relays.RelayToggle(3);       // toggle relay 3
relays.SetRelay(255, false); // turn off all relays

```

## Run the demo

### DemoToggleRelays

* Using Package Manager, find XR Interaction Toolkit, Samples and import the Hands interaction demo
* Fix project validation issues if any by importing missing examples from other packages.
* Open the scene `Assets/Murmuration/Scenes/DemoToggleRelays.unity`
* Play in Unity Editor and click the cubes to toggle the relays
* Build and run in the headset to toggle the relays in VR

## Changelog

See [CHANGELOG.md](CHANGELOG.md)
