using MLAPI;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Handle the movements of the player's camera
/// The camera manager need to be attached to the GameObject containing the camera
/// 
/// Some input and points of view may not be available depending on the difficulty (for exemple following enemies airplane is not allowed in advanced difficulty)
/// </summary>
[RequireComponent(typeof(Camera), typeof(PlayerManager))]
public class CameraManager : NetworkBehaviour, GPULandscapePhysicInterface
{
    /// <summary>
    /// The game UI (containing crosshair aso...)
    /// </summary>
    public GameObject playerUI;
    private GameObject instanciedPlayerUI;

    /// <summary>
    /// Fov / zoom min / max values
    /// </summary>
    public Vector2 ZoomBounds = new Vector2(5, 1000);
    public Vector2 FovBounds = new Vector2(30, 120);
    /// <summary>
    /// Is currently in FPS or TPS
    /// </summary>
    public bool Indoor = true;

    /// <summary>
    /// Current inputs
    /// </summary>
    private float zoomInput = 50;
    private float fov = 60;
    private Vector2 lookVector = new Vector2();
    private Vector2 indoorLookVector = new Vector2();


    /// <summary>
    /// The actual distance from the ground
    /// </summary>
    float groundAltitude = 0;

    /// <summary>
    /// The point of view in first person mode
    /// </summary>
    private PilotEyePoint fpsViewPoint;
    private Camera controlledCamera;

    /// <summary>
    /// The plane we are currently following
    /// </summary>
    private PlaneActor focusedPlane;

    /// <summary>
    /// The plane we are controlling
    /// </summary>
    private PlaneActor possessedPlane;

    public bool IsFreeCamera() { return !focusedPlane; }

    private void Start()
    {
        // Remove camera for other players
        if (!IsLocalPlayer)
            Destroy(this);
    }

    private void OnEnable()
    {
        Cursor.lockState = CursorLockMode.Locked;
        gameObject.GetComponent<PlayerManager>().OnPossessPlane.AddListener(PossessPlane);


        controlledCamera = gameObject.GetComponent<Camera>();
        GPULandscapePhysic.Singleton.AddListener(this);

        instanciedPlayerUI = Instantiate(playerUI);
    }

    private void OnDisable()
    {
        GPULandscapePhysic.Singleton.RemoveListener(this);
        gameObject.GetComponent<PlayerManager>().OnPossessPlane.RemoveListener(PossessPlane);

        if (instanciedPlayerUI)
            Destroy(instanciedPlayerUI);
    }

    /// <summary>
    /// Take the controls of the given plane
    /// and automatically focuse it
    /// </summary>
    /// <param name="inPlane"></param>
    void PossessPlane(PlaneActor inPlane)
    {
        possessedPlane = inPlane;
        SetFocusedPlane(inPlane);
    }

    /// <summary>
    /// Detach the camera from the followed plane before it get destroyed
    /// </summary>
    /// <param name="destroyedPlane"></param>
    void OnFocusedPlaneDestroyed(PlaneActor destroyedPlane)
    {
        transform.parent = null;
        transform.position = transform.position + transform.forward * -10 + transform.up * 5;
        SetFocusedPlane(null);
    }

    /// <summary>
    /// Set the plane we are following
    /// Set to null to switch to free camera
    /// </summary>
    /// <param name="inPlane"></param>
    void SetFocusedPlane(PlaneActor inPlane)
    {
        if (inPlane == focusedPlane)
            return;

        if (focusedPlane)
        {
            // Destroy interior of the last viewed plane
            focusedPlane.OnDestroyed.RemoveListener(OnFocusedPlaneDestroyed);
            focusedPlane.EnableIndoor(false);
        }
        fpsViewPoint = null;
        focusedPlane = inPlane;
        if (focusedPlane)
        {
            // Spawn the interior of the newly followed plane if valid
            focusedPlane.OnDestroyed.AddListener(OnFocusedPlaneDestroyed);
            gameObject.transform.parent = focusedPlane.transform;
            focusedPlane.EnableIndoor(true);
        }

        GetComponent<PlanePlayerInputs>().EnableInputs = focusedPlane && focusedPlane == possessedPlane ? true : false;
    }

    void Update()
    {
        // If we are attached to a plane
        if (focusedPlane)
        {
            if (!fpsViewPoint)
            {
                fpsViewPoint = focusedPlane.GetComponentInChildren<PilotEyePoint>();

                if (!fpsViewPoint)
                    Debug.LogError("bah zut");
            }

            // Compute the camera rotation
            Quaternion horiz = Quaternion.AngleAxis(Indoor ? indoorLookVector.x : lookVector.x, Vector3.up);
            Quaternion vert = Quaternion.AngleAxis(Indoor ? indoorLookVector.y : lookVector.y, Vector3.right);
            gameObject.transform.rotation = Indoor ? focusedPlane.transform.rotation * horiz * vert : horiz * vert;

            // Compute the camera position
            gameObject.transform.position =
                gameObject.transform.forward * (Indoor ? 0 : -zoomInput) +
                fpsViewPoint.GetCameraLocation() +
                focusedPlane.transform.up * indoorLookVector.y * -0.002f +
                focusedPlane.transform.right * indoorLookVector.x * 0.0017f;

            // Update the internal FOV
            controlledCamera.fieldOfView = Indoor ? fov : 60;

            // Update the GForce effects
            GForcePostProcessEffect.GForceIntensity = Indoor ? fpsViewPoint.GetGforceEffect() * 10 : 0;

            if (Indoor)
                RaycastSwitchs();
        }
        else // else free cam movements
        {
            GForcePostProcessEffect.GForceIntensity = 0;
            FreeCamMovements();
        }

        // Alway move the camera above the ground
        if (gameObject.transform.position.y < groundAltitude + 1)
            gameObject.transform.position = new Vector3(gameObject.transform.position.x, groundAltitude + 1, gameObject.transform.position.z);
    }

    /// <summary>
    /// Mouse rotation
    /// </summary>
    /// <param name="input"></param>
    public void OnLook(InputValue input)
    {
        if (gameObject.GetComponent<PlayerManager>().disableInputs)
            return;
        if (Indoor && focusedPlane)
            indoorLookVector = new Vector2(Mathf.Clamp(input.Get<Vector2>().x * 0.5f + indoorLookVector.x, -170, 170), Mathf.Clamp(input.Get<Vector2>().y * 0.5f + indoorLookVector.y, -90, 90));
        else
            lookVector = new Vector2(input.Get<Vector2>().x * 0.5f + lookVector.x, Mathf.Clamp(input.Get<Vector2>().y * 0.5f + lookVector.y, -90, 90));
    }

    /// <summary>
    /// Zoom / FOV input
    /// </summary>
    /// <param name="input"></param>
    public void OnZoom(InputValue input)
    {
        if (gameObject.GetComponent<PlayerManager>().disableInputs)
            return;
        if (GameplayManager.Singleton.Menu)
            return;
        if (Indoor)
            fov = Mathf.Clamp(fov + input.Get<float>(), FovBounds.x, FovBounds.y);
        else
            zoomInput = Mathf.Clamp(zoomInput + input.Get<float>() * 5, ZoomBounds.x, ZoomBounds.y);
    }

    /// <summary>
    /// Switch between fps and tps view
    /// </summary>
    public void OnSwitchView()
    {
        if (GameplayManager.Singleton.NextSettings.Difficulty == Difficulty.Realistic)
        {
            Indoor = true;
            return;
        }
        Indoor = !Indoor;
    }

    /// <summary>
    /// Focus the plane we are controlling, and enter in first person view
    /// </summary>
    void OnFocusMyPlane()
    {
        if (!possessedPlane)
        {
            OnFreeCam();
            return;
        }
        Indoor = true;
        SetFocusedPlane(possessedPlane);
    }

    /// <summary>
    /// Switch between available planes. If in first person, firstly switch to third person view
    /// </summary>
    void OnSwitchPlanes()
    {
        if (GameplayManager.Singleton.NextSettings.Difficulty == Difficulty.Realistic)
            return;

        if (focusedPlane || !possessedPlane)
        {
            if (Indoor && focusedPlane)
            {
                Indoor = false;
            }
            else
            {
                // Switch to the next plane in the list
                if (GameplayManager.Singleton.NextSettings.Difficulty == Difficulty.Casual)
                {
                    for (int i = 0; i < PlaneActor.PlaneList.Count; ++i)
                    {
                        if (PlaneActor.PlaneList[i] == focusedPlane || !focusedPlane)
                        {
                            SetFocusedPlane(PlaneActor.PlaneList[(i + 1) % PlaneActor.PlaneList.Count]);
                            return;
                        }
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

    /// <summary>
    /// Switch to free cam mode
    /// </summary>
    void OnFreeCam()
    {
        if (GameplayManager.Singleton.NextSettings.Difficulty == Difficulty.Realistic)
            return;

        if (!focusedPlane)
            transform.parent = null;

        SetFocusedPlane(null);
    }

    /// <summary>
    /// Free cam current inputs
    /// </summary>
    float free_forwardinput = 0;
    float free_rightInput = 0;
    float free_upInput = 0;
    float free_speed = 100;

    /// <summary>
    /// Move forward
    /// </summary>
    /// <param name="input"></param>
    void OnFreeCam_Forward(InputValue input)
    {
        free_forwardinput = Mathf.Clamp(input.Get<float>(), -1, 1);
    }

    /// <summary>
    /// Move right
    /// </summary>
    /// <param name="input"></param>
    void OnFreeCam_Right(InputValue input)
    {
        free_rightInput = Mathf.Clamp(input.Get<float>(), -1, 1);
    }

    /// <summary>
    /// Move up
    /// </summary>
    /// <param name="input"></param>
    void OnFreeCam_Up(InputValue input)
    {
        free_upInput = Mathf.Clamp(input.Get<float>(), -1, 1);
    }

    /// <summary>
    /// Movement speed
    /// </summary>
    /// <param name="input"></param>
    void OnFreeCam_Speed(InputValue input)
    {
        if (GameplayManager.Singleton.Menu)
            return;
        if (IsFreeCamera())
            free_speed *= Mathf.Clamp(input.Get<float>() + 1, 0.5f, 1.5f);
    }

    /// <summary>
    /// handle the movements for the free camera
    /// </summary>
    void FreeCamMovements()
    {
        if (GameplayManager.Singleton.Menu)
            return;
        transform.position += transform.forward * free_forwardinput * free_speed * Time.deltaTime;
        transform.position += transform.right * free_rightInput * free_speed * Time.deltaTime;
        transform.position += transform.up * free_upInput * free_speed * Time.deltaTime;

        Quaternion horiz = Quaternion.AngleAxis(lookVector.x, Vector3.up);
        Quaternion vert = Quaternion.AngleAxis(lookVector.y, Vector3.right);
        transform.rotation = horiz * vert;
    }

    public void OnPause()
    {
        GameplayManager.Singleton.Menu = !GameplayManager.Singleton.Menu;
    }



    /************** 
     * CLICKABLE COCKPIT SYSTEM
     **************/

    /// <summary>
    /// is click pressed
    /// </summary>
    private bool pressed = false;

    /// <summary>
    /// Was click just pressed
    /// </summary>
    private bool hasClicked = false;

    /// <summary>
    /// The switch we are currently pressing
    /// </summary>
    private SwitchBase waitingReleaseSwitch;

    /// <summary>
    /// The switch we are currently seeing
    /// </summary>
    [HideInInspector]
    public SwitchBase selectedSwitch;

    /// <summary>
    /// Trace a ray in front of the camera to find clickable object
    /// </summary>
    public void RaycastSwitchs()
    {
        // While we are holding the click button, we don't release the switch
        if (waitingReleaseSwitch)
            return;

        if (selectedSwitch)
            selectedSwitch.StopOver();

        // Unselect the last switch
        selectedSwitch = null;

        Ray rayTarget = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        RaycastHit hit;
        Debug.DrawRay(rayTarget.origin, rayTarget.direction);

        // Raycast the next switch under the cursor
        if (Physics.Raycast(rayTarget, out hit, 10))
        {
            SwitchBase sw = hit.collider.GetComponent<SwitchBase>();
            if (sw)
            {
                selectedSwitch = sw;
                selectedSwitch.StartOver();
                if (hasClicked)
                {
                    // If we just clicked, call Switch
                    selectedSwitch.Switch();
                    waitingReleaseSwitch = selectedSwitch;
                }
            }
        }
        hasClicked = false;
    }

    /// <summary>
    /// On click on switch
    /// </summary>
    /// <param name="input"></param>
    private void OnClickButton(InputValue input)
    {
        if (gameObject.GetComponent<PlayerManager>().disableInputs)
            return;
        bool newPressed = Mathf.Clamp(input.Get<float>(), 0, 1) > 0.5f;
        if (!pressed && newPressed) // The next frame, we will tell the switch it has been clicked
        {
            pressed = newPressed;
            hasClicked = true;
        }
        else if (pressed && !newPressed)
        {
            pressed = newPressed;
            hasClicked = false;
            if (waitingReleaseSwitch)
                waitingReleaseSwitch.Release(); // Notify the switch it has been released
            waitingReleaseSwitch = null;
        }
    }


    /// <summary>
    /// Get camera ground altitude
    /// </summary>
    /// <returns></returns>
    public Vector2[] Collectpoints()
    {
        return new Vector2[] { new Vector2(transform.position.x, transform.position.z) };
    }

    /// <summary>
    /// On camera altitude computed
    /// </summary>
    /// <param name="processedPoints"></param>
    public void OnPointsProcessed(float[] processedPoints)
    {
        groundAltitude = processedPoints[0];
    }

}
