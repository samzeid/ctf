using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowGameObjectWithKey : MonoBehaviour {

  public bool showByDefault;
  public GameObject objectToShow;
  public KeyCode keyCode;

  private bool show;

	// Use this for initialization
	void Start () {
    show = showByDefault;
    UpdateIsActive();
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyUp(keyCode))
    {
      show = !show;
      UpdateIsActive();
    }
	}

  private void UpdateIsActive()
  {
    objectToShow.SetActive(show);
  }
}
