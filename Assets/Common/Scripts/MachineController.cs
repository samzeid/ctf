using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MachineController : MonoBehaviour {

  private bool movementIsEnabled = true;
  protected float slew_rotation_y;

  // Use this for initialization
  void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

  public void SetMovementEnabled(bool enabled)
  {
    movementIsEnabled = enabled;
  }

  public bool GetMovementEnabled()
  {
    return movementIsEnabled;
  }

  public float GetSlewRotationY()
  {
    return slew_rotation_y;
  }

  public abstract bool IsMoving();
  public abstract void Reset();
  public abstract bool HasAnyInput();
}
