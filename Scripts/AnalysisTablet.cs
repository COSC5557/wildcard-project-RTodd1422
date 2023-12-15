using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Runtime;

public class AnalysisTablet : MonoBehaviour
{
    public TMP_Text applesPickedText;
    public TMP_Text appleMixupText;
    public TMP_Text applesMissedText;

    public TMP_Text tenMinText;
    public TMP_Text oneMinText;
    public TMP_Text tenSecText;
    public TMP_Text oneSecText;

    void Start()
    {
        
    }

    void Update()
    {
        UpdateAnalysisSlide1();
    }

    public void UpdateAnalysisSlide1()
    {
        applesPickedText.text = ApplePickingGame.jsonRecord.repsCompleted.ToString();
        appleMixupText.text = ApplePickingGame.jsonRecord.repsMixedUp.ToString();
        applesMissedText.text = ApplePickingGame.jsonRecord.repsMissed.ToString();

        if(ApplePickingGame.gameFinished !=true)
        {
            tenMinText.text = Mathf.Floor((float)AppleTimer.timer.Elapsed.TotalSeconds / 600).ToString();
            oneMinText.text = Mathf.Floor(((float)AppleTimer.timer.Elapsed.TotalSeconds/ 60) % 10).ToString();
            tenSecText.text = Mathf.Floor(((float)AppleTimer.timer.Elapsed.TotalSeconds / 10) % 6).ToString();
            oneSecText.text = Mathf.Floor((float)AppleTimer.timer.Elapsed.TotalSeconds  % 10).ToString();
        }
    }
}
