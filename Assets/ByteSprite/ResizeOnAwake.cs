using System.Threading.Tasks;
using UnityEngine;

namespace ByteSprite
{
    public class ResizeOnAwake : MonoBehaviour
    {
        public Transform targetTransform;
        public Vector3 increaseAmount;

        private void Start()
        {
        
        }

        public async void EnlargeOverTime()
        {
            targetTransform.gameObject.SetActive(true);
            while (targetTransform.localScale.magnitude < Vector3.one.magnitude)
            {
                targetTransform.localScale += increaseAmount;
                await Task.Delay(10);
            }

            targetTransform.localScale = Vector3.one;
        }
        
        public async void DecreaseOverTime()
        {
            while (targetTransform.localScale.magnitude > Vector3.zero.magnitude)
            {
                targetTransform.localScale -= increaseAmount;
                await Task.Delay(10);
            }
            targetTransform.gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            EnlargeOverTime();
        }

        private void OnDisable()
        {
            targetTransform.localScale = Vector3.zero;
        }
    }
}
