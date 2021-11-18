using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class RotatingWingmill : MonoBehaviour
{
    float speed;
    float rot = 0;
    public bool axizY = true;
    public bool flipX = false;

    public float minSpeed = 40;
    public float maxSpeed = 180;

    // Start is called before the first frame update
    void Start()
    {
        speed = Random.Range(minSpeed, maxSpeed);
    }


    // Update is called once per frame
    void Update()
    {
        rot += Time.deltaTime * speed;
        transform.localRotation = Quaternion.Euler(flipX ? 180 : 0, axizY ? rot : 0, axizY ? 0 : rot);
    }
}
