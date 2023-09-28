// This is a placeholder to compile the project without SteamVR now that it's been removed.
// You can use this file to find the SteamVR API calls that need to be replaced.

using Valve.VR;

namespace ByteSprite {
    public static class VR {
        static SteamVR_Action_Boolean headsetOnHead = SteamVR_Input.GetBooleanAction("HeadsetOnHead");
        public static bool IsHeadsetWorn() {
            return headsetOnHead.GetState(SteamVR_Input_Sources.Head);
        }
    }
}