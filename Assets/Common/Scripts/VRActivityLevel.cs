/*using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;
using Valve.VR;

public class VRActivityLevel : MonoBehaviour {

  public Text statusText;

  // Update is called once per frame
  void Update() {
    if (XRSettings.loadedDeviceName == "OpenVR")
    {
      switch (OpenVR.System.GetTrackedDeviceActivityLevel(0))
      {
        case EDeviceActivityLevel.k_EDeviceActivityLevel_Unknown:
          statusText.text = "Unknown";
          break;
        case EDeviceActivityLevel.k_EDeviceActivityLevel_Idle:
          statusText.text = "Idle";
          break;
        case EDeviceActivityLevel.k_EDeviceActivityLevel_Standby:
          statusText.text = "Standby";
          break;
        case EDeviceActivityLevel.k_EDeviceActivityLevel_UserInteraction:
          statusText.text = "UserInteraction";
          break;
        case EDeviceActivityLevel.k_EDeviceActivityLevel_UserInteraction_Timeout:
          statusText.text = "UserInteractionTimeout";
          break;
      }
    }
    else
    {
      statusText.text = "No HMD Device Loaded";
    }
  }
}
*/