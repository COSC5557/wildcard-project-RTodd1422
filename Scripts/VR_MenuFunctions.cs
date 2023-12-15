using ViveSR.anipal.Eye;
using UnityEngine;

public class VR_MenuFunctions : MonoBehaviour
{
    public void Quit()
    {
        Application.Quit();
    }

    public void RunEyeCalibration()
    {
        SRanipal_Eye_v2.LaunchEyeCalibration();
    }
}
