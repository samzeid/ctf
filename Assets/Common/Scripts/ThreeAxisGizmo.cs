using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThreeAxisGizmo : MonoBehaviour {
  public GameObject leftArrow;
  public GameObject rightArrow;
  public GameObject upArrow;
  public GameObject downArrow;
  public GameObject outArrow;
  public GameObject inArrow;

  private Transform target;
  private MachineController machineController;
  private Transform hook;

  const float MAX_SLEW_DIFF = 3.0f;
  const float MAX_TROLLEY_DIFF = 0.5f;
  const float MAX_HOOK_DIFF = 1.0f;

  // Use this for initialization
  void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

    // abort if no target set
    if (target == null)
    {
      return;
    }

    // get axial distances from crane to target
    float x_diff = target.position.x - machineController.transform.position.x;
    float y_diff = target.position.y - machineController.transform.position.y;
    float z_diff = target.position.z - machineController.transform.position.z;
    //Debug.Log("x_diff:" + x_diff + " y_diff:" + y_diff + " z_diff:" + z_diff);

    // find absolute slew angle
    float theta = 90.0f - Mathf.Atan2(z_diff, x_diff) * Mathf.Rad2Deg;

    // find relative slew angle
    float machineControllerRotation = machineController.GetSlewRotationY();

    // debug
    //Debug.Log("theta " + theta + " machine rotation " + machineControllerRotation);

    // check for slew
    if (Mathf.Abs(theta - machineControllerRotation) > MAX_SLEW_DIFF)
    {
      bool showRight = machineControllerRotation < theta;
      rightArrow.SetActive(showRight);

      bool showLeft =  machineControllerRotation > theta;
      leftArrow.SetActive(showLeft);
    }
    else
    {
      rightArrow.SetActive(false);
      leftArrow.SetActive(false);
    }

    // measure XZ distance to target
    float r = Mathf.Sqrt(Mathf.Pow(x_diff, 2.0f) + Mathf.Pow(z_diff, 2.0f));
    float y = y_diff;

    float trolley_position_z = hook.transform.position.z - machineController.transform.position.z;
    float hook_position_y = hook.transform.position.y - machineController.transform.position.y;

    //Debug.Log("CranePickupTarget " + gameObject.name + " theta " + theta + " r " + r + " y " + y);

   

    // check for trolley
    if (Mathf.Abs(trolley_position_z - r) > MAX_TROLLEY_DIFF)
    {
      bool showIn = trolley_position_z > r;
      inArrow.SetActive(showIn);
      bool showOut = trolley_position_z < r;
      outArrow.SetActive(showOut);
    }
    else
    {
      inArrow.SetActive(false);
      outArrow.SetActive(false);
    }

    // check for hook
    //Debug.Log(hook_position_y + " " + y);
    if (Mathf.Abs(hook_position_y - y) > MAX_HOOK_DIFF)
    {
      bool showDown = hook_position_y > y;
      downArrow.SetActive(showDown);

      bool showUp = hook_position_y < y;
      upArrow.SetActive(showUp);
    }
    else
    {
      downArrow.SetActive(false);
      upArrow.SetActive(false);
    }
  }

  public void SetTarget(Transform target)
  {
    this.target = target;

    if (target == null)
    {
      if (rightArrow)
        rightArrow.SetActive(false);
      if (leftArrow)
        leftArrow.SetActive(false);
      if (downArrow)
        downArrow.SetActive(false);
      if (upArrow)
        upArrow.SetActive(false);
      if (inArrow)
        inArrow.SetActive(false);
      if (outArrow)
        outArrow.SetActive(false);
    }
  }

  public void SetMachineController(MachineController machineController)
  {
    this.machineController = machineController;
  }

  public void SetHook(Transform hook)
  {
    this.hook = hook;
  }
}
