/* Russell Todd
 * The idea for this script is to have a main timer that is not frame-locked that other scripts will be able to reference.
 * It will simply  be a constantly updating timer that is publicly accessible. 
 */

using System.Diagnostics;
using UnityEngine;

public class MoveTimer : MonoBehaviour
{
    public static Stopwatch timer;

    public static System.DateTime startTime;

    void Awake()
    {
        timer = new Stopwatch();

        startTime = System.DateTime.Now;
    }

    private void Start()
    {
        timer.Start();
    }
}