using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace ByteSprite {
    public class HeadsetWornHack : MonoBehaviour
    {
        public bool headsetHasBeenWorn = false;

        public InputAction userPresenceAction;
    
        void OnEnable() {
            userPresenceAction.Enable();
            userPresenceAction.started += OnHeadsetWorn;
            userPresenceAction.canceled += OnHeadsetNotWorn;
        }
    
        void OnDisable() {
            userPresenceAction.Disable();
            userPresenceAction.started -= OnHeadsetWorn;
            userPresenceAction.canceled -= OnHeadsetNotWorn;
        }

        void OnHeadsetWorn(InputAction.CallbackContext context) {
            headsetHasBeenWorn = true;
        }

        void OnHeadsetNotWorn(InputAction.CallbackContext context) {
            if(headsetHasBeenWorn)
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
