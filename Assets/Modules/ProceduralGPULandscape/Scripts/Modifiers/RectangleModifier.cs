using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class RectangleModifier : GPULandscapeModifier
{
    public float margins = 200;

    public struct RectangleModifierData
    {
        public float margins;
    }

    private static ModifierGPUArray<RectangleModifier, RectangleModifierData> gpuData = new ModifierGPUArray<RectangleModifier, RectangleModifierData>("RectangleModifier");

    public RectangleModifierData data;

    private void OnEnable()
    {
        gpuData.TrackModifier(this);
    }

    private void OnDisable()
    {
        gpuData.UntrackModifier(this);
    }

    private void OnDrawGizmos()
    {
#if UNITY_EDITOR
        Gizmos.color = new Color(affectAltitude ? 1 : 0.5f, affectFoliage ? 1 : 0.5f, affectGrass ? 1 : 0.5f, UnityEditor.Selection.activeGameObject == this.gameObject ? 0.5f : 0.1f);
        Gizmos.DrawCube(new Vector3(transform.position.x, transform.position.y, transform.position.z), new Vector3(transform.localScale.x * 2, 10, transform.localScale.z * 2));
#endif
    }

    public override void OnUpdateData()
    {
        gpuData.UpdateValue(this);
    }

    public override byte[] GetCustomData()
    {
        data.margins = margins;
        return StructToBytes(data);
    }
}
