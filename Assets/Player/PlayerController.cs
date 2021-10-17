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

    private bool enableEngine = false;
    private bool ExtractGear = false;
    private float GearExtract = 1.0f;


    private float zoomInput = 50;
    Vector2 lookVector = new Vector2();
    GameObject controledPlane = null;
    PlaneInputInterface planeInput = null;


    private bool Indoor = false;

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

        thrustValue = Mathf.Clamp(thrustValue + thrustInput * Time.deltaTime * 10, 0, 1);
        upValue = Mathf.Clamp(upValue + upInput * Time.deltaTime, -1, 1);
        rightValue = Mathf.Clamp(rightValue + rightInput * Time.deltaTime, -1, 1);
        rollValue = Mathf.Lerp(rollValue, rollInput, Time.deltaTime * 2);
        /*
        upValue = Mathf.Lerp(upValue, upInput, Time.deltaTime * 2);
        rightValue = Mathf.Lerp(rightValue, rightInput, Time.deltaTime * 2);
        */

        planeInput.SetThrustInput(enableEngine ? thrustValue * 0.9f + 0.1f : 0);
        planeInput.setPitchInput(upValue);
        planeInput.setYawInput(rightValue);
        planeInput.setRollInput(rollValue);

        // Compute camera location
        Quaternion horiz = Quaternion.AngleAxis(lookVector.x, Vector3.up);
        Quaternion vert = Quaternion.AngleAxis(lookVector.y, Vector3.right);
        gameObject.transform.rotation = Indoor ? controledPlane.transform.rotation * horiz * vert : horiz * vert;
        gameObject.transform.position = controledPlane.transform.position + gameObject.transform.forward * (Indoor ? 0 : -zoomInput) + controledPlane.transform.forward * 3.3f + controledPlane.transform.up * 1f;
    }

    public void OnThrust(InputValue input) => thrustInput = Mathf.Clamp(input.Get<float>(), -1, 1);
    public void OnUp(InputValue input) => upInput = Mathf.Clamp(input.Get<float>(), -1, 1);
    public void OnRight(InputValue input) => rightInput = Mathf.Clamp(input.Get<float>(), -1, 1);
    public void OnRoll(InputValue input) => rollInput = Mathf.Clamp(input.Get<float>(), -1, 1);
    public void OnLook(InputValue input) => lookVector = new Vector2(input.Get<Vector2>().x + lookVector.x, Mathf.Clamp(input.Get<Vector2>().y + lookVector.y, -90, 90));
    public void OnZoom(InputValue input) => zoomInput = Mathf.Clamp(zoomInput + input.Get<float>(), ZoomBounds.x, ZoomBounds.y);
    public void OnSwitchAPU() => planeInput.switchApu();
    public void onSwitchBattery() { }
    public void OnSwitchEngine() => enableEngine = !enableEngine;
    public void OnSwitchGear() => ExtractGear = !ExtractGear;
    public void OnSwitchView() => Indoor = !Indoor;
}
