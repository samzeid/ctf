using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SignalFeedbackCanvasHandler : MonoBehaviour {
  public HandSignalWatcher HandSignalScript;
  public Animator isStop;
  public Animator isLower;
  public Animator isRaise;
  public Animator isSwingLeft;

	// Use this for initialization
	void Start () {
    /*
    isStop = gameObject.transform.Find("Container/Stop").GetComponent<Animator>();
    isLower = gameObject.transform.Find("Container/Lower").GetComponent<Animator>();
    isRaise = gameObject.transform.Find("Container/Raise").GetComponent<Animator>();
    isSwingLeft = gameObject.transform.Find("Container/SwingLeft").GetComponent<Animator>();
    */
	}

	// Update is called once per frame
	void Update () {
      isRaise.SetBool("isOn", HandSignalScript.IsHoist);
      isLower.SetBool("isOn", HandSignalScript.IsLower);
      isStop.SetBool("isOn", HandSignalScript.IsStop);
      isSwingLeft.SetBool("isOn",HandSignalScript.IsSwingLeft);
	}
}
