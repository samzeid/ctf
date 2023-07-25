using System.Collections;
using System.Collections.Generic;
using Common.Scripts;
using UnityEngine;
using Valve.VR;

public class GestureController : MonoBehaviour {

  private SteamVR_TrackedObject trackedObject;
  //TOFIX: Compilation placeholder
  private SteamVR_Input_Sources device;

  private string currentGesture;

  public GestureEventListener gestureEventListener;
  public LoadController loadController;
  public MobileCraneController mobileCraneController;
  public Animator handAnimator;

  private const float TOUCHPAD_CENTER_THRESHOLD = 0.4f;
  private GameObject grabbedObject;
  private HandSignal handSignal;

  // Use this for initialization
  void Start () {
    trackedObject = GetComponent<SteamVR_TrackedObject>();
    handSignal = GetComponent<HandSignal>();
	}

	// Update is called once per frame
	void Update () {
    //TOFIX: Compilation placeholder
    device = SteamVR_Input_Sources.Any; //SteamVR_Controller.Input((int)trackedObject.index);

    // send analog input from trigger button to the hand animator
    //TOFIX: Compilation placeholder
    float triggerInput = SteamVR_Input.GetVector2("TriggerInput", SteamVR_Input_Sources.Any).x;//device.GetAxis(Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger).x;
    handAnimator.SetFloat("Closed", triggerInput);

    //TOFIX: Compilation placeholder
    //This should be changed to Steam 2.0 action inputs
    if (SteamVR_Input.GetBooleanAction("TriggerPull").GetStateDown(SteamVR_Input_Sources.Any))
    {
      Grab();
    }

    //TOFIX: Compilation placeholder
    //This should be changed to Steam 2.0 action inputs
    if (SteamVR_Input.GetBooleanAction("TriggerPull").GetStateUp(SteamVR_Input_Sources.Any))
    {
      Drop();
    }

    // check for hand signals and send info to handAnimator
    if (handSignal.CheckHoist)
    {
      handAnimator.SetInteger("Pose", 3);
    } else if (handSignal.CheckStop)
    {
      handAnimator.SetInteger("Pose", 2);
    } else if (handSignal.CheckSwingLeft || handSignal.CheckSwingRight || handSignal.CheckLower)
    {
      handAnimator.SetInteger("Pose", 2);
    } else
    {
      handAnimator.SetInteger("Pose", 0);
    }
  }

  void GestureStart(string gestureName)
  {
    if (gestureEventListener == null)
    {
      Debug.LogWarning("No GestureEventListener");
    }
    else
    {
      gestureEventListener.GestureStart(gestureName);
    }
    currentGesture = gestureName;
  }

  void GestureEnd()
  {
    if (gestureEventListener == null)
    {
      Debug.LogWarning("No GestureEventListener");
    }
    else
    {
      gestureEventListener.GestureEnd(currentGesture);
    }
  }

  void Grab()
  {
    /*Debug.Log("Grab()");

    // get closest grabbable object
    grabbedObject = mobileCraneController.GetClosestHookObject(transform.position);

    if (grabbedObject == null)
    {
      return;
    }

    // make the grabbed object a child of the hand controller
    grabbedObject.GetComponent<Rigidbody>().isKinematic = true;
    grabbedObject.transform.parent = transform;*/
  }

  void Drop()
  {
    /*Debug.Log("Drop()");

    // abort if nothing is grabbed
    if (grabbedObject == null)
    {
      return;
    }

    GameObject hookupObject = loadController.GetClosestRingObject(grabbedObject.transform.position);

    // hookup
    if (hookupObject != null)
    {
      Debug.Log("Hookup! " + grabbedObject.name + " to " + hookupObject.name);

      // set hookX connected
      loadController.SetConnected(hookupObject);
      mobileCraneController.SetConnected(grabbedObject);

      // stop following controller
      grabbedObject.transform.parent = null;
      grabbedObject.GetComponent<Rigidbody>().isKinematic = false;

      FixedJoint fixedJoint = grabbedObject.AddComponent<FixedJoint>();
      fixedJoint.connectedBody = hookupObject.GetComponent<Rigidbody>();
      grabbedObject = null;

      return;
    }

    // drop
    if (grabbedObject.transform.parent == transform)
    {
      Debug.Log("Drop!");
      grabbedObject.transform.parent = null;
      grabbedObject.GetComponent<Rigidbody>().isKinematic = false;
      grabbedObject = null;
    }*/
  }
}
