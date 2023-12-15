using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;


[Serializable]
public class Export2JSON 
{
    public DateTime sessionDate;        // DateTime they launched program
    public DateTime sessionEndTime;        // Time when program was closed
    public string activityName;

    public int repsAssigned;            // How many reps (ex: apples to be picked) are assigned for each round of the activity
    public int setsAssigned;          // How many rounds of the activity
    public int difficultyLevel;         // 1, 2, 3, etc. Higher number, higher difficulty level. If difficulty levels are not in use, can set it as 1.

    public List<double> repStartTimes;   // Time each rep is started (ex: time each apple is grabbed)
    public List<double> repEndTimes;     // Time each rep is completed (ex: time each apple enters bag)
    public List<double> setEndTimes;   // List of times of end of each round
    public List<Vector3> failedPositions;

    public double totalActivityTime;     // time of last rep - time of first rep, time doing activity in seconds

    public int repsCompleted;
    public int repsMissed;
    public int repsMixedUp;
    public int setsCompleted;
}
