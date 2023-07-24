using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CycleMaterials : MonoBehaviour {

  public Material[] materials;
  public float time;

  private MeshRenderer meshRenderer;
  private int materialIndex = 0;
  private float lastSwapTime = 0;

	// Use this for initialization
	void Start () {
    meshRenderer = GetComponent<MeshRenderer>();
	}
	
	// Update is called once per frame
	void Update () {
		if (Time.time >= lastSwapTime + time)
    {
      SwapMaterial();
    }
	}

  public void SwapMaterial()
  {
    materialIndex = (materialIndex + 1) % materials.Length;
    //Debug.Log("SwapMaterial() " + materialIndex);
    meshRenderer.material = materials[materialIndex];
    lastSwapTime = Time.time;
  }
}
