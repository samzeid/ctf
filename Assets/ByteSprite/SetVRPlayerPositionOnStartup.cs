using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

public class SetVRPlayerPositionOnStartup : MonoBehaviour {
    [SerializeField]
    Transform player;
    
    [SerializeField]
    Transform playerCamera;
    
    [SerializeField]
    Transform resetPosition;

    const string ResetPositionButtonString = "ResetPosition";
    
    // Set the OpenXR player's position to the position of this object on startup. This is to counteract the positioning of the player in the scene by the OpenXR plugin.
    // In order to do this, we need to wait until the OpenXR plugin has finished positioning the player, which is why we use a coroutine.
    void Start() {
        //ResetPlayerPosition();
        //StartCoroutine(ResetPlayerPositionCoroutine());
    }
    
    void Update() {
        if (Input.GetButtonDown(ResetPositionButtonString)) {
            ResetPlayerPosition();
        }
    }
    
    IEnumerator ResetPlayerPositionCoroutine() {
        yield return new WaitForSeconds(0.1f);
        ResetPlayerPosition();
    }
    
    [ContextMenu("ResetPosition")]
    public void ResetPlayerPosition() {
        var rotationY = resetPosition.rotation.eulerAngles.y - playerCamera.rotation.eulerAngles.y;
        player.rotation = Quaternion.Euler(0, rotationY, 0);
        
        var distance = resetPosition.position - playerCamera.position;
        player.position += distance;
    }
}
