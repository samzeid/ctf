using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoundCreationController : MonoBehaviour {

    [SerializeField]    GameObject MouldCreatorObject;
    [SerializeField]    Deformer deformer;
    [SerializeField]    Terrain terrain;

    public void Start()
    {
        //deformer.UpdateAreaPosition();
    }

    public void CreateMound(Vector3 _position)
    {
        MouldCreatorObject.transform.position = _position;

        StartCoroutine(ABC());
    }

    IEnumerator ABC()
    {
        //returning 0 will make it wait 1 frame
        yield return 0;

        deformer.UpdateAreaPosition();
        deformer.UpdateLocalMaps();
        //deformer.ShowAreaVertices();

        deformer.Add();
        deformer.Smooth();
        deformer.SetHeights();
        //deformer.TextureNow();
        terrain.ApplyDelayedHeightmapModification();
    }
}