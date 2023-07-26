using UnityEngine;

public class HeadsetDetectTest : MonoBehaviour
{
    void Update() {
        Debug.Log("Headset is worn: " + ByteSprite.VR.IsHeadsetWorn());
    }
    /*public InputAction userPresenceAction;
    
    void Start() {
        userPresenceAction.started += ctx => Debug.Log("Headset is worn");
        userPresenceAction.canceled += ctx => Debug.Log("Headset is not worn");
    }

    void OnEnable() {
        userPresenceAction.Enable();
    }
    
    void OnDisable() {
        userPresenceAction.Disable();
    }*/
}