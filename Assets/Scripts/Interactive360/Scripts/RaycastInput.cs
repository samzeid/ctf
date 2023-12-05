//This script handles raycasting from the main camera into the scene. It is also responsible for knowing what the user is
//looking at and telling the interactable objects what the player is doing (looking at them, looking away, clicking them, and 
//releasing them). The script acheives this by simulating a tradition input using a raycast and then sending it to the event system
//as if it were a real input event

using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Interactive360
{
    [RequireComponent(typeof(VRLineRenderer))]
    public class RaycastInput : MonoBehaviour
    {
        //Assign input asset in editor using new Unity action system
        [SerializeField] InputActionAsset actions;
        InputAction pointerAction;
        
        [SerializeField] LayerMask whatIsInteractable;      //The layers that this raycast affects
        [SerializeField] float rayDistance = 20f;           //The distance that the input ray should be cast
        [SerializeField] bool drawDebugLine;                //Should we draw a debug ray?
        [SerializeField] VRLineRenderer line;               //A reference to the VR Line Renderer component
        [SerializeField] float defaultLineDistance = 10f;   //The distance in front of the player that the line ends if nothing is highlighted

        Ray ray;                                            //A container for the ray
        RaycastHit rayHit;                                  //The results of a raycast
        PointerEventData eventData;                         //The data for our simulated events

        public Color interactionColor = Color.white;
        private Color defaultColor = Color.white;

        public GameObject interactionGizmo;

        bool interactedThisFrame = false;
        
        void Reset()
        {
            //Set the Layer Mask to interact with everything but the Ignore Raycast layer using bitwise operations
            whatIsInteractable = ~(1 << LayerMask.NameToLayer("Ignore Raycast"));
            line = GetComponent<VRLineRenderer>();
        }

        //Setup input using new Unity action system  
        void Awake() { pointerAction = actions.FindActionMap("Gameplay").FindAction("PointerActivate"); }
        void OnEnable() { actions.FindActionMap("Gameplay").Enable(); }
        void OnDisable() { actions.FindActionMap("Gameplay").Disable(); }

        void Start()
        {
            defaultColor = GetComponent<Renderer>().material.color;

            //Give the line renderer 2 vertices
            line.SetVertexCount(2);

            //Generate a new event data container
            eventData = new PointerEventData(EventSystem.current);
            eventData.pointerId = 0;

            //If VR is enabled, set the event position to the center of the HMD. Otherwise set it to the center of the Game window
            if (UnityEngine.XR.XRSettings.enabled)
                eventData.position = new Vector2(UnityEngine.XR.XRSettings.eyeTextureWidth / 2f, UnityEngine.XR.XRSettings.eyeTextureHeight / 2f);
            else
                eventData.position = new Vector2(Screen.width / 2f, Screen.height / 2f);

            //Set the press position to the same as the event data position
            eventData.pressPosition = eventData.position;
        }

        void Update()
        {
            //Every frame we look for interactables and check for hardware inputs
            LookForInteractables();
            CheckInput();
        }


        void LookForInteractables()
        {
            //Generate a new ray at our input object facing forward
            ray = new Ray(transform.position, transform.forward);

            //If we want, draw a debug line in the scene view
            if (drawDebugLine)
                Debug.DrawLine(transform.position, transform.position + transform.forward * rayDistance, Color.red);

            //Cast the ray
            Physics.Raycast(ray, out rayHit, rayDistance, whatIsInteractable);

            //Draw the ray so that the player can see it
            DrawLine();

            /*if (rayHit.transform == null)
            {
                //See if we hit a canvas element
                Quaternion r = Quaternion.LookRotation(ray.direction);
                float x = r.eulerAngles.x;
                float y = r.eulerAngles.y;
                if (x > 180.0f)
                    x -= 360.0f;
                if (y > 180.0f)
                    y -= 360.0f;

                Vector2 canvasPos = new Vector2(x, -y);
                //print(r.eulerAngles + "  " + canvasPos);

                //TODO: raycast in canvas for colliders
            }*/

            //We didn't hit anything
            if (rayHit.transform == null)
            {
                GetComponent<Renderer>().material.color = defaultColor;
                if (interactionGizmo != null)
                {
                    interactionGizmo.transform.position = transform.position + transform.forward * rayDistance;
                    interactionGizmo.GetComponent<Animator>().SetBool("selected", false);
                    //interactionGizmo.SetActive(false);
                }

                //Look away from anything we were previously looking at
                LookAway();
                return;
            }
            else
            {
                //If a modal is open then discard any other ray hits
                if (TooltipCollider.ModalOpen != null && !TooltipCollider.IsSelectedModal(rayHit.transform.gameObject))
                    //TooltipCollider.ModalOpen != rayHit.transform.gameObject)
                {
                    return;
                }

                GetComponent<Renderer>().material.color = interactionColor;
                if (interactionGizmo != null)
                {
                    //interactionGizmo.SetActive(true);
                    if (rayHit.transform.gameObject.layer == 9)
                        interactionGizmo.GetComponent<Animator>().SetBool("selected", true);
                    else
                    {
                        GetComponent<Renderer>().material.color = defaultColor;
                        interactionGizmo.GetComponent<Animator>().SetBool("selected", false);
                    }
                    interactionGizmo.transform.position = rayHit.point;
                }
            }

            //We are looking at something, so record its data
            eventData.pointerCurrentRaycast = ConvertRaycastHitToRaycastResult(rayHit);

            //If we are looking at the same object that we were looking at, we don't need to do anything and can exit
            if (eventData.pointerEnter == rayHit.transform.gameObject)
                return;

            //Otherwise we are looking at something new and should look away from the old object
            LookAway();

            //Record this data and tell the object that we are pointing at them (OnPointerEnter)
            eventData.pointerEnter = rayHit.transform.gameObject;
            ExecuteEvents.ExecuteHierarchy(eventData.pointerEnter, eventData, ExecuteEvents.pointerEnterHandler);
        }

        // OpenXR: Confirm primaryInputAxis (SecondaryIndexTrigger) binding is setup correctly
        void CheckInput() {
            var pointerAxis = pointerAction.ReadValue<float>();
            //If we press the primary input axis...
            if (pointerAxis >= .1 && eventData.pointerEnter != null)
            {
                //...tell the object that we have pressed it (OnPointerDown)
                eventData.pointerPressRaycast = eventData.pointerCurrentRaycast;
                eventData.pointerPress = ExecuteEvents.ExecuteHierarchy(eventData.pointerEnter, eventData, ExecuteEvents.pointerDownHandler);
            }
            //Otherwise, if we just released the primary input axis...
            else if (pointerAxis == 0)
            {
                //...tell the object than we have stopped pressing it (OnPointerUp)
                if (eventData.pointerPress != null)
                    ExecuteEvents.ExecuteHierarchy(eventData.pointerPress, eventData, ExecuteEvents.pointerUpHandler);
                
                //...finally, if we pressed and released the same object, then we have clicked it (OnPointerClick)
                if (eventData.pointerPress == eventData.pointerEnter)
                {
                    ExecuteEvents.ExecuteHierarchy(eventData.pointerEnter, eventData, ExecuteEvents.pointerClickHandler);
                }

                eventData.pointerPress = null;
            }
        }

        void LookAway()
        {
            //If we are currently looking at something, stop looking at it and tell the object (OnPointerExit)
            if (eventData.pointerEnter != null)
            {
                ExecuteEvents.ExecuteHierarchy(eventData.pointerEnter, eventData, ExecuteEvents.pointerExitHandler);
                eventData.pointerEnter = null;
            }
        }

        //This method converts a RaycastHit data type that we get from a raycast into a RaycastResult type that is
        //used by the event system
        RaycastResult ConvertRaycastHitToRaycastResult(RaycastHit hit)
        {
            RaycastResult rayResult = new RaycastResult();
            rayResult.gameObject = hit.transform.gameObject;
            rayResult.distance = rayHit.distance;
            rayResult.worldPosition = rayHit.point;
            rayResult.worldNormal = rayHit.normal;

            return rayResult;
        }

        void DrawLine()
        {
            //If we are looking at something, draw the line to it. Otherwise, draw the line ending at the default distance in front of us
            Vector3 pos = rayHit.transform != null ? rayHit.point : ray.GetPoint(defaultLineDistance);
            line.SetPosition(0, ray.origin);
            line.SetPosition(1, pos);
        }
    }
}
