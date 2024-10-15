using UnityEngine;

/// <summary>
/// Day night cycle system
/// </summary>
[ExecuteInEditMode]
public class DayNightCycle : MonoBehaviour
{
    /// <summary>
    /// The inclinaison of the sun
    /// </summary>
    public float azimut = 0;

    /// <summary>
    /// Initially, the rotation of the sun is randomized between these 2 values
    /// </summary>
    public Vector2 initialRotationBounds = new Vector2(0, 180);

    /// <summary>
    /// Current sun rotation in degrees
    /// </summary>
    float orientation;
    float lastOrientation = 0;

    /// <summary>
    /// Rotation speed
    /// </summary>
    public float rotationSpeed = 0.1f;

    // Start is called before the first frame update
    void Start()
    {
        orientation = Random.Range(initialRotationBounds.x, initialRotationBounds.y);
        if (Application.isPlaying)
        {
            DontDestroyOnLoad(this);
        }
    }

    public void SetRotation(float rotation)
    {
        orientation = rotation;
    }

    void Update()
    {
        if (!Application.isPlaying)
            return;

        // Rotate the sun
        orientation += rotationSpeed * Time.deltaTime;
        lastOrientation = orientation;
        transform.rotation = Quaternion.Euler(lastOrientation, azimut, 0);
    }
}
