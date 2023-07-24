using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JoinToController : MonoBehaviour {

    public string controllerFindString = "[CameraRig]/Controller (right)";
    public GameObject _controller = null;
    public FixedJoint _joint = null;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

        GameObject controller = GameObject.Find(controllerFindString);

        if (controller == null)
        {
            Debug.Log("No controller found");

            _controller = null;
            Destroy(_joint);
        }
        else if (_controller == null)
        {
            Debug.Log("Connecting to controller");

            if (controller)
            {
                _controller = controller;

                //GetComponent<Rigidbody>().MovePosition(_controller.transform.position);
                //GetComponent<Rigidbody>().MoveRotation(_controller.transform.rotation);

                transform.position = _controller.transform.position;
                transform.rotation = _controller.transform.rotation;

                _joint = gameObject.AddComponent<FixedJoint>();
                _joint.connectedBody = controller.GetComponent<Rigidbody>();
            }
        }


	}
}
