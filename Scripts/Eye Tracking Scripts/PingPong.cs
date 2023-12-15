using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PingPong : MonoBehaviour
{
    private Vector3 pos1, pos2;
    public float speed;

    private void Start()
    {
        speed = 0.2f;
        pos1 = new Vector3(transform.position.x-4, transform.position.y, transform.position.z-2);
        pos2 = new Vector3(transform.position.x+4, transform.position.y, transform.position.z+2);
    }
    void Update()
    {
        transform.position = Vector3.Lerp(pos1, pos2, Mathf.PingPong(Time.time * speed, 1.0f));
    }
}
