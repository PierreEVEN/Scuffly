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
    public bool Indoor = false;

    private float zoomInput = 50;
    private Vector2 lookVector = new Vector2();

    private PlayerManager playerManager;


    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        playerManager = gameObject.GetComponent<PlayerManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (playerManager.viewPlane.Value)
        {
            Quaternion horiz = Quaternion.AngleAxis(lookVector.x, Vector3.up);
            Quaternion vert = Quaternion.AngleAxis(lookVector.y, Vector3.right);
            gameObject.transform.rotation = Indoor ? playerManager.viewPlane.Value.transform.rotation * horiz * vert : horiz * vert;
            gameObject.transform.position = playerManager.viewPlane.Value.transform.position + gameObject.transform.forward * (Indoor ? 0 : -zoomInput) + playerManager.viewPlane.Value.transform.forward * 3.3f + playerManager.viewPlane.Value.transform.up * 1f;
        }
        else
        {
            gameObject.transform.position = Vector3.zero;
            gameObject.transform.rotation = Quaternion.identity;
        }
    }

    public void OnLook(InputValue input) => lookVector = new Vector2(input.Get<Vector2>().x + lookVector.x, Mathf.Clamp(input.Get<Vector2>().y + lookVector.y, -90, 90));
    public void OnZoom(InputValue input) => zoomInput = Mathf.Clamp(zoomInput + input.Get<float>(), ZoomBounds.x, ZoomBounds.y);
    public void OnSwitchView() => Indoor = !Indoor;
}
