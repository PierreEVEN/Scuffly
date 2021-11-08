using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class RotatingWingmill : MonoBehaviour
{
    float speed;
    float rot = 0;
    // Start is called before the first frame update
    void Start()
    {
        speed = (float)Random.RandomRange(40, 180);
    }

    // Update is called once per frame
    void Update()
    {
        rot += Time.deltaTime * speed;
        transform.localRotation = Quaternion.Euler(0, rot, 0);
    }
}
