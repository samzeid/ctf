using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadController : MonoBehaviour
{
  public GameObject ring0;
  public GameObject ring1;
  public GameObject ring2;
  public GameObject ring3;

  public MobileCraneController mobileCraneController;

  bool ring0Connected;
  bool ring1Connected;
  bool ring2Connected;
  bool ring3Connected;

  bool holdingGuideRope;

  private const float MAX_CONNECT_DISTANCE = 0.2f;

  // Use this for initialization
  void Start()
  {
    ring0Connected = false;
    ring1Connected = false;
    ring2Connected = false;
    ring3Connected = false;

    holdingGuideRope = false;
  }

  // Update is called once per frame
  void Update()
  {
    if (Input.GetKeyDown(KeyCode.L) || Input.GetButtonDown("Fire2"))
    {
      ConnectRope();
    }

    if (Input.GetKeyDown(KeyCode.G) || Input.GetButtonDown("Fire3"))
    {
      ToggleHoldingGuideRope();
    }
  }

  public void ConnectRope()
  {
    Debug.LogWarning("ConnectRope() -only use in non-VR mode");

    if (!ring0Connected)
    {
      ring0Connected = true;
      mobileCraneController.SetHook0Connected(true);
      Debug.Log("num connected " + GetNumRopesConnected());
      return;
    }
    if (!ring1Connected)
    {
      ring1Connected = true;
      mobileCraneController.SetHook1Connected(true);
      Debug.Log("num connected " + GetNumRopesConnected());
      return;
    }
    if (!ring2Connected)
    {
      ring2Connected = true;
      mobileCraneController.SetHook2Connected(true);
      Debug.Log("num connected " + GetNumRopesConnected());
      return;
    }
    if (!ring3Connected)
    {
      ring3Connected = true;
      mobileCraneController.SetHook3Connected(true);
      Debug.Log("num connected " + GetNumRopesConnected());
      return;
    }
  }

  public void ToggleHoldingGuideRope()
  {
    holdingGuideRope = !holdingGuideRope;
    Debug.Log("Holding Guide Rope: " + holdingGuideRope);
  }

  public int GetNumRopesConnected()
  {
    int result = 0;

    if (ring0Connected) result += 1;
    if (ring1Connected) result += 1;
    if (ring2Connected) result += 1;
    if (ring3Connected) result += 1;

    Debug.Log("GetNumRopesConnected() : " + result);

    return result;
  }

  public bool GetHoldingGuideRope()
  {
    return holdingGuideRope;
  }

  public void SetRing0Connected(bool connected)
  {
    ring0Connected = connected;
  }

  public void SetRing1Connected(bool connected)
  {
    ring1Connected = connected;
  }

  public void SetRing2Connected(bool connected)
  {
    ring2Connected = connected;
  }

  public void SetRing3Connected(bool connected)
  {
    ring3Connected = connected;
  }

  public bool GetRing0Connected()
  {
    return ring0Connected;
  }

  public bool GetRing1Connected()
  {
    return ring1Connected;
  }

  public bool GetRing2Connected()
  {
    return ring2Connected;
  }

  public bool GetRing3Connected()
  {
    return ring3Connected;
  }

  public void SetConnected(GameObject connectedObject)
  {
    if (connectedObject == ring0)
    {
      ring0Connected = true;
    }
    if (connectedObject == ring1)
    {
      ring1Connected = true;
    }
    if (connectedObject == ring2)
    {
      ring2Connected = true;
    }
    if (connectedObject == ring3)
    {
      ring3Connected = true;
    }

    if (GetNumRopesConnected() == 4)
    {
      // debug
      Debug.Log("Load turned off isKinematic");
      GetComponent<Rigidbody>().isKinematic = false;
    }
  }

  public GameObject GetClosestRingObject(Vector3 position)
  {
    GameObject closestObject = null;
    float closestDistance = float.MaxValue;
    float distance;

    Debug.Log("GetClosestRingObject() " + "r0:" + ring0Connected + " r1:" + ring1Connected + " r2:" + ring2Connected + " r3:" + ring3Connected);

    // check ring0
    if (!ring0Connected) {
      distance = Vector3.Distance(position, ring0.transform.position);
      if (distance < closestDistance)
      {
        closestObject = ring0;
        closestDistance = distance;
      }
    }

    // check ring1
    if (!ring1Connected)
    {
      distance = Vector3.Distance(position, ring1.transform.position);
      if (distance < closestDistance)
      {
        closestObject = ring1;
        closestDistance = distance;
      }
    }

    // check ring2
    if (!ring2Connected)
    {
      distance = Vector3.Distance(position, ring2.transform.position);
      if (distance < closestDistance)
      {
        closestObject = ring2;
        closestDistance = distance;
      }
    }

    // check craneHook3
    if (!ring3Connected)
    {
      distance = Vector3.Distance(position, ring3.transform.position);
      if (distance < closestDistance)
      {
        closestObject = ring3;
        closestDistance = distance;
      }
    }

    // if closestDistance is too far, return null
    if (closestDistance > MAX_CONNECT_DISTANCE)
    {
      Debug.Log("closest hookup still too far away (" + closestDistance + ")");
      return null;
    }
    else
    {
      Debug.Log("closest hookup " + closestObject.name);
    }

    return closestObject;
  }
}
