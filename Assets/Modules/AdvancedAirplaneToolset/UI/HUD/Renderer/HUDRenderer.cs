using UnityEngine;

[ExecuteInEditMode]
public class HUDRenderer : PlaneComponent
{
    public GameObject HUDClass;
    GameObject instancedHUD;

    private void OnEnable()
    {
        if (!instancedHUD && HUDClass)
        {
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
        if (instancedHUD)
            if (Application.isPlaying)
                Destroy(instancedHUD);
            else
                DestroyImmediate(instancedHUD);
    }
}
