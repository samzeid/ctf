using UnityEngine;

namespace ByteSprite {
    public class RestartButton : MonoBehaviour {
        const string RestartButtonString = "Restart";
        
        void Update() {
            if (Input.GetButtonDown(RestartButtonString)) {
                ResetScene();
            }
        }

        public void ResetScene() {
            Debug.Log("Restart scene");
            UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);        
        }
    }
}
