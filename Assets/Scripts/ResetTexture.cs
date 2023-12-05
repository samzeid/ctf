using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class ResetTexture : MonoBehaviour
{
	void Awake ()
    {
        RenderTexture rt = UnityEngine.RenderTexture.active;
        VideoPlayer v = GetComponent<VideoPlayer>();
        if (!v.isPrepared)
        {
            UnityEngine.RenderTexture.active = v.targetTexture;
            GL.Clear(true, true, Color.black);
            UnityEngine.RenderTexture.active = rt;
        }
    }

    private void FixedUpdate()
    {
        VideoPlayer v = GetComponent<VideoPlayer>();
        //DebugOutput.Log(v.isPrepared + "   " + v.isPlaying + "  " + v.targetTexture + "   " + v.time);
        if (!v.isPlaying)
        {
            v.Play();
        }
    }
}
