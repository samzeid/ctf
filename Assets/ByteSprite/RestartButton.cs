using UnityEngine;
using UnityEngine.SceneManagement;

namespace ByteSprite {
    public class RestartButton : MonoBehaviour {
        const string RestartButtonString = "Restart";
        
        void Update() {
            if (Input.GetButtonDown(RestartButtonString)) {
                ResetScene();
            }
            
            if (Input.GetKeyDown(KeyCode.Space))
            {
                SceneManager.LoadScene("Main");
            }
        }

        public void ResetScene() {
            Debug.Log("_LM Restart scene");
            UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);        
        }

        public void LoadMainMenu()
        {
            print("_LM LoadingMainMenu");
            UnityEngine.SceneManagement.SceneManager.LoadScene("Main");
        }
    }
}
