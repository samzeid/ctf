// This is a placeholder to compile the project without SteamVR now that it's been removed.
// You can use this file to find the SteamVR API calls that need to be replaced.

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

namespace ByteSprite {
    public static class VR {
        public static bool isHeadsetWorn = false;
        
        public static bool IsHeadsetWorn() {
            //return (isHeadsetWorn);
                //return false;
            
            var devices = new List<InputDevice>();
            InputDevices.GetDevices(devices);

            InputDevices.GetDevicesWithCharacteristics(InputDeviceCharacteristics.HeadMounted, devices);

            foreach (var device in devices) {
                if (device.TryGetFeatureValue(CommonUsages.userPresence, out bool value)) {
                    if (value)
                        return true;
                }
            }
            return false;
        }
    }
}

namespace Valve.VR {
    public class SteamVR_TrackedObject {
    }
    
    public enum VRInputSource {
        Any,
        Head,
        LeftHand,
        RightHand,
    }
    
    public class SteamVR_Input {
        public static SteamVR_Action_Boolean GetBooleanAction(string actionName) {
            return new SteamVR_Action_Boolean();
        }
        
        public static SteamVR_Action_Vector2 GetVector2(string actionName, VRInputSource any) {
            return new SteamVR_Action_Vector2();
        }
    }
    
    public class SteamVR_Action_Boolean {
        public bool GetStateDown(VRInputSource any) {
            return true;
        }
        
        public bool GetStateUp(VRInputSource any) {
            return true;
        }
    }
    
    public class SteamVR_Action_Vector2 {
        public float x;
    }
}