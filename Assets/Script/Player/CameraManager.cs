using MLAPI;
using UnityEngine;
using UnityEngine.InputSystem;

/**
 * 
 * The camera manager need to be attached to the GameObject containing the camera
 * 
 */

[RequireComponent(typeof(Camera), typeof(PlayerManager))]
public class CameraManager : NetworkBehaviour, GPULandscapePhysicInterface
{
    public Vector2 ZoomBounds = new Vector2(5, 1000);
    public Vector2 FovBounds = new Vector2(30, 120);
    public bool Indoor = true;

    private float zoomInput = 50;
    private float fov = 60;
    private Vector2 lookVector = new Vector2();
    private Vector2 indoorLookVector = new Vector2();
    private Camera controlledCamera;

    float groundAltitude = 0;

    private PilotEyePoint fpsViewPoint;

    private PlaneManager focusedPlane;
    private PlaneManager possessedPlane;

    private void Start()
    {
        if (!IsLocalPlayer)
            Destroy(this);
    }

    private void OnEnable()
    {
        Cursor.lockState = CursorLockMode.Locked;
        gameObject.GetComponent<PlayerManager>().OnPossessPlane.AddListener(PossessPlane);


        controlledCamera = gameObject.GetComponent<Camera>();
        GPULandscapePhysic.Singleton.AddListener(this);
    }

    private void OnDisable()
    {
        GPULandscapePhysic.Singleton.RemoveListener(this);
        gameObject.GetComponent<PlayerManager>().OnPossessPlane.RemoveListener(PossessPlane);
    }

    void PossessPlane(PlaneManager inPlane)
    {
        possessedPlane = inPlane;
        SetFocusedPlane(inPlane);
    }

    void SetFocusedPlane(PlaneManager inPlane)
    {
        if (focusedPlane)
            focusedPlane.EnableIndoor(false);
        fpsViewPoint = null;
        focusedPlane = inPlane;
        gameObject.transform.parent = focusedPlane.transform;
        Debug.Log("focus " + inPlane.GetInstanceID());
        if (focusedPlane)
            focusedPlane.EnableIndoor(true);

        GetComponent<PlanePlayerInputs>().EnableInputs = focusedPlane && focusedPlane == possessedPlane ? true : false;
    }

    // Update is called once per frame
    void Update()
    {
        if (focusedPlane)
        {
            if (!fpsViewPoint)
            {
                fpsViewPoint = focusedPlane.GetComponentInChildren<PilotEyePoint>();

                if (!fpsViewPoint)
                    Debug.LogError("bah zut");
            }

            Quaternion horiz = Quaternion.AngleAxis(Indoor ? indoorLookVector.x : lookVector.x, Vector3.up);
            Quaternion vert = Quaternion.AngleAxis(Indoor ? indoorLookVector.y : lookVector.y, Vector3.right);
            gameObject.transform.rotation = Indoor ? focusedPlane.transform.rotation * horiz * vert : horiz * vert;

            gameObject.transform.position =
                gameObject.transform.forward * (Indoor ? 0 : -zoomInput) +
                fpsViewPoint.GetCameraLocation() +
                focusedPlane.transform.up * indoorLookVector.y * -0.002f +
                focusedPlane.transform.right * indoorLookVector.x * 0.0017f;

            if (gameObject.transform.position.y < groundAltitude + 1)
                gameObject.transform.position = new Vector3(gameObject.transform.position.x, groundAltitude + 1, gameObject.transform.position.z);
            controlledCamera.fieldOfView = Indoor ? fov : 60;

            GForcePostProcessEffect.GForceIntensity = Indoor ? fpsViewPoint.GetGforceEffect() * 10 : 0;

            if (Indoor)
                RaycastSwitchs();
        }
        else // free cam movements
        {
            GForcePostProcessEffect.GForceIntensity = 0;
            FreeCamMovements();
        }
    }

    public void OnLook(InputValue input)
    {
        if (Indoor && focusedPlane)
            indoorLookVector = new Vector2(Mathf.Clamp(input.Get<Vector2>().x * 0.5f + indoorLookVector.x, -170, 170), Mathf.Clamp(input.Get<Vector2>().y * 0.5f + indoorLookVector.y, -90, 90));
        else
            lookVector = new Vector2(input.Get<Vector2>().x * 0.5f + lookVector.x, Mathf.Clamp(input.Get<Vector2>().y * 0.5f + lookVector.y, -90, 90));
    }
    public void OnZoom(InputValue input)
    {
        if (Indoor)
            fov = Mathf.Clamp(fov + input.Get<float>(), FovBounds.x, FovBounds.y);
        else
            zoomInput = Mathf.Clamp(zoomInput + input.Get<float>() * 5, ZoomBounds.x, ZoomBounds.y);
    }
    public void OnSwitchView() => Indoor = !Indoor;

    public Vector2[] Collectpoints()
    {
        return new Vector2[] { new Vector2(transform.position.x, transform.position.z) };
    }

    public void OnPointsProcessed(float[] processedPoints)
    {
        groundAltitude = processedPoints[0];
    }

    private bool hasClicked = false;
    private SwitchBase lastSwitch;
    public void RaycastSwitchs()
    {
        if (lastSwitch)
            lastSwitch.StopOver();
        lastSwitch = null;

        Ray rayTarget = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        RaycastHit hit;
        Debug.DrawRay(rayTarget.origin, rayTarget.direction);
        if (Physics.Raycast(rayTarget, out hit, 10))
        {
            SwitchBase sw = hit.collider.GetComponent<SwitchBase>();
            if (sw)
            {
                lastSwitch = sw;
                lastSwitch.StartOver();
                if (hasClicked)
                    lastSwitch.Switch();
            }
        }
        hasClicked = false;
    }

    void OnFocusMyPlane()
    {
        Indoor = true;
        SetFocusedPlane(possessedPlane);
    }
    void OnSwitchPlanes()
    {
        if (focusedPlane)
        {
            if (Indoor)
            {
                Indoor = false;
            }
            else
            {
                for (int i = 0; i < PlaneManager.PlaneList.Count; ++i)
                {
                    if (PlaneManager.PlaneList[i] == focusedPlane)
                    {
                        SetFocusedPlane(PlaneManager.PlaneList[(i + 1) % PlaneManager.PlaneList.Count]);
                        return;
                    }
                }
            }
        }
        else
        {
            Indoor = false;
            SetFocusedPlane(possessedPlane);
        }
    }

    void OnFreeCam()
    {
        if (!focusedPlane)
            transform.parent = null;

        SetFocusedPlane(null);
    }

    private void OnClickButton()
    {
        hasClicked = true;
    }


    float free_forwardinput = 0;
    float free_rightInput = 0;
    float free_upInput = 0;
    float free_speed = 10;
    void OnFreeCam_Forward(InputValue input)
    {
        free_forwardinput = Mathf.Clamp(input.Get<float>(), -1, 1);
    }
    void OnFreeCam_Right(InputValue input)
    {
        free_rightInput = Mathf.Clamp(input.Get<float>(), -1, 1);
    }
    void OnFreeCam_Up(InputValue input)
    {
        free_upInput = Mathf.Clamp(input.Get<float>(), -1, 1);
    }
    void OnFreeCam_Speed(InputValue input)
    {
        free_speed *= Mathf.Clamp(input.Get<float>() + 1, 0.5f, 1.5f);
    }

    void FreeCamMovements()
    {
        transform.position += transform.forward * free_forwardinput * free_speed * Time.deltaTime;
        transform.position += transform.right * free_rightInput * free_speed * Time.deltaTime;
        transform.position += transform.up * free_upInput * free_speed * Time.deltaTime;

        Quaternion horiz = Quaternion.AngleAxis(lookVector.x, Vector3.up);
        Quaternion vert = Quaternion.AngleAxis(lookVector.y, Vector3.right);
        transform.rotation = horiz * vert;
    }
}
