using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestingUI : MonoBehaviour {

    //Version 0.2 (Monday 23/10/2017 5pm WST)

    [SerializeField] HandSignalWatcher HandSignalScript;
    [SerializeField] Text IsHoistText;
    [SerializeField] Text IsLowerText;
    [SerializeField] Text IsSwingLeftText;
    [SerializeField] Text IsSwingRightText;
    [SerializeField] Text IsStopText;
	
	void Update ()
    {
        if (HandSignalScript.IsHoist){IsHoistText.color = Color.green;}
        else {IsHoistText.color = Color.grey;}

        if (HandSignalScript.IsLower) { IsLowerText.color = Color.green; }
        else { IsLowerText.color = Color.grey; }

        if (HandSignalScript.IsSwingLeft) { IsSwingLeftText.color = Color.green; }
        else { IsSwingLeftText.color = Color.grey; }

        if (HandSignalScript.IsSwingRight) { IsSwingRightText.color = Color.green; }
        else { IsSwingRightText.color = Color.grey; }

        if (HandSignalScript.IsStop) { IsStopText.color = Color.green; }
        else { IsStopText.color = Color.grey; }
    }
}
