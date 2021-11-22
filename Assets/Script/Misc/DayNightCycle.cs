using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class DayNightCycle : MonoBehaviour
{
    public float azimut = 0;
    public Vector2 initialRotationBounds = new Vector2(0, 180);
    float orientation;

    float lastOrientation = 0;

    public float rotationSpeed = 0.1f;
    public float step = 0.1f;

    // Start is called before the first frame update
    void Start()
    {
        orientation = Random.Range(initialRotationBounds.x, initialRotationBounds.y);
        if (Application.isPlaying)
        {
            DontDestroyOnLoad(this);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!Application.isPlaying)
            return;

        orientation += rotationSpeed * Time.deltaTime;

        if (Mathf.Abs(orientation - lastOrientation) > step)
        {
            lastOrientation = orientation;
            transform.rotation = Quaternion.Euler(lastOrientation, azimut, 0);
        }
    }
}
