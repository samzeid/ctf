using UnityEngine;
using UnityEngine.SceneManagement;
using Valve.VR;

public class HeadsetDetectionVive : MonoBehaviour {
    bool wasWarn = false;
    void Update() {
        var headset = UnityEngine.XR.InputDevices.GetDeviceAtXRNode(UnityEngine.XR.XRNode.Head);
        // detect if headset is worn
        if (headset.TryGetFeatureValue(UnityEngine.XR.CommonUsages.userPresence, out bool headsetIsWorn)) {
            if (headsetIsWorn) {
                wasWarn = true;
            } else {
                if (wasWarn) {
                    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
                }
            }
        }
    }
}
