// Version of Trackable that reports screenspace coords instead of euler world coords.

using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
using System.Threading;
using UnityEngine;

public class ScreenspaceTrackable : MonoBehaviour
{
    string ObjName;
    GameObject thisGameObject;
    private string savePath;
    Camera screenCam;
    public TrackableJSON trackJSON;

    void Start()
    {
        trackJSON = new TrackableJSON();
        thisGameObject = this.gameObject; // just saves a tiny bit more human readable. remember just 'this' will refer to the script itself.
        ObjName = this.gameObject.name;
        trackJSON.dataRecord = new List<PosDataPoint>();
        savePath = Application.dataPath + "\\" + "Session_Recordings";
        screenCam = Camera.main;
        //print(savePath);
    }

    void Update()
    {
        // just pushes data into the data structure to be written later
        PosDataPoint tempArray = new PosDataPoint { dataPoint = new float[] { screenCam.WorldToViewportPoint(thisGameObject.transform.position).x, screenCam.WorldToViewportPoint(thisGameObject.transform.position).y, (float)MoveTimer.timer.Elapsed.TotalSeconds } };
        trackJSON.dataRecord.Add(tempArray);
    }

    private void OnDestroy()
    {
        WriteDataCSV();
        WriteDataJSON();
    }

    private void WriteDataCSV()
    {
        System.IO.Directory.CreateDirectory(savePath + "\\" + GetFolderName());
        using (StreamWriter dataWriter = File.AppendText(savePath + "\\" + GetFolderName() + "\\" + ObjName + "_" + GetFilenameLegalDateTime() + ".csv"))
        {
            for (int i = 0; i < trackJSON.dataRecord.Count; i++)
            {
                dataWriter.WriteLine(trackJSON.dataRecord[i].dataPoint[0].ToString() + "," + trackJSON.dataRecord[i].dataPoint[1].ToString() + "," + trackJSON.dataRecord[i].dataPoint[2].ToString());
            }
        }
    }

    private void WriteDataJSON()
    {
        System.IO.Directory.CreateDirectory(savePath + "\\" + GetFolderName());
        using (StreamWriter dataWriter = File.AppendText(savePath + "\\" + GetFolderName() + "\\" + ObjName + "_" + GetFilenameLegalDateTime() + ".JSON"))
        {
            dataWriter.WriteLine(JsonUtility.ToJson(trackJSON));
        }
    }

    private string GetFilenameLegalDateTime()
    {
        return (MoveTimer.startTime.Year.ToString() + "--" + MoveTimer.startTime.Month.ToString() + "--" + MoveTimer.startTime.Day.ToString() + "--" + MoveTimer.startTime.Hour.ToString() + "-" + MoveTimer.startTime.Minute.ToString() + "-" + MoveTimer.startTime.Second.ToString());
    }

    private string GetFolderName()
    {
        return ("Session_" + GetFilenameLegalDateTime());
    }
}
