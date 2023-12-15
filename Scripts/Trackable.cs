/*
Russell Todd

The idea for this script is that it can be place on any GameObject and will then record the position data for that GameObject. This recording method is FRAME-LOCKED and records where the trackable object was during each frame. It does not enforce a steady recording rate either.
However, it is simple and very easily applied. It should meet our needs as the framerate should be at least 90-120 hz depending on the refresh rate of the head-mounted device. If the framerate dips is below that, we would want to fix that first.

Behaviors to be included: 
1. Saving to a data structure to be printed at the end of the program, to avoid overhead of recording while the program is in use.
2. The output files will be named using the  GameObject's name and the starting date/time so we don't have to worry about overwriting any previous data.
3. The date/time part of the name will be what is used to group datasets together. (Could also potentially include a unique string at the beginning of the output files that would match between all the tracked objects in a single session)
4. Data to be tracked includes Pos X, Pos Y, Pos Z in native Unity units (conversion later? or enforce a scale?) and time according to system time all saved as a simple csv.  (conversion to excel workbooks can be done later) 

Note: It's probably a bad idea to attach this to any object that is destroyed during playtime (for example an apple prefab) as it would then write the data record while play is still ongoing. 
Note: Should probably switch this over to use the CsvHelper library.
*/

using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
using System.Threading;
using UnityEngine;

public class Trackable : MonoBehaviour
{
    string ObjName;
    GameObject thisGameObject;
    private string savePath;

    public TrackableJSON trackJSON;

    void Start()
    {
        trackJSON = new TrackableJSON();
        thisGameObject = this.gameObject; // just saves a tiny bit more human readable. remember just 'this' will refer to the script itself.
        ObjName = this.gameObject.name;
        trackJSON.dataRecord = new List<PosDataPoint>();
        savePath = Application.dataPath + "\\" + "Session_Recordings";

        //print(savePath);
    }

    void Update()
    {
        // just pushes data into the data structure to be written later
        PosDataPoint tempArray = new PosDataPoint { dataPoint = new float[] { thisGameObject.transform.position.x, thisGameObject.transform.position.y, thisGameObject.transform.position.z, (float)AppleTimer.timer.Elapsed.TotalSeconds } };
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
                dataWriter.WriteLine(trackJSON.dataRecord[i].dataPoint[0].ToString() + "," + trackJSON.dataRecord[i].dataPoint[1].ToString() + "," + trackJSON.dataRecord[i].dataPoint[2].ToString() + "," + trackJSON.dataRecord[i].dataPoint[3].ToString());
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
        return (AppleTimer.startTime.Year.ToString() + "--" + AppleTimer.startTime.Month.ToString() + "--" + AppleTimer.startTime.Day.ToString() + "--" + AppleTimer.startTime.Hour.ToString() + "-" + AppleTimer.startTime.Minute.ToString() + "-" + AppleTimer.startTime.Second.ToString());
    }

    private string GetFolderName()
    {
        return ("Session_" + GetFilenameLegalDateTime());
    }
}
