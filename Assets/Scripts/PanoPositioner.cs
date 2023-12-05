using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PanoPositioner : MonoBehaviour
{
    public float offset = 480.0f;
    public float scale = 0.5f;
    public Vector2 baseOffset;

    public float width = 8192;
    public float height = 4096;

    public Dictionary<Transform, Vector3> originalPositions = new Dictionary<Transform, Vector3>();

    void Start()
    {
        foreach(Transform t in transform)
        {
            originalPositions[t] = t.position;
            float rx = (t.position.x + baseOffset.x) / width;
            float ry = (t.position.y + baseOffset.y) / height;

            print(t.position.x + "  " + rx);

            t.rotation = Quaternion.Euler(new Vector3(-ry * 360.0f, rx * 360.0f, 0));
            //t.Rotate(new Vector3(-ry * 360.0f, rx * 360.0f, 0));
            t.transform.position = t.forward * offset;
            t.transform.localScale = Vector3.one * scale;
            //t.position = new Vector3(x, t.position.y, z);
        }
    }
   
    /*void Update()
    {
        foreach (Transform t in transform)
        {
            float rx = (originalPositions[t].x + baseOffset.x) / width;
            float ry = (originalPositions[t].y + baseOffset.y) / height;

            t.rotation = Quaternion.Euler(new Vector3(-ry * 180.0f, rx * 360.0f, 0));
            //t.Rotate(new Vector3(-ry * 360.0f, rx * 360.0f, 0));

            t.transform.position = t.forward * offset;
            t.transform.localScale = Vector3.one * scale;
        }
    }*/
}
