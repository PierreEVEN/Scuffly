using System.Collections.Generic;
using UnityEngine;

public static class MaskMode
{
    public const int OVERRIDE = 1;
    public const int ALTITUDE_MASK = 2;
    public const int FOLIAGE_MASK = 4;
    public const int TREE_MASK = 8;
    public const int NOISE_MASK = 16;
}

public abstract class GPULandscapeModifier : MonoBehaviour
{
    [Range(short.MinValue, short.MaxValue)]

    public short priority = 0;

    public bool overwrite = false;
    public bool affectAltitude = true;
    public bool affectFoliage = false;
    public bool affectGrass = false;
    public bool affectNoise = false;


    bool isReady = false;
    private void Start()
    {
        isReady = true;
    }
    private void OnDestroy()
    {
        isReady = false;
    }

    private void OnEnable()
    {
        UpdateData();
    }

    private void OnValidate()
    {
        if (isReady)
            UpdateData();
    }

    Vector3 lastPosition;
    Quaternion lastRotation;
    Vector3 lastScale;

    public void Update()
    {
        InternalUpdate();
        if (
            !transform.position.Equals(lastPosition) ||
            !transform.rotation.Equals(lastRotation) ||
            !transform.localScale.Equals(lastScale)
            )
        {
            lastPosition = transform.position;
            lastRotation = transform.rotation;
            lastScale = transform.localScale;
            UpdateData();
        }
    }

    public virtual void InternalUpdate() { }

    public void UpdateData()
    {
        if (GPULandscape.Singleton)
            GPULandscape.Singleton.Reset = true;
        OnUpdateData();
    }

    public virtual LandscapeModifierGenericData GetGenericData()
    {
        LandscapeModifierGenericData data;
        data.priority = this.priority;
        data.mode = 0;
        if (overwrite)
            data.mode |= MaskMode.OVERRIDE;
        if (affectAltitude)
            data.mode |= MaskMode.ALTITUDE_MASK;
        if (affectFoliage)
            data.mode |= MaskMode.FOLIAGE_MASK;
        if (affectGrass)
            data.mode |= MaskMode.TREE_MASK;
        if (affectNoise)
            data.mode |= MaskMode.NOISE_MASK;

        data.position = transform.position;
        data.scale = transform.localScale;
        return data;
    }

    public byte[] StructToBytes<Struct_T>(Struct_T data) where Struct_T : struct
    {
        int size = System.Runtime.InteropServices.Marshal.SizeOf(data);
        byte[] byteArray = new byte[size];
        System.IntPtr dataPtr = System.Runtime.InteropServices.Marshal.AllocHGlobal(size);
        System.Runtime.InteropServices.Marshal.StructureToPtr(data, dataPtr, true);
        System.Runtime.InteropServices.Marshal.Copy(dataPtr, byteArray, 0, size);
        System.Runtime.InteropServices.Marshal.FreeHGlobal(dataPtr);
        return byteArray;
    }

    public abstract byte[] GetCustomData();

    public abstract void OnUpdateData();
}
