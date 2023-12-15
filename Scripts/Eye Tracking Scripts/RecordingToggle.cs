using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecordingToggle : MonoBehaviour
{
    long framesOnTarget;
    long framesOffTarget;
    long totalFrames;
    bool isRecording;

    public static bool isOnTarget;
    void Start()
    {
        isRecording = false;
        isOnTarget = false;
        sxr.StartTimer("RecordTimer");
    }

    // Update is called once per frame
    void Update()
    {
        totalFrames++;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            RecordingSwitch(isRecording);
        }

        //if (isRecording == true)
        //{
        //    if (isOnTarget == true)
        //    {
        //        framesOnTarget++;
        //    }
        //    else
        //    {
        //        framesOffTarget++;
        //    }
        //}
    }


    void RecordingSwitch(bool curState)
    {
        if (curState == false)
        {
            isRecording = true;
            print("RecordTimer start: " + sxr.TimePassed("RecordTimer"));
        }
        else
        {
            isRecording = false;
            //print("framesOnTarget: " + framesOnTarget + ", framesOffTarget: " + framesOffTarget + ", totalFrames: " + totalFrames);
            //print("Percentage on target: " + (framesOnTarget / totalFrames) * 100 + "%");
            print("RecordTimer stop: " + sxr.TimePassed("RecordTimer"));
        }
    }
}
