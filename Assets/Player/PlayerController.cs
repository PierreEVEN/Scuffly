using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{

    public Vector2 ZoomBounds = new Vector2(5, 200);

    private float thrustInput = 0;
    private float upInput = 0;
    private float rightInput = 0;
    private float rollInput = 0;

    private float thrustValue = 0;
    private float upValue = 0;
    private float rightValue = 0;
    private float rollValue = 0;


    private float zoomInput = 50;
    Vector2 lookVector = new Vector2();
    GameObject controledPlane = null;
    PlaneInputInterface planeInput = null;


    void OnGUI()
    {
    }

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

        thrustValue = Mathf.Clamp(thrustValue + thrustInput * Time.deltaTime, -1, 1);
        upValue = Mathf.Clamp(upValue + upInput * Time.deltaTime, -1, 1);
        rightValue = Mathf.Clamp(rightValue + rightInput * Time.deltaTime, -1, 1);
        rollValue = Mathf.Lerp(rollValue, rollInput, Time.deltaTime * 2);
        /*
        upValue = Mathf.Lerp(upValue, upInput, Time.deltaTime * 2);
        rightValue = Mathf.Lerp(rightValue, rightInput, Time.deltaTime * 2);
        */

        planeInput.SetThrustInput(thrustValue);
        planeInput.setPitchInput(upValue);
        planeInput.setYawInput(rightValue);
        planeInput.setRollInput(rollValue);

        // Compute camera location
        Quaternion horiz = Quaternion.AngleAxis(lookVector.x, Vector3.up);
        Quaternion vert = Quaternion.AngleAxis(lookVector.y, Vector3.right);
        gameObject.transform.rotation = horiz * vert;
        gameObject.transform.position = controledPlane.transform.position + gameObject.transform.forward * -zoomInput;
    }

    public void OnThrust(InputValue input) => thrustInput = Mathf.Clamp(input.Get<float>(), -1, 1);
    public void OnUp(InputValue input) => upInput = Mathf.Clamp(input.Get<float>(), -1, 1);
    public void OnRight(InputValue input) => rightInput = Mathf.Clamp(input.Get<float>(), -1, 1);
    public void OnRoll(InputValue input) => rollInput = Mathf.Clamp(input.Get<float>(), -1, 1);
    public void OnLook(InputValue input) => lookVector += input.Get<Vector2>();
    public void OnZoom(InputValue input) => zoomInput = Mathf.Clamp(zoomInput + input.Get<float>(), ZoomBounds.x, ZoomBounds.y);
}
