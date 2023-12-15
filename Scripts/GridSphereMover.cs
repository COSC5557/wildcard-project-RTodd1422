using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using TMPro;
using UnityEngine;

public class GridSphereMover : MonoBehaviour
{

    GameObject focalSphere, GridVertMid, GridVertMidLeft, GridVertFarLeft, GridVertMidRight, GridVertFarRight, GridHorizMid, GridHorizMidUp, GridHorizTop, GridHorizMidDown, GridHorizBottom;
    List<int> currentOrder, OrderLRTB, OrderRLTB, OrderSnake, OrderRandom;
    int orderIndex, moveMode;
    float timePassed, t, timeForPoint;
    bool sphereMoving;
    private string savePath; 
    public TrackableJSON trackJSON;

    public static Stopwatch gridTimer, sceneTimer;
    public static System.DateTime moveStartTime, sceneStartTime;

    public Vector3 oldPos, newPos;

    TMP_Dropdown patternDropdown, moveModeDropdown, timeDropdown;

    private static System.Random rand = new System.Random();

    private void Awake()
    {
        sceneTimer = new Stopwatch();
        sceneStartTime = System.DateTime.Now;
    }

    // Start is called before the first frame update
    void Start()
    {
        focalSphere = GameObject.Find("FocalSphere");
        patternDropdown = GameObject.Find("PatternDropdown").GetComponent<TMP_Dropdown>();
        moveModeDropdown = GameObject.Find("ModeDropdown").GetComponent<TMP_Dropdown>();
        timeDropdown = GameObject.Find("TimeDropdown").GetComponent<TMP_Dropdown>();

        GridVertMid = GameObject.Find("GridVertMid");
        GridVertMidLeft = GameObject.Find("GridVertMidLeft");
        GridVertFarLeft = GameObject.Find("GridVertFarLeft");
        GridVertMidRight = GameObject.Find("GridVertMidRight");
        GridVertFarRight = GameObject.Find("GridVertFarRight");
        GridHorizMid = GameObject.Find("GridHorizMid");
        GridHorizMidUp = GameObject.Find("GridHorizMidUp");
        GridHorizTop = GameObject.Find("GridHorizTop");
        GridHorizMidDown = GameObject.Find("GridHorizMidDown");
        GridHorizBottom = GameObject.Find("GridHorizBottom");


        // Left to Right, Top to Bottom
        OrderLRTB =  new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24 };
        // Right to Left, Top to Bottom
        OrderRLTB =  new List<int>() { 4, 3, 2, 1, 0, 9, 8, 7, 6, 5, 14, 13, 12, 11, 10, 19, 18, 17, 16, 15, 24, 23, 22, 21, 20 };
        // Snake pattern starting in top left and going right
        OrderSnake = new List<int>() { 0, 1, 2, 3, 4, 9, 8, 7, 6, 5, 10, 11, 12, 13, 14, 19, 18, 17, 16, 15, 20, 21, 22, 23, 24 };

        OrderRandom = OrderLRTB;

        // redundant for now, but can allow for different defaults in future. 
        currentOrder = OrderLRTB;
        moveMode = 0;

        orderIndex = 0;

        sphereMoving = false;

        timePassed = 0f;
        t = 0f;
        timeForPoint = 2f;

        savePath = Application.dataPath + "\\" + "Session_Recordings";
        trackJSON = new TrackableJSON();
    }

    // Update is called once per frame
    void Update()
    {
        if (sphereMoving == true)
        {
            timePassed += Time.deltaTime;
            if(timePassed > timeForPoint)
            {
                timePassed = 0f;
                MoveSphereToNextPos();
            }
            if(moveMode == 1)
            {
                focalSphere.transform.position = Vector3.Lerp(oldPos, newPos, timePassed / (timeForPoint/2f));
            }
            else if(moveMode == 2)
            {
                t = timePassed / (timeForPoint / 2f);
                t = t * t * t * (t * (6f * t - 15f) + 10f);
                focalSphere.transform.position = Vector3.Lerp(oldPos, newPos, t);
            }
        }
    }

    public void StartMovingSphere()
    {
        sphereMoving = true;
        gridTimer = new Stopwatch();
        moveStartTime = System.DateTime.Now;
        focalSphere.transform.position = ConvertPointToPosition(currentOrder[0]);

        System.IO.Directory.CreateDirectory(savePath + "\\" + GetFolderName());
        using (StreamWriter dataWriter = File.AppendText(savePath + "\\" + GetFolderName() + "\\" + "StartEvents_" + GetFilenameLegalDateTime() + ".csv"))
        {
            dataWriter.WriteLine("Start Time: " + MoveTimer.timer.Elapsed.TotalSeconds.ToString());
            
        }
    }

    public void StopAndResetMovingSphere()
    {
        sphereMoving = false;
        orderIndex = 0;
        oldPos = ConvertPointToPosition(currentOrder[0]);
        newPos = ConvertPointToPosition(currentOrder[0]);
        MoveSphereToNextPos();
    }

    public void SetPattern()
    {
        print("optionvalue: " + patternDropdown.value.ToString());
        switch(patternDropdown.value)
        {
            case 0:
                {
                    currentOrder = OrderLRTB;
                    //string testoutput = "LRTB: ";
                    //for (int i = 0; i < currentOrder.Count; i++)
                    //{
                    //    testoutput += (currentOrder[i].ToString() + ",");
                    //}
                    //print(testoutput);
                    break;
                }
            case 1:
                {
                    currentOrder = OrderSnake;
                    //string testoutput = "snake: ";
                    //for (int i = 0; i < currentOrder.Count; i++)
                    //{
                    //    testoutput += (currentOrder[i].ToString() + ",");
                    //}
                    //print(testoutput);
                    break;
                }
            case 2:
                {
                    currentOrder = ShuffleList(OrderRandom);
                    //string testoutput = "random: ";
                    //for (int i = 0; i < currentOrder.Count; i++)
                    //{
                    //    testoutput += (currentOrder[i].ToString() + ",");
                    //}
                    //print(testoutput);
                    break;
                }
        }
    }

    public void SetMode()
    {
        print("modevalue: " + moveModeDropdown.value.ToString());
        switch (moveModeDropdown.value)
        {
            case 0:
                {
                    moveMode = 0;
                    break;
                }
            case 1:
                {
                    moveMode = 1;
                    break;
                }
            case 2:
                {
                    moveMode = 2;
                    break;
                }
        }
    }

    public void SetPointTime()
    {
        print("pointTime: " + timeDropdown.value.ToString());
        switch (timeDropdown.value)
        {
            case 0:
                {
                    timeForPoint = 2;
                    break;
                }
            case 1:
                {
                    timeForPoint = 1.5f;
                    break;
                }
            case 2:
                {
                    timeForPoint = 1;
                    break;
                }
        }
    }

    List<int> ShuffleList(List<int> inputList)
    {
        List<int> tempList = inputList;

        for (int i = tempList.Count - 1; i > 0; i--)
        {
            int k = rand.Next(i + 1);
            int value = tempList[k];
            tempList[k] = tempList[i];
            tempList[i] = value;
        }
        
        return tempList;
    }

    void MoveSphereToNextPos()
    {
        if(orderIndex < currentOrder.Count)
        {
            switch (moveMode)
            {
                case 0:
                    {
                        focalSphere.transform.position = ConvertPointToPosition(currentOrder[orderIndex]);
                        break;
                    }
                case 1:
                    {
                        if(orderIndex == 0)
                        {
                            oldPos = ConvertPointToPosition(currentOrder[orderIndex]);
                            newPos = ConvertPointToPosition(currentOrder[orderIndex]);
                        }
                        else
                        {
                            oldPos = ConvertPointToPosition(currentOrder[orderIndex - 1]);
                            newPos = ConvertPointToPosition(currentOrder[orderIndex]);
                        }
                        break;
                    }
                case 2:
                    {
                        if (orderIndex == 0)
                        {
                            oldPos = ConvertPointToPosition(currentOrder[orderIndex]);
                            newPos = ConvertPointToPosition(currentOrder[orderIndex]);
                        }
                        else
                        {
                            oldPos = ConvertPointToPosition(currentOrder[orderIndex - 1]);
                            newPos = ConvertPointToPosition(currentOrder[orderIndex]);
                        }
                        break;
                    }
            }

            orderIndex++;
        }
        else
        {
            StopAndResetMovingSphere();
        }
    }

    // Is this function that amounts to a giant switch statement an inelegant abomination? Yes. But it was also quick and easy to code. Think up something smarter later if you like.
    Vector3 ConvertPointToPosition(int inputPointNum) 
    {
        switch(inputPointNum)
        {
            case 0:
                {
                    return new Vector3(GridVertFarLeft.transform.position.x, GridHorizTop.transform.position.y, GridVertMid.transform.position.z);
                }
            case 1:
                {
                    return new Vector3(GridVertMidLeft.transform.position.x, GridHorizTop.transform.position.y, GridVertMid.transform.position.z);
                }
            case 2:
                {
                    return new Vector3(GridVertMid.transform.position.x, GridHorizTop.transform.position.y, GridVertMid.transform.position.z);
                }
            case 3:
                {
                    return new Vector3(GridVertMidRight.transform.position.x, GridHorizTop.transform.position.y, GridVertMid.transform.position.z);
                }
            case 4:
                {
                    return new Vector3(GridVertFarRight.transform.position.x, GridHorizTop.transform.position.y, GridVertMid.transform.position.z);
                }
            case 5:
                {
                    return new Vector3(GridVertFarLeft.transform.position.x, GridHorizMidUp.transform.position.y, GridVertMid.transform.position.z);
                }
            case 6:
                {
                    return new Vector3(GridVertMidLeft.transform.position.x, GridHorizMidUp.transform.position.y, GridVertMid.transform.position.z);
                }
            case 7:
                {
                    return new Vector3(GridVertMid.transform.position.x, GridHorizMidUp.transform.position.y, GridVertMid.transform.position.z);
                }
            case 8:
                {
                    return new Vector3(GridVertMidRight.transform.position.x, GridHorizMidUp.transform.position.y, GridVertMid.transform.position.z);
                }
            case 9:
                {
                    return new Vector3(GridVertFarRight.transform.position.x, GridHorizMidUp.transform.position.y, GridVertMid.transform.position.z);
                }
            case 10:
                {
                    return new Vector3(GridVertFarLeft.transform.position.x, GridHorizMid.transform.position.y, GridVertMid.transform.position.z);
                }
            case 11:
                {
                    return new Vector3(GridVertMidLeft.transform.position.x, GridHorizMid.transform.position.y, GridVertMid.transform.position.z);
                }
            case 12:
                {
                    return new Vector3(GridVertMid.transform.position.x, GridHorizMid.transform.position.y, GridVertMid.transform.position.z);
                }
            case 13:
                {
                    return new Vector3(GridVertMidRight.transform.position.x, GridHorizMid.transform.position.y, GridVertMid.transform.position.z);
                }
            case 14:
                {
                    return new Vector3(GridVertFarRight.transform.position.x, GridHorizMid.transform.position.y, GridVertMid.transform.position.z);
                }
            case 15:
                {
                    return new Vector3(GridVertFarLeft.transform.position.x, GridHorizMidDown.transform.position.y, GridVertMid.transform.position.z);
                }
            case 16:
                {
                    return new Vector3(GridVertMidLeft.transform.position.x, GridHorizMidDown.transform.position.y, GridVertMid.transform.position.z);
                }
            case 17:
                {
                    return new Vector3(GridVertMid.transform.position.x, GridHorizMidDown.transform.position.y, GridVertMid.transform.position.z);
                }
            case 18:
                {
                    return new Vector3(GridVertMidRight.transform.position.x, GridHorizMidDown.transform.position.y, GridVertMid.transform.position.z);
                }
            case 19:
                {
                    return new Vector3(GridVertFarRight.transform.position.x, GridHorizMidDown.transform.position.y, GridVertMid.transform.position.z);
                }
            case 20:
                {
                    return new Vector3(GridVertFarLeft.transform.position.x, GridHorizBottom.transform.position.y, GridVertMid.transform.position.z);
                }
            case 21:
                {
                    return new Vector3(GridVertMidLeft.transform.position.x, GridHorizBottom.transform.position.y, GridVertMid.transform.position.z);
                }
            case 22:
                {
                    return new Vector3(GridVertMid.transform.position.x, GridHorizBottom.transform.position.y, GridVertMid.transform.position.z);
                }
            case 23:
                {
                    return new Vector3(GridVertMidRight.transform.position.x, GridHorizBottom.transform.position.y, GridVertMid.transform.position.z);
                }
            case 24:
                {
                    return new Vector3(GridVertFarRight.transform.position.x, GridHorizBottom.transform.position.y, GridVertMid.transform.position.z);
                }
            default:
                {
                    print("invalid input position number, return default of (0,0,0)");
                    return new Vector3(0, 0, 0);
                }
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

    public void QuitGame()
    {
        Application.Quit();
    }
}
