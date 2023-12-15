/* Russell Todd
 * The idea for this script is to give the 'bucket hole' the ability to destroy apples that enter it and then inform ApplePickingGame that it happened.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrateHole : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Apple")
        {
            Destroy(other.gameObject);
            ApplePickingGame.score++;
            ApplePickingGame.jsonRecord.repsCompleted++;
        }
        if (other.tag == "Banana")
        {
            Destroy(other.gameObject);
            ApplePickingGame.score++;
            ApplePickingGame.jsonRecord.repsCompleted++;
        }
    }
}