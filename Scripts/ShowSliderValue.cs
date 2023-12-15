using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine;

public class ShowSliderValueMoVE : MonoBehaviour
{
    public TMP_Text roundSliderValueText;
    public TMP_Text appleSliderValueText;
    public Slider roundSlider;
    public Slider appleSlider;

    private void Start()
    {
        roundSlider.value = Settings.numOfRounds;
        appleSlider.value = Settings.applesPerRound;

        UpdateAppleValue();
        UpdateRoundValue();
    }

    public void UpdateRoundValue()
    {
        roundSliderValueText.text = roundSlider.value.ToString();
        Settings.numOfRounds = (int)roundSlider.value;
    }
    public void UpdateAppleValue()
    {
        appleSliderValueText.text = appleSlider.value.ToString();
        Settings.applesPerRound = (int)appleSlider.value;
    }
}
