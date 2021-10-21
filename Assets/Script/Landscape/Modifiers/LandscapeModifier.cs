using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[ExecuteInEditMode]
public class LandscapeModifier : MonoBehaviour
{
    Vector3 lastPosition;
    Quaternion lastRotation;
    Vector3 lastScale;

    public int priority;

    [HideInInspector]
    public Rect worldBounds;

    static UnityEvent OnHotReload = new UnityEvent();

    [UnityEditor.Callbacks.DidReloadScripts]
    private static void OnScriptsReloaded()
    {
        OnHotReload.Invoke();
    }

    // Start is called before the first frame update
    void Start()
    {
        lastPosition = gameObject.transform.position;
        lastRotation = gameObject.transform.rotation;
        lastScale = gameObject.transform.localScale;
        UpdateBounds();
    }

    private void OnDestroy()
    {
        HeightGenerator.Singleton.RemoveModifier(this);
    }

    public LandscapeModifier()
    {
        OnHotReload.AddListener(UpdateBounds);
    }
    ~LandscapeModifier()
    {
        OnHotReload.RemoveListener(UpdateBounds);
    }

    void UpdateBounds()
    {
        Rect newBounds = computeBounds();
        if (worldBounds == null) worldBounds = newBounds;
        HeightGenerator.Singleton.MoveModifier(this, worldBounds, newBounds);
        worldBounds = newBounds;
    }

    // Update is called once per frame
    void Update()
    {
        if (lastPosition.x != gameObject.transform.position.x || lastPosition.y != gameObject.transform.position.y || lastPosition.z != gameObject.transform.position.z)
        {
            lastPosition = gameObject.transform.position;
            UpdateBounds();
        }

        if (lastRotation.x != gameObject.transform.rotation.x || lastRotation.y != gameObject.transform.rotation.y || lastRotation.z != gameObject.transform.rotation.z || lastRotation.w != gameObject.transform.rotation.w)
        {
            lastRotation = gameObject.transform.rotation;
            UpdateBounds();
        }

        if (lastScale.x != gameObject.transform.localScale.x || lastScale.y != gameObject.transform.localScale.y || lastScale.z != gameObject.transform.localScale.z)
        {
            lastScale = gameObject.transform.localScale;
            UpdateBounds();
        }
    }

    public virtual Rect computeBounds()
    {
        return new Rect(new Vector2(gameObject.transform.position.x, gameObject.transform.position.z), new Vector2(0, 0));
    }
    public virtual float GetAltitudeAtLocation(float PosX, float PosZ)
    {
        return 0.0f;
    }

    public virtual float GetIncidenceAtLocation(float PosX, float PosZ)
    {
        return 1.0f;
    }

}
