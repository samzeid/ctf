using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeepUnderTrolley : MonoBehaviour {

    public Transform target;
    public float magic = 0.1f;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void FixedUpdate()
    {
        Vector3 underTarget = new Vector3(target.position.x, transform.position.y - magic, target.position.z);
        Vector3 interpolated = transform.position + magic * (underTarget - transform.position);
        GetComponent<Rigidbody>().MovePosition(interpolated);
    }
}
