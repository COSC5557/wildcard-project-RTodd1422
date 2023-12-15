/* Russell Todd
 * The idea for this script is to give the 'bucket hole' the ability to destroy apples that enter it and then inform ApplePickingGame that it happened.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BucketHole : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(this.CompareTag("RedAppleSack"))
        {
            if (other.tag == "Apple")
            {
                print("Sack is for red apples and it received a: ");
                if (other.name == "RedApplePrefab(Clone)") // if the apple is red
                {
                    print("red apple, correctly");
                    Destroy(other.gameObject);
                    ApplePickingGame.score++;
                    ApplePickingGame.jsonRecord.repsCompleted++;
                }
                else if(other.name == "GreenApplePrefab(Clone)")
                {
                    print("green apple, incorrectly");
                    other.GetComponent<Apples>().ColorMixupHandling();
                }
            }
        }
        else if(this.CompareTag("GreenAppleSack"))
        {
            if (other.tag == "Apple")
            {
                print("Sack is for green apples and it received a: ");
                if (other.name == "GreenApplePrefab(Clone)") // if the apple is green
                {
                    print("green apple, correctly");
                    Destroy(other.gameObject);
                    ApplePickingGame.score++;
                    ApplePickingGame.jsonRecord.repsCompleted++;
                }
                else if(other.name == "RedApplePrefab(Clone)")
                {
                    print("red apple, incorrectly");
                    other.GetComponent<Apples>().ColorMixupHandling();
                }
            }
        }
    }
}
