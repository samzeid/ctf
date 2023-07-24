using UnityEngine;
using UnityEngine.InputSystem;

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

        [SerializeField]
        InputActionProperty triggerAction;
        
        static readonly int Grip = Animator.StringToHash("Grip");
        static readonly int IsPointing = Animator.StringToHash("IsPointing");
        static readonly int IsFlat = Animator.StringToHash("IsFlat");

        void Update() {
            float triggerValue = triggerAction.action.ReadValue<float>();
            // If we are performing a swing animation, we want to set the IsFlat parameter to true for the given hand.
            bool isFlat = hand == Hand.Left && handSignal.CheckSwingLeft || hand == Hand.Right && handSignal.CheckSwingRight;
            
            animator.SetFloat(Grip, triggerValue);
            animator.SetBool(IsPointing, handSignal.CheckHoist);
            animator.SetBool(IsFlat, handSignal.CheckLower || isFlat);
        }
    }
}
