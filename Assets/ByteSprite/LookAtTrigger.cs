using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace ByteSprite
{
    public class LookAtTrigger : MonoBehaviour
    {
        [SerializeField]
        private Transform playerCamera;

        [SerializeField]
        private float triggerDistance = 100f;

        [SerializeField]
        private float lookAtAngle = 30f; //

        //current 
        private float criticalLookValue;

        [SerializeField]
        private float increaseOverTime;

        [SerializeField]
        private RestartButton restartButton;

        [SerializeField]
        private Image imageFillAmount;

        public GameObject root;

        public Transform targetCricle;
        
        public enum buttonState { reload, mainMenu}

        public buttonState currentButtonState;

       private void Start()
        {
            root.SetActive(false);
            Invoke("EnableAfterDelay", 2f);
        }

        void EnableAfterDelay()
        {
            root.SetActive(true);
        }
    

        // Update is called once per frame
        void Update()
        {
            
            // Check if the camera is looking at the object
            if (IsCameraLookingAt())
            {
                if (criticalLookValue < 1)
                {
                    criticalLookValue += increaseOverTime * Time.deltaTime;
                    imageFillAmount.fillAmount = criticalLookValue;
                }
                else
                {
                    switch (currentButtonState)
                    {
                        case buttonState.reload:
                            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); 
                            break;
                        
                        case buttonState.mainMenu:
                            SceneManager.LoadScene("Main");
                            break;
                    }
                   
                }
            }
            else if (criticalLookValue > 0)
            {
                criticalLookValue = 0;
                imageFillAmount.fillAmount = criticalLookValue;
            }
        }
    
        private bool IsCameraLookingAt()
        {
            // Check if player camera is looking at the object 
            if (playerCamera != null)
            {
                Vector3 toObject = transform.position - playerCamera.position;
                float angle = Vector3.Angle(playerCamera.forward, toObject);

                if (angle < lookAtAngle && toObject.magnitude < triggerDistance)
                {
                    return true;
                }
            }

            return false;
        }

    }
}
