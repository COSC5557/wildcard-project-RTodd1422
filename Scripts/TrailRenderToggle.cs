using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrailRenderToggle : MonoBehaviour
{
    public static bool toggleState = true;

    public static GameObject rightHand;
    public static GameObject leftHand;
    public static GameObject toggleIndicator;

    private void Start()
    {
        rightHand = GameObject.Find("RightTrailPoint");
        leftHand = GameObject.Find("LeftTrailPoint");
        toggleIndicator = GameObject.Find("ToggleIndicator");
        ToggleTrailRenderers();
    }

    public static void ToggleTrailRenderers()
    {
        if(toggleState == false)
        {
            rightHand.GetComponent<TrailRenderer>().emitting = true;
            leftHand.GetComponent<TrailRenderer>().emitting = true;

            ChangeObjColor(toggleIndicator, Color.green);

            toggleState = true;
        }
        else
        {
            rightHand.GetComponent<TrailRenderer>().emitting = false;
            leftHand.GetComponent<TrailRenderer>().emitting = false;

            ChangeObjColor(toggleIndicator, Color.red);
            toggleState = false;
        }
    }

    private static void ChangeObjColor(GameObject obj, Color newColor)
    {
        Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();
        for (int rendererIndex = 0; rendererIndex < renderers.Length; rendererIndex++)
        {
            renderers[rendererIndex].material.color = newColor;
        }
    }
}
