using System.Collections;
using System.Collections.Generic;
using Common.Scripts;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.XR;
using Valve.VR;

public abstract class TutorialEvent
{
  public virtual void Begin(GameObject manager)
  {
  }

  public virtual bool IsComplete(GameObject manager)
  {
    return true;
  }

  public virtual void End(GameObject manager)
  {
  }
}

public class TutorialShowGUIEvent : TutorialEvent
{
  public GameObject gui;

  public TutorialShowGUIEvent(GameObject gui)
  {
    this.gui = gui;
  }

  public override void Begin(GameObject manager)
  {
    base.Begin(manager);

    Debug.Log("TutorialShowGUIEvent::Begin()");

    gui.SetActive(true);
  }

  public override bool IsComplete(GameObject manager)
  {
    return gui.activeInHierarchy;
  }

  public override void End(GameObject manager)
  {
    base.End(manager);

    Debug.Log("TutorialShowGUIEvent::End()");
  }
}

public class TutorialHideGUIEvent : TutorialEvent
{
  public GameObject gui;

  public TutorialHideGUIEvent(GameObject gui)
  {
    this.gui = gui;
  }

  public override void Begin(GameObject manager)
  {
    base.Begin(manager);

    Debug.Log("TutorialHideGUIEvent::Begin()");

    gui.SetActive(false);
  }

  public override bool IsComplete(GameObject manager)
  {
    return !gui.activeInHierarchy;
  }

  public override void End(GameObject manager)
  {
    base.End(manager);

    Debug.Log("TutorialHideGUIEvent::End()");
  }
}

public class TutorialPlayAudioEvent : TutorialEvent
{
  public AudioSource audioSource;
  public AudioClip audioClip;

  public TutorialPlayAudioEvent(AudioSource audioSource, AudioClip audioClip)
  {
    this.audioSource = audioSource;
    this.audioClip = audioClip;
  }

  public override void Begin(GameObject manager)
  {
    base.Begin(manager);

    Debug.Log("TutorialPlayAudioEvent::Begin()");

    audioSource.PlayOneShot(audioClip);
  }

  public override bool IsComplete(GameObject manager)
  {
    return !audioSource.isPlaying;
  }

  public override void End(GameObject manager)
  {
    base.End(manager);

    Debug.Log("TutorialPlayAudioEvent::End()");
  }
}

public class TutorialPlayAnimationEvent : TutorialEvent
{
  public Animator animator;
  public DogmanRiggerTutorial.Animation animation;

  public TutorialPlayAnimationEvent(Animator animator, DogmanRiggerTutorial.Animation animation)
  {
    this.animator = animator;
    this.animation = animation;
  }

  public override void Begin(GameObject manager)
  {
    base.Begin(manager);

    Debug.Log("TutorialPlayAnimationEvent::Begin()");

    animator.SetInteger("CurrentGesture", (int) animation);
  }

  public override bool IsComplete(GameObject manager)
  {
    return base.IsComplete(manager);
  }

  public override void End(GameObject manager)
  {
    base.End(manager);

    Debug.Log("TutorialPlayAnimationEvent::End()");
  }
}

public class TutorialSetAnimationBoolEvent : TutorialEvent
{
  public Animator animator;
  public string parameter;
  public bool state;

  public TutorialSetAnimationBoolEvent(Animator animator, string parameter, bool state)
  {
    this.animator = animator;
    this.parameter = parameter;
    this.state = state;
  }

  public override void Begin(GameObject manager)
  {
    base.Begin(manager);

    Debug.Log("TutorialSetAnimationBoolEvent::Begin()");

    animator.SetBool(parameter, state);
  }

  public override bool IsComplete(GameObject manager)
  {
    return base.IsComplete(manager);
  }

  public override void End(GameObject manager)
  {
    base.End(manager);

    Debug.Log("TutorialSetAnimationBoolEvent::End()");
  }
}

public class TutorialWaitForButtonDownEvent : TutorialEvent
{
  public string buttonName;

  public TutorialWaitForButtonDownEvent(string buttonName)
  {
    this.buttonName = buttonName;
  }

  public override void Begin(GameObject manager)
  {
    base.Begin(manager);

    Debug.Log("TutorialWaitForButtonDownEvent::Begin()");
  }

  public override bool IsComplete(GameObject manager)
  {
    return Input.GetButtonDown(buttonName);
  }

  public override void End(GameObject manager)
  {
    base.End(manager);

    Debug.Log("TutorialWaitForButtonDownEvent::End()");
  }
}

public class TutorialWaitForFunctionTrueEvent : TutorialEvent
{
  public delegate bool MyDelegateType();
  MyDelegateType function;

  public TutorialWaitForFunctionTrueEvent(MyDelegateType function)
  {
    this.function = function;
  }

  public override void Begin(GameObject manager)
  {
    base.Begin(manager);

    Debug.Log("TutorialWaitForFunctionTrueEvent::Begin()");
  }

  public override bool IsComplete(GameObject manager)
  {
    return function();
  }

  public override void End(GameObject manager)
  {
    base.End(manager);

    Debug.Log("TutorialWaitForFunctionTrueEvent::End()");
  }
}

public class TutorialWaitForTwoFunctionTrueEvent : TutorialEvent
{
  public delegate bool MyDelegateType();
  MyDelegateType function0;
  MyDelegateType function1;
  bool function0HasBeenTrue;
  bool function1HasBeenTrue;

  public TutorialWaitForTwoFunctionTrueEvent(MyDelegateType function0, MyDelegateType function1)
  {
    this.function0 = function0;
    this.function1 = function1;
    function0HasBeenTrue = false;
    function1HasBeenTrue = false;
  }

  public override void Begin(GameObject manager)
  {
    base.Begin(manager);

    Debug.Log("TutorialWaitForTwoFunctionTrueEvent::Begin()");
  }

  public override bool IsComplete(GameObject manager)
  {
    if (function0())
      function0HasBeenTrue = true;
    if (function1())
      function1HasBeenTrue = true;

    return function0HasBeenTrue && function1HasBeenTrue;
  }

  public override void End(GameObject manager)
  {
    base.End(manager);

    Debug.Log("TutorialWaitForTwoFunctionTrueEvent::End()");
  }
}

public class TutorialWaitForFunctionTrueForDurationEvent : TutorialEvent
{
  public delegate bool MyDelegateType();
  MyDelegateType function;
  float duration;
  private float startTime;
  private bool isTrue = false;

  public TutorialWaitForFunctionTrueForDurationEvent(MyDelegateType function, float duration)
  {
    this.function = function;
    this.duration = duration;
  }

  public override void Begin(GameObject manager)
  {
    base.Begin(manager);

    Debug.Log("TutorialWaitForFunctionTrueForDurationEvent::Begin()");
  }

  public override bool IsComplete(GameObject manager)
  {
    bool stillTrue = function();

    if (isTrue)
    {
      if (stillTrue)
      {
        //check time
        if (Time.time >= startTime + duration)
        {
          return true;
        }
      }
      else
      {
        isTrue = false;
      }
    }
    else
    {
      if (stillTrue) // just became true
      {
        startTime = Time.time;
        isTrue = true;
      }
    }
    return false;
  }

  public override void End(GameObject manager)
  {
    base.End(manager);

    Debug.Log("TutorialWaitForFunctionTrueForDurationEvent::End()");
  }
}

public class TutorialWaitForIntFunctionValueEvent : TutorialEvent
{
  public delegate int MyDelegateType();
  MyDelegateType function;
  int value;

  public TutorialWaitForIntFunctionValueEvent(MyDelegateType function, int value)
  {
    this.function = function;
    this.value = value;
  }

  public override void Begin(GameObject manager)
  {
    base.Begin(manager);

    Debug.Log("TutorialWaitForIntFunctionValueEvent::Begin()");
  }

  public override bool IsComplete(GameObject manager)
  {
    return function() == value;
  }

  public override void End(GameObject manager)
  {
    base.End(manager);

    Debug.Log("TutorialWaitForIntFunctionValueEvent::End()");
  }
}

public class TutorialWaitForIntFunctionAtLeastValueEvent : TutorialEvent
{
  public delegate int MyDelegateType();
  MyDelegateType function;
  int value;

  public TutorialWaitForIntFunctionAtLeastValueEvent(MyDelegateType function, int value)
  {
    this.function = function;
    this.value = value;
  }

  public override void Begin(GameObject manager)
  {
    base.Begin(manager);

    Debug.Log("TutorialWaitForIntFunctionAtLeastValueEvent::Begin()");
  }

  public override bool IsComplete(GameObject manager)
  {
    return function() >= value;
  }

  public override void End(GameObject manager)
  {
    base.End(manager);

    Debug.Log("TutorialWaitForIntFunctionAtLeastValueEvent::End()");
  }
}

public class TutorialWaitForFloatFunctionGreaterThanValueEvent : TutorialEvent
{
  public delegate float MyDelegateType();
  MyDelegateType function;
  float value;

  public TutorialWaitForFloatFunctionGreaterThanValueEvent(MyDelegateType function, float value)
  {
    this.function = function;
    this.value = value;
  }

  public override void Begin(GameObject manager)
  {
    base.Begin(manager);

    Debug.Log("TutorialWaitForFloatFunctionGreaterThanValueEvent::Begin()");
  }

  public override bool IsComplete(GameObject manager)
  {
    float eval = function();
    bool result = eval > value;
    Debug.Log("eval: " + eval + " value: " + value + " IsComplete: " + result);
    return function() > value;
  }

  public override void End(GameObject manager)
  {
    base.End(manager);

    Debug.Log("TutorialWaitForFloatFunctionGreaterThanValueEvent::End()");
  }
}

public class TutorialWaitForFloatFunctionLessThanValueEvent : TutorialEvent
{
  public delegate float MyDelegateType();
  MyDelegateType function;
  float value;

  public TutorialWaitForFloatFunctionLessThanValueEvent(MyDelegateType function, float value)
  {
    this.function = function;
    this.value = value;
  }

  public override void Begin(GameObject manager)
  {
    base.Begin(manager);

    Debug.Log("TutorialWaitForFloatFunctionLessThanValueEvent::Begin()");
  }

  public override bool IsComplete(GameObject manager)
  {
    float eval = function();
    bool result = eval < value;
    Debug.Log("eval: " + eval + " value: " + value + " IsComplete: " + result);
    return function() < value;
  }

  public override void End(GameObject manager)
  {
    base.End(manager);

    Debug.Log("TutorialWaitForFloatFunctionLessThanValueEvent::End()");
  }
}

public class TutorialCallFunctionWithBoolValueEvent : TutorialEvent
{
  public delegate void MyDelegateType(bool value);
  MyDelegateType function;
  bool value;

  public TutorialCallFunctionWithBoolValueEvent(MyDelegateType function, bool value)
  {
    this.function = function;
    this.value = value;
  }

  public override void Begin(GameObject manager)
  {
    base.Begin(manager);

    Debug.Log("TutorialCallFunctionWithBoolValueEvent::Begin()");

    function(value);
  }

  public override bool IsComplete(GameObject manager)
  {
    return base.IsComplete(manager);
  }

  public override void End(GameObject manager)
  {
    base.End(manager);

    Debug.Log("TutorialCallFunctionWithBoolValueEvent::End()");
  }
}

public class TutorialWaitForSecondsEvent : TutorialEvent
{
  float beginTime;
  float duration;

  public TutorialWaitForSecondsEvent(float duration)
  {
    this.duration = duration;
  }

  public override void Begin(GameObject manager)
  {
    base.Begin(manager);

    Debug.Log("TutorialWaitForSecondsEvent::Begin()");

    beginTime = Time.time;
  }

  public override bool IsComplete(GameObject manager)
  {
    return Time.time > (this.beginTime + this.duration);
  }

  public override void End(GameObject manager)
  {
    base.End(manager);

    Debug.Log("TutorialWaitForSecondsEvent::End()");
  }
}

public class TutorialWaitForObjectAboveHeightEvent : TutorialEvent
{
  Transform object0;
  float height;

  public TutorialWaitForObjectAboveHeightEvent(Transform object0, float height)
  {
    this.object0 = object0;
    this.height = height;
  }

  public override void Begin(GameObject manager)
  {
    base.Begin(manager);

    Debug.Log("TutorialWaitForObjectAboveHeightEvent::Begin()");
  }

  public override bool IsComplete(GameObject manager)
  {
    //Debug.Log("object at height: " + object0.position.y + " required height: " + height);
    return object0.position.y >= height;
  }

  public override void End(GameObject manager)
  {
    base.End(manager);

    Debug.Log("TutorialWaitForObjectAboveHeightEvent::End()");
  }
}

public class TutorialWaitForObjectsInRangeEvent : TutorialEvent
{
  Transform object0;
  Transform object1;
  float range;

  public TutorialWaitForObjectsInRangeEvent(Transform object0, Transform object1, float range)
  {
    this.object0 = object0;
    this.object1 = object1;
    this.range = range;
  }

  public override void Begin(GameObject manager)
  {
    base.Begin(manager);

    Debug.Log("TutorialWaitForObjectsInRangeEvent::Begin()");
  }

  public override bool IsComplete(GameObject manager)
  {
    float dist = Vector3.Distance(object0.position, object1.position);
    Debug.Log("object0 (" + object0.name + ") : " + object0.position + " object1 (" + object1.name + ") : " + object1.position + " dist: " + dist + " range: " + range);
    return dist <= range;
  }

  public override void End(GameObject manager)
  {
    base.End(manager);

    Debug.Log("TutorialWaitForObjectsInRangeEvent::End()");
  }
}

public class TutorialWaitForObjectsInXZRangeEvent : TutorialEvent
{
  Transform object0;
  Transform object1;
  float range;

  public TutorialWaitForObjectsInXZRangeEvent(Transform object0, Transform object1, float range)
  {
    this.object0 = object0;
    this.object1 = object1;
    this.range = range;
  }

  public override void Begin(GameObject manager)
  {
    base.Begin(manager);

    Debug.Log("TutorialWaitForObjectsInXZRangeEvent::Begin()");
  }

  public override bool IsComplete(GameObject manager)
  {
    float x_diff = object0.position.x - object1.position.x;
    float z_diff = object0.position.z - object1.position.z;
    float dist = Mathf.Sqrt(Mathf.Pow(x_diff, 2) + Mathf.Pow(z_diff, 2));
    Debug.Log("object0 (" + object0.name + ") : " + object0.position + " object1 (" + object1.name + ") : " + object1.position + " dist: " + dist + " range: " + range);
    return dist <= range;
  }

  public override void End(GameObject manager)
  {
    base.End(manager);

    Debug.Log("TutorialWaitForObjectsInXZRangeEvent::End()");
  }
}

public class TutorialEnableMachineEvent : TutorialEvent
{
  MachineController machineController;
  bool enable;

  public TutorialEnableMachineEvent(MachineController machineController, bool enable)
  {
    this.machineController = machineController;
    this.enable = enable;
  }

  public override void Begin(GameObject manager)
  {
    base.Begin(manager);

    Debug.Log("TutorialEnableMachineEvent::Begin()");

    machineController.SetMovementEnabled(enable);
  }

  public override bool IsComplete(GameObject manager)
  {
    return machineController.GetMovementEnabled() == enable;
  }

  public override void End(GameObject manager)
  {
    base.End(manager);

    Debug.Log("TutorialEnableMachineEvent::End()");
  }
}

public class TutorialSetGestureMaskEvent : TutorialEvent
{
  MobileCraneController mobileCraneController;
  MobileCraneController.GestureMask gestureMask;

  public TutorialSetGestureMaskEvent(MobileCraneController mobileCraneController, MobileCraneController.GestureMask gestureMask)
  {
    this.mobileCraneController = mobileCraneController;
    this.gestureMask = gestureMask;
  }

  public override void Begin(GameObject manager)
  {
    base.Begin(manager);

    Debug.Log("TutorialSetGestureMaskEvent::Begin()");

    mobileCraneController.SetGestureMask(gestureMask);
  }

  public override bool IsComplete(GameObject manager)
  {
    return mobileCraneController.GetGestureMask() == gestureMask;
  }

  public override void End(GameObject manager)
  {
    base.End(manager);

    Debug.Log("TutorialSetGestureMaskEvent::End()");
  }
}

public class TutorialSetControlMaskEvent : TutorialEvent
{
  CraneController craneController;
  CraneController.ControlMask controlMask;

  public TutorialSetControlMaskEvent(CraneController craneController, CraneController.ControlMask controlMask)
  {
    this.craneController = craneController;
    this.controlMask = controlMask;
  }

  public override void Begin(GameObject manager)
  {
    base.Begin(manager);

    Debug.Log("TutorialSetControlMaskEvent::Begin()");

    craneController.SetControlMask(controlMask);
  }

  public override bool IsComplete(GameObject manager)
  {
    return craneController.GetControlMask() == controlMask;
  }

  public override void End(GameObject manager)
  {
    base.End(manager);

    Debug.Log("TutorialSetControlMaskEvent::End()");
  }
}

public class TutorialMakeParentEvent : TutorialEvent
{
  GameObject parent;
  GameObject child;

  public TutorialMakeParentEvent(GameObject parent, GameObject child)
  {
    this.parent = parent;
    this.child = child;
  }

  public override void Begin(GameObject manager)
  {
    base.Begin(manager);

    Debug.Log("TutorialMakeParentEvent::Begin()");

    if (parent == null)
    {
      child.transform.SetParent(null);
      //child.GetComponent<Rigidbody>().isKinematic = false;
    }
    else
    {
      child.transform.SetParent(parent.transform.parent);
      //child.GetComponent<Rigidbody>().isKinematic = true;
    }
  }

  public override bool IsComplete(GameObject manager)
  {
    return base.IsComplete(manager);
  }

  public override void End(GameObject manager)
  {
    base.End(manager);

    Debug.Log("TutorialMakeParentEvent::End()");
  }
}

public class TutorialSetGizmoTargetEvent : TutorialEvent
{
  ThreeAxisGizmo gizmo;
  Transform target;

  public TutorialSetGizmoTargetEvent(ThreeAxisGizmo gizmo, Transform target)
  {
    this.gizmo = gizmo;
    this.target = target;
  }

  public override void Begin(GameObject manager)
  {
    base.Begin(manager);

    Debug.Log("TutorialSetGizmoTargetEvent::Begin()");

    gizmo.SetTarget(target);
  }

  public override bool IsComplete(GameObject manager)
  {
    return base.IsComplete(manager);
  }

  public override void End(GameObject manager)
  {
    base.End(manager);

    Debug.Log("TutorialSetGizmoTargetEvent::End()");
  }
}

public class TutorialReloadSceneEvent : TutorialEvent
{
  public TutorialReloadSceneEvent()
  {
  }

  public override void Begin(GameObject manager)
  {
    base.Begin(manager);

    Debug.Log("TutorialReloadSceneEvent::Begin()");

    SceneManager.LoadScene(SceneManager.GetActiveScene().name);
  }

  public override bool IsComplete(GameObject manager)
  {
    return base.IsComplete(manager);
  }

  public override void End(GameObject manager)
  {
    base.End(manager);

    Debug.Log("TutorialReloadSceneEvent::End()");
  }
}

public class TutorialCraneHookupLoadEvent : TutorialEvent
{
  CraneController craneController;
  LoadController loadController;

  public TutorialCraneHookupLoadEvent(CraneController craneController, LoadController loadController)
  {
    this.craneController = craneController;
    this.loadController = loadController;
  }

  public override void Begin(GameObject manager)
  {
    base.Begin(manager);

    Debug.Log("TutorialCraneHookupLoadEvent::Begin()");
        /*
        // move load (nicely)
        //loadController.transform.position = craneController.hook.transform.position + new Vector3(0, 5f, 0);
        loadController.GetComponent<Rigidbody>().MovePosition(craneController.hook.transform.position + new Vector3(0, 5f, 0));

        // connect load
        GameObject hook0 = craneController.hook0;
        GameObject hook1 = craneController.hook1;
        GameObject hook2 = craneController.hook2;
        GameObject hook3 = craneController.hook3;

        GameObject ring0 = loadController.ring0;
        GameObject ring1 = loadController.ring1;
        GameObject ring2 = loadController.ring2;
        GameObject ring3 = loadController.ring3;

        // set hook0 connected
        hook0.transform.position = new Vector3(ring0.transform.position.x, ring0.transform.position.y, ring0.transform.position.z);
        loadController.SetConnected(ring0);
        FixedJoint fixedJoint0 = hook0.AddComponent<FixedJoint>();
        fixedJoint0.connectedBody = ring0.GetComponent<Rigidbody>();

        // set hook1 connected
        hook1.transform.position = new Vector3(ring1.transform.position.x, ring1.transform.position.y, ring1.transform.position.z);
        loadController.SetConnected(ring1);
        FixedJoint fixedJoint1 = hook1.AddComponent<FixedJoint>();
        fixedJoint1.connectedBody = ring1.GetComponent<Rigidbody>();

        // set hook2 connected
        hook2.transform.position = new Vector3(ring2.transform.position.x, ring2.transform.position.y, ring2.transform.position.z);
        loadController.SetConnected(ring2);
        FixedJoint fixedJoint2 = hook2.AddComponent<FixedJoint>();
        fixedJoint2.connectedBody = ring2.GetComponent<Rigidbody>();

        // set hook3 connected
        hook3.transform.position = new Vector3(ring3.transform.position.x, ring3.transform.position.y, ring3.transform.position.z);
        loadController.SetConnected(ring3);
        FixedJoint fixedJoint3 = hook3.AddComponent<FixedJoint>();
        fixedJoint3.connectedBody = ring3.GetComponent<Rigidbody>();
        */
        loadController.transform.SetParent(craneController.hook.transform);
        loadController.transform.localPosition = new Vector3(0f, -4.25f, 0f);
  }

  public override bool IsComplete(GameObject manager)
  {
    return base.IsComplete(manager);
  }

  public override void End(GameObject manager)
  {
    base.End(manager);

    Debug.Log("TutorialCraneHookupLoadEvent::End()");
  }
}

public class TutorialAchievementEvent : TutorialEvent
{
  Animator animator;
  Text text;
  string achievement;

  public TutorialAchievementEvent(Animator animator, Text text, string achievement)
  {
    this.animator = animator;
    this.text = text;
    this.achievement = achievement;
  }

  public override void Begin(GameObject manager)
  {
    base.Begin(manager);

    Debug.Log("TutorialAchievementEvent::Begin()");

    text.text = "Achievement\n\n" + achievement;
    animator.SetTrigger("NextAchievement");
  }

  public override bool IsComplete(GameObject manager)
  {
    return base.IsComplete(manager);
  }

  public override void End(GameObject manager)
  {
    base.End(manager);

    Debug.Log("TutorialAchievementEvent::End()");
  }
}

public class TutorialTriggerHelpEvent : TutorialEvent
{
  Animator animator;
  Text text;
  string achievement;

  public TutorialTriggerHelpEvent(Animator animator, Text text, string achievement)
  {
    this.animator = animator;
    this.text = text;
    this.achievement = achievement;
  }

  public override void Begin(GameObject manager)
  {
    base.Begin(manager);

    Debug.Log("TutorialTriggerHelpEvent::Begin()");

    text.text = "Achievement\n\n" + achievement;
    animator.SetTrigger("NextAchievement");
  }

  public override bool IsComplete(GameObject manager)
  {
    return base.IsComplete(manager);
  }

  public override void End(GameObject manager)
  {
    base.End(manager);

    Debug.Log("TutorialTriggerHelpEvent::End()");
  }
}

public class TutorialVRHeadsetOnEvent : TutorialEvent
{
  
  //TOFIX: Compilation placeholder
  public TutorialVRHeadsetOnEvent()
  {
  }

  public override void Begin(GameObject manager)
  {
    base.Begin(manager);

    Debug.Log("TutorialVRHeadsetOnEvent::Begin() : use 'P' key to trigger manually.");
  }

  public override bool IsComplete(GameObject manager)
  {
    //TOFIX: Compilation placeholder
    bool proximitySensor = ByteSprite.VR.IsHeadsetWorn();

    return proximitySensor || Input.GetKey(KeyCode.P);
  }

  public override void End(GameObject manager)
  {
    base.End(manager);

    Debug.Log("TutorialVRHeadsetOnEvent::End()");
  }
}

public class TutorialWaitForAnyInputEvent : TutorialEvent
{
  MachineController machineController;

  public TutorialWaitForAnyInputEvent(MachineController machineController)
  {
    this.machineController = machineController;
  }

  public override void Begin(GameObject manager)
  {
    base.Begin(manager);

    Debug.Log("TutorialWaitForAnyInputEvent::Begin()");
  }

  public override bool IsComplete(GameObject manager)
  {
    return machineController.HasAnyInput();
  }

  public override void End(GameObject manager)
  {
    base.End(manager);

    Debug.Log("TutorialWaitForAnyInputEvent::End()");
  }
}

public class TutorialImpossibleEvent : TutorialEvent
{
  public TutorialImpossibleEvent()
  {
  }

  public override void Begin(GameObject manager)
  {
    base.Begin(manager);

    Debug.Log("TutorialImpossibleEvent::Begin()");
  }

  public override bool IsComplete(GameObject manager)
  {
    return false;
  }

  public override void End(GameObject manager)
  {
    base.End(manager);

    Debug.LogError("TutorialImpossibleEvent::End()");
  }
}

public class TutorialAutoconnectToHookEvent : TutorialEvent
{
    public TutorialAutoconnectToHookEvent()
    {
    }

    public override void Begin(GameObject manager)
    {
        base.Begin(manager);

        Debug.Log("TutorialAutoconnectToHookEvent::Begin()");
    }

    public override bool IsComplete(GameObject manager)
    {
        foreach (AutoconnectToHook a in GameObject.FindObjectsOfType<AutoconnectToHook>())
        {
            if (a.levelComplete == false)
                return false;
        }

        return true;
    }

    public override void End(GameObject manager)
    {
        base.End(manager);

        Debug.Log("TutorialAutoconnectToHookEvent::End()");
    }
}

public class TutorialEndingEvent : TutorialEvent
{
  GameObject endingPopup;

  public TutorialEndingEvent(GameObject endingPopup)
  {
    this.endingPopup = endingPopup;
  }

  public override void Begin(GameObject manager)
  {
    base.Begin(manager);

    Debug.Log("TutorialEndingEvent::Begin()");

    BGM.GetInstance().PlayHighScoreBGM();
    endingPopup.SetActive(true);
  }

  public override bool IsComplete(GameObject manager)
  {
    return base.IsComplete(manager);
  }

  public override void End(GameObject manager)
  {
    base.End(manager);

    Debug.Log("TutorialEndingEvent::End()");
  }
}