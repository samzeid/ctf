using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ByteSprite {
    public class HeadsetWornHack : MonoBehaviour
    {
        // Uses the OpenXR userPresence to detect if the headset is worn by subscribing to the event.
        // If the headset is not worn, and has been worn before, then the game is restarted.
        // If the headset has not been worn at all, then the game is not restarted.
        
        public bool headsetHasBeenWorn = false;

        public void Start() {
            
        }

        /*int framesSinceLastMove = 0;
        int framesMoved = 0;
        int framesToWait = 15;
        Vector3 lastPosition;
        bool headsetHasBeenWorn = false;
        
        public void Update() {
            // Wait a number of frames, if the headset has not moved, then assume it is not worn. If it isn't want we restart the game.
            // We also don't want to restart the game if the headset has not been worn at all. To check for this we use the headsetHasBeenWorn variable.
            if (framesSinceLastMove > framesToWait && headsetHasBeenWorn) {
                Debug.Log("Headset has not been worn for " + framesToWait + " frames. Restarting game.");
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
            
            // If the headset has moved, reset the framesSinceLastMove counter and set the lastPosition to the current position.
            if (transform.position != lastPosition) {
                // we need to put some grace period, as initialization of vr can cause it to move a bit.
                // as such we only set headsetHasBeenWorn to true if it has moved after the first 60 frames.
                if (framesMoved > 2) {
                    headsetHasBeenWorn = true;
                    //Debug.Log("Headset has been worn for " + framesSinceLastMove + " frames.");
                }
                
                framesMoved++;
                framesSinceLastMove = 0;
                lastPosition = transform.position;
            }
            
            framesSinceLastMove++;
            //Debug.Log("Frames since last move: " + framesSinceLastMove);
        }*/
    }
}
