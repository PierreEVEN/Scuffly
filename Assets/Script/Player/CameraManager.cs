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
    public Vector2 ZoomBounds = new Vector2(5, 200);
    public Vector2 FovBounds = new Vector2(30, 120);
    public bool Indoor = false;

    private float zoomInput = 50;
    private float fov = 60;
    private Vector2 lookVector = new Vector2();
    private Vector2 indoorLookVector = new Vector2();
    private Camera controlledCamera;

    float groundAltitude = 0;
    private PlayerManager playerManager;

    private GameObject lastLookedObject;

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
            Quaternion horiz = Quaternion.AngleAxis(Indoor ? indoorLookVector.x : lookVector.x, Vector3.up);
            Quaternion vert = Quaternion.AngleAxis(Indoor ? indoorLookVector.y : lookVector.y, Vector3.right);
            gameObject.transform.rotation = Indoor ? playerManager.viewPlane.transform.rotation * horiz * vert : horiz * vert;
            gameObject.transform.position = playerManager.viewPlane.transform.position + gameObject.transform.forward * (Indoor ? 0 : -zoomInput) + playerManager.viewPlane.transform.forward * 3.3f + playerManager.viewPlane.transform.up * (0.92f - indoorLookVector.y * 0.002f) + playerManager.viewPlane.transform.right * indoorLookVector.x * 0.002f;
            if (gameObject.transform.position.y < groundAltitude + 1)
                gameObject.transform.position = new Vector3(gameObject.transform.position.x, groundAltitude + 1, gameObject.transform.position.z);
            controlledCamera.fieldOfView = Indoor ? fov : 60;
        }
        else
        {
            gameObject.transform.position = Vector3.zero;
            gameObject.transform.rotation = Quaternion.identity;
        }
        Ray rayTarget = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        RaycastHit hit;
        Debug.DrawRay(rayTarget.origin, rayTarget.direction);
        if (Physics.Raycast(rayTarget, out hit, 10))
        {
            ClickableSwitch sw = hit.collider.GetComponent<ClickableSwitch>();
            if (sw)
            {
                if (lastLookedObject != sw.gameObject)
                {
                    if (lastLookedObject)
                        lastLookedObject.layer = 0;

                    lastLookedObject = sw.gameObject;
                    lastLookedObject.layer = 3;
                }
            }
            else if (lastLookedObject)
            {
                lastLookedObject.layer = 0;
                lastLookedObject = null;
            }
        }
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
            zoomInput = Mathf.Clamp(zoomInput + input.Get<float>(), ZoomBounds.x, ZoomBounds.y);
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
        Ray rayTarget = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        RaycastHit hit;
        Debug.DrawRay(rayTarget.origin, rayTarget.direction);
        if (Physics.Raycast(rayTarget, out hit, 10))
        {
            ClickableSwitch sw = hit.collider.GetComponent<ClickableSwitch>();
            if (sw)
            {
                Debug.Log("click");
                sw.Switch();
            }
            else
            {
                Debug.Log(hit.collider.gameObject.name);
            }
        }
    }
}
