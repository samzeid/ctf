using UnityEngine;
using UnityEngine.SceneManagement;
using Valve.VR;

public class HeadsetDetectionVive : MonoBehaviour {
    bool isLoading = false;
    float wasRemovedFromHead = 0;
    bool wasWorn = false;
    SteamVR_Action_Boolean headsetOnHead = SteamVR_Input.GetBooleanAction("HeadsetOnHead");
    
    void Update() {
        if(isLoading)
            return;
        
        bool isHeadsetOnHead = headsetOnHead.GetState(SteamVR_Input_Sources.Head);

        if (isHeadsetOnHead)
            wasWorn = true;
        else
            wasRemovedFromHead += Time.deltaTime;

        if (wasWorn && wasRemovedFromHead > 2f) {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            isLoading = true;
        }
    }
}