using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnTarget : MonoBehaviour
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
    }

    // Update is called once per frame
    void Update()
    {
        totalFrames++;
        if (Input.GetKeyDown(KeyCode.Space))
        {
            RecordingSwitch(isRecording);
        }

        if(isRecording == true)
        {
            if(isOnTarget == true)
            {
                framesOnTarget++;
            }
            else
            {
                framesOffTarget++;
            }
        }
    }


    void RecordingSwitch(bool curState)
    {
        if(curState == false)
        {
            isRecording = true;
        }
        else
        {
            isRecording = false;
            print("framesOnTarget: " + framesOnTarget + ", framesOffTarget: " + framesOffTarget + ", totalFrames: " + totalFrames);
            print("Percentage on target: " + (framesOnTarget / totalFrames) * 100 + "%");
        }
    }


}
