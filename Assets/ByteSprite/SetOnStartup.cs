using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetOnStartup : MonoBehaviour {
    [SerializeField]
    Transform headsetParent;
    
    [SerializeField]
    Transform headset;

    void Start() {
        headsetParent.transform.position -= headset.transform.position;
    }
}
