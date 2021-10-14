using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{

    public Vector2 ZoomBounds = new Vector2(5, 200);

    private float thrustInput = 0;
    private float upInput = 0;
    private float rightInput = 0;
    private float rollInput = 0;
    private float zoomInput = 50;
    Vector2 lookVector = new Vector2();
    GameObject controledPlane = null;

    static bool Focused
    {
        get => Cursor.lockState == CursorLockMode.Locked;
        set
        {
            Cursor.lockState = value ? CursorLockMode.Locked : CursorLockMode.None;
            Cursor.visible = value == false;
        }
    }
    void Start()
    {
        Focused = true;
        AttachTo(GameObject.FindWithTag("Plane"));
        if (!controledPlane)
            Debug.Log("controlled plane is null");
    }

    void AttachTo(GameObject plane)
    {
        if (plane)
        {
            controledPlane = plane;
        }
        else
        {
            controledPlane = null;
            gameObject.transform.parent = null;
        }
    }

    void Update()
    {
        if (!controledPlane)
            return;

        SetThrustInput(thrustInput);

        // Compute camera location
        Quaternion horiz = Quaternion.AngleAxis(lookVector.x, Vector3.up);
        Quaternion vert = Quaternion.AngleAxis(lookVector.y, Vector3.right);
        gameObject.transform.rotation = horiz * vert;
        gameObject.transform.position = controledPlane.transform.position + gameObject.transform.forward * -zoomInput;
    }

    public void OnThrust(InputValue input) => thrustInput += input.Get<float>() * 0.05f;
    public void OnUp(InputValue input) => upInput = input.Get<float>() * 0.05f;
    public void OnRight(InputValue input) => rightInput = input.Get<float>() * 0.05f;
    public void OnRoll(InputValue input) => rollInput = input.Get<float>() * 0.05f;
    public void OnLook(InputValue input) => lookVector += input.Get<Vector2>();
    public void OnZoom(InputValue input) => zoomInput = Mathf.Clamp(zoomInput + input.Get<float>(), ZoomBounds.x, ZoomBounds.y);

    void SetThrustInput(float value)
    {
        if (!controledPlane) return;
        foreach (var thruster in controledPlane.GetComponentsInChildren<Thruster>())
        {
            thruster.set_thrust_input(value);
        }
    }

}
