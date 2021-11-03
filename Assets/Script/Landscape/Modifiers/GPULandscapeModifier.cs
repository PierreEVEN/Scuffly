using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

class GPULandscapeTextureMask
{
    struct AtlasTextureReference
    {
        public int referenceCounter;
        public int ID;
    }



    static Dictionary<Texture2D, AtlasTextureReference> textureMap = new Dictionary<Texture2D, AtlasTextureReference>();
    static Texture2D packedAtlas;
    static Rect[] texturePositions;
    static ComputeBuffer texturePositionBuffer;

    public static UnityEvent OnRebuildAtlas = new UnityEvent();

    public static void BuildAtlas()
    {
        if (textureMap.Count == 0)
            return;

        Texture2D[] textures = new Texture2D[textureMap.Count];
        int textId = 0;
        Texture2D[] Keys = new Texture2D[textureMap.Count];
        textureMap.Keys.CopyTo(Keys, 0);
        foreach (var key in Keys)
        {
            textures[textId] = key;

            var texRef = textureMap[key];
            texRef.ID = textId;
            textureMap[key] = texRef;
            textId++;
        }

        if (texturePositionBuffer != null)
            texturePositionBuffer.Dispose();


        texturePositionBuffer = new ComputeBuffer(textureMap.Count, System.Runtime.InteropServices.Marshal.SizeOf(typeof(Rect)));
        if (!packedAtlas)
            packedAtlas = new Texture2D(1, 1, TextureFormat.RGBAFloat, false);
        texturePositions = packedAtlas.PackTextures(textures, 2);
        texturePositionBuffer.SetData(texturePositions);
        OnRebuildAtlas.Invoke();
    }

    public static void UpdateMaterial(Material mat)
    {
        if (packedAtlas != null && texturePositionBuffer != null && texturePositions != null)
        {
            mat.SetTexture("LandscapeMaskAtlas", packedAtlas);
            mat.SetBuffer("TextureMasksRefs", texturePositionBuffer);
            mat.SetInt("TextureMasksRefs_Count", texturePositions.Length);
        }
    }


    public static int GetTextureID(Texture2D inTexture)
    {
        if (!textureMap.ContainsKey(inTexture))
            return 0;
        return textureMap[inTexture].ID;
    }

    public static void AddTexture(Texture2D texture)
    {
        if (!texture) return;

        if (!texture.isReadable)
        {
            Debug.LogError("texture " + texture.name + " should be marked as \"readable\"");
            return;
        }

        if (textureMap.ContainsKey(texture))
        {
            var data = textureMap[texture];
            data.referenceCounter++;
            textureMap[texture] = data;
        }
        else
        {
            var data = new AtlasTextureReference();
            data.referenceCounter = 1;
            textureMap.Add(texture, data);
            BuildAtlas();
        }
    }

    public static void RemoveTexture(Texture2D texture)
    {
        if (!texture) return;
        if (textureMap == null) return;

        if (textureMap.ContainsKey(texture))
        {
            var data = textureMap[texture];
            data.referenceCounter--;
            if (data.referenceCounter <= 0)
            {
                textureMap.Remove(texture);
                BuildAtlas();

                if (texturePositionBuffer != null)
                {
                    texturePositionBuffer.Dispose();
                    texturePositionBuffer = null;
                }
            }
            else
            {
                textureMap[texture] = data;
            }
        }


    }
}

interface IModifierGPUArray
{
    ComputeBuffer GetBuffer();

    private static Dictionary<string, IModifierGPUArray> ModifierArrays = new Dictionary<string, IModifierGPUArray>();

    public static void RegisterModifierType(string modifierName, IModifierGPUArray newType)
    {
        if (!ModifierArrays.ContainsKey(modifierName))
            ModifierArrays.Add(modifierName, newType);
    }

    public static void UpdateMaterial(Material mat)
    {
        foreach (var buffer in ModifierArrays)
        {
            if (buffer.Value.GetBuffer() != null)
            {
                mat.SetBuffer(buffer.Key, buffer.Value.GetBuffer());
                mat.SetInt(buffer.Key + "_Count", buffer.Value.GetBuffer().count);
            }
            else
                mat.SetInt(buffer.Key + "_Count", 0);
        }

        GPULandscapeTextureMask.UpdateMaterial(mat);
    }
}


class ModifierGPUArray<Modifier_T, Data_T> : IModifierGPUArray
{
    Dictionary<Modifier_T, Data_T> trackedModifiers = new Dictionary<Modifier_T, Data_T>();

    ComputeBuffer GPUBuffer;

    public ModifierGPUArray(string modifierName)
    {
        IModifierGPUArray.RegisterModifierType(modifierName, this);
    }

    public void TrackModifier(Modifier_T newElement)
    {
        if (!trackedModifiers.ContainsKey(newElement))
            trackedModifiers.Add(newElement, default(Data_T));
    }

    public void UntrackModifier(Modifier_T removedElement)
    {
        trackedModifiers.Remove(removedElement);
        UpdateBuffer();
    }

    public void UpdateValue(Modifier_T modifier, Data_T value)
    {
        if (trackedModifiers.ContainsKey(modifier))
        {
            trackedModifiers[modifier] = value;
            UpdateBuffer();
        }
    }

    void UpdateBuffer()
    {
        if (trackedModifiers.Count == 0)
        {
            if (GPUBuffer != null)
            {
                GPUBuffer.Dispose();
                GPUBuffer = null;
            }
        }
        else
        {
            if (GPUBuffer == null || GPUBuffer.count != trackedModifiers.Count)
            {
                if (GPUBuffer != null)
                    GPUBuffer.Dispose();
                GPUBuffer = new ComputeBuffer(trackedModifiers.Count, System.Runtime.InteropServices.Marshal.SizeOf(typeof(Data_T)));
            }
        }

        if (GPUBuffer == null)
            return;
        Data_T[] data = new Data_T[trackedModifiers.Count];
        trackedModifiers.Values.CopyTo(data, 0);
        GPUBuffer.SetData(data);
    }


    ComputeBuffer IModifierGPUArray.GetBuffer()
    {
        return GPUBuffer;
    }
}




public abstract class GPULandscapeModifier : MonoBehaviour
{
    [Range(short.MinValue, short.MaxValue)]
    public short priority = 0;
    public bool overwrite = false;

    bool isReady = false;
    private void Start()
    {
        isReady = true;
    }
    private void OnDestroy()
    {
        isReady = false;
    }

    private void OnValidate()
    {
        if (isReady)
            OnUpdateData();
    }

    public abstract void OnUpdateData();
}
