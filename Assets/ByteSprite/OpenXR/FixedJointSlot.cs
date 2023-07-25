using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace ByteSprite.OpenXR {
    public class FixedJointSlot : MonoBehaviour {
        public bool isConnected;
        
        [SerializeField]
        Rigidbody rigidbody;
        
        [SerializeField]
        XRSocketInteractor socketInteractor;
        
        public void SetAttached() {
            var slottedItem = socketInteractor.GetOldestInteractableSelected();
            slottedItem.transform.gameObject.AddComponent<FixedJoint>().connectedBody = rigidbody;
            slottedItem.transform.GetComponent<Collider>().enabled = false;
            slottedItem.transform.GetComponent<XRGrabInteractable>().interactionLayers = InteractionLayerMask.GetMask("Disabled");
            enabled = false;
            socketInteractor.enabled = false;
            isConnected = true;
        }
    }
}
