
using UnityEngine;
using UnityEngine.Events;

[ExecuteInEditMode]
public class LandscapeModifier : MonoBehaviour
{
    Vector3 lastPosition;
    Quaternion lastRotation;
    Vector3 lastScale;

    public int priority;
    private int lastPriority;

    [HideInInspector]
    public Rect worldBounds;

#if (UNITY_EDITOR)
    static UnityEvent OnHotReload = new UnityEvent();

    [UnityEditor.Callbacks.DidReloadScripts]
    private static void OnScriptsReloaded()
    {
        OnHotReload.Invoke();
    }
 #endif

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
#if (UNITY_EDITOR)
        OnHotReload.AddListener(UpdateBounds);
#endif
    }
    ~LandscapeModifier()
    {
#if (UNITY_EDITOR)
        OnHotReload.RemoveListener(UpdateBounds);
#endif
    }

    public void UpdateBounds()
    {
        Rect newBounds = computeBounds();
        if (worldBounds == null) worldBounds = newBounds;
        HeightGenerator.Singleton.MoveModifier(this, worldBounds, newBounds);
        worldBounds = newBounds;
    }

    // Update is called once per frame
    void Update()
    {
        if (lastPosition != gameObject.transform.position)
        {
            lastPosition = gameObject.transform.position;
            UpdateBounds();
        }

        if (lastRotation != gameObject.transform.rotation)
        {
            lastRotation = gameObject.transform.rotation;
            UpdateBounds();
        }

        if (lastScale != gameObject.transform.localScale)
        {
            lastScale = gameObject.transform.localScale;
            UpdateBounds();
        }
        if (priority != lastPriority)
        {
            lastPriority = priority;
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
