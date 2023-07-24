using UnityEngine;

public class MobileCraneController : MachineController {

  public enum GestureMask
  {
    None,
    Stop,
    Hoist,
    Lower,
    Left,
    Right,
    All,
  }

  private GestureEventListener gestureEventListener;
  public HandSignalWatcher handSignalWatcher;

  const float SLEW_RATE = 2.0f;
  const float BOOM_RAISE_RATE = 1.0f; // raise and lower
  const float BOOM_EXTEND_RATE = 1.0f; // extend and retract
  const float HOOK_RATE = 0.5f; // raise and lower hook

  const float DEFAULT_SLEW_ROTATION = -43.6f;
  const float MIN_BOOM1_POSITION = 1.0f;
  const float MAX_BOOM1_POSITION = 9.6f;
  const float MIN_BOOM1_ROTATION = 25.0f;
  const float DEFAULT_BOOM1_ROTATION = 61.216f;
  const float MAX_BOOM1_ROTATION = 90.0f;
  const float MIN_BOOM2_POSITION = 0.0f;
  const float MAX_BOOM2_POSITION = 8.8f;
  const float MIN_BOOM3_POSITION = 0.0f;
  const float MAX_BOOM3_POSITION = 8.8f;
  const float MIN_HOOK_POSITION = 1.25f;
  const float MAX_HOOK_POSITION = 12.0f;
  const float DEFAULT_HOOK_POSITION = 5.0f;

  const float MAX_SLEW_DIFF = 3.0f;
  const float MAX_TROLLEY_DIFF = 0.5f;
  const float MAX_HOOK_DIFF = 0.5f;

  public Transform slew;
  public Transform boom1_rotate; // T1 bone
  public Transform boom1_extend; // T2 bone
  public Transform boom2; // T3 bone
  public Transform boom3; // T4 bone
  public Transform end;
  public GameObject arrows;

  public GameObject cable;
  public GameObject hook;
  public GameObject sling;

  [Header("Sling Hooks")]
  private bool hooksEnabled = false;
  public GameObject hook0;
  public GameObject hook1;
  public GameObject hook2;
  public GameObject hook3;

  private bool hook0Connected = false;
  private bool hook1Connected = false;
  private bool hook2Connected = false;
  private bool hook3Connected = false;

  float boom1_position;
  float boom1_rotation;
  float boom2_position;
  float boom3_position;
  float hook_position;
  float old_hook_position;

  private const float MAX_CONNECT_DISTANCE = 0.2f;

  private GestureMask gestureMask = GestureMask.All;

  // Use this for initialization
  void Start () {
    Reset();

    gestureEventListener = GetComponent<GestureEventListener>();

    gestureEventListener.AddGesture("SlewLeft");
    gestureEventListener.AddGesture("SlewRight");

    gestureEventListener.AddGesture("RaiseLoad");
    gestureEventListener.AddGesture("LowerLoad");
    gestureEventListener.AddGesture("Stop");
  }

  // Update is called once per frame
  void Update () {
    old_hook_position = hook_position;

    if (GetMovementEnabled())
    {
      if (GetSlewLeft())
      {
        slew_rotation_y += SLEW_RATE * Time.deltaTime;
      }
      if (GetSlewRight())
      {
        slew_rotation_y -= SLEW_RATE * Time.deltaTime;
      }

      if (GetRaiseLoad())
      {
        hook_position -= HOOK_RATE * Time.deltaTime;
      }
      if (GetLowerLoad())
      {
        hook_position += HOOK_RATE * Time.deltaTime;
      }
    }

    // clamp values to physical bounds
    boom1_position = Mathf.Clamp(boom1_position, MIN_BOOM1_POSITION, MAX_BOOM1_POSITION);
    boom2_position = Mathf.Clamp(boom2_position, MIN_BOOM2_POSITION, MAX_BOOM2_POSITION);
    hook_position = Mathf.Clamp(hook_position, MIN_HOOK_POSITION, MAX_HOOK_POSITION);

    // update crane positions and rotations
    slew.localRotation = Quaternion.Euler(0.0f, slew_rotation_y, 0.0f);
    //boom1_rotate.localRotation = Quaternion.Euler(boom1_rotation, 90.0f, 0.0f);
    //boom1_extend.localPosition = new Vector3(boom1_extend.localPosition.x, boom1_position, boom1_extend.localPosition.z);
    //boom2.localPosition = new Vector3(boom2.localPosition.x, boom2_position, boom2.localPosition.z);
    //boom3.localPosition = new Vector3(boom3.localPosition.x, boom3_position, boom3.localPosition.z);

    // update hook position and rotation
    float hook_position_delta = hook_position - old_hook_position;
    if (hook_position_delta != 0)
    {
      Debug.Log("hook_position: " + hook_position);
      cable.GetComponent<UltimateRope>().ExtendRope(UltimateRope.ERopeExtensionMode.LinearExtensionIncrement, hook_position_delta);
    }
  }

  public override bool IsMoving()
  {
    //return GetMovementEnabled() && (gestureEventListener.IsOn("SlewLeft") || gestureEventListener.IsOn("SlewRight") || gestureEventListener.IsOn("RaiseLoad") || gestureEventListener.IsOn("LowerLoad"));
    return GetMovementEnabled() && HasAnyInput();
  }

  public void SetGestureMask(GestureMask gestureMask)
  {
    Debug.Log("SetGestureMask:" + gestureMask);
    this.gestureMask = gestureMask;
  }

  public GestureMask GetGestureMask()
  {
    return gestureMask;
  }

  public bool CheckInput(string gesture)
  {
    switch (gesture)
    {
      case "Stop":
        return handSignalWatcher.IsStop && (gestureMask == GestureMask.All || gestureMask == GestureMask.Stop);
      case "Hoist":
        return handSignalWatcher.IsHoist && (gestureMask == GestureMask.All || gestureMask == GestureMask.Hoist);
      case "Lower":
        return handSignalWatcher.IsLower && (gestureMask == GestureMask.All || gestureMask == GestureMask.Lower);
      case "Left":
        return handSignalWatcher.IsSwingLeft && (gestureMask == GestureMask.All || gestureMask == GestureMask.Left);
      case "Right":
        return handSignalWatcher.IsSwingRight && (gestureMask == GestureMask.All || gestureMask == GestureMask.Right);
      default:
        return false;
    }
  }

  public bool CheckGestureListener(string gesture)
  {
    switch (gesture) {
      case "SlewLeft":
        return gestureEventListener.IsOn("SlewLeft") && (gestureMask == GestureMask.All || gestureMask == GestureMask.Left);
      case "SlewRight":
        return gestureEventListener.IsOn("SlewRight") && (gestureMask == GestureMask.All || gestureMask == GestureMask.Right);
      case "LowerLoad":
        return gestureEventListener.IsOn("LowerLoad") && (gestureMask == GestureMask.All || gestureMask == GestureMask.Lower);
      case "RaiseLoad":
        //Debug.Log("GE Raise" + gestureEventListener.IsOn("RaiseLoad") + " gesture mask all" + (gestureMask == GestureMask.All) + " gesture mask hoist " + (gestureMask == GestureMask.Hoist));
        return gestureEventListener.IsOn("RaiseLoad") && (gestureMask == GestureMask.All || gestureMask == GestureMask.Hoist);
      case "Stop":
        //Debug.Log("GE Stop" + gestureEventListener.IsOn("Stop") + " gesture mask all" + (gestureMask == GestureMask.All) + " gesture mask stop " + (gestureMask == GestureMask.Stop));
        return gestureEventListener.IsOn("Stop") && (gestureMask == GestureMask.All || gestureMask == GestureMask.Stop);
      default:
        return false;
    }
  }

  public override bool HasAnyInput()
  {
    return CheckInput("Left") || CheckInput("Right") || CheckInput("Hoist") || CheckInput("Lower");
  }

  public override void Reset()
  {
    slew_rotation_y = DEFAULT_SLEW_ROTATION;
    boom1_position = MIN_BOOM1_POSITION;
    boom1_rotation = DEFAULT_BOOM1_ROTATION;
    boom2_position = MIN_BOOM2_POSITION;
    boom3_position = MAX_BOOM3_POSITION;
    hook_position = DEFAULT_HOOK_POSITION;
  }

  public void SetHook0Connected(bool connected)
  {
    hook0Connected = connected;
  }

  public void SetHook1Connected(bool connected)
  {
    hook1Connected = connected;
  }

  public void SetHook2Connected(bool connected)
  {
    hook2Connected = connected;
  }

  public void SetHook3Connected(bool connected)
  {
    hook3Connected = connected;
  }

  public bool GetHook0Connected()
  {
    return hook0Connected;
  }

  public bool GetHook1Connected()
  {
    return hook1Connected;
  }

  public bool GetHook2Connected()
  {
    return hook2Connected;
  }

  public bool GetHook3Connected()
  {
    return hook3Connected;
  }

  public void SetConnected(GameObject connectedObject)
  {
    if (connectedObject == hook0)
    {
      hook0Connected = true;
    }
    if (connectedObject == hook1)
    {
      hook1Connected = true;
    }
    if (connectedObject == hook2)
    {
      hook2Connected = true;
    }
    if (connectedObject == hook3)
    {
      hook3Connected = true;
    }
  }

  public bool GetSlewLeft()
  {
    return CheckGestureListener("SlewLeft") || CheckInput("Left");
  }

  public bool GetSlewRight()
  {
    return CheckGestureListener("SlewRight") || CheckInput("Right");
  }

  public bool GetLowerLoad()
  { 
    return CheckGestureListener("LowerLoad") || CheckInput("Lower");
  }

  public bool GetRaiseLoad()
  {
    return CheckGestureListener("RaiseLoad") || CheckInput("Hoist");
  }

  public bool GetStop()
  {
    return CheckGestureListener("Stop") || CheckInput("Stop");
  }

  public bool GetThumbpadStop()
  {
    return gestureEventListener.IsOn("Stop");
  }

  public void SetHooksEnabled(bool enabled)
  {
    hooksEnabled = enabled;
  }

  public GameObject GetHook()
  {
    return hook;
  }

  // TODO: factor out similar code here (use a GetClosestObject(Vector3 position, GameObject[] objects) function)
  public GameObject GetClosestHookObject(Vector3 position)
  {
    GameObject closestObject = null;
    float closestDistance = float.MaxValue;
    float distance;

    // if the hooks havent been enabled (i.e. are not ready for user interaction) always return null
    if (!hooksEnabled)
    {
      return null;
    }

    //Debug.Log("GetClosestHookupObject() " + "h0:" + hook0Connected + " h1:" + hook1Connected + " h2:" + hook2Connected + " h3:" + hook3Connected);

    // check craneHook0
    if (!hook0Connected)
    {
      //Debug.Log("Check 0");
      distance = Vector3.Distance(position, hook0.transform.position);
      if (distance < closestDistance)
      {
        closestObject = hook0;
        closestDistance = distance;
      }
    }

    // check craneHook1
    if (!hook1Connected)
    {
      //Debug.Log("Check 1");
      distance = Vector3.Distance(position, hook1.transform.position);
      if (distance < closestDistance)
      {
        closestObject = hook1;
        closestDistance = distance;
      }
    }

    // check craneHook2
    if (!hook2Connected)
    {
      //Debug.Log("Check 2");
      distance = Vector3.Distance(position, hook2.transform.position);
      if (distance < closestDistance)
      {
        closestObject = hook2;
        closestDistance = distance;
      }
    }

    // check craneHook3
    if (!hook3Connected)
    {
      //Debug.Log("Check 3");
      distance = Vector3.Distance(position, hook3.transform.position);
      if (distance < closestDistance)
      {
        closestObject = hook3;
        closestDistance = distance;
      }
    }

    // if closestDistance is too far, return null
    if (closestDistance > MAX_CONNECT_DISTANCE)
    {
      Debug.Log("closest object still too far away (" + closestDistance + ")");
      return null;
    }
    else
    {
      Debug.Log("closest object " + closestObject.name);
    }

    return closestObject;
  }
}
