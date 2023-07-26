using System.Collections;
using System.Collections.Generic;
using Common.Scripts;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.XR.OpenXR.Input;
using Valve.VR;

[RequireComponent(typeof(AudioSource))]
public class DogmanRiggerTutorial : MonoBehaviour
{
  public enum Animation
  {
    Idle,
    Raise,
    Lower,
    Left,
    Right,
    Stop
  }

  public MobileCraneController mobileCraneController;
  public LoadController loadController;

  [Header("GUIs")]
  public GameObject startUI;
  public GameObject restartUI;
  public GameObject leftHandUI;
  public GameObject rightHandUI;
  public GameObject splashUI;
  public GameObject endingPopup;

  [Header("Audio Clips")]
  public AudioClip[] dialogue;

  [Header("Objects")]
  public GameObject load;
  public Transform loadStartPosition;
  public Transform loadRaisePosition;
  public Transform loadEndPosition;
  public GameObject loadEndGizmo;
  public ThreeAxisGizmo gizmo;
  public Animator avatarAnimator;
  public Animator achievementsAnimator;
  public Animator triggerHelpAnimator;
  public Text achievementsText;
  public Text triggerHelpText;

  [Header("Debug")]
  public bool checkProximity;
  private float lastProximity;

  private TutorialManager tutorialManager;
  private AudioSource audioSource;

  //TOFIX: Compilation placeholder
  private VRInputSource headset;
  private bool hasStarted;

  // Use this for initialization
  void Start()
  {
    Debug.Assert(dialogue[0] == null);
    
    //TOFIX: Compilation placeholder
    headset = VRInputSource.Head;
    
    tutorialManager = new TutorialManager();
    audioSource = GetComponent<AudioSource>();

    // setup gizmo
    gizmo.SetTarget(null);
    gizmo.SetHook(mobileCraneController.GetHook().transform);
    gizmo.SetMachineController(mobileCraneController);

    // init
    tutorialManager.AddEvent(new TutorialSetGestureMaskEvent(mobileCraneController, MobileCraneController.GestureMask.None));
    tutorialManager.AddEvent(new TutorialHideGUIEvent(gizmo.leftArrow));
    tutorialManager.AddEvent(new TutorialHideGUIEvent(gizmo.rightArrow));
    tutorialManager.AddEvent(new TutorialHideGUIEvent(gizmo.upArrow));
    tutorialManager.AddEvent(new TutorialHideGUIEvent(gizmo.downArrow));
    //tutorialManager.AddEvent(new TutorialHideGUIEvent(gizmo.inArrow));
    //tutorialManager.AddEvent(new TutorialHideGUIEvent(gizmo.outArrow));

    // init - hide UI
    tutorialManager.AddEvent(new TutorialHideGUIEvent(startUI));
    tutorialManager.AddEvent(new TutorialHideGUIEvent(restartUI));
    tutorialManager.AddEvent(new TutorialHideGUIEvent(leftHandUI));
    tutorialManager.AddEvent(new TutorialHideGUIEvent(rightHandUI));

    // init - hide gizmos
    tutorialManager.AddEvent(new TutorialHideGUIEvent(loadEndGizmo));

    // wait for the start of tutorial
    tutorialManager.AddEvent(new TutorialShowGUIEvent(startUI));
    //tutorialManager.AddEvent(new TutorialWaitForButtonDownEvent("Fire1"));
    //tutorialManager.AddEvent(new TutorialWaitForFunctionTrueEvent(mobileCraneController.GetThumbpadStop));
    tutorialManager.AddEvent(new TutorialVRHeadsetOnEvent(headset));
    tutorialManager.AddEvent(new TutorialHideGUIEvent(splashUI));
    tutorialManager.AddEvent(new TutorialCallFunctionWithBoolValueEvent(SetHasStarted, true));
    tutorialManager.AddEvent(new TutorialHideGUIEvent(startUI));
    tutorialManager.AddEvent(new TutorialWaitForSecondsEvent(3.0f));

    // intro
    tutorialManager.AddEvent(new TutorialPlayAudioEvent(audioSource, dialogue[1]));
    tutorialManager.AddEvent(new TutorialWaitForSecondsEvent(0.5f));
    tutorialManager.AddEvent(new TutorialPlayAudioEvent(audioSource, dialogue[2]));
    tutorialManager.AddEvent(new TutorialWaitForSecondsEvent(0.5f));
    tutorialManager.AddEvent(new TutorialPlayAudioEvent(audioSource, dialogue[3]));
    tutorialManager.AddEvent(new TutorialWaitForSecondsEvent(0.5f));
    tutorialManager.AddEvent(new TutorialPlayAudioEvent(audioSource, dialogue[4]));
    tutorialManager.AddEvent(new TutorialWaitForSecondsEvent(0.5f));

    /*
    // show stop event
    tutorialManager.AddEvent(new TutorialPlayAudioEvent(audioSource, dialogue[5]));
    tutorialManager.AddEvent(new TutorialPlayAnimationEvent(avatarAnimator, Animation.Stop));
    tutorialManager.AddEvent(new TutorialWaitForSecondsEvent(0.5f));
    tutorialManager.AddEvent(new TutorialPlayAudioEvent(audioSource, dialogue[6]));
    tutorialManager.AddEvent(new TutorialWaitForSecondsEvent(0.5f));
    tutorialManager.AddEvent(new TutorialPlayAudioEvent(audioSource, dialogue[7]));

    // wait for stop signal
    tutorialManager.AddEvent(new TutorialSetGestureMaskEvent(mobileCraneController, MobileCraneController.GestureMask.Stop));
    tutorialManager.AddEvent(new TutorialWaitForFunctionTrueEvent(mobileCraneController.GetStop));
    tutorialManager.AddEvent(new TutorialSetGestureMaskEvent(mobileCraneController, MobileCraneController.GestureMask.None));
    tutorialManager.AddEvent(new TutorialAchievementEvent(achievementsAnimator, achievementsText, "Signal Stop"));
    tutorialManager.AddEvent(new TutorialPlayAnimationEvent(avatarAnimator, Animation.Idle));
    tutorialManager.AddEvent(new TutorialPlayAudioEvent(audioSource, dialogue[8]));
    tutorialManager.AddEvent(new TutorialWaitForSecondsEvent(0.5f));
    */

    // show lower hook signal
    tutorialManager.AddEvent(new TutorialPlayAudioEvent(audioSource, dialogue[9]));
    tutorialManager.AddEvent(new TutorialWaitForSecondsEvent(0.5f));
    tutorialManager.AddEvent(new TutorialPlayAnimationEvent(avatarAnimator, Animation.Lower));
    tutorialManager.AddEvent(new TutorialPlayAudioEvent(audioSource, dialogue[10])); // "lower hook looks like this"
    tutorialManager.AddEvent(new TutorialSetGestureMaskEvent(mobileCraneController, MobileCraneController.GestureMask.Lower));
    tutorialManager.AddEvent(new TutorialWaitForSecondsEvent(0.5f));
    //tutorialManager.AddEvent(new TutorialPlayAudioEvent(audioSource, dialogue[11]));
    //tutorialManager.AddEvent(new TutorialWaitForSecondsEvent(0.5f));
    tutorialManager.AddEvent(new TutorialPlayAudioEvent(audioSource, dialogue[12])); // "lower the hook now"
    tutorialManager.AddEvent(new TutorialShowGUIEvent(gizmo.downArrow));

    // encouragement to get hook close to load
    tutorialManager.AddEvent(new TutorialWaitForObjectsInRangeEvent(mobileCraneController.GetHook().transform, load.transform, 5.5f));
    tutorialManager.AddEvent(new TutorialPlayAudioEvent(audioSource, dialogue[13]));
    tutorialManager.AddEvent(new TutorialWaitForObjectsInRangeEvent(mobileCraneController.GetHook().transform, load.transform, 4.2f));
    tutorialManager.AddEvent(new TutorialPlayAudioEvent(audioSource, dialogue[15]));
    tutorialManager.AddEvent(new TutorialWaitForObjectsInRangeEvent(mobileCraneController.GetHook().transform, load.transform, 2.8f));
    tutorialManager.AddEvent(new TutorialAchievementEvent(achievementsAnimator, achievementsText, "Signal Lower Hook"));
    tutorialManager.AddEvent(new TutorialSetGestureMaskEvent(mobileCraneController, MobileCraneController.GestureMask.None));
    tutorialManager.AddEvent(new TutorialPlayAnimationEvent(avatarAnimator, Animation.Idle));
    tutorialManager.AddEvent(new TutorialSetGizmoTargetEvent(gizmo, null));
    tutorialManager.AddEvent(new TutorialPlayAudioEvent(audioSource, dialogue[16]));
    tutorialManager.AddEvent(new TutorialWaitForSecondsEvent(1));

    // now rigging
    tutorialManager.AddEvent(new TutorialTriggerHelpEvent(triggerHelpAnimator, triggerHelpText, "Step1")); // TRIGGER HELP ANIMATION
    tutorialManager.AddEvent(new TutorialShowGUIEvent(leftHandUI));
    tutorialManager.AddEvent(new TutorialShowGUIEvent(rightHandUI));

    tutorialManager.AddEvent(new TutorialCallFunctionWithBoolValueEvent(mobileCraneController.SetHooksEnabled, true));
    tutorialManager.AddEvent(new TutorialPlayAudioEvent(audioSource, dialogue[17])); // "now we'll rig up the load"
    tutorialManager.AddEvent(new TutorialWaitForSecondsEvent(0.5f));
    tutorialManager.AddEvent(new TutorialPlayAudioEvent(audioSource, dialogue[18]));
    tutorialManager.AddEvent(new TutorialWaitForSecondsEvent(0.5f));

    // wait for ropes to connect
    tutorialManager.AddEvent(new TutorialWaitForIntFunctionAtLeastValueEvent(loadController.GetNumRopesConnected, 1));
    //tutorialManager.AddEvent(new TutorialPlayAudioEvent(audioSource, dialogue[19]));
    tutorialManager.AddEvent(new TutorialWaitForIntFunctionAtLeastValueEvent(loadController.GetNumRopesConnected, 2));
    //tutorialManager.AddEvent(new TutorialPlayAudioEvent(audioSource, dialogue[20]));
    tutorialManager.AddEvent(new TutorialWaitForIntFunctionAtLeastValueEvent(loadController.GetNumRopesConnected, 3));
    //tutorialManager.AddEvent(new TutorialPlayAudioEvent(audioSource, dialogue[21]));
    tutorialManager.AddEvent(new TutorialWaitForIntFunctionAtLeastValueEvent(loadController.GetNumRopesConnected, 4));

    tutorialManager.AddEvent(new TutorialTriggerHelpEvent(triggerHelpAnimator, triggerHelpText, "Step2")); // TRIGGER HELP ANIMATION
    tutorialManager.AddEvent(new TutorialHideGUIEvent(leftHandUI));
    tutorialManager.AddEvent(new TutorialHideGUIEvent(rightHandUI));

    tutorialManager.AddEvent(new TutorialPlayAudioEvent(audioSource, dialogue[22]));
    tutorialManager.AddEvent(new TutorialWaitForSecondsEvent(0.5f));

    // show raise load
    tutorialManager.AddEvent(new TutorialPlayAudioEvent(audioSource, dialogue[25])); // "now we need to signal the driver to raise the load"
    tutorialManager.AddEvent(new TutorialSetGestureMaskEvent(mobileCraneController, MobileCraneController.GestureMask.Hoist));

    tutorialManager.AddEvent(new TutorialWaitForSecondsEvent(0.5f));
    tutorialManager.AddEvent(new TutorialPlayAnimationEvent(avatarAnimator, Animation.Raise));
    tutorialManager.AddEvent(new TutorialPlayAudioEvent(audioSource, dialogue[26])); // "looks like this"
    tutorialManager.AddEvent(new TutorialWaitForSecondsEvent(0.5f));
    tutorialManager.AddEvent(new TutorialPlayAudioEvent(audioSource, dialogue[27])); // "raise the hook now"

    // wait for raise load
    tutorialManager.AddEvent(new TutorialShowGUIEvent(gizmo.upArrow));
    tutorialManager.AddEvent(new TutorialWaitForFunctionTrueEvent(mobileCraneController.GetRaiseLoad));
    tutorialManager.AddEvent(new TutorialPlayAudioEvent(audioSource, dialogue[28]));
    //tutorialManager.AddEvent(new TutorialWaitForObjectsInRangeEvent(mobileCraneController.GetHook().transform, loadRaisePosition, 0.5f));
    tutorialManager.AddEvent(new TutorialWaitForObjectAboveHeightEvent(mobileCraneController.GetHook().transform, 19.25f));
    tutorialManager.AddEvent(new TutorialAchievementEvent(achievementsAnimator, achievementsText, "Signal Raise Hook"));
    tutorialManager.AddEvent(new TutorialHideGUIEvent(gizmo.upArrow));
    tutorialManager.AddEvent(new TutorialPlayAnimationEvent(avatarAnimator, Animation.Idle));
    tutorialManager.AddEvent(new TutorialSetGestureMaskEvent(mobileCraneController, MobileCraneController.GestureMask.None));
    tutorialManager.AddEvent(new TutorialPlayAudioEvent(audioSource, dialogue[29]));
    tutorialManager.AddEvent(new TutorialWaitForSecondsEvent(0.5f));

    // show slew
    tutorialManager.AddEvent(new TutorialPlayAudioEvent(audioSource, dialogue[30]));
    tutorialManager.AddEvent(new TutorialSetGestureMaskEvent(mobileCraneController, MobileCraneController.GestureMask.Left));

    tutorialManager.AddEvent(new TutorialShowGUIEvent(loadEndGizmo));
    tutorialManager.AddEvent(new TutorialWaitForSecondsEvent(0.5f));
    tutorialManager.AddEvent(new TutorialPlayAnimationEvent(avatarAnimator, Animation.Left));
    tutorialManager.AddEvent(new TutorialPlayAudioEvent(audioSource, dialogue[31]));
    tutorialManager.AddEvent(new TutorialWaitForSecondsEvent(0.5f));
    //tutorialManager.AddEvent(new TutorialPlayAudioEvent(audioSource, dialogue[32]));
    //tutorialManager.AddEvent(new TutorialWaitForSecondsEvent(0.5f));
    //tutorialManager.AddEvent(new TutorialPlayAudioEvent(audioSource, dialogue[33]));
    tutorialManager.AddEvent(new TutorialShowGUIEvent(gizmo.leftArrow));
    tutorialManager.AddEvent(new TutorialWaitForSecondsEvent(0.5f));

    // encouragement to get load close to destination
    tutorialManager.AddEvent(new TutorialWaitForObjectsInXZRangeEvent(mobileCraneController.GetHook().transform, loadEndPosition, 7.0f));
    tutorialManager.AddEvent(new TutorialPlayAudioEvent(audioSource, dialogue[34]));
    tutorialManager.AddEvent(new TutorialWaitForObjectsInXZRangeEvent(mobileCraneController.GetHook().transform, loadEndPosition, 4.0f));
    tutorialManager.AddEvent(new TutorialPlayAudioEvent(audioSource, dialogue[35]));
    tutorialManager.AddEvent(new TutorialWaitForObjectsInXZRangeEvent(mobileCraneController.GetHook().transform, loadEndPosition, 0.5f));
    tutorialManager.AddEvent(new TutorialAchievementEvent(achievementsAnimator, achievementsText, "Slew Load"));
    tutorialManager.AddEvent(new TutorialSetGestureMaskEvent(mobileCraneController, MobileCraneController.GestureMask.None));
    tutorialManager.AddEvent(new TutorialSetGizmoTargetEvent(gizmo, null));
    tutorialManager.AddEvent(new TutorialPlayAnimationEvent(avatarAnimator, Animation.Idle));

    /*
    // now ask for stop
    //tutorialManager.AddEvent(new TutorialPlayAudioEvent(audioSource, dialogue[36]));
    //tutorialManager.AddEvent(new TutorialPlayAnimationEvent(avatarAnimator, Animation.Stop));

    // wait for stop
    //tutorialManager.AddEvent(new TutorialSetGestureMaskEvent(mobileCraneController, MobileCraneController.GestureMask.Stop));
    //tutorialManager.AddEvent(new TutorialWaitForFunctionTrueEvent(mobileCraneController.GetStop));
    //tutorialManager.AddEvent(new TutorialSetGestureMaskEvent(mobileCraneController, MobileCraneController.GestureMask.None));
    //tutorialManager.AddEvent(new TutorialAchievementEvent(achievementsAnimator, achievementsText, "Signal Stop"));
    */
    tutorialManager.AddEvent(new TutorialPlayAnimationEvent(avatarAnimator, Animation.Idle));
    tutorialManager.AddEvent(new TutorialPlayAudioEvent(audioSource, dialogue[37]));
    tutorialManager.AddEvent(new TutorialWaitForSecondsEvent(0.5f));

    // now ask for lower
    tutorialManager.AddEvent(new TutorialPlayAudioEvent(audioSource, dialogue[38]));
    tutorialManager.AddEvent(new TutorialWaitForSecondsEvent(0.5f));
    tutorialManager.AddEvent(new TutorialSetGestureMaskEvent(mobileCraneController, MobileCraneController.GestureMask.Lower));
    tutorialManager.AddEvent(new TutorialPlayAudioEvent(audioSource, dialogue[39]));
    tutorialManager.AddEvent(new TutorialShowGUIEvent(gizmo.downArrow));
    tutorialManager.AddEvent(new TutorialPlayAnimationEvent(avatarAnimator, Animation.Lower));

    // wait for lower
    tutorialManager.AddEvent(new TutorialWaitForObjectsInRangeEvent(mobileCraneController.GetHook().transform, loadEndPosition, 1.8f));
    tutorialManager.AddEvent(new TutorialAchievementEvent(achievementsAnimator, achievementsText, "Signal Lower Hook"));
    tutorialManager.AddEvent(new TutorialPlayAnimationEvent(avatarAnimator, Animation.Idle));
    tutorialManager.AddEvent(new TutorialSetGestureMaskEvent(mobileCraneController, MobileCraneController.GestureMask.None));
    tutorialManager.AddEvent(new TutorialHideGUIEvent(gizmo.downArrow));

    // great, finish up
    tutorialManager.AddEvent(new TutorialPlayAudioEvent(audioSource, dialogue[40]));
    tutorialManager.AddEvent(new TutorialHideGUIEvent(loadEndGizmo));
    tutorialManager.AddEvent(new TutorialWaitForSecondsEvent(3f));
    //tutorialManager.AddEvent(new TutorialAchievementEvent(achievementsAnimator, achievementsText, "Finish Training"));
    tutorialManager.AddEvent(new TutorialEndingEvent(endingPopup));
    tutorialManager.AddEvent(new TutorialWaitForSecondsEvent(2));
    tutorialManager.AddEvent(new TutorialShowGUIEvent(restartUI));
    tutorialManager.AddEvent(new TutorialWaitForSecondsEvent(10));

    // reload scene
    tutorialManager.AddEvent(new TutorialWaitForSecondsEvent(2));
    tutorialManager.AddEvent(new TutorialReloadSceneEvent());

    tutorialManager.Restart(gameObject);
  }

  // Update is called once per frame
  void Update()
  {
    if (tutorialManager.IsComplete())
    {
      Reset();
      tutorialManager.Restart(gameObject);
    }
    else
    {
      tutorialManager.Update(gameObject);
    }

    // check proximity sensor
    //TOFIX: Compilation placeholder

    bool proximitySensor = ByteSprite.VR.IsHeadsetWorn();//SteamVR_Input.GetBooleanAction("HeadsetOnHead").GetStateDown(VRInputSource.Head); //headset.GetPress(Valve.VR.EVRButtonId.k_EButton_ProximitySensor);

    if (!hasStarted || proximitySensor || !checkProximity)
    {
      lastProximity = Time.time;
    }

    // if proximity gone too long, restart scene
    if (Time.time >= lastProximity + TutorialManager.PROXIMITY_SENSOR_TIMEOUT)
    {
      Debug.Log("User left VR. Restarting...");
      ReloadScene();
    }
  }

  // reset crane and load back to starting position ready for running the tutorial again
  private void Reset()
  {
    ReplaceLoad();
    mobileCraneController.Reset();
    SetHasStarted(false);
  }

  private void ReloadScene()
  {
    SceneManager.LoadScene(SceneManager.GetActiveScene().name);
  }

  public void ReplaceLoad()
  {
    Debug.Log("ReplaceLoad()");
    load.transform.position = new Vector3(loadStartPosition.transform.position.x, loadStartPosition.transform.position.y, loadStartPosition.transform.position.z);
    load.transform.rotation = Quaternion.Euler(loadStartPosition.transform.rotation.eulerAngles.x, loadStartPosition.transform.rotation.eulerAngles.y, loadStartPosition.transform.rotation.eulerAngles.z);
  }

  public void SetHasStarted(bool hasStarted)
  {
    Debug.Log("SetHasStarted():" + hasStarted);
    this.hasStarted = hasStarted;
  }
}


