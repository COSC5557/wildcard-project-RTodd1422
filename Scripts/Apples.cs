/* Russell Todd
 * The idea for this script is for it to be attached to each apple (via prefab) so that it can perform the functions needed for that apple.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class Apples : MonoBehaviour
{
    public bool grabbedOnce;
    public bool mixedUpOnce;
    bool ranOnce;
    double timeGrabbed;
    Vector3 originalPos;
    bool respawned;
    Coroutine missRoutine;

    void Start()
    {
        AppleManager.numOfApples = GameObject.FindGameObjectsWithTag("Apple").Length;
        //AppleManager.numOfApples++; //inform manager that an apple has been created/instantiated in the scene
        grabbedOnce = false;
        ranOnce = false;
        respawned = false;
        mixedUpOnce = false;

        this.gameObject.GetComponent<Rigidbody>().isKinematic = true;
        this.gameObject.GetComponent<Rigidbody>().useGravity = false;

        originalPos = transform.position;
    }

    private void Update()
    {
        if(grabbedOnce == true && ranOnce == false)
        {
            this.gameObject.GetComponent<Rigidbody>().isKinematic = false;
            this.gameObject.GetComponent<Rigidbody>().useGravity = true;

            ranOnce = true;
            timeGrabbed = AppleTimer.timer.Elapsed.TotalSeconds;
        }
    }

    private void OnDestroy()
    {
        
        ApplePickingGame.jsonRecord.repStartTimes.Add(timeGrabbed);                         // both times added when destroyed so that the times betweeen lists correspond to same object
        ApplePickingGame.jsonRecord.repEndTimes.Add(AppleTimer.timer.Elapsed.TotalSeconds);
        try
        {
            StopCoroutine(missRoutine);
        }
        catch
        {
            Debug.Log("no missRoutine to stop");
        }
        //AppleManager.numOfApples--; //inform manager that an apple has been destroyed
        AppleManager.numOfApples = GameObject.FindGameObjectsWithTag("Apple").Length;
    }

    private void OnDetachedFromHand(Hand hand)
    {
        grabbedOnce = true;
        gameObject.GetComponent<Rigidbody>().isKinematic = false;
        gameObject.GetComponent<Rigidbody>().useGravity = true;
        missRoutine = StartCoroutine(MissedHandling());
    }

    private IEnumerator MissedHandling()
    {
        yield return new WaitForSeconds(5);

        if(ApplePickingGame.difficultyLevel == 1)
        {
            ApplePickingGame.score++;
            ApplePickingGame.jsonRecord.repsMissed++;
            Destroy(gameObject);
        }
        else
        {
            if(respawned == false)
            {
                RespawnApple();
            }
            else
            {
                ApplePickingGame.score++;
                ApplePickingGame.jsonRecord.repsMissed++;
                ApplePickingGame.jsonRecord.failedPositions.Add(originalPos);
                Destroy(gameObject);
            }
        }
    }

    private void RespawnApple()
    {
        transform.position = originalPos;
        transform.rotation = Quaternion.identity;

        grabbedOnce = false;
        ranOnce = false;
        respawned = true;

        this.gameObject.GetComponent<Rigidbody>().isKinematic = true;
        this.gameObject.GetComponent<Rigidbody>().useGravity = false;
    }

    public void ColorMixupHandling()
    {
        StopCoroutine(missRoutine);

        if(mixedUpOnce == false)
        {
            transform.position = originalPos;

            grabbedOnce = false;
            ranOnce = false;
            respawned = true;
            mixedUpOnce = true;
            ApplePickingGame.jsonRecord.repsMixedUp++;
            this.gameObject.GetComponent<Rigidbody>().isKinematic = true;
            this.gameObject.GetComponent<Rigidbody>().useGravity = false;
        }
        else
        {
            ApplePickingGame.score++;
            ApplePickingGame.jsonRecord.repsMixedUp++;
            ApplePickingGame.jsonRecord.repsCompleted++;
            ApplePickingGame.jsonRecord.failedPositions.Add(originalPos);
            Destroy(gameObject);
        }
    }
}
