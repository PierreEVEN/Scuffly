using UnityEngine;

public enum MobilePartBinding
{
    Custom,
    Roll,
    Pitch,
    Yaw,
    Thrust,
    Canopy
}

/// <summary>
/// A mobile part is a component of the plane that will move following a desired input.
/// It can optionnaly require power to move
/// //@TODO : implement the hydraulics of the airplane
/// </summary>
[ExecuteInEditMode]
public class MobilePart : PlaneComponent
{
    /// <summary>
    /// The rotation of the part in neutral (0), negative (-1), and positive (1) position. The rotation is interpolated between these 3 values
    /// The rotation is in euler angles for the user
    /// </summary>
    public Vector3 neutralRotation = new Vector3(0, 0, 0);
    public Vector3 minRotation = new Vector3(0, 0, 0);
    public Vector3 maxRotation = new Vector3(0, 0, 0);

    /// <summary>
    /// Internally, the rotation is internally in quaternion
    /// </summary>
    private Quaternion intNeutralRotation = Quaternion.identity;
    private Quaternion intMinRotation = Quaternion.identity;
    private Quaternion intMaxRotation = Quaternion.identity;

    /// <summary>
    /// The input rotation the component should try to follow
    /// </summary>
    [Range(-1, 1)]
    public float desiredInput = 0;

    /// <summary>
    /// The current rotation of the component
    /// </summary>
    [HideInInspector]
    public float currentInput = 0;

    /// <summary>
    /// The rotation is smoothed depending on this parameter //@TODO : make the smoothing linear
    /// </summary>
    public float InterpolationSpeed = 2;

    /// <summary>
    /// Some predefined values are available like pitch / roll / yaw ....
    /// If not set to Custom, the part will automatically retrieve the correct input
    /// </summary>
    public MobilePartBinding binding = MobilePartBinding.Custom;

    /// <summary>
    /// Does this part require power to move
    /// </summary>
    public bool RequirePlanePower = true;

    void Start()
    {
        intNeutralRotation.eulerAngles = neutralRotation;
        intMinRotation.eulerAngles = minRotation;
        intMaxRotation.eulerAngles = maxRotation;

        if (Application.isPlaying && binding == MobilePartBinding.Custom)
            desiredInput = 0;
        
        currentInput = desiredInput;
    }

    /// <summary>
    /// Set the rotation (between -1 and 1) of the part.
    /// Not required if not using the Custom binding value
    /// </summary>
    /// <param name="input"></param>
    public void setInput(float input)
    {
        desiredInput = Mathf.Clamp(input, -1, 1);
    }

    void Update()
    {
#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            intNeutralRotation.eulerAngles = neutralRotation;
            intMinRotation.eulerAngles = minRotation;
            intMaxRotation.eulerAngles = maxRotation;
        }
#endif

        // Ensure we have enough power
        if (Plane && (!RequirePlanePower || Plane.GetCurrentPower() > 95))
        {
            // Retrieve the predefined inputs
            switch (binding)
            {
                case MobilePartBinding.Custom:
                    break;
                case MobilePartBinding.Roll:
                    desiredInput = Plane.RollInput;
                    break;
                case MobilePartBinding.Pitch:
                    desiredInput = Plane.PitchInput;
                    break;
                case MobilePartBinding.Yaw:
                    desiredInput = Plane.YawInput;
                    break;
                case MobilePartBinding.Thrust:
                    desiredInput = Plane.ThrottleNotch ? Plane.ThrustInput : 0;
                    break;
                case MobilePartBinding.Canopy:
                    desiredInput = Plane.OpenCanopy ? 1 : 0;
                    break;
            }
        }

        SetRotationValue(desiredInput);
    }
    
    /// <summary>
    /// Apply the rotation between -1 and 1.
    /// The fonction will smooth and interpolate the rotation
    /// </summary>
    /// <param name="value"></param>
    void SetRotationValue(float value)
    {
        // Smoothing
        float delta = Time.deltaTime * InterpolationSpeed;
        currentInput = Mathf.Clamp(currentInput + Mathf.Clamp(value - currentInput, -delta, delta), -1, 1);

        Quaternion finalRotation = Quaternion.identity;

        // Interpolate between the 3 predefined positions
        if (currentInput > 0.0f)
            finalRotation = Quaternion.Lerp(intNeutralRotation, intMaxRotation, currentInput);
        else
            finalRotation = Quaternion.Lerp(intNeutralRotation, intMinRotation, -currentInput);

        transform.localRotation = finalRotation;
    }
}
