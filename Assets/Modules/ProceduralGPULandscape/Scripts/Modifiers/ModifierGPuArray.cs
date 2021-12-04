
using System.Collections.Generic;
using UnityEngine;

public struct LandscapeModifierGenericData
{
    public int priority;
    public int mode;
    public Vector3 position;
    public Vector3 scale;
}

interface IModifierGPUArray
{
    ComputeBuffer GetGenericDataBuffer();
    ComputeBuffer GetCustomDataBuffer();
    string GetTypeName();
    int GetInstanceCount();

    // Liste contenant les differents types utilises de modifiers (enregistres automatiquement)
    private static List<IModifierGPUArray> ModifierArrays = new List<IModifierGPUArray>();

    public static void RegisterModifierType(IModifierGPUArray newType)
    {
        if (!ModifierArrays.Contains(newType))
            ModifierArrays.Add(newType);
    }

    public static void ApplyToMaterial(Material mat)
    {
        foreach (var buffer in ModifierArrays)
        {
            if (buffer.GetGenericDataBuffer() != null && buffer.GetCustomDataBuffer() != null)
            {
                mat.SetBuffer(buffer.GetTypeName(), buffer.GetGenericDataBuffer());
                mat.SetBuffer(buffer.GetTypeName() + "_CustomData", buffer.GetCustomDataBuffer());
                mat.SetInt(buffer.GetTypeName() + "_Count", buffer.GetInstanceCount());
            }
        }

        GPULandscapeTextureMask.ApplyToMaterial(mat);
    }

    public static void ApplyToComputeBuffer(ComputeShader mat, int kernelIndex)
    {
        foreach (var buffer in ModifierArrays)
        {
            if (buffer.GetGenericDataBuffer() != null && buffer.GetCustomDataBuffer() != null)
            {
                mat.SetBuffer(kernelIndex, buffer.GetTypeName(), buffer.GetGenericDataBuffer());
                mat.SetBuffer(kernelIndex, buffer.GetTypeName() + "_CustomData", buffer.GetCustomDataBuffer());
                mat.SetInt(buffer.GetTypeName() + "_Count", buffer.GetGenericDataBuffer().count);
            }
            else
                mat.SetInt(buffer.GetTypeName() + "_Count", 0);
        }

        GPULandscapeTextureMask.ApplyToComputeBuffer(mat, kernelIndex);
    }
}

class ModifierGPUArray<Modifier_T, CustomData_T> : IModifierGPUArray where Modifier_T : GPULandscapeModifier where CustomData_T : struct
{
    List<Modifier_T> trackedModifiers = new List<Modifier_T>();

    ComputeBuffer GenericData;
    ComputeBuffer CustomData;
    public string modifierTypeName;

    public ModifierGPUArray(string modifierName)
    {
        modifierTypeName = modifierName;
        IModifierGPUArray.RegisterModifierType(this);
    }

    public void TrackModifier(Modifier_T newElement)
    {
        if (!trackedModifiers.Contains(newElement))
        {
            trackedModifiers.Add(newElement);
            UpdateBuffer();
        }
    }

    public void UntrackModifier(Modifier_T removedElement)
    {
        trackedModifiers.Remove(removedElement);
        UpdateBuffer();
    }

    public void UpdateValue(Modifier_T modifier)
    {
        if (trackedModifiers.Contains(modifier))
        {
            UpdateBuffer();
        }
    }
    Struct_T BytesToStruct<Struct_T>(byte[] dataArray) where Struct_T : struct
    {
        Struct_T data = new Struct_T();
        int size = System.Runtime.InteropServices.Marshal.SizeOf(data);
        System.IntPtr dataPtr = System.Runtime.InteropServices.Marshal.AllocHGlobal(size);
        System.Runtime.InteropServices.Marshal.Copy(dataArray, 0, dataPtr, size);
        data = (Struct_T)System.Runtime.InteropServices.Marshal.PtrToStructure(dataPtr, data.GetType());
        System.Runtime.InteropServices.Marshal.FreeHGlobal(dataPtr);
        return data;
    }

    void UpdateBuffer()
    {
        // Cree ou recree les buffers si besoin
        if (trackedModifiers.Count == 0)
        {
            if (GenericData != null)
            {
                GenericData.Dispose();
                GenericData = null;
            }
            if (CustomData != null)
            {
                CustomData.Dispose();
                CustomData = null;
            }
        }
        else
        {
            if (GenericData == null || GenericData.count != trackedModifiers.Count)
            {
                if (GenericData != null)
                    GenericData.Dispose();
                GenericData = new ComputeBuffer(trackedModifiers.Count, System.Runtime.InteropServices.Marshal.SizeOf(typeof(LandscapeModifierGenericData)));
            }
            if (CustomData == null || CustomData.count != trackedModifiers.Count)
            {
                if (CustomData != null)
                    CustomData.Dispose();
                CustomData = new ComputeBuffer(trackedModifiers.Count, System.Runtime.InteropServices.Marshal.SizeOf(typeof(CustomData_T)));
            }
        }

        // Verifie que les buffers sont valides
        if (GenericData == null || CustomData == null)
            return;

        // Allocation des resources CPU
        LandscapeModifierGenericData[] genericData = new LandscapeModifierGenericData[trackedModifiers.Count];
        CustomData_T[] customData = new CustomData_T[trackedModifiers.Count];

        // Recuperation des donnees CPU
        for (int i = 0; i < trackedModifiers.Count; ++i )
        {
            genericData[i] = trackedModifiers[i].GetGenericData();
            customData[i] = BytesToStruct<CustomData_T>(trackedModifiers[i].GetCustomData());
        }

        GenericData.SetData(genericData);
        CustomData.SetData(customData);
    }

    ComputeBuffer IModifierGPUArray.GetGenericDataBuffer() { return GenericData; }
    ComputeBuffer IModifierGPUArray.GetCustomDataBuffer() { return CustomData; }
    public string GetTypeName() { return modifierTypeName; }
    public int GetInstanceCount() { return trackedModifiers.Count; }
}
