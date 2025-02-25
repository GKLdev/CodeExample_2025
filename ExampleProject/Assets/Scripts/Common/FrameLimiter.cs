using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrameLimiter : MonoBehaviour
{
    public int  targetFramerate = 60;
    public bool limitFramerate  = false;

    // *****************************
    // OnEnable
    // *****************************
    private void OnEnable()
    {
        LimitFramerate();
    }

    // *****************************
    // LimitFramerate
    // *****************************
    void LimitFramerate()
    {
        if (limitFramerate)
        {
            Application.targetFrameRate = targetFramerate;
        }
        else
        {
            Application.targetFrameRate = -1;
        }
    }
}
