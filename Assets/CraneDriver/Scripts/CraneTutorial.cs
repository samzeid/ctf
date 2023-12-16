using Common.Scripts;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Valve.VR;

[RequireComponent(typeof(AudioSource))]
public class CraneTutorial : MonoBehaviour
{
  [Header("Controllers")]
  public CraneController craneController;
  public LoadController loadController;

  [Header("GUIs")]
  public GameObject startUI;
  public GameObject restartUI;
  public GameObject splashUI;
  public GameObject endingPopup;

  /*
  public GameObject stage1UI;
  public GameObject slewLeftIncomplete;
  public GameObject slewLeftComplete;
  public GameObject slewRightIncomplete;
  public GameObject slewRightComplete;
  public GameObject trolleyOutIncomplete;
  public GameObject trolleyOutComplete;
  public GameObject trolleyInIncomplete;
  public GameObject trolleyInComplete;
  public GameObject hookDownIncomplete;
  public GameObject hookDownComplete;
  public GameObject hookUpIncomplete;
  public GameObject hookUpComplete;

  public GameObject stage2UI;
  public GameObject moveHookIncomplete;
  public GameObject moveHookComplete;
  public GameObject hookUpLoadIncomplete;
  public GameObject hookUpLoadComplete;
  public GameObject pickUpLoadIncomplete;
  public GameObject pickUpLoadComplete;
  */

  [Header("Audio Clips")]
  public AudioClip[] dialogue;

  [Header("Objects")]
  public GameObject hook;
  public GameObject load;
  public Transform loadStartPosition;
  public Transform loadEndPosition;
  public GameObject loadStartGizmo;
  public GameObject loadEndGizmo;
  public ThreeAxisGizmo gizmo;
  public Animator achievementsAnimator;
  public Text achievementsText;

  [Header("Debug")]
  public bool checkProximity;
  private float lastProximity;

  private TutorialManager tutorialManager = new TutorialManager();
  private AudioSource audioSource;

  private bool hasStarted;

  // Use this for initialization
  void Start()
  {
    Debug.Assert(dialogue[0] == null);

    audioSource = GetComponent<AudioSource>();

    // setup gizmo
    gizmo.SetTarget(null);
    gizmo.SetHook(hook.transform);
    gizmo.SetMachineController(craneController);

    // indicate that the user should pick a load up first, put a load down later
    loadStartGizmo.SetActive(true);
    loadEndGizmo.SetActive(false);

    // reset loads count
    AutoconnectToHook.Reset();

    /*
        // init
        //tutorialManager.AddEvent(new TutorialEnableMachineEvent(craneController, false));
        tutorialManager.AddEvent(new TutorialSetControlMaskEvent(craneController, CraneController.ControlMask.None));
        tutorialManager.AddEvent(new TutorialHideGUIEvent(gizmo.leftArrow));
        tutorialManager.AddEvent(new TutorialHideGUIEvent(gizmo.rightArrow));
        tutorialManager.AddEvent(new TutorialHideGUIEvent(gizmo.upArrow));
        tutorialManager.AddEvent(new TutorialHideGUIEvent(gizmo.downArrow));
        tutorialManager.AddEvent(new TutorialHideGUIEvent(gizmo.inArrow));
        tutorialManager.AddEvent(new TutorialHideGUIEvent(gizmo.outArrow));

        //tutorialManager.AddEvent(new TutorialHideGUIEvent(startUI));
        //tutorialManager.AddEvent(new TutorialHideGUIEvent(stage1UI));
        //tutorialManager.AddEvent(new TutorialHideGUIEvent(stage2UI));
        //tutorialManager.AddEvent(new TutorialHideGUIEvent(restartUI));

        tutorialManager.AddEvent(new TutorialHideGUIEvent(loadStartGizmo));
        tutorialManager.AddEvent(new TutorialHideGUIEvent(loadEndGizmo));
    */

    // wait for the start button
    //tutorialManager.AddEvent(new TutorialShowGUIEvent(startUI));
    //tutorialManager.AddEvent(new TutorialWaitForButtonDownEvent("Fire1"));
    tutorialManager.AddEvent(new TutorialVRHeadsetOnEvent());
        tutorialManager.AddEvent(new TutorialHideGUIEvent(splashUI));
        tutorialManager.AddEvent(new TutorialCallFunctionWithBoolValueEvent(SetHasStarted, true));
        tutorialManager.AddEvent(new TutorialWaitForSecondsEvent(1.5f));
        tutorialManager.AddEvent(new TutorialShowGUIEvent(loadStartGizmo));
        //tutorialManager.AddEvent(new TutorialHideGUIEvent(startUI));

        // intro
        tutorialManager.AddEvent(new TutorialPlayAudioEvent(audioSource, dialogue[1])); // gday, thanks for driving the crane...
        tutorialManager.AddEvent(new TutorialWaitForSecondsEvent(0.5f));
        //tutorialManager.AddEvent(new TutorialPlayAudioEvent(audioSource, dialogue[2])); // let's start by checking the crane controls are working
        //tutorialManager.AddEvent(new TutorialWaitForSecondsEvent(0.5f));

        // stage 1 - start
        tutorialManager.AddEvent(new TutorialPlayAudioEvent(audioSource, dialogue[3])); // you can slew the crane arm left and right using the left joystick.
        //tutorialManager.AddEvent(new TutorialShowGUIEvent(stage1UI));
        //tutorialManager.AddEvent(new TutorialSetControlMaskEvent(craneController, CraneController.ControlMask.LeftJoystick));

        // stage 1 - slew left
        //tutorialManager.AddEvent(new TutorialShowGUIEvent(gizmo.leftArrow));
        //tutorialManager.AddEvent(new TutorialPlayAudioEvent(audioSource, dialogue[4])); // press left now to try slewing the crane arm left
        //tutorialManager.AddEvent(new TutorialWaitForTwoFunctionTrueEvent(craneController.GetLeft, craneController.GetRight));
        //tutorialManager.AddEvent(new TutorialPlayAudioEvent(audioSource, dialogue[6])); // great job
        //tutorialManager.AddEvent(new TutorialHideGUIEvent(slewLeftIncomplete));
        //tutorialManager.AddEvent(new TutorialShowGUIEvent(slewLeftComplete));
       // tutorialManager.AddEvent(new TutorialAchievementEvent(achievementsAnimator, achievementsText, "Slew Left and Right"));
        //tutorialManager.AddEvent(new TutorialWaitForSecondsEvent(2));
        //tutorialManager.AddEvent(new TutorialHideGUIEvent(gizmo.leftArrow));

        // stage 1 - slew right
        //tutorialManager.AddEvent(new TutorialShowGUIEvent(gizmo.rightArrow));
        //tutorialManager.AddEvent(new TutorialPlayAudioEvent(audioSource, dialogue[5])); // now press right to slew the crane arm right
        //tutorialManager.AddEvent(new TutorialWaitForFunctionTrueEvent(craneController.GetRight));
        //tutorialManager.AddEvent(new TutorialHideGUIEvent(slewRightIncomplete));
        //tutorialManager.AddEvent(new TutorialShowGUIEvent(slewRightComplete));
        //tutorialManager.AddEvent(new TutorialAchievementEvent(achievementsAnimator, achievementsText, "Slew Right"));
        //tutorialManager.AddEvent(new TutorialWaitForSecondsEvent(2));
        //tutorialManager.AddEvent(new TutorialHideGUIEvent(gizmo.rightArrow));

        // stage 1 - trolley out
        tutorialManager.AddEvent(new TutorialPlayAudioEvent(audioSource, dialogue[7])); // pressing up and down on the left joystick will move the trolley in and out
        //tutorialManager.AddEvent(new TutorialShowGUIEvent(gizmo.outArrow));
        //tutorialManager.AddEvent(new TutorialPlayAudioEvent(audioSource, dialogue[8])); // try moving the trolley out now by pressing up on the left joystick
        //tutorialManager.AddEvent(new TutorialWaitForTwoFunctionTrueEvent(craneController.GetUp, craneController.GetDown));
        //tutorialManager.AddEvent(new TutorialPlayAudioEvent(audioSource, dialogue[10])); // noice!
        //tutorialManager.AddEvent(new TutorialSetControlMaskEvent(craneController, CraneController.ControlMask.None));
        //tutorialManager.AddEvent(new TutorialHideGUIEvent(trolleyOutIncomplete));
        //tutorialManager.AddEvent(new TutorialShowGUIEvent(trolleyOutComplete));
        //tutorialManager.AddEvent(new TutorialAchievementEvent(achievementsAnimator, achievementsText, "Trolley In and Out"));
        //tutorialManager.AddEvent(new TutorialWaitForSecondsEvent(2));
        //tutorialManager.AddEvent(new TutorialHideGUIEvent(gizmo.outArrow));

        // stage 1 - trolley in
        //tutorialManager.AddEvent(new TutorialShowGUIEvent(gizmo.inArrow));
        //tutorialManager.AddEvent(new TutorialPlayAudioEvent(audioSource, dialogue[9]));
        //tutorialManager.AddEvent(new TutorialWaitForFunctionTrueEvent(craneController.GetDown));
        //tutorialManager.AddEvent(new TutorialHideGUIEvent(trolleyInIncomplete));
        //tutorialManager.AddEvent(new TutorialShowGUIEvent(trolleyInComplete));
        //tutorialManager.AddEvent(new TutorialAchievementEvent(achievementsAnimator, achievementsText, "Trolley In"));
        //tutorialManager.AddEvent(new TutorialWaitForSecondsEvent(2));
        //tutorialManager.AddEvent(new TutorialHideGUIEvent(gizmo.inArrow));

        // stage 1 - hook down
        //tutorialManager.AddEvent(new TutorialShowGUIEvent(gizmo.downArrow));
        tutorialManager.AddEvent(new TutorialPlayAudioEvent(audioSource, dialogue[11])); // lastly, pressing up and down on the right joystick will raise and lower the hook
        //tutorialManager.AddEvent(new TutorialSetControlMaskEvent(craneController, CraneController.ControlMask.RightJoystick));
        //tutorialManager.AddEvent(new TutorialWaitForSecondsEvent(1));
        //tutorialManager.AddEvent(new TutorialPlayAudioEvent(audioSource, dialogue[12])); // lower the hook now by pressing downward on the right joystick
        //tutorialManager.AddEvent(new TutorialWaitForTwoFunctionTrueEvent(craneController.GetHookDown, craneController.GetHookUp));
        //tutorialManager.AddEvent(new TutorialWaitForSecondsEvent(0.5f));
        //tutorialManager.AddEvent(new TutorialSetControlMaskEvent(craneController, CraneController.ControlMask.None));
        //tutorialManager.AddEvent(new TutorialHideGUIEvent(hookDownIncomplete));
        //tutorialManager.AddEvent(new TutorialShowGUIEvent(hookDownComplete));
        //tutorialManager.AddEvent(new TutorialAchievementEvent(achievementsAnimator, achievementsText, "Raise and Lower Hook"));
        //tutorialManager.AddEvent(new TutorialWaitForSecondsEvent(2));
        //tutorialManager.AddEvent(new TutorialHideGUIEvent(gizmo.downArrow));

        // stage 1 - hook up
        //tutorialManager.AddEvent(new TutorialShowGUIEvent(gizmo.upArrow));
        //tutorialManager.AddEvent(new TutorialPlayAudioEvent(audioSource, dialogue[13]));
        //tutorialManager.AddEvent(new TutorialWaitForFunctionTrueEvent(craneController.GetHookUp));
        //tutorialManager.AddEvent(new TutorialHideGUIEvent(hookUpIncomplete));
        //tutorialManager.AddEvent(new TutorialShowGUIEvent(hookUpComplete));
        //tutorialManager.AddEvent(new TutorialAchievementEvent(achievementsAnimator, achievementsText, "Hook Up"));
        //tutorialManager.AddEvent(new TutorialWaitForSecondsEvent(2));
        //tutorialManager.AddEvent(new TutorialHideGUIEvent(gizmo.upArrow));
        //*/
        // stage 1 - end
        //tutorialManager.AddEvent(new TutorialPlayAudioEvent(audioSource, dialogue[14])); // looks like everything is in order, we're ready to go
    //tutorialManager.AddEvent(new TutorialHideGUIEvent(stage1UI));
    //tutorialManager.AddEvent(new TutorialWaitForSecondsEvent(0.5f));

    // debug
    //tutorialManager.AddEvent(new TutorialEnableMachineEvent(craneController, true));

    // stage 2 - start
    tutorialManager.AddEvent(new TutorialPlayAudioEvent(audioSource, dialogue[15])); // look down on the ground to find the first load we're moving
    tutorialManager.AddEvent(new TutorialWaitForSecondsEvent(1));
    tutorialManager.AddEvent(new TutorialSetControlMaskEvent(craneController, CraneController.ControlMask.All));
        //tutorialManager.AddEvent(new TutorialPlayAudioEvent(audioSource, dialogue[16])); // it's just out in front of you and on the right
        //tutorialManager.AddEvent(new TutorialWaitForSecondsEvent(2));

        /*
    // stage 2 - find the load
    //tutorialManager.AddEvent(new TutorialShowGUIEvent(stage2UI));
    //tutorialManager.AddEvent(new TutorialSetGizmoTargetEvent(gizmo, load.transform));
    tutorialManager.AddEvent(new TutorialPlayAudioEvent(audioSource, dialogue[17])); // use the controls to bring the hook near the load

    // stage 2 - encouragement for getting close to load
    tutorialManager.AddEvent(new TutorialWaitForObjectsInRangeEvent(hook.transform, loadStartPosition, 20.0f));
    tutorialManager.AddEvent(new TutorialPlayAudioEvent(audioSource, dialogue[18]));
    tutorialManager.AddEvent(new TutorialWaitForObjectsInRangeEvent(hook.transform, loadStartPosition, 12.0f));
    tutorialManager.AddEvent(new TutorialPlayAudioEvent(audioSource, dialogue[19]));
    tutorialManager.AddEvent(new TutorialWaitForObjectsInRangeEvent(hook.transform, loadStartPosition, 8.0f));
    tutorialManager.AddEvent(new TutorialPlayAudioEvent(audioSource, dialogue[21]));
    tutorialManager.AddEvent(new TutorialWaitForObjectsInRangeEvent(hook.transform, loadStartPosition, 3.0f));
    tutorialManager.AddEvent(new TutorialSetControlMaskEvent(craneController, CraneController.ControlMask.None));
    tutorialManager.AddEvent(new TutorialSetGizmoTargetEvent(gizmo, null));
    tutorialManager.AddEvent(new TutorialPlayAudioEvent(audioSource, dialogue[22]));
    //tutorialManager.AddEvent(new TutorialHideGUIEvent(moveHookIncomplete));
    //tutorialManager.AddEvent(new TutorialShowGUIEvent(moveHookComplete));
    tutorialManager.AddEvent(new TutorialAchievementEvent(achievementsAnimator, achievementsText, "Move Hook To Load"));
    tutorialManager.AddEvent(new TutorialHideGUIEvent(loadStartGizmo));
    tutorialManager.AddEvent(new TutorialWaitForSecondsEvent(1));

    // stage 2 - hook up
    tutorialManager.AddEvent(new TutorialPlayAudioEvent(audioSource, dialogue[23]));
    tutorialManager.AddEvent(new TutorialWaitForSecondsEvent(1.5f));
    //tutorialManager.AddEvent(new TutorialHideGUIEvent(hookUpLoadIncomplete));
    //tutorialManager.AddEvent(new TutorialShowGUIEvent(hookUpLoadComplete));
    tutorialManager.AddEvent(new TutorialAchievementEvent(achievementsAnimator, achievementsText, "Hook Up Load"));
    tutorialManager.AddEvent(new TutorialCraneHookupLoadEvent(craneController, loadController));
    tutorialManager.AddEvent(new TutorialPlayAudioEvent(audioSource, dialogue[24]));
    tutorialManager.AddEvent(new TutorialMakeParentEvent(hook, load));
    tutorialManager.AddEvent(new TutorialCallFunctionWithBoolValueEvent(craneController.SetHasLoad, true));
    tutorialManager.AddEvent(new TutorialWaitForSecondsEvent(1));

    // stage 2 - lift
    //tutorialManager.AddEvent(new TutorialEnableMachineEvent(craneController, true));
    //tutorialManager.AddEvent(new TutorialPlayAudioEvent(audioSource, dialogue[25])); // gently raise the hook to lift the load off the ground
    //tutorialManager.AddEvent(new TutorialShowGUIEvent(gizmo.upArrow));
    //tutorialManager.AddEvent(new TutorialWaitForFunctionTrueEvent(craneController.GetHookUp));
    //tutorialManager.AddEvent(new TutorialHideGUIEvent(gizmo.upArrow));
    //tutorialManager.AddEvent(new TutorialHideGUIEvent(pickUpLoadIncomplete));
    //tutorialManager.AddEvent(new TutorialShowGUIEvent(pickUpLoadComplete));
    //tutorialManager.AddEvent(new TutorialAchievementEvent(achievementsAnimator, achievementsText, "Lift Load"));
    //tutorialManager.AddEvent(new TutorialWaitForSecondsEvent(0.5f));
    //tutorialManager.AddEvent(new TutorialEnableMachineEvent(craneController, false));
    //tutorialManager.AddEvent(new TutorialPlayAudioEvent(audioSource, dialogue[26])); // love your work
    //tutorialManager.AddEvent(new TutorialHideGUIEvent(stage2UI));
    //tutorialManager.AddEvent(new TutorialWaitForSecondsEvent(2));

    // stage 3 - find destination
    tutorialManager.AddEvent(new TutorialPlayAudioEvent(audioSource, dialogue[27])); // now we're going to move the load
    tutorialManager.AddEvent(new TutorialWaitForSecondsEvent(1));
    tutorialManager.AddEvent(new TutorialShowGUIEvent(loadEndGizmo));
    //tutorialManager.AddEvent(new TutorialPlayAudioEvent(audioSource, dialogue[28])); // you'll see the place we're moving it to out your left window
    //tutorialManager.AddEvent(new TutorialWaitForSecondsEvent(1));

    // stage 3 - move load to destination
    tutorialManager.AddEvent(new TutorialSetControlMaskEvent(craneController, CraneController.ControlMask.All));
    tutorialManager.AddEvent(new TutorialPlayAudioEvent(audioSource, dialogue[29])); // when you're ready, use the two joysticks to move the load to the destination
    tutorialManager.AddEvent(new TutorialSetGizmoTargetEvent(gizmo, loadEndPosition.transform));
    tutorialManager.AddEvent(new TutorialWaitForObjectsInRangeEvent(hook.transform, loadEndPosition, 6.0f));
    tutorialManager.AddEvent(new TutorialPlayAudioEvent(audioSource, dialogue[18]));
    tutorialManager.AddEvent(new TutorialWaitForObjectsInRangeEvent(hook.transform, loadEndPosition, 3.0f));
    tutorialManager.AddEvent(new TutorialPlayAudioEvent(audioSource, dialogue[30]));
    tutorialManager.AddEvent(new TutorialWaitForObjectsInRangeEvent(hook.transform, loadEndPosition, 1.5f));
    tutorialManager.AddEvent(new TutorialAchievementEvent(achievementsAnimator, achievementsText, "Move Load To Destination"));
    tutorialManager.AddEvent(new TutorialSetGizmoTargetEvent(gizmo, null));
    tutorialManager.AddEvent(new TutorialSetControlMaskEvent(craneController, CraneController.ControlMask.RightJoystick));
    //tutorialManager.AddEvent(new TutorialEnableMachineEvent(craneController, false));
    tutorialManager.AddEvent(new TutorialPlayAudioEvent(audioSource, dialogue[31]));
    tutorialManager.AddEvent(new TutorialShowGUIEvent(gizmo.downArrow));
    */

        tutorialManager.AddEvent(new TutorialAutoconnectToHookEvent());

        // stage 3 - end
        tutorialManager.AddEvent(new TutorialWaitForFunctionTrueEvent(craneController.GetHookDown));
    tutorialManager.AddEvent(new TutorialHideGUIEvent(gizmo.downArrow));
    tutorialManager.AddEvent(new TutorialSetControlMaskEvent(craneController, CraneController.ControlMask.None));
    //tutorialManager.AddEvent(new TutorialAchievementEvent(achievementsAnimator, achievementsText, "Drop Load"));
    tutorialManager.AddEvent(new TutorialMakeParentEvent(null, load));
    tutorialManager.AddEvent(new TutorialCallFunctionWithBoolValueEvent(craneController.SetHasLoad, false));
    tutorialManager.AddEvent(new TutorialPlayAudioEvent(audioSource, dialogue[32]));
    tutorialManager.AddEvent(new TutorialWaitForSecondsEvent(3f));
    //tutorialManager.AddEvent(new TutorialAchievementEvent(achievementsAnimator, achievementsText, "Finish Training"));
    tutorialManager.AddEvent(new TutorialEndingEvent(endingPopup));

    tutorialManager.AddEvent(new TutorialWaitForSecondsEvent(2));
    //tutorialManager.AddEvent(new TutorialShowGUIEvent(restartUI));
    tutorialManager.AddEvent(new TutorialWaitForSecondsEvent(6));

    // reload scene
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
      AutoconnectToHook.Reset();
    }
    else
    {
      tutorialManager.Update(gameObject);
    }

    //TOFIX: Compilation placeholder
    bool proximitySensor = ByteSprite.VR.IsHeadsetWorn(); //headset.GetPress(Valve.VR.EVRButtonId.k_EButton_ProximitySensor);

    if (!hasStarted || proximitySensor || !checkProximity)
    {
      lastProximity = Time.time;
    }

    // if proximity gone too long, restart scene
    if (Time.time >= lastProximity + TutorialManager.PROXIMITY_SENSOR_TIMEOUT)
    {
      //Debug.Log("User left VR. Restarting...");
      //ReloadScene();
    }
  }

  // reset crane and load back to starting position ready for running the tutorial again
  private void Reset()
  {
    ReplaceLoad();
    craneController.Reset();
    SetHasStarted(false);
  }

  private void ReloadScene()
  {
    SceneManager.LoadScene(SceneManager.GetActiveScene().name);
  }

  public void ReplaceLoad()
  {
    ReloadScene();
   
    /*
    Debug.Log("ReplaceLoad()");
    load.transform.position = new Vector3(loadStartPosition.transform.position.x, loadStartPosition.transform.position.y, loadStartPosition.transform.position.z);
    load.transform.rotation = Quaternion.Euler(loadStartPosition.transform.rotation.eulerAngles.x, loadStartPosition.transform.rotation.eulerAngles.y, loadStartPosition.transform.rotation.eulerAngles.z);
    */
  }

  public void SetHasStarted(bool hasStarted)
  {
    Debug.Log("SetHasStarted():" + hasStarted);
    this.hasStarted = hasStarted;
  }

  public void SetIsCarrying(bool isCarrying)
  {
    Debug.Log("SetIsCarrying():" + isCarrying);
    loadStartGizmo.SetActive(!isCarrying);
    loadEndGizmo.SetActive(isCarrying);
  }

  public bool GetIsCarrying()
  {
    return loadEndGizmo.activeSelf;
  }

  public void TriggerAchievement(string achievement)
  {
    Debug.Log("TriggerAchievement():" + achievement);
    achievementsText.text = "Achievement\n\n" + achievement;
    achievementsAnimator.SetTrigger("NextAchievement");
  }
}

