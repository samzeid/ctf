using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;
using System;
using System.Runtime.InteropServices;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.UIElements.Experimental;
using UnityEngine.XR;


public class CraneController : MachineController
{

  public enum ControlMask
  {
    None,
    LeftJoystick,
    RightJoystick,
    All,
  }

  public bool usingQuest;
  private InputData inputData;
  

  public int playerId;
  private Player player;

  public const float MAX_SLEW_DIFF = 3.0f;
  public const float MAX_TROLLEY_DIFF = 0.5f;
  public const float MAX_HOOK_DIFF = 0.5f;

  public const float MIN_SLEW_POSITION = -23f;
  public const float MAX_SLEW_POSITION = 50f;

  public const float MIN_TROLLEY_POSITION = 0.05f;
  public const float MAX_TROLLEY_POSITION = 0.30f;
  public const float DEFAULT_TROLLEY_POSITION = 0.1203813f;
  public const float MIN_HOOK_POSITION = 2.0f;
  public const float MAX_HOOK_POSITION = 37.0f;
  public const float DEFAULT_HOOK_POSITION = 3.0f;

  public const float SLEW_RATE = 8.0f;
  public const float TROLLEY_RATE = 0.025f;
  public const float HOOK_RATE = 3.0f; // raise and lower hook

  public Transform slew;
  public Transform trolley;

  public GameObject cable;
  public GameObject cable2;
  
  public GameObject hook;
  public GameObject sling;

  float trolley_position_z = 0.0f;

  float joystick1Horz;
  float joystick1Vert;
  float joystick2Horz;
  float joystick2Vert;

  bool joy1Up = false;
  bool joy1Down = false;
  bool joy1Left = false;
  bool joy1Right = false;
  bool joy2Up = false;
  bool joy2Down = false;
  bool joy2Left = false;
  bool joy2Right = false;

  bool hasLoad = false;

  [Header("Sling Hooks")]
  public GameObject hook0;
  public GameObject hook1;
  public GameObject hook2;
  public GameObject hook3;

  public Animator joystickAnimator;

  float hook_position;
  float old_hook_position;

  private ControlMask controlMask = ControlMask.All;

  

  // Use this for initialization
  void Start()
  {
    if (usingQuest)
      inputData = GetComponent<InputData>();
    
    player = ReInput.players.GetPlayer(playerId);

    Reset();
  }

  // Update is called once per frame
  void Update()
  {

    old_hook_position = hook_position;

    joystick1Horz = player.GetAxis("Joystick 1 Horz");
    joystick1Vert = player.GetAxis("Joystick 1 Vert");
    joystick2Horz = player.GetAxis("Joystick 2 Horz");
    joystick2Vert = player.GetAxis("Joystick 2 Vert");

    joy1Left = joystick1Horz < 0.0f;
    joy1Right = joystick1Horz > 0.0f;
    joy1Up = joystick1Vert > 0.0f;
    joy1Down = joystick1Vert < 0.0f;
    joy2Left = joystick2Horz < 0.0f;
    joy2Right = joystick2Horz > 0.0f;
    joy2Up = joystick2Vert < 0.0f;
    joy2Down = joystick2Vert > 0.0f;

    /*
    if (GetMovementEnabled())
    {
      down = Input.GetAxis("Vertical") >= 1.0f || Input.GetKey(KeyCode.S);
      up = Input.GetAxis("Vertical") <= -1.0f || Input.GetKey(KeyCode.W);
      left = Input.GetAxis("Horizontal") <= -1.0f || Input.GetKey(KeyCode.A);
      right = Input.GetAxis("Horizontal") >= 1.0f || Input.GetKey(KeyCode.D);
      down2 = Input.GetAxis("Axis 5") <= -1.0f || Input.GetKey(KeyCode.DownArrow);
      up2 = Input.GetAxis("Axis 5") >= 1.0f || Input.GetKey(KeyCode.UpArrow);
    }
    else
    {
      down = false;
      up = false;
      left = false;
      right = false;
      down2 = false;
      up2 = false;
    }
    */
    

    if (GetMovementEnabled())
    {
      if (CheckInput("joy1Left"))
      {
        slew_rotation_y -= SLEW_RATE * Time.deltaTime;
        //Debug.Log("slew_rotation_y:" + (90.0f - slew_rotation_y));
      }

      if (CheckInput("joy1Right"))
      {
        slew_rotation_y += SLEW_RATE * Time.deltaTime;
        //Debug.Log("slew_rotation_y:" + (90.0f - slew_rotation_y));
      }

      if (CheckInput("joy1Up"))
      {
        trolley_position_z += TROLLEY_RATE * Time.deltaTime;
      }

      if (CheckInput("joy1Down"))
      {
        trolley_position_z -= TROLLEY_RATE * Time.deltaTime;
      }

      if (CheckInput("joy2Up"))
      {
        hook_position -= HOOK_RATE * Time.deltaTime;
      }

      if (CheckInput("joy2Down"))
      {
        hook_position += HOOK_RATE * Time.deltaTime;
        //Debug.Log("hook_position: " + hook_position);
      }

      if (Input.GetKey(KeyCode.A) && (controlMask == ControlMask.All || controlMask == ControlMask.LeftJoystick))
      {
        slew_rotation_y -= SLEW_RATE * Time.deltaTime;
        joy1Left = true;
        joystick1Horz = -1;
      }
      if (Input.GetKey(KeyCode.D) && (controlMask == ControlMask.All || controlMask == ControlMask.LeftJoystick))
      {
        slew_rotation_y += SLEW_RATE * Time.deltaTime;
        joy1Right = true;
        joystick1Horz = 1;
      }
      if (Input.GetKey(KeyCode.S) && (controlMask == ControlMask.All || controlMask == ControlMask.LeftJoystick))
      {
        trolley_position_z -= TROLLEY_RATE * Time.deltaTime;
        joy1Down = true;
        joystick1Vert = -1;
      }
      if (Input.GetKey(KeyCode.W) && (controlMask == ControlMask.All || controlMask == ControlMask.LeftJoystick))
      {
        trolley_position_z += TROLLEY_RATE * Time.deltaTime;
        joy1Up = true;
        joystick1Vert = 1;
      }
      if (Input.GetKey(KeyCode.LeftArrow) && (controlMask == ControlMask.All || controlMask == ControlMask.RightJoystick))
      {
        joy2Left = true;
        joystick2Horz = -1;
      }
      if (Input.GetKey(KeyCode.RightArrow) && (controlMask == ControlMask.All || controlMask == ControlMask.RightJoystick))
      {
        joy2Right = true;
        joystick2Horz = 1;
      }
      if (Input.GetKey(KeyCode.UpArrow) && (controlMask == ControlMask.All || controlMask == ControlMask.RightJoystick))
      {
        hook_position -= HOOK_RATE * Time.deltaTime;
        joy2Up = true;
        joystick2Vert = 1;
      }
      if (Input.GetKey(KeyCode.DownArrow) && (controlMask == ControlMask.All || controlMask == ControlMask.RightJoystick))
      {
        hook_position += HOOK_RATE * Time.deltaTime;
        joy2Down = true;
        joystick2Vert = -1;
      }

      if (usingQuest)
      {
        if (inputData.leftController.TryGetFeatureValue(CommonUsages.primary2DAxis, out Vector2 leftThumbstick));
        {
          if (leftThumbstick.x < -0.5f && (controlMask == ControlMask.All || controlMask == ControlMask.LeftJoystick))
          {
            slew_rotation_y -= SLEW_RATE * Time.deltaTime;
            joy1Left = true;
            joystick1Horz = -1;
          }
          if (leftThumbstick.x > 0.5f  && (controlMask == ControlMask.All || controlMask == ControlMask.LeftJoystick))
          {
            slew_rotation_y += SLEW_RATE * Time.deltaTime;
            joy1Right = true;
            joystick1Horz = 1;
          }
          if (leftThumbstick.y < -0.5f && (controlMask == ControlMask.All || controlMask == ControlMask.LeftJoystick))
          {
            trolley_position_z -= TROLLEY_RATE * Time.deltaTime;
            joy1Down = true;
            joystick1Vert = -1;
          }
          if (leftThumbstick.y > 0.5f && (controlMask == ControlMask.All || controlMask == ControlMask.LeftJoystick))
          {
            trolley_position_z += TROLLEY_RATE * Time.deltaTime;
            joy1Up = true;
            joystick1Vert = 1;
          }
        }
        
        if (inputData.rightController.TryGetFeatureValue(CommonUsages.primary2DAxis, out Vector2 rightThumbstick));
        {
          if (rightThumbstick.x < -0.5f && (controlMask == ControlMask.All || controlMask == ControlMask.RightJoystick))
          {
            joy2Left = true;
            joystick2Horz = -1;
          }
          if (rightThumbstick.x > 0.5f && (controlMask == ControlMask.All || controlMask == ControlMask.RightJoystick))
          {
            joy2Right = true;
            joystick2Horz = 1;
          }
          if (rightThumbstick.y > 0.5f && (controlMask == ControlMask.All || controlMask == ControlMask.RightJoystick))
          {
            hook_position -= HOOK_RATE * Time.deltaTime;
            joy2Up = true;
            joystick2Vert = 1;
          }
          if (rightThumbstick.y < -0.5f && (controlMask == ControlMask.All || controlMask == ControlMask.RightJoystick))
          {
            hook_position += HOOK_RATE * Time.deltaTime;
            joy2Down = true;
            joystick2Vert = -1;
          }
        }
      }

      // clamp values to physical bounds
      trolley_position_z = Mathf.Clamp(trolley_position_z, MIN_TROLLEY_POSITION, MAX_TROLLEY_POSITION);
      hook_position = Mathf.Clamp(hook_position, MIN_HOOK_POSITION, MAX_HOOK_POSITION);

      slew_rotation_y = Mathf.Clamp(slew_rotation_y, MIN_SLEW_POSITION, MAX_SLEW_POSITION);

      // update crane positions and rotations
      slew.localRotation = Quaternion.Euler(-89.98f, slew_rotation_y, 0.0f);
      trolley.localPosition = new Vector3(trolley.localPosition.x, trolley.localPosition.y, trolley_position_z);

      // update hook position and rotation
      float hook_position_delta = hook_position - old_hook_position;
      if (hook_position_delta != 0)
      {
        //Debug.Log("hook_position: " + hook_position + " hook_position_delta: " + hook_position_delta);
        cable.GetComponent<UltimateRope>().ExtendRope(UltimateRope.ERopeExtensionMode.LinearExtensionIncrement, hook_position_delta);
		cable2.GetComponent<UltimateRope>().ExtendRope(UltimateRope.ERopeExtensionMode.LinearExtensionIncrement, hook_position_delta);
      }
    }

    // feed inputs to the joystick animator
    if (joystickAnimator)
    {
      joystickAnimator.SetFloat("LeftX", joystick1Horz);
      joystickAnimator.SetFloat("LeftY", joystick1Vert);
      joystickAnimator.SetFloat("RightX", joystick2Horz);
      joystickAnimator.SetFloat("RightY", joystick2Vert);
    }

  }

  #region Quest-Specific Code
  
  
  #endregion
  
  public override bool IsMoving()
  {
    return GetMovementEnabled() && HasAnyInput();
  }

  public void SetControlMask(ControlMask controlMask)
  {
    Debug.Log("SetControlMask:" + controlMask);
    this.controlMask = controlMask;
  }

  public ControlMask GetControlMask()
  {
    return controlMask;
  }

  public bool CheckInput(string control)
  {
    switch (control)
    {
      case "joy1Left":
        return joy1Left && (controlMask == ControlMask.All || controlMask == ControlMask.LeftJoystick);
      case "joy1Right":
        return joy1Right && (controlMask == ControlMask.All || controlMask == ControlMask.LeftJoystick);
      case "joy1Up":
        return joy1Up && (controlMask == ControlMask.All || controlMask == ControlMask.LeftJoystick);
      case "joy1Down":
        return joy1Down && (controlMask == ControlMask.All || controlMask == ControlMask.LeftJoystick);
      case "joy2Up":
        return joy2Up && (controlMask == ControlMask.All || controlMask == ControlMask.RightJoystick);
      case "joy2Down":
        return joy2Down && (controlMask == ControlMask.All || controlMask == ControlMask.RightJoystick);
      default:
        return false;
    }
  }

  public override bool HasAnyInput()
  {
    return CheckInput("joy1Left") || CheckInput("joy1Right") || CheckInput("joy1Up") || CheckInput("joy1Down") || CheckInput("joy2Up") || CheckInput("joy2Down");
  }

  public override void Reset()
  {
    slew_rotation_y = 0;
    trolley_position_z = DEFAULT_TROLLEY_POSITION;
    hook_position = DEFAULT_HOOK_POSITION;
  }

  public bool GetLeft()
  {
    return CheckInput("joy1Left");
  }

  public bool GetRight()
  {
    return CheckInput("joy1Right");
  }

  public bool GetUp()
  {
    return CheckInput("joy1Up");
  }

  public bool GetDown()
  {
    return CheckInput("joy1Down");
  }

  public bool GetHookDown()
  {
    return CheckInput("joy2Down");
  }

  public bool GetHookUp()
  {
    return CheckInput("joy2Up");
  }

  public float GetSlewRotation()
  {
    return slew_rotation_y;
  }

  public float GetTrolleyPosition()
  {
    return trolley_position_z;
  }

  public float GetHookPosition()
  {
    return hook_position;
  }

  public void SetHasLoad(bool hasLoad)
  {
    this.hasLoad = hasLoad;
  }

  public bool GetHasLoad()
  {
    return hasLoad;
  }
}
