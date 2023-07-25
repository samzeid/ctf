using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace ByteSprite.OpenXR {
    public class FixedJointSlot : MonoBehaviour
    {
        [SerializeField]
        Rigidbody rigidbody;
        
        [SerializeField]
        XRSocketInteractor socketInteractor;
        
        public void SetAttached() {
            var slottedItem = socketInteractor.GetOldestInteractableSelected().transform;
            slottedItem.GetComponent<Collider>().enabled = false;
            slottedItem.gameObject.AddComponent<FixedJoint>().connectedBody = rigidbody;
        }
    }
}
