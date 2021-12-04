
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

    static Texture2D PackTexture(ref Texture2D[] textures, ref Rect[] transforms)
    {
        int width = 0;
        int maxHeight = 0;
        foreach (var text in textures)
        {
            width += text.width + 2;
            if (text.height > maxHeight)
                maxHeight = text.height;
        }
        if (width > 2)
            width -= 2;

        if (maxHeight <= 0 || width <= 0)
            return new Texture2D(1, 1);

        Texture2D txt = new Texture2D(width, maxHeight, TextureFormat.RGBAFloat, false);

        int currentWidth = 0;
        int textID = 0;
        transforms = new Rect[textures.Length];

        foreach (var text in textures)
        {
            txt.SetPixels(currentWidth, 0, text.width, text.height, text.GetPixels());
            transforms[textID++] = new Rect(currentWidth / (float)width, 0, text.width / (float)width, text.height / (float)maxHeight);
            currentWidth += text.width + 2;
        }

        txt.Apply();
        return txt;
    }


    public static void BuildAtlas()
    {
        Texture2D[] textures = new Texture2D[textureMap.Count];
        int textId = 0;
        Texture2D[] Keys = new Texture2D[textureMap.Count];
        if (texturePositionBuffer != null)
            texturePositionBuffer.Dispose();
        texturePositionBuffer = new ComputeBuffer(textureMap.Count == 0 ? 1 : textureMap.Count, System.Runtime.InteropServices.Marshal.SizeOf(typeof(Rect)));


        if (textureMap.Count == 0)
        {
            packedAtlas = new Texture2D(1, 1);
            texturePositions = new Rect[0];
            return;
        }

        textureMap.Keys.CopyTo(Keys, 0);
        foreach (var key in Keys)
        {
            textures[textId] = key;

            var texRef = textureMap[key];
            texRef.ID = textId;
            textureMap[key] = texRef;
            textId++;

        }
        packedAtlas = PackTexture(ref textures, ref texturePositions);

        texturePositionBuffer.SetData(texturePositions);
        OnRebuildAtlas.Invoke();
    }

    public static void ApplyToMaterial(Material mat)
    {
        if (packedAtlas == null || texturePositionBuffer == null || texturePositions == null)
            BuildAtlas();

        mat.SetTexture("LandscapeMaskAtlas", packedAtlas);
        mat.SetBuffer("TextureMasksRefs", texturePositionBuffer);
        mat.SetInt("TextureMasksRefs_Count", texturePositions.Length);
    }

    public static void ApplyToComputeBuffer(ComputeShader mat, int kernelIndex)
    {
        if (packedAtlas == null || texturePositionBuffer == null || texturePositions == null)
            BuildAtlas();

        mat.SetTexture(kernelIndex, "LandscapeMaskAtlas", packedAtlas);
        mat.SetBuffer(kernelIndex, "TextureMasksRefs", texturePositionBuffer);
        mat.SetInt("TextureMasksRefs_Count", texturePositions.Length);
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

