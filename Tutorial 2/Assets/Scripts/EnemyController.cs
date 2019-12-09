using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{

    public Vector3 startMarker;
    public Vector3 endMarker;

    public float speed;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.Lerp(startMarker, endMarker, Mathf.PingPong(Time.time / 2 * speed, 1) * speed);
    }

}
