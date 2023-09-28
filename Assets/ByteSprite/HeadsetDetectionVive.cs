using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.XR.OpenXR;

public class HeadsetDetectionVive : MonoBehaviour {
    [Tooltip("Action Reference that represents the control")]
    [SerializeField] private InputActionReference _actionReference = null;
    bool wasWarn = false;
    
    void Update() {
        Debug.Log(_actionReference.action.ReadValue<float>());
        
        // Read the bit mask values of the htc vive headset:


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