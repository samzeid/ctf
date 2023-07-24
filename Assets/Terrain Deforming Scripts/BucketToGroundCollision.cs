using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BucketToGroundCollision : MonoBehaviour {

    [SerializeField] DeformerController deformerController;
    [SerializeField] DirtCubeSpawner dirtBallController;
    [SerializeField] GameObject BucketTipObject;
    [SerializeField] GameObject[] ScoopRaycastObjects;
    [SerializeField] GameObject[] ScoopDirtSpawnPositions;

    private float raycastDistance = 0.85f;
    private float delayBetweenDirtSpawns = 0.2f;
    private bool deformGround = false;
    private float[] dirtTimers; //this is how long before another piece of dirt can spawn at that dirt spawn location

    private float MinimumBucketVelocitySQR = 0.001f; //The ecavator bucket must be moving this fast, before ground will be dug. 0.01 = 0.1m/s (this parameter is velocity squared, for optimisation) i.e. 4 = 2m/s, 100 = 10m/s, 
    private Vector3 BucketTipPositionLastFrame;
    private float BucketTipVelocitySQR;

    void Start ()
    {
        dirtTimers = new float[ScoopDirtSpawnPositions.Length];
        BucketTipPositionLastFrame = BucketTipObject.transform.position;
    }
	
	void Update ()
    {
        BucketTipVelocitySQR = (BucketTipObject.transform.position - BucketTipPositionLastFrame).sqrMagnitude / (Time.deltaTime * Time.deltaTime);
        BucketTipPositionLastFrame = BucketTipObject.transform.position;

        if (BucketTipVelocitySQR > MinimumBucketVelocitySQR)
        {
            for (int i = 0; i < ScoopRaycastObjects.Length; i++)
            {
                dirtTimers[i] += Time.deltaTime;

                if (dirtTimers[i] > delayBetweenDirtSpawns && Physics.Raycast(ScoopRaycastObjects[i].transform.position, ScoopRaycastObjects[i].transform.forward, raycastDistance, LayerMask.GetMask("Terrain")))
                {
                    dirtTimers[i] = 0f;
                    deformerController.DeformGround();
                    deformGround = true;
                    dirtBallController.SpawnDirtBall(ScoopDirtSpawnPositions[i].transform.position, ScoopDirtSpawnPositions[i].transform.forward, deformerController.DirtVolumeRemoved);
                }
            }

            if (deformGround == true)
            {
                deformGround = false;
                deformerController.TriggerDeformGround();
            }
        }
    }
}
