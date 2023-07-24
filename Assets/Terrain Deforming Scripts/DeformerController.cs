using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeformerController : MonoBehaviour {

    [SerializeField] Deformer deformer;
    [SerializeField] Terrain terrain;

    private float dirtVolumeRemoved = 0f;

    public void Start()
    {
        //deformer.ShowAreaVertices();
    }

    public float DirtVolumeRemoved
    {
        get
        {
            return dirtVolumeRemoved;
        }
    }

    //Gets ready to deform the ground
    public void DeformGround()
    {
        deformer.Subtract();
        dirtVolumeRemoved = deformer.GetDirtDisplacement;
    }

    //actually deforms the ground now (more optimised approach, according to line 42 in Deformer.cs 
    public void TriggerDeformGround()
    {
        //deformer.UpdateAreaPosition();
        deformer.Smooth();
        deformer.SetHeights();
        deformer.TextureNow();
        terrain.ApplyDelayedHeightmapModification();
    }
}
