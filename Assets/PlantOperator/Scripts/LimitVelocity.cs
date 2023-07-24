using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LimitVelocity : MonoBehaviour {

  public float limit;

  private Rigidbody rigidBody;

	// Use this for initialization
	void Start () {
    rigidBody = GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

  void FixedUpdate()
  {
    if (rigidBody.velocity.magnitude > limit)
    {
      rigidBody.velocity = Vector3.ClampMagnitude(rigidBody.velocity, limit);
      Debug.Log("Object " + name + " exceeded 'Rock Prefab' velocity limit!");
    }
  }
}
