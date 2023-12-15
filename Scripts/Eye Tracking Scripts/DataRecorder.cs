using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
using System.Threading;
using UnityEngine;

public class DataRecorder : MonoBehaviour
{
    private string savePath;

    void Start()
    {
        savePath = Application.dataPath + "\\" + "Session_Recordings";
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
