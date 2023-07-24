using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoilArea : MonoBehaviour {

  float depth;

	// Use this for initialization
	void Start () {
    Reset();
	}
	
	// Update is called once per frame
	void Update () {
    if (Input.GetKeyDown(KeyCode.O))
    {
      depth -= 0.1f;
      Debug.Log("depth: " + depth);
    }

    if (Input.GetKeyDown(KeyCode.P))
    {
      depth += 0.1f;
      Debug.Log("depth: " + depth);
    }
  }

  public void Reset()
  {
    depth = 0;
  }

  public float GetDepth()
  {
    return depth;
  }
}
