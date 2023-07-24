using System.Collections.Generic;
using UnityEngine;

public class GestureEventListener : MonoBehaviour
{
  Dictionary<string, bool> gestureStates = new Dictionary<string, bool>();

  public void AddGesture(string gestureName)
  {
    //Debug.Log("AddGesture() " + gestureName);
    gestureStates[gestureName] = false;
  }

  public void GestureStart(string gestureName)
  {
    Debug.Log("GestureStart() " + gestureName);
    gestureStates[gestureName] = true;
  }

  public void GestureEnd(string gestureName)
  {
    Debug.Log("GestureEnd() " + gestureName);
    gestureStates[gestureName] = false;
  }

  public bool IsOn(string gestureName)
  {
    if (!gestureStates.ContainsKey(gestureName))
    {
      Debug.LogWarning("GestureEventListener.IsOn() called with unknown gesture name: " + gestureName + ". Known gestures are: " + GetGestures() );
      return false;
    }

    return gestureStates[gestureName];
  }

  public string GetGestures()
  {
    string result = "";

    foreach(string key in gestureStates.Keys)
    {
      result += gestureStates[key] + ", ";
    }

    return result;
  }
}

