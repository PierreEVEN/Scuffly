using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/**
 * 
 * The camera manager need to be attached to the GameObject containing the camera
 * 
 */

[RequireComponent(typeof(Camera), typeof(PlayerManager))]
public class CameraManager : MonoBehaviour
{
    public Vector2 ZoomBounds = new Vector2(5, 200);
    public Vector2 FovBounds = new Vector2(30, 120);
    public bool Indoor = false;

    private float zoomInput = 50;
    private float fov = 60;
    private Vector2 lookVector = new Vector2();
    private Vector2 indoorLookVector = new Vector2();
    private Camera controlledCamera;

    private PlayerManager playerManager;


    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        playerManager = gameObject.GetComponent<PlayerManager>();
        controlledCamera = gameObject.GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        if (playerManager.viewPlane.Value)
        {
            Quaternion horiz = Quaternion.AngleAxis(Indoor ? indoorLookVector.x : lookVector.x, Vector3.up);
            Quaternion vert = Quaternion.AngleAxis(Indoor ? indoorLookVector.y : lookVector.y, Vector3.right);
            gameObject.transform.rotation = Indoor ? playerManager.viewPlane.Value.transform.rotation * horiz * vert : horiz * vert;
            gameObject.transform.position = playerManager.viewPlane.Value.transform.position + gameObject.transform.forward * (Indoor ? 0 : -zoomInput) + playerManager.viewPlane.Value.transform.forward * 3.3f + playerManager.viewPlane.Value.transform.up * (0.92f - indoorLookVector.y * 0.002f) + playerManager.viewPlane.Value.transform.right * indoorLookVector.x * 0.002f;
            controlledCamera.fieldOfView = Indoor ? fov : 60;
        }
        else
        {
            gameObject.transform.position = Vector3.zero;
            gameObject.transform.rotation = Quaternion.identity;
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
}
