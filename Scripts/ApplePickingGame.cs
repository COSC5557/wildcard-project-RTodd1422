/* Russell Todd
 * The idea for this script is for it to handle/manage all the game elements of the program.
 * This should cover:
 * 1. The settings for the apple picking game
 * 2. Keeping track of the rounds and score.
 * 3. Managing game-related effects
 * 4. Reporting simple facts about the game progress. (I like the idea of making a dedicated reporter script in the future).
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApplePickingGame : MonoBehaviour
{
    // Game Settings
    public static int applesPerRound;
    public static int numOfRounds;
    public static int difficultyLevel;
    public static int score; // number of apples placed in the bucket
    public static int round; 

    public static bool gameFinished;
    public static bool report; //bool that can be flipped to have a report printed.
    public static bool noReport; // true until a report has been saved & written

    public static GameObject scriptHolder;
    public static GameObject greenAppleSack;
    public static GameObject victoryTitle;
    public static GameObject pickCheck;
    public static GameObject mixCheck;
    public static GameObject missCheck;

    public static Export2JSON jsonRecord;


    void Start() 
    {
        scriptHolder = GameObject.Find("ScriptHolder");
        greenAppleSack = GameObject.Find("GreenAppleSack");
        victoryTitle = GameObject.FindGameObjectWithTag("VictoryTitle");
        pickCheck = GameObject.Find("PickCheck");
        mixCheck = GameObject.Find("MixCheck");
        missCheck = GameObject.Find("MissCheck");
        noReport = true;

        victoryTitle.SetActive(false);
        pickCheck.SetActive(false);
        mixCheck.SetActive(false);
        missCheck.SetActive(false);
        ResetGame();
    }

    void Update()
    {
        if(score == applesPerRound && gameFinished == false)
        {
            print("Round " + round + " completed.");

            jsonRecord.setsCompleted++;
            jsonRecord.setEndTimes.Add(AppleTimer.timer.Elapsed.TotalSeconds);

            if(round < numOfRounds)
            {
                round++;
                score = 0;
            }
            else if (round == numOfRounds)
            {
                print("All rounds Completed! Good Job!");

                victoryTitle.SetActive(true);

                if (jsonRecord.repsCompleted == (applesPerRound * numOfRounds))
                {
                    pickCheck.SetActive(true);
                }
                if (jsonRecord.repsMixedUp == 0)
                {
                    mixCheck.SetActive(true);
                }
                if (jsonRecord.repsMissed == 0)
                {
                    missCheck.SetActive(true);
                }

                gameFinished = true;
                AppleManager.DestroyAllApples();

                jsonRecord.totalActivityTime = jsonRecord.repEndTimes[jsonRecord.repEndTimes.Count - 1] - jsonRecord.repStartTimes[0];
                Reporter.SaveReport();
            }
        }


    }

    public static void AppleGameReport()
    {
        print("Round: " + round);
        print("# of apples: " + AppleManager.numOfApples);
        print("Score: " + score);
    }

    public static void ResetGame()
    {
        score = 0;
        round = 1;
        gameFinished = false;

        victoryTitle.SetActive(false);
        pickCheck.SetActive(false);
        mixCheck.SetActive(false);
        missCheck.SetActive(false);

        applesPerRound = Settings.applesPerRound;
        numOfRounds = Settings.numOfRounds;
        difficultyLevel = Settings.difficultyLevel;

        jsonRecord = new Export2JSON
        {
            sessionDate = AppleTimer.startTime,
            activityName = "ApplePicking",

            repsAssigned = applesPerRound,
            setsAssigned = numOfRounds,
            difficultyLevel = difficultyLevel,

            repStartTimes = new List<double>(),
            repEndTimes = new List<double>(),
            setEndTimes = new List<double>(),
            failedPositions = new List<Vector3>(),

            repsCompleted = 0,
            setsCompleted = 0,
            repsMissed = 0,
            repsMixedUp = 0
        };

        AppleManager.DestroyAllApples();
        AppleManager.applePosits = new List<Vector3>();

        if (difficultyLevel == 3)
        {
            scriptHolder.GetComponent<AppleManager>().SpawnMixedApples(applesPerRound, AppleManager.spawnPoint.transform);
            greenAppleSack.SetActive(true);
        }
        else
        {
            scriptHolder.GetComponent<AppleManager>().SpawnApples(applesPerRound, AppleManager.spawnPoint.transform);
            greenAppleSack.SetActive(false);
        }

        AppleTimer.timer.Restart();
    }
}
