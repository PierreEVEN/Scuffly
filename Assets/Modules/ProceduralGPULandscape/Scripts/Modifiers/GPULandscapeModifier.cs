using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A modifier can affect dirrent kind of data, with different mode
/// </summary>
public static class MaskMode
{
    public const int OVERRIDE = 1; // Should override the current value
    public const int ALTITUDE_MASK = 2; // Should affect the altitude
    public const int FOLIAGE_MASK = 4; // Shoudl affect the foliage
    public const int TREE_MASK = 8; // Should only affect the trees
    public const int NOISE_MASK = 16; // Should affect the noise
}

/// <summary>
/// Basically, the landscape is fully dynamic and procedural, generated using noises.
/// You can manually edit some location by using modifiers to change the altitude, or foliage spawn locally
/// </summary>
public abstract class GPULandscapeModifier : MonoBehaviour
{
    //@TODO : implement priority
    [Range(short.MinValue, short.MaxValue)]
    public short priority = 0;
    /// <summary>
    /// Should override the current value
    /// </summary>
    public bool overwrite = false;
    public bool affectAltitude = true;
    public bool affectFoliage = false;
    public bool affectGrass = false;
    public bool affectNoise = false;

    /// <summary>
    /// Store the last informations to detect when the mask is moved
    /// </summary>
    Vector3 lastPosition;
    Quaternion lastRotation;
    Vector3 lastScale;

    //@Todo : understand why i need to check if it has been generated... D:
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

    /// <summary>
    /// Retrieve the generic data for this mask (position / rotation / scale / mode / priority)
    /// </summary>
    /// <returns></returns>
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

    /// <summary>
    /// Convert a structure to raw byte data
    /// </summary>
    /// <typeparam name="Struct_T"></typeparam>
    /// <param name="data"></param>
    /// <returns></returns>
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
