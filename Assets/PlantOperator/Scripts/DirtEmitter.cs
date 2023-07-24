using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirtEmitter : MonoBehaviour
{
    public ParticleSystem[] particleSystems;

    public int emissionCount = 2;
    public int initialEmissionCount = 2;

    private Vector3 lastPos;

    private void EmitPFX(int count)
    {
        foreach (ParticleSystem ps in particleSystems)
            ps.Emit(count);
    }

    private void OnTriggerEnter(Collider c)
    {
        if (c.gameObject.layer == 8)
        {
            EmitPFX(initialEmissionCount);
        }
    }

    private void OnTriggerStay(Collider c)
    //public void OnCollisionStay(Collision c)
    {
        if (c.gameObject.layer == 8) //If colliding with terrain, emit PFX
        {
            if (transform.position != lastPos)
            {
                EmitPFX(emissionCount);
                lastPos = transform.position;
            }
            //ParticleSystem.EmissionModule em = ps.emission;
            //em.enabled = true;
        }
    }
}
