using UnityEngine;

/// <summary>
/// A component that can be used to make a spinning object
/// </summary>
[ExecuteInEditMode]
public class RotatingWingmill : MonoBehaviour
{
    float speed;
    float rot = 0;

    /// <summary>
    /// Select the axis of spinning
    /// </summary>
    public bool axizY = true;
    public bool flipX = false;

    public float minSpeed = 40;
    public float maxSpeed = 180;

    void Start()
    {
        speed = Random.Range(minSpeed, maxSpeed);
    }

    void Update()
    {
        // Update the rotation
        rot += Time.deltaTime * speed;
        transform.localRotation = Quaternion.Euler(flipX ? 180 : 0, axizY ? rot : 0, axizY ? 0 : rot);
    }
}
