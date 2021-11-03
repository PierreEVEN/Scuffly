using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RectangleModifier : GPULandscapeModifier
{
    struct RectangleModifierData
    {
        Vector2 position;
        Vector2 halfExtent;
        Vector2 margins;
        float altitude;
    }

    private static List<RectangleModifierData> modifierList = new List<RectangleModifierData>();
    private static ComputeBuffer computeBuffer;

    public static ComputeBuffer GetBuffer()
    {
        return computeBuffer;
    }
    public static int GetModifierCount()
    {
        return modifierList.Count;
    }

    private static void UpdateBuffer()
    {
        if (computeBuffer != null)
            computeBuffer.Release();
        int stride = System.Runtime.InteropServices.Marshal.SizeOf(typeof(Vector4));
        computeBuffer = new ComputeBuffer(modifierList.Count, stride, ComputeBufferType.Default);
    }

    private void AddModifier()
    {

    }
    private void RemoveModifier()
    {

    }

    private void OnEnable()
    {
        
    }

    private void OnDisable()
    {
        
    }
}
