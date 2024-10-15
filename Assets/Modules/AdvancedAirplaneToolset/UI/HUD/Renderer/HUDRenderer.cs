using UnityEngine;

/// <summary>
/// The HUD of the aircraft is rendered at the origin of the world in a texture through a HUD renderer component
/// </summary>
[ExecuteInEditMode]
public class HUDRenderer : PlaneComponent
{
    /// <summary>
    /// The ui that will be instanced and rendered into a texture
    /// </summary>
    public GameObject HUDClass;
    GameObject instancedHUD;

    private void OnEnable()
    {
        if (!instancedHUD && HUDClass)
        {
            // Create the UI to render
            instancedHUD = Instantiate(HUDClass, transform);
            instancedHUD.hideFlags = HideFlags.DontSave;
            HUDManager hudManager = instancedHUD.GetComponent<HUDManager>();
            hudManager.transform.position = Vector3.zero;
            hudManager.transform.rotation = Quaternion.identity;
            hudManager.transform.localScale = Vector3.one;
            if (!hudManager)
                Debug.LogError("failed to find hud manager in instanced HUD");
            hudManager.Plane = Plane;
            hudManager.transform.parent = null;
        }
    }

    private void OnDisable()
    {
        // Destroy the spawned UI
        if (instancedHUD)
            if (Application.isPlaying)
                Destroy(instancedHUD);
            else
                DestroyImmediate(instancedHUD);
    }
}
