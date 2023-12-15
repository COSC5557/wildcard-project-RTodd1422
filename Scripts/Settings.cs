using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Settings : MonoBehaviour
{
    public static int applesPerRound;
    public static int numOfRounds;
    public static int difficultyLevel;

    public GameObject difficultyButton;
    public Vector3 floorPoint;
    public GameObject difficultyIndicator;
    public GameObject diffText;


    void Awake()
    {
        applesPerRound = 5;
        numOfRounds = 3;
        difficultyLevel = 1; // 1, 2, or 3

        difficultyButton = GameObject.Find("DifficultyButton");
        
        difficultyIndicator = GameObject.Find("DifficultyIndicator");

        diffText = GameObject.Find("DifficultyButtonText");

        floorPoint = GameObject.Find("FloorPoint").transform.position;


        switch (difficultyLevel)
        {
            case 1:
                {
                    ChangeObjColor(difficultyIndicator, Color.green);
                    diffText.GetComponent<TextMeshProUGUI>().text = "Difficulty: \nEasy";
                    break;
                }
            case 2:
                {
                    ChangeObjColor(difficultyIndicator, Color.yellow);
                    diffText.GetComponent<TextMeshProUGUI>().text = "Difficulty: \nMedium";
                    break;
                }
            case 3:
                {
                    ChangeObjColor(difficultyIndicator, Color.red);
                    diffText.GetComponent<TextMeshProUGUI>().text = "Difficulty: \nHard";
                    break;
                }
        }
    }

    public void cycleDifficulty()
    {
        if(difficultyLevel<3)
        {
            difficultyLevel++;
            Debug.Log("DifficultyLevel is now: " + difficultyLevel);
        }
        else
        {
            difficultyLevel = 1;
            Debug.Log("DifficultyLevel is now: " + difficultyLevel);
        }

        switch(difficultyLevel)
        {
            case 1:
                {
                    ChangeObjColor(difficultyIndicator, Color.green);
                    diffText.GetComponent<TextMeshProUGUI>().text = "Difficulty:\nEasy";
                    ApplePickingGame.ResetGame();
                    break;
                }
            case 2:
                {
                    ChangeObjColor(difficultyIndicator, Color.yellow);
                    diffText.GetComponent<TextMeshProUGUI>().text = "Difficulty:\nMedium";
                    ApplePickingGame.ResetGame();
                    break;
                }
            case 3:
                {
                    ChangeObjColor(difficultyIndicator, Color.red);
                    diffText.GetComponent<TextMeshProUGUI>().text = "Difficulty:\nHard";
                    ApplePickingGame.ResetGame();
                    break;
                }
        }
    }

    private void ChangeObjColor(GameObject obj, Color newColor)
    {
        Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();
        for (int rendererIndex = 0; rendererIndex < renderers.Length; rendererIndex++)
        {
            renderers[rendererIndex].material.color = newColor;
        }
    }
}
