// Russell Todd     9/21/2023
// Will simply read data from the data files and store it in a List of Arrays

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class DataLoadFromTxt : MonoBehaviour
{
    public TextAsset csvData;

    //public static string tobiiDataFilepath;
    //public static string screenDataFilepath;

    public static bool tobiiDataLoaded;
    //   [Data Entry][timestamp,
    //   gaze2d_x,gaze2d_y,
    //   gaze3d_x,gaze3d_y,gaze3d_z,
    //   eyeleft_gazeorigin_x,  eyeleft_gazeorigin_y,   eyeleft_gazeorigin_z,   eyeleft_gazedirection_x,    eyeleft_gazedirection_y,    eyeleft_gazedirection_z,
    //   eyeright_gazeorigin_x, eyeright_gazeorigin_y,  eyeright_gazeorigin_z,  eyeright_gazedirection_x,   eyeright_gazedirection_y,   eyeright_gazedirection_z]
    public static List<decimal[]> tobiiDataList;


    struct motiveMarkerData
    {
        string markerName;
        List<decimal[]> dataPoints; // [X, Y, Z, quality, frame, time]
    }

    public static bool motiveDataLoaded;
    //  [Data Entry][]
    public static List<decimal[]> motiveDataList;


    public static bool screenDataLoaded;
    //  [Data Entry][WorldToViewportPoint.x, WorldToViewportPoint.y, timestamp]
    public static List<decimal[]> screenDataList;

    public static decimal tobiiTimeOffset;
    public static decimal screenTimeOffset;

    public static int completeRowSize;
    public static int numOfMarkers;
    public static int framesTossed;

    private void Start()
    {
        tobiiDataLoaded = false;
        motiveDataLoaded = false;
        screenDataLoaded = false;
        //tobiiDataFilepath = "Assets/Resources/ValidGridTest/tobiieyedata.txt";
        //screenDataFilepath = "Assets/Resources/ValidGridTest/screenData.csv";
    }

    public static List<decimal[]> LoadTobiiData(string tobiiDatafileName)
    {
        //Debug.Log("LoadTobiiDataCalled");

        tobiiDataList = new List<decimal[]>();

        //Debug.Log(tobiiDatafileName);

        TextAsset csvData = new TextAsset(File.ReadAllText(tobiiDatafileName));

        //Debug.Log(csvData.text);

        string[] dataRows = csvData.text.Split(new char[] { '\n' });

        //Debug.Log("Number of tobiiData Entries: " + data.Length);

        for (int i = 1; i < dataRows.Length; i++) // starting at 1 to skip column names
        {
            //Debug.Log("i: " + i + " Data: " + data[i]);

            // Split, Trim, & Parse
            string[] tempRow = dataRows[i].Split(new char[] { ',' });
            decimal[] tempDecimals = new decimal[20];
            //Debug.Log("temprow length: " + tempRow.Length);
            for (int j = 0; j < tempRow.Length; j++) // Trim spaces from string
            {
                //Debug.Log("j: " + j);

                tempRow[j] = tempRow[j].Trim();
                //Debug.Log("tempRow[" + j + "]: " + tempRow[j]);
                if (decimal.TryParse(tempRow[j], out tempDecimals[j]) == false) 
                {
                    Debug.Log("Failed to parse Tobii data row: " + i + ", column: " + j);
                }
                //tempDecimals[j] = decimal.Parse(tempRow[j]);

                //Debug.Log("tempDecimals element #" + j + " = " + tempRow0[j]);
            }

            tobiiDataList.Add(tempDecimals);
        }

        tobiiTimeOffset = tobiiDataList[0][0];

        for(int i = 0; i < tobiiDataList.Count; i++)
        {
            tobiiDataList[i][0] = tobiiDataList[i][0] - tobiiTimeOffset;
        }

        tobiiDataLoaded = true;
        Debug.Log("Tobii Data Loaded");
        return tobiiDataList;
    }

    public static List<decimal[]> LoadScreenData(string screenDatafileName)
    {
        //Debug.Log("LoadTobiiDataCalled");

        screenDataList = new List<decimal[]>();

        TextAsset csvData = new TextAsset(System.IO.File.ReadAllText(screenDatafileName));

        string[] dataRows = csvData.text.Split(new char[] { '\n' });

        //Debug.Log("Number of screenData Entries: " + data.Length);

        for (int i = 0; i < dataRows.Length; i++) // for the number of data entries
        {
            //Debug.Log("i: " + i);

            // Split, Trim, & Parse
            string[] tempRow = dataRows[i].Split(new char[] { ',' });
            decimal[] tempDecimals = new decimal[3];
            //Debug.Log("temprow length: " + tempRow.Length);

            for (int j = 0; j < tempRow.Length; j++) // Trim spaces from string
            {
                //Debug.Log("j: " + j);

                tempRow[j] = tempRow[j].Trim();
                //Debug.Log("tempRow[" + j + "]: " + tempRow[j]);
                tempDecimals[j] = decimal.Parse(tempRow[j]);

                //Debug.Log("tempDecimals element #" + j + " = " + tempRow0[j]);
            }

            screenDataList.Add(tempDecimals);
        }

        screenTimeOffset = screenDataList[0][2];

        for (int i = 0; i < screenDataList.Count; i++)
        {
            screenDataList[i][2] = screenDataList[i][2] - screenTimeOffset;
        }
        screenDataLoaded = true;
        Debug.Log("Screen Data Loaded");
        return screenDataList;
    }

    public static List<decimal[]> LoadMotiveData(string motiveDatafileName) // incomplete
    {
        //Debug.Log("LoadMotiveData called");
        framesTossed = 0;
        motiveDataList = new List<decimal[]>();

        TextAsset csvData = (TextAsset)Resources.Load(motiveDatafileName);

        string[] dataRows = csvData.text.Split(new char[] { '\n' });

        //Debug.Log("Number of motiveData Entries: " + data.Length);

        completeRowSize = dataRows[6].Split(new char[] { ',' }).Length;
        numOfMarkers = (completeRowSize - 2) / 4;

        for (int i = 7; i < dataRows.Length; i++) // for the number of data entries
        { // 7 is first row past header rows

            //Debug.Log("i: " + i);

            // Split, Trim, & Parse
            string[] tempRow = dataRows[i].Split(new char[] { ',' });

            if(tempRow.Length !< completeRowSize)
            { 
                decimal[] tempDecimals = new decimal[completeRowSize];

                for (int j = 0; j < tempRow.Length; j++) // Trim spaces from string
                {
                    //Debug.Log("j: " + j);

                    tempRow[j] = tempRow[j].Trim();
                    tempDecimals[j] = decimal.Parse(tempRow[j]);

                    //Debug.Log("tempDecimals element #" + j + " = " + tempRow0[j]);
                }

                motiveDataList.Add(tempDecimals);
            }
            else
            {
                framesTossed++;
            }
        }

        Debug.Log("Frames Tossed: " + framesTossed);

        for (int i = 0; i < numOfMarkers; i++)
        {

        }



        motiveDataLoaded = true;
        Debug.Log("Motive Data Loaded");
        return motiveDataList;
    }
}
