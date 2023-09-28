using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using Valve.VR;

namespace ByteSprite.OpenXR {
    public class HandAnimationController : MonoBehaviour {
        enum Hand {
            Left,
            Right
        }
        
        [SerializeField]
        Hand hand;
        
        [SerializeField]
        Animator animator;
        
        [SerializeField]
        HandSignal handSignal;

        //[SerializeField]
        //InputActionProperty triggerAction;
        
        static readonly int Grip = Animator.StringToHash("Grip");
        static readonly int IsPointing = Animator.StringToHash("IsPointing");
        static readonly int IsFlat = Animator.StringToHash("IsFlat");

        SteamVR_Action_Single triggerInput = SteamVR_Input.GetSingleAction("Squeeze");
        SteamVR_Action_Boolean gripAction = SteamVR_Input.GetBooleanAction("GrabGrip");

        float triggerValue;

        [SerializeField]
        FixedJoint joint;

        void Update() {
            var inputSource = (hand == Hand.Left) ? SteamVR_Input_Sources.LeftHand : SteamVR_Input_Sources.RightHand;
            // get squeeze value from the correct hand directly for steam 2.0
            float squeeze = triggerInput.GetAxis(inputSource);
            triggerValue = gripAction.GetState(inputSource) ? 1f : squeeze;

            Debug.Log("TriggerValue: " + triggerValue);
            // If we are performing a swing animation, we want to set the IsFlat parameter to true for the given hand.
            bool isFlat = hand == Hand.Left && handSignal.CheckSwingLeft || hand == Hand.Right && handSignal.CheckSwingRight;
            
            animator.SetFloat(Grip, triggerValue);
            animator.SetBool(IsPointing, handSignal.CheckHoist);
            animator.SetBool(IsFlat, handSignal.CheckLower || isFlat);

            if (joint.connectedBody != null) {
                joint.connectedBody.transform.position = joint.transform.position;
            }

            if (triggerValue < 0.9f) {
                joint.connectedBody.GetComponent<Collider>().enabled = true;
                joint.connectedBody.isKinematic = false;
                joint.connectedBody = null;
            }
        }

        void OnTriggerStay(Collider other) {
            if (other.GetComponent<XRGrabInteractable>() != null) {
                if (triggerValue >= 0.9f && joint.connectedBody == null) {
                    other.transform.position = transform.position;
                    joint.connectedBody = other.GetComponent<Rigidbody>();
                    // turn off collisions for the attached object
                    joint.connectedBody.GetComponent<Collider>().enabled = false;
                    joint.connectedBody.isKinematic = true;
                }
            }
        }
    }
}
