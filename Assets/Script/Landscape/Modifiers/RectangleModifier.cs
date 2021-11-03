using UnityEngine;

[ExecuteInEditMode]
public class RectangleModifier : GPULandscapeModifier
{
    public float margins = 200;

    public struct RectangleModifierData
    {
        public Vector2 position;
        public Vector2 halfExtent;
        public float margins;
        public float altitude;
    }

    private static ModifierGPUArray<RectangleModifier, RectangleModifierData> gpuData = new ModifierGPUArray<RectangleModifier, RectangleModifierData>("RectangleModifier");

    public RectangleModifierData data;

    private void OnEnable()
    {
        gpuData.TrackModifier(this);
        OnUpdateData();
    }

    private void OnDisable()
    {
        gpuData.UntrackModifier(this);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(0.5f, 0.5f, 0, 0.2f);
        Gizmos.DrawCube(new Vector3(data.position.x, data.altitude, data.position.y), new Vector3(data.halfExtent.x * 2, 10, data.halfExtent.y * 2));
    }

    public override void OnUpdateData()
    {
        data.position = new Vector2(transform.position.x, transform.position.z);
        data.margins = margins;
        data.halfExtent = new Vector2(transform.localScale.x, transform.localScale.z);
        data.altitude = transform.position.y;

        gpuData.UpdateValue(this, data);
    }

    private void Update()
    {
        if (transform.position.x != data.position.x || transform.position.y != data.altitude || transform.position.z != data.position.y || transform.localScale.x != data.halfExtent.x || transform.localScale.z != data.halfExtent.y)
            OnUpdateData();
    }
}
