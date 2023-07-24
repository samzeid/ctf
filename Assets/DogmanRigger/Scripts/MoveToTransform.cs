using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveToTransform : MonoBehaviour
{

    public Transform target;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    /*
	void FixedUpdate () {
        GetComponent<Rigidbody>().MovePosition(target.position);
	}
    */

    // http://www.vrinflux.com/newton-vr-physics-based-interaction-on-the-vive/
    void FixedUpdate()
    {
        Vector3 PositionDelta = (target.transform.position - transform.position);
        GetComponent<Rigidbody>().velocity = PositionDelta * 10000f * Time.fixedDeltaTime;

        //GetComponent<Rigidbody>().MovePosition(target.position);
        GetComponent<Rigidbody>().MoveRotation(target.rotation);
    }
}
