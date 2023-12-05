using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlignToCamera : MonoBehaviour
{
    public Vector3 rotationOffset = Vector3.zero;
    public float positionOffset = 0;

    Quaternion angleOffset = Quaternion.identity;

    private void Start()
    {
        angleOffset = Quaternion.Euler(rotationOffset);
        if (positionOffset > 0)
            transform.position = (transform.position - Camera.main.transform.position).normalized * positionOffset;
    }

    void LateUpdate ()
    {
        Quaternion rotation = Quaternion.LookRotation(Camera.main.transform.position - transform.position);
        transform.rotation = rotation * angleOffset;
        //if (positionOffset > 0)
        //    transform.position = (transform.position - Camera.main.transform.position).normalized * positionOffset;
    }
}
