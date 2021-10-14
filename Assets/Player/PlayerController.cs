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
    PlaneInputInterface planeInput = null;

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
            planeInput = controledPlane.GetComponent<PlaneInputInterface>();
        }
        else
        {
            controledPlane = null;
            planeInput = null;
            gameObject.transform.parent = null;
        }
    }

    void Update()
    {
        if (!controledPlane || !planeInput)
            return;

        planeInput.SetThrustInput(thrustInput);
        planeInput.setPitchInput(upInput);
        planeInput.setYawInput(rightInput);
        planeInput.setRollInput(rollInput);

        // Compute camera location
        Quaternion horiz = Quaternion.AngleAxis(lookVector.x, Vector3.up);
        Quaternion vert = Quaternion.AngleAxis(lookVector.y, Vector3.right);
        gameObject.transform.rotation = horiz * vert;
        gameObject.transform.position = controledPlane.transform.position + gameObject.transform.forward * -zoomInput;
    }

    public void OnThrust(InputValue input) => thrustInput = Mathf.Clamp(thrustInput + input.Get<float>() * 0.2f, -1, 1);
    public void OnUp(InputValue input) => upInput = Mathf.Clamp(upInput + input.Get<float>() * 1, -1, 1);
    public void OnRight(InputValue input) => rightInput = Mathf.Clamp(rightInput + input.Get<float>() * 1, -1, 1);
    public void OnRoll(InputValue input) => rollInput = Mathf.Clamp(rollInput + input.Get<float>() * 1, -1, 1);
    public void OnLook(InputValue input) => lookVector += input.Get<Vector2>();
    public void OnZoom(InputValue input) => zoomInput = Mathf.Clamp(zoomInput + input.Get<float>(), ZoomBounds.x, ZoomBounds.y);
}
