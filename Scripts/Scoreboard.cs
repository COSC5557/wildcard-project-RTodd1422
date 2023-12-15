using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Scoreboard : MonoBehaviour
{
    public TMP_Text roundProgressText;
    public TMP_Text applesProgressText;

    void Start()
    {
        UpdateApplesProgressText();
        UpdateRoundProgressText();

        //roundProgressText = GameObject.Find("RoundProgressText").GetComponent<TextMeshPro>();
        //applesProgressText = GameObject.Find("ApplesProgressText").GetComponent<TextMeshProUGUI>();
        //print(roundProgressText);
    }

    void Update()
    {
        UpdateApplesProgressText();
        UpdateRoundProgressText();
    }

    public void UpdateRoundProgressText()
    {
        roundProgressText.text = ApplePickingGame.round.ToString() + "/" + ApplePickingGame.numOfRounds.ToString();
    }

    public void UpdateApplesProgressText()
    {
        applesProgressText.text = ApplePickingGame.score.ToString() + "/" + ApplePickingGame.applesPerRound.ToString();
    }
}
