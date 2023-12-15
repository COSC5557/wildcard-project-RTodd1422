using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class Reporter : MonoBehaviour
{
    static string savePath;
    private void OnApplicationQuit()
    {
        if(ApplePickingGame.noReport == true)
        {
            SaveReport();
        }
    }

    private static void WriteReport()
    {
        string jsonOutput = JsonUtility.ToJson(ApplePickingGame.jsonRecord);

        System.IO.Directory.CreateDirectory(savePath + "\\" + GetFolderName());
        using (StreamWriter dataWriter = File.AppendText(savePath + "\\" + GetFolderName() + "\\" + "Session_Report" + "_" + GetFilenameLegalDateTime() + ".JSON"))
        {
            dataWriter.WriteLine(jsonOutput);
        }
    }

    private static string GetFilenameLegalDateTime()
    {
        return (AppleTimer.startTime.Year.ToString() + "--" + AppleTimer.startTime.Month.ToString() + "--" + AppleTimer.startTime.Day.ToString() + "--" + AppleTimer.startTime.Hour.ToString() + "-" + AppleTimer.startTime.Minute.ToString() + "-" + AppleTimer.startTime.Second.ToString());
    }

    private static string GetFolderName()
    {
        return ("Session_" + GetFilenameLegalDateTime());
    }

    public static void SaveReport()
    {
        savePath = Application.dataPath + "\\" + "Session_Reports";

        ApplePickingGame.jsonRecord.sessionEndTime = System.DateTime.Now;

        WriteReport();
        ApplePickingGame.noReport = false;
    }
}
