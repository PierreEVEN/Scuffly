using UnityEngine;

// Cycle jour - nuit
// La rotation se fait par cran pour eviter une mise a jour du lighting a chaque frame
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
