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
    private PlayerManager playerManager;

    private PilotEyePoint viewPoint;

    private void Start()
    {
        if (!IsLocalPlayer)
            Destroy(this);
    }

    private void OnEnable()
    {
        Cursor.lockState = CursorLockMode.Locked;
        playerManager = gameObject.GetComponent<PlayerManager>();
        controlledCamera = gameObject.GetComponent<Camera>();
        GPULandscapePhysic.Singleton.AddListener(this);
    }

    private void OnDisable()
    {
        GPULandscapePhysic.Singleton.RemoveListener(this);
    }
    // Update is called once per frame
    void Update()
    {
        if (playerManager.viewPlane)
        {
            if (!viewPoint)
                viewPoint = playerManager.viewPlane.GetComponentInChildren<PilotEyePoint>();

            gameObject.transform.parent = playerManager.viewPlane.transform;
            Quaternion horiz = Quaternion.AngleAxis(Indoor ? indoorLookVector.x : lookVector.x, Vector3.up);
            Quaternion vert = Quaternion.AngleAxis(Indoor ? indoorLookVector.y : lookVector.y, Vector3.right);
            gameObject.transform.rotation = Indoor ? playerManager.viewPlane.transform.rotation * horiz * vert : horiz * vert;
            gameObject.transform.position = gameObject.transform.forward * (Indoor ? 0 : -zoomInput) + viewPoint.GetCameraLocation() + playerManager.viewPlane.transform.up * indoorLookVector.y * -0.002f + playerManager.viewPlane.transform.right * indoorLookVector.x * 0.0017f;
            if (gameObject.transform.position.y < groundAltitude + 1)
                gameObject.transform.position = new Vector3(gameObject.transform.position.x, groundAltitude + 1, gameObject.transform.position.z);
            controlledCamera.fieldOfView = Indoor ? fov : 60;

            GForcePostProcessEffect.GForceIntensity = Indoor ? viewPoint.GetGforceEffect() * 10 : 0;

        }
        else
        {
            GForcePostProcessEffect.GForceIntensity = 0;

            viewPoint = null;
            gameObject.transform.parent = null;
            gameObject.transform.position = Vector3.zero;
            gameObject.transform.rotation = Quaternion.identity;
        }

        RaycastSwitchs();
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

    public void OnLook(InputValue input)
    {
        if (Indoor)
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

    private void OnClickButton()
    {
        hasClicked = true;
    }
}
