using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SimpleGizmoTestTutorial : MonoBehaviour
{
  public CraneController craneController;

  [Header("Objects")]
  public GameObject hook;
  public Transform loadStartPosition;
  public Transform loadEndPosition;
  public GameObject loadStartGizmo;
  public GameObject loadEndGizmo;
  public ThreeAxisGizmo gizmo;

  private TutorialManager tutorialManager = new TutorialManager();

  // Use this for initialization
  void Start()
  {
    // setup gizmo
    gizmo.SetTarget(null);
    gizmo.SetHook(hook.transform);
    gizmo.SetMachineController(craneController);

    // hide gizmos
    tutorialManager.AddEvent(new TutorialHideGUIEvent(loadStartGizmo));
    tutorialManager.AddEvent(new TutorialHideGUIEvent(loadEndGizmo));

    tutorialManager.AddEvent(new TutorialEnableMachineEvent(craneController, true));

    // move to end pos
    tutorialManager.AddEvent(new TutorialShowGUIEvent(loadEndGizmo));
    tutorialManager.AddEvent(new TutorialSetGizmoTargetEvent(gizmo, loadEndPosition));
    tutorialManager.AddEvent(new TutorialWaitForObjectsInRangeEvent(hook.transform, loadEndPosition, 0.2f));
    tutorialManager.AddEvent(new TutorialHideGUIEvent(loadEndGizmo));

    // move to start pos
    tutorialManager.AddEvent(new TutorialShowGUIEvent(loadStartGizmo));
    tutorialManager.AddEvent(new TutorialSetGizmoTargetEvent(gizmo, loadStartPosition));
    tutorialManager.AddEvent(new TutorialWaitForObjectsInRangeEvent(hook.transform, loadStartPosition, 0.2f));
    tutorialManager.AddEvent(new TutorialHideGUIEvent(loadStartGizmo));

    // move to end pos
    tutorialManager.AddEvent(new TutorialShowGUIEvent(loadEndGizmo));
    tutorialManager.AddEvent(new TutorialSetGizmoTargetEvent(gizmo, loadEndPosition));
    tutorialManager.AddEvent(new TutorialWaitForObjectsInRangeEvent(hook.transform, loadEndPosition, 0.2f));
    tutorialManager.AddEvent(new TutorialHideGUIEvent(loadEndGizmo));

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
  }

  // reset crane and load back to starting position ready for running the tutorial again
  private void Reset()
  {
    craneController.Reset();
  }
}


