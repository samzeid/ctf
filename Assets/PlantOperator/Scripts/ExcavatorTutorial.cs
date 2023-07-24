using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
public class ExcavatorTutorial : MonoBehaviour {

  private float INACTIVITY_RELOAD_TIMEOUT = 20.0f;
  private float VUMARK_DELAY = 2.0f;

  public ExcavatorController excavatorController;

  [Header("GUIs")]
  public GameObject startUI;
  public GameObject restartUI;
  public GameObject splashUI;

  [Header("Audio Clips")]
  public AudioClip[] dialogue;

  [Header("Objects")]
  public Transform digPosition;
  public Transform dumpPosition;
  public GameObject digGizmo;
  public GameObject dumpGizmo;
  public ThreeAxisGizmo gizmo;
  public SoilArea trench;
  public Animator achievementsAnimator;
  public Text achievementsText;

  private TutorialManager tutorialManager;
  private AudioSource audioSource;

  private bool hasStarted;

  private float lastActivityTime;

  // Use this for initialization
  void Start () {
    Debug.Assert(dialogue[0] == null);

    tutorialManager = new TutorialManager();
    audioSource = GetComponent<AudioSource>();

    lastActivityTime = Time.time;

    // setup gizmo
    gizmo.SetTarget(null);
    gizmo.SetHook(excavatorController.GetBucket().transform);
    gizmo.SetMachineController(excavatorController);

    // new simple tutorial
    tutorialManager.AddEvent(new TutorialEnableMachineEvent(excavatorController, true));
    tutorialManager.AddEvent(new TutorialWaitForAnyInputEvent(excavatorController));
    //tutorialManager.AddEvent(new TutorialHideGUIEvent(splashUI));
    tutorialManager.AddEvent(new TutorialSetAnimationBoolEvent(splashUI.GetComponent<Animator>(), "ShowSplash", false));
    tutorialManager.AddEvent(new TutorialCallFunctionWithBoolValueEvent(SetHasStarted, true));
    tutorialManager.AddEvent(new TutorialImpossibleEvent());

    /*
    // init
    tutorialManager.AddEvent(new TutorialEnableMachineEvent(excavatorController, false));
    tutorialManager.AddEvent(new TutorialHideGUIEvent(gizmo.leftArrow));
    tutorialManager.AddEvent(new TutorialHideGUIEvent(gizmo.rightArrow));
    tutorialManager.AddEvent(new TutorialHideGUIEvent(gizmo.upArrow));
    tutorialManager.AddEvent(new TutorialHideGUIEvent(gizmo.downArrow));
    tutorialManager.AddEvent(new TutorialHideGUIEvent(gizmo.inArrow));
    tutorialManager.AddEvent(new TutorialHideGUIEvent(gizmo.outArrow));

    // init - hide UI
    tutorialManager.AddEvent(new TutorialHideGUIEvent(startUI));
    tutorialManager.AddEvent(new TutorialHideGUIEvent(restartUI));

    // init - hide gizmos
    tutorialManager.AddEvent(new TutorialHideGUIEvent(digGizmo));
    tutorialManager.AddEvent(new TutorialHideGUIEvent(dumpGizmo));

    // wait for the start button
    //tutorialManager.AddEvent(new TutorialShowGUIEvent(startUI));
    //tutorialManager.AddEvent(new TutorialWaitForButtonDownEvent("Fire1"));
    tutorialManager.AddEvent(new TutorialWaitForAnyInputEvent(excavatorController));
    tutorialManager.AddEvent(new TutorialCallFunctionWithBoolValueEvent(SetHasStarted, true));
    //tutorialManager.AddEvent(new TutorialHideGUIEvent(startUI));

    // intro
    tutorialManager.AddEvent(new TutorialPlayAudioEvent(audioSource, dialogue[1]));
    tutorialManager.AddEvent(new TutorialWaitForSecondsEvent(0.5f));

    // show left joystick
    tutorialManager.AddEvent(new TutorialPlayAudioEvent(audioSource, dialogue[2]));
    tutorialManager.AddEvent(new TutorialWaitForSecondsEvent(0.5f));
    tutorialManager.AddEvent(new TutorialPlayAudioEvent(audioSource, dialogue[3]));
    tutorialManager.AddEvent(new TutorialWaitForSecondsEvent(0.5f));
    tutorialManager.AddEvent(new TutorialPlayAudioEvent(audioSource, dialogue[4]));

    // enable machine
    tutorialManager.AddEvent(new TutorialEnableMachineEvent(excavatorController, true));

    // wait for move left
    tutorialManager.AddEvent(new TutorialShowGUIEvent(gizmo.leftArrow));
    tutorialManager.AddEvent(new TutorialWaitForFunctionTrueEvent(excavatorController.GetSlewLeft));
    tutorialManager.AddEvent(new TutorialAchievementEvent(achievementsAnimator, achievementsText, "Slew Left"));
    tutorialManager.AddEvent(new TutorialWaitForSecondsEvent(0.5f));
    tutorialManager.AddEvent(new TutorialHideGUIEvent(gizmo.leftArrow));
    tutorialManager.AddEvent(new TutorialPlayAudioEvent(audioSource, dialogue[5]));
    tutorialManager.AddEvent(new TutorialWaitForSecondsEvent(0.5f));

    // show move right
    tutorialManager.AddEvent(new TutorialPlayAudioEvent(audioSource, dialogue[6]));

    // wait for move right
    tutorialManager.AddEvent(new TutorialShowGUIEvent(gizmo.rightArrow));
    tutorialManager.AddEvent(new TutorialWaitForFunctionTrueEvent(excavatorController.GetSlewRight));
    tutorialManager.AddEvent(new TutorialAchievementEvent(achievementsAnimator, achievementsText, "Slew Right"));
    tutorialManager.AddEvent(new TutorialWaitForSecondsEvent(0.5f));
    tutorialManager.AddEvent(new TutorialHideGUIEvent(gizmo.rightArrow));
    tutorialManager.AddEvent(new TutorialPlayAudioEvent(audioSource, dialogue[7]));
    tutorialManager.AddEvent(new TutorialWaitForSecondsEvent(0.5f));

    // show move up
    tutorialManager.AddEvent(new TutorialPlayAudioEvent(audioSource, dialogue[8]));
    tutorialManager.AddEvent(new TutorialWaitForSecondsEvent(0.5f));
    tutorialManager.AddEvent(new TutorialPlayAudioEvent(audioSource, dialogue[9]));
    tutorialManager.AddEvent(new TutorialWaitForSecondsEvent(0.5f));
    tutorialManager.AddEvent(new TutorialPlayAudioEvent(audioSource, dialogue[10]));

    // wait for move up
    tutorialManager.AddEvent(new TutorialShowGUIEvent(gizmo.upArrow));
    tutorialManager.AddEvent(new TutorialWaitForFunctionTrueEvent(excavatorController.GetArmUp));
    tutorialManager.AddEvent(new TutorialAchievementEvent(achievementsAnimator, achievementsText, "Arm Up"));
    tutorialManager.AddEvent(new TutorialWaitForSecondsEvent(0.5f));
    tutorialManager.AddEvent(new TutorialHideGUIEvent(gizmo.upArrow));
    tutorialManager.AddEvent(new TutorialPlayAudioEvent(audioSource, dialogue[11]));
    tutorialManager.AddEvent(new TutorialWaitForSecondsEvent(0.5f));

    // show move down
    tutorialManager.AddEvent(new TutorialPlayAudioEvent(audioSource, dialogue[12]));

    // wait for move down
    tutorialManager.AddEvent(new TutorialShowGUIEvent(gizmo.downArrow));
    tutorialManager.AddEvent(new TutorialWaitForFunctionTrueEvent(excavatorController.GetArmDown));
    tutorialManager.AddEvent(new TutorialAchievementEvent(achievementsAnimator, achievementsText, "Arm Down"));
    tutorialManager.AddEvent(new TutorialWaitForSecondsEvent(0.5f));
    tutorialManager.AddEvent(new TutorialHideGUIEvent(gizmo.downArrow));
    tutorialManager.AddEvent(new TutorialPlayAudioEvent(audioSource, dialogue[13]));
    tutorialManager.AddEvent(new TutorialWaitForSecondsEvent(0.5f));

    // show right joystick
    tutorialManager.AddEvent(new TutorialPlayAudioEvent(audioSource, dialogue[14]));
    tutorialManager.AddEvent(new TutorialWaitForSecondsEvent(0.5f));
    tutorialManager.AddEvent(new TutorialPlayAudioEvent(audioSource, dialogue[15]));
    tutorialManager.AddEvent(new TutorialWaitForSecondsEvent(0.5f));
    tutorialManager.AddEvent(new TutorialPlayAudioEvent(audioSource, dialogue[16]));
    tutorialManager.AddEvent(new TutorialWaitForSecondsEvent(0.5f));
    tutorialManager.AddEvent(new TutorialPlayAudioEvent(audioSource, dialogue[17]));

    // wait for boom up
    tutorialManager.AddEvent(new TutorialShowGUIEvent(gizmo.outArrow));
    tutorialManager.AddEvent(new TutorialWaitForFunctionTrueEvent(excavatorController.GetBoomUp));
    tutorialManager.AddEvent(new TutorialAchievementEvent(achievementsAnimator, achievementsText, "Boom Up"));
    tutorialManager.AddEvent(new TutorialWaitForSecondsEvent(0.5f));
    tutorialManager.AddEvent(new TutorialHideGUIEvent(gizmo.outArrow));
    tutorialManager.AddEvent(new TutorialPlayAudioEvent(audioSource, dialogue[18]));
    tutorialManager.AddEvent(new TutorialWaitForSecondsEvent(0.5f));

    // show boom down
    tutorialManager.AddEvent(new TutorialPlayAudioEvent(audioSource, dialogue[19]));

    // wait for boom down
    tutorialManager.AddEvent(new TutorialShowGUIEvent(gizmo.inArrow));
    tutorialManager.AddEvent(new TutorialWaitForFunctionTrueEvent(excavatorController.GetBoomDown));
    tutorialManager.AddEvent(new TutorialAchievementEvent(achievementsAnimator, achievementsText, "Boom Down"));
    tutorialManager.AddEvent(new TutorialWaitForSecondsEvent(0.5f));
    tutorialManager.AddEvent(new TutorialHideGUIEvent(gizmo.inArrow));
    tutorialManager.AddEvent(new TutorialPlayAudioEvent(audioSource, dialogue[13])); // TODO: re-used placeholder sound
    tutorialManager.AddEvent(new TutorialWaitForSecondsEvent(0.5f));

    // show curl in
    tutorialManager.AddEvent(new TutorialPlayAudioEvent(audioSource, dialogue[20]));
    tutorialManager.AddEvent(new TutorialWaitForSecondsEvent(0.5f));
    tutorialManager.AddEvent(new TutorialPlayAudioEvent(audioSource, dialogue[21]));
    tutorialManager.AddEvent(new TutorialWaitForSecondsEvent(0.5f));
    tutorialManager.AddEvent(new TutorialPlayAudioEvent(audioSource, dialogue[22]));

    // wait for curl in
    tutorialManager.AddEvent(new TutorialWaitForFunctionTrueEvent(excavatorController.GetBucketCurlIn));
    tutorialManager.AddEvent(new TutorialAchievementEvent(achievementsAnimator, achievementsText, "Curl Bucket In"));
    tutorialManager.AddEvent(new TutorialWaitForSecondsEvent(0.5f));
    tutorialManager.AddEvent(new TutorialPlayAudioEvent(audioSource, dialogue[23]));
    tutorialManager.AddEvent(new TutorialWaitForSecondsEvent(0.5f));

    // show curl out
    tutorialManager.AddEvent(new TutorialPlayAudioEvent(audioSource, dialogue[24]));

    // wait for curl out
    tutorialManager.AddEvent(new TutorialWaitForFunctionTrueEvent(excavatorController.GetBucketCurlOut));
    tutorialManager.AddEvent(new TutorialAchievementEvent(achievementsAnimator, achievementsText, "Curl Bucket Out"));
    tutorialManager.AddEvent(new TutorialWaitForSecondsEvent(0.5f));
    tutorialManager.AddEvent(new TutorialPlayAudioEvent(audioSource, dialogue[25]));
    tutorialManager.AddEvent(new TutorialWaitForSecondsEvent(0.5f));

    // explain digging trench
    tutorialManager.AddEvent(new TutorialPlayAudioEvent(audioSource, dialogue[26]));
    tutorialManager.AddEvent(new TutorialWaitForSecondsEvent(0.5f));
    tutorialManager.AddEvent(new TutorialPlayAudioEvent(audioSource, dialogue[27]));
    tutorialManager.AddEvent(new TutorialShowGUIEvent(digGizmo));
    tutorialManager.AddEvent(new TutorialSetGizmoTargetEvent(gizmo, digPosition));

    // wait for slew toward the trench
    tutorialManager.AddEvent(new TutorialWaitForObjectsInXZRangeEvent(excavatorController.GetBucket(), digPosition, 1.45f));
    tutorialManager.AddEvent(new TutorialSetGizmoTargetEvent(gizmo, null));

    // instruct to dig trench
    tutorialManager.AddEvent(new TutorialPlayAudioEvent(audioSource, dialogue[28]));
    tutorialManager.AddEvent(new TutorialWaitForSecondsEvent(0.5f));

    // wait for digging
    tutorialManager.AddEvent(new TutorialWaitForObjectsInRangeEvent(excavatorController.GetBucket(), digPosition, 1.7f));
    tutorialManager.AddEvent(new TutorialAchievementEvent(achievementsAnimator, achievementsText, "Dig Trench"));
    tutorialManager.AddEvent(new TutorialHideGUIEvent(digGizmo));
    tutorialManager.AddEvent(new TutorialPlayAudioEvent(audioSource, dialogue[29]));
    tutorialManager.AddEvent(new TutorialWaitForSecondsEvent(0.5f));

    // instruction for slew to dump pile
    tutorialManager.AddEvent(new TutorialPlayAudioEvent(audioSource, dialogue[30]));
    tutorialManager.AddEvent(new TutorialShowGUIEvent(dumpGizmo));
    tutorialManager.AddEvent(new TutorialSetGizmoTargetEvent(gizmo, dumpPosition));

    // encouragement to get near the dump pile
    tutorialManager.AddEvent(new TutorialWaitForObjectsInXZRangeEvent(excavatorController.GetBucket(), dumpPosition, 2.0f));
    tutorialManager.AddEvent(new TutorialEnableMachineEvent(excavatorController, false));
    tutorialManager.AddEvent(new TutorialHideGUIEvent(dumpGizmo));
    tutorialManager.AddEvent(new TutorialSetGizmoTargetEvent(gizmo, null));
    tutorialManager.AddEvent(new TutorialPlayAudioEvent(audioSource, dialogue[31]));  // that's it
    tutorialManager.AddEvent(new TutorialWaitForSecondsEvent(0.5f));

    // instruction to dump soil on pile
    tutorialManager.AddEvent(new TutorialPlayAudioEvent(audioSource, dialogue[32]));
    tutorialManager.AddEvent(new TutorialEnableMachineEvent(excavatorController, true));

    // wait for dump soil
    tutorialManager.AddEvent(new TutorialWaitForFloatFunctionLessThanValueEvent(excavatorController.GetBucketRotation, -10.0f));
    tutorialManager.AddEvent(new TutorialAchievementEvent(achievementsAnimator, achievementsText, "Dump Dirt on Pile"));
    tutorialManager.AddEvent(new TutorialPlayAudioEvent(audioSource, dialogue[33]));
    tutorialManager.AddEvent(new TutorialWaitForSecondsEvent(0.5f));
    tutorialManager.AddEvent(new TutorialPlayAudioEvent(audioSource, dialogue[34]));
    tutorialManager.AddEvent(new TutorialWaitForSecondsEvent(0.5f));
    */

    // encouragement during digging
    /*
    tutorialManager.AddEvent(new TutorialWaitForFloatFunctionGreaterThanValueEvent(trench.GetDepth, 0.1f));
    tutorialManager.AddEvent(new TutorialPlayAudioEvent(audioSource, dialogue[35]));
    tutorialManager.AddEvent(new TutorialWaitForFloatFunctionGreaterThanValueEvent(trench.GetDepth, 0.25f));
    tutorialManager.AddEvent(new TutorialPlayAudioEvent(audioSource, dialogue[36]));
    tutorialManager.AddEvent(new TutorialWaitForFloatFunctionGreaterThanValueEvent(trench.GetDepth, 0.4f));
    tutorialManager.AddEvent(new TutorialPlayAudioEvent(audioSource, dialogue[37]));
    tutorialManager.AddEvent(new TutorialWaitForFloatFunctionGreaterThanValueEvent(trench.GetDepth, 0.65f));
    tutorialManager.AddEvent(new TutorialPlayAudioEvent(audioSource, dialogue[38]));
    tutorialManager.AddEvent(new TutorialWaitForFloatFunctionGreaterThanValueEvent(trench.GetDepth, 0.8f));
    tutorialManager.AddEvent(new TutorialPlayAudioEvent(audioSource, dialogue[39]));
    tutorialManager.AddEvent(new TutorialWaitForFloatFunctionGreaterThanValueEvent(trench.GetDepth, 0.9f));
    tutorialManager.AddEvent(new TutorialPlayAudioEvent(audioSource, dialogue[40]));
    tutorialManager.AddEvent(new TutorialWaitForFloatFunctionGreaterThanValueEvent(trench.GetDepth, 1.0f));
    */

    // debug encouragement
    /*
    tutorialManager.AddEvent(new TutorialShowGUIEvent(digGizmo));
    tutorialManager.AddEvent(new TutorialWaitForObjectsInXZRangeEvent(excavatorController.GetBucket(), digPosition, 1.45f));
    tutorialManager.AddEvent(new TutorialWaitForFloatFunctionGreaterThanValueEvent(excavatorController.GetBucketRotation, 100.0f));
    tutorialManager.AddEvent(new TutorialPlayAudioEvent(audioSource, dialogue[13])); // good
    tutorialManager.AddEvent(new TutorialHideGUIEvent(digGizmo));
    tutorialManager.AddEvent(new TutorialShowGUIEvent(dumpGizmo));
    tutorialManager.AddEvent(new TutorialWaitForObjectsInXZRangeEvent(excavatorController.GetBucket(), dumpPosition, 2.0f));
    tutorialManager.AddEvent(new TutorialWaitForFloatFunctionLessThanValueEvent(excavatorController.GetBucketRotation, -10.0f));
    tutorialManager.AddEvent(new TutorialHideGUIEvent(dumpGizmo));
    tutorialManager.AddEvent(new TutorialPlayAudioEvent(audioSource, dialogue[38]));
    tutorialManager.AddEvent(new TutorialShowGUIEvent(digGizmo));
    tutorialManager.AddEvent(new TutorialWaitForObjectsInXZRangeEvent(excavatorController.GetBucket(), digPosition, 1.45f));
    tutorialManager.AddEvent(new TutorialWaitForFloatFunctionGreaterThanValueEvent(excavatorController.GetBucketRotation, 100.0f));
    tutorialManager.AddEvent(new TutorialPlayAudioEvent(audioSource, dialogue[13])); // good
    tutorialManager.AddEvent(new TutorialHideGUIEvent(digGizmo));
    tutorialManager.AddEvent(new TutorialShowGUIEvent(dumpGizmo));
    tutorialManager.AddEvent(new TutorialWaitForObjectsInXZRangeEvent(excavatorController.GetBucket(), dumpPosition, 2.0f));
    tutorialManager.AddEvent(new TutorialWaitForFloatFunctionLessThanValueEvent(excavatorController.GetBucketRotation, -10.0f));
    tutorialManager.AddEvent(new TutorialHideGUIEvent(dumpGizmo));
    tutorialManager.AddEvent(new TutorialPlayAudioEvent(audioSource, dialogue[40]));
    tutorialManager.AddEvent(new TutorialShowGUIEvent(digGizmo));
    tutorialManager.AddEvent(new TutorialWaitForObjectsInXZRangeEvent(excavatorController.GetBucket(), digPosition, 1.45f));
    tutorialManager.AddEvent(new TutorialWaitForFloatFunctionGreaterThanValueEvent(excavatorController.GetBucketRotation, 100.0f));
    tutorialManager.AddEvent(new TutorialPlayAudioEvent(audioSource, dialogue[13])); // good
    tutorialManager.AddEvent(new TutorialHideGUIEvent(digGizmo));
    tutorialManager.AddEvent(new TutorialShowGUIEvent(dumpGizmo));
    tutorialManager.AddEvent(new TutorialWaitForObjectsInXZRangeEvent(excavatorController.GetBucket(), dumpPosition, 2.0f));
    tutorialManager.AddEvent(new TutorialWaitForFloatFunctionLessThanValueEvent(excavatorController.GetBucketRotation, -10.0f));
    tutorialManager.AddEvent(new TutorialHideGUIEvent(dumpGizmo));
    */

    /*
    // announce end of tutorial
    tutorialManager.AddEvent(new TutorialPlayAudioEvent(audioSource, dialogue[41]));
    tutorialManager.AddEvent(new TutorialAchievementEvent(achievementsAnimator, achievementsText, "Finish Training"));

    // wait 10s before restart!
    //tutorialManager.AddEvent(new TutorialShowGUIEvent(restartUI));
    tutorialManager.AddEvent(new TutorialWaitForSecondsEvent(10.0f));

    // reload scene
    tutorialManager.AddEvent(new TutorialReloadSceneEvent());
    */

    tutorialManager.Restart(gameObject);
  }

	// Update is called once per frame
	void Update () {
    if (tutorialManager.IsComplete())
    {
      Reset();
      tutorialManager.Restart(gameObject);
    }
    else
    {
      tutorialManager.Update(gameObject);
    }

    // check for activity
    if (excavatorController.HasAnyInput())
    {
      lastActivityTime = Time.time;
      // not sure how efficient this is
      splashUI.GetComponent<Animator>().SetBool("ShowVuMark",false);
    }

    if (hasStarted && Time.time > lastActivityTime + VUMARK_DELAY){
        splashUI.GetComponent<Animator>().SetBool("ShowVuMark",true);
    }

    // check for inactivity timeout (only check this after tutorial has started)
    //Debug.Log("hasStarted:" + hasStarted + " time since last activity:" + (Time.time - lastActivityTime));
    if (hasStarted && Time.time > lastActivityTime + INACTIVITY_RELOAD_TIMEOUT)
    {
      Debug.Log("Inactivity Reload Timeout");
      ReloadScene();
    } else
    {
      //Debug.Log("Time since last activity = " + (Time.time - lastActivityTime));
    }
  }

  // reset crane and load back to starting position ready for running the tutorial again
  private void Reset()
  {
    ReplaceDirt();
    excavatorController.Reset();
    SetHasStarted(false);
  }

  private void ReloadScene()
  {
    SceneManager.LoadScene(SceneManager.GetActiveScene().name);
  }

  // TODO!
  public void ReplaceDirt()
  {
    Debug.LogError("ReplaceDirt() incomplete");

    trench.Reset();
  }

  public void SetHasStarted(bool hasStarted)
  {
    Debug.Log("SetHasStarted():" + hasStarted);
    this.hasStarted = hasStarted;
  }
}
