using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandSignalWatcher : MonoBehaviour {

    //Version 0.2 (Monday 23/10/2017 5pm WST)

    [SerializeField] HandSignal HandSignalLeft;
    [SerializeField] HandSignal HandSignalRight;

    public bool IsHoist
    {
        get
        {
            if (HandSignalLeft.CheckHoist || HandSignalRight.CheckHoist)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
    public bool IsLower
    {
        get
        {
            if (HandSignalLeft.CheckLower || HandSignalRight.CheckLower)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
    public bool IsSwingLeft
    {
        get
        {
            if (HandSignalLeft.CheckSwingLeft || HandSignalRight.CheckSwingLeft)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
    public bool IsSwingRight
    {
        get
        {
            if (HandSignalLeft.CheckSwingRight || HandSignalRight.CheckSwingRight)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
    public bool IsStop
    {
        get
        {
            if (HandSignalLeft.CheckStop || HandSignalRight.CheckStop)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
