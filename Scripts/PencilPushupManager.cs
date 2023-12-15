using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PencilPushupManager : MonoBehaviour
{
    public static GameObject nearPoint, farPoint, focalObject, focusGuide, pencilPushControls;
    Vector3 initNearPointPos, initFarPointPos, initFocusGuidePos, beeLine;
    public float goalSpeed, focalObjSize, guideObjSize, timeOutside, ppGameTime, ppSetTime, ppRepTime, guidePos;
    public int ppReps, ppRepIter, ppSets, ppSetIter, ppDifficultyLevel, pathPattern;
    public bool ppDirection, ppMoving, ppStartPoint, gameComplete, ppGameReported, pointAvailable;
    public Material focalObjectBadMat, focalObjectGoodMat;
    public TMP_Text farText, nearText, ppRepProgText, ppSetProgText, repsProgressText, setProgressText, scoreText;
    public TMP_InputField ppSetSpeedIF, ppSetRepsIF, ppSetSetsIF, ppSetFocalSizeIF, ppSetGuideSizeIF;


    void Start()
    {
        nearPoint = GameObject.Find("nearPoint");
        farPoint = GameObject.Find("farPoint");
        focalObject = GameObject.Find("focalObject");
        focusGuide = GameObject.Find("focusGuide");
        pencilPushControls = GameObject.Find("PencilPushControls");

        initNearPointPos = nearPoint.transform.position;
        initFarPointPos = farPoint.transform.position;
        initFocusGuidePos = focusGuide.transform.position;

        goalSpeed = 3; // number of seconds to go from one point to another
        focalObjSize = 1;
        guideObjSize = 1;
        pathPattern = 1;

        ppReps = 3;
        ppRepIter = 0;
        ppSets = 2;
        ppRepIter = 0;
        ppDifficultyLevel = 1;

        ppStartPoint = false; // false is far, true is near
        if(ppStartPoint == false)
        {
            ppDirection = true; // true is backwards
        }
        else
        {
            ppDirection = false; // false is forwards 
        }
        
        ppMoving = false;
        gameComplete = false;
        ppGameReported = false;
        pointAvailable = false;
        pencilPushControls.SetActive(false);
        ppGameTime = 0f;
        ppSetTime = 0f;
        ppRepTime = 0f;
        guidePos = 0;

        ppSetSpeedIF.placeholder.GetComponent<TMP_Text>().text = goalSpeed.ToString();
        ppSetRepsIF.placeholder.GetComponent<TMP_Text>().text = ppReps.ToString();
        ppSetSetsIF.placeholder.GetComponent<TMP_Text>().text = ppSets.ToString();
        ppSetFocalSizeIF.placeholder.GetComponent<TMP_Text>().text = focalObjSize.ToString();
        ppSetGuideSizeIF.placeholder.GetComponent<TMP_Text>().text = guideObjSize.ToString();
    }

    void Update()
    {
        if (ppMoving == true && gameComplete == false)
        {
            ppGameTime += Time.deltaTime;
            ppSetTime += Time.deltaTime;
            ppRepTime += Time.deltaTime;
            
            if (ppStartPoint == false) // starting at far point
            {
                float theta = ppSetTime / (goalSpeed/Mathf.PI);
                float distance = (beeLine.magnitude / 2) * Mathf.Cos(theta);
                Vector3 midpoint = ((nearPoint.transform.position + farPoint.transform.position) / 2);
                focusGuide.transform.position = midpoint - beeLine.normalized * distance;
            }
            else // starting at near point
            {
                float theta = ppSetTime / (goalSpeed / Mathf.PI);
                float distance = (beeLine.magnitude / 2) * Mathf.Cos(theta);
                Vector3 midpoint = ((nearPoint.transform.position + farPoint.transform.position) / 2);
                focusGuide.transform.position = midpoint + beeLine.normalized * distance;
            }

            if(ppRepTime >= goalSpeed*2) // guide has returned to starting position
            {
                ppRepIter++;
                ppRepTime = 0;

                if (ppRepIter == ppReps)
                {
                    ppSetIter++;

                    ppSetTime = 0f;
                    if (ppSetIter == ppSets)
                    {
                        gameComplete = true;
                    }
                    else
                    {
                        ppRepIter = 0;
                    }
                }
                
            }

            /* First attempt with lerp
            if(ppDirection == false) //forwards: near to far
            {
                print("ppdirection false");
                //farPoint.GetComponent<Renderer>().enabled = true;
                //nearPoint.GetComponent<Renderer>().enabled = false;
                //focusGuide.transform.Translate(goalSpeed * Time.deltaTime * -beeLine.normalized);
                guidePos = ppRepTime / goalSpeed;
                guidePos = guidePos * guidePos * (3f - 2f * guidePos);
                focusGuide.transform.position = Vector3.Lerp(nearPoint.transform.position, farPoint.transform.position, guidePos);
                if(farPoint.GetComponent<Collider>().bounds.Contains(focusGuide.transform.position))
                {
                    ppDirection = true;
                    print("ppdirection set to true");
                    if(ppStartPoint == false)
                    {
                        if(pointAvailable == true)
                        {
                            ppRepIter++;
                            print("repIterated1");
                            ppRepTime = 0;
                            if (ppRepIter == ppReps)
                            {
                                ppRepIter = 0;
                                ppSetIter++;
                                print("setIterated1");
                                ppSetTime = 0f;
                                if (ppSetIter == ppSets)
                                {
                                    gameComplete = true;
                                }
                            }
                            ScoreBoardUpdate();
                            pointAvailable = false;
                            print("point not available");
                        }
                    }
                    else
                    {
                        pointAvailable = true;
                        print("point available");
                    }
                }
            }
            else //backwards: far to near
            {
                //farPoint.GetComponent<Renderer>().enabled = false;
                //nearPoint.GetComponent<Renderer>().enabled = true;
                //focusGuide.transform.Translate(goalSpeed * Time.deltaTime * beeLine.normalized);
                guidePos = ppRepTime / goalSpeed;
                guidePos = guidePos * guidePos * (3f - (2f * guidePos));
                focusGuide.transform.position = Vector3.Lerp(farPoint.transform.position, nearPoint.transform.position, guidePos);
                if (nearPoint.GetComponent<Collider>().bounds.Contains(focusGuide.transform.position))
                {
                    ppDirection = false;
                    print("ppdirection set to false");
                    if (ppStartPoint == true)
                    {
                        if(pointAvailable == true)
                        {
                            ppRepIter++;
                            print("repIterated2");
                            ppRepTime = 0;
                            if (ppRepIter == ppReps)
                            {
                                ppRepIter = 0;
                                ppSetIter++;
                                print("setIterated2");
                                ppSetTime = 0f;
                                if (ppSetIter == ppSets)
                                {
                                    gameComplete = true;
                                }
                            }
                            ScoreBoardUpdate();
                            pointAvailable = false;
                            print("point not available");
                        }
                    }
                    else
                    {
                        pointAvailable = true;
                        print("point available");
                    }
                }
            }
            */
            if (focusGuide.GetComponent<Collider>().bounds.Contains(focalObject.transform.position))
            {
                focalObject.GetComponent<Renderer>().material = focalObjectGoodMat;
            }
            else
            {
                focalObject.GetComponent<Renderer>().material = focalObjectBadMat;
                timeOutside += Time.deltaTime;
            }
        }
        if(gameComplete == true && ppGameReported == false)
        {
            print("timeOutside: " + timeOutside);
            print("% time inside Guide: " + (100 - (timeOutside / ppGameTime)));
            ppGameReported = true;
        }
        ScoreBoardUpdate();
    }

    public void ResetPPGame()
    {
        nearPoint.transform.position = initNearPointPos;
        farPoint.transform.position = initFarPointPos;
        focusGuide.transform.position = initFocusGuidePos;
        ppRepIter = 0;
        ppSetIter = 0;
        gameComplete = false;
        ppMoving = false;
        ppGameReported = false;
        farPoint.GetComponent<Renderer>().enabled = true;
        nearPoint.GetComponent<Renderer>().enabled = true;
        farText.enabled = true;
        nearText.enabled = true;
    }

    public void SetPPReps()
    {
        int temp;
        int.TryParse(ppSetRepsIF.text, out temp); 
        if(temp > 0)
        {
            ppReps = temp;
        }
    }

    public void SetPPSets()
    {
        int temp;
        int.TryParse(ppSetSetsIF.text, out temp);
        if (temp > 0)
        {
            ppSets = temp;
        }
    }

    public void SetPPSpeed()
    {
        float temp;
        float.TryParse(ppSetSpeedIF.text, out temp);
        if (temp > 0)
        {
            goalSpeed = temp;
        }
    }

    public void SetFocalObjectSize()
    {
        float temp;
        float.TryParse(ppSetFocalSizeIF.text, out temp);
        if (temp > 0)
        {
            focalObjSize = temp;
        }
        focalObject.transform.localScale = focalObject.transform.localScale * focalObjSize;
    }

    public void SetGuideObjectSize()
    {
        float temp;
        float.TryParse(ppSetGuideSizeIF.text, out temp);
        if (temp > 0)
        {
            guideObjSize = temp;
        }
        focusGuide.transform.localScale = focusGuide.transform.localScale * guideObjSize;
    }

    public void ppInvokeGS()
    {
        if (ppStartPoint == true)
        {
            focusGuide.transform.position = nearPoint.transform.position;
        }
        else
        {
            focusGuide.transform.position = farPoint.transform.position;
        }
        farText.enabled = false;
        nearText.enabled = false;
        beeLine = nearPoint.transform.position - farPoint.transform.position;
        //print("beeline magnitude: " + beeLine.magnitude.ToString());
        Invoke("ppGameStart", 3);
    }

    public void ppGameStart()
    {
        ppMoving = true;
        farPoint.GetComponent<Renderer>().enabled = false;
        nearPoint.GetComponent<Renderer>().enabled = false;
        ScoreBoardUpdate();
    }

    public void ppGamePause()
    {
        ppMoving = false;
    }

    public void ScoreBoardUpdate()
    {
        UpdateRepsProgressText();
        UpdateSetProgressText();
        UpdateScoreText();
    }

    public void TogglePencilPushControls()
    {
        if(pencilPushControls.activeSelf == false)
        {
            pencilPushControls.SetActive(true);
        }
        else
        {
            pencilPushControls.SetActive(false);
        }
    }

    public void UpdateSetProgressText()
    {
        setProgressText.text = ppSetIter.ToString() + "/" + ppSets.ToString();
    }

    public void UpdateRepsProgressText()
    {
        repsProgressText.text = ppRepIter.ToString() + "/" + ppReps.ToString();
    }

    public void UpdateScoreText()
    {
        scoreText.text = (Mathf.Round(10 * (100 - (100*(timeOutside / ppGameTime)))) * 0.1).ToString() + "%";
    }
}
