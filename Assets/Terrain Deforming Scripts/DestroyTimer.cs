using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyTimer : MonoBehaviour {

    [SerializeField] float DestroyAfterDuration = 60f;
    private float timer;
	
	void Update ()
    {
        timer += Time.deltaTime;

        if (timer > DestroyAfterDuration)
        {
            Destroy(this.gameObject);
        }
    }
}
