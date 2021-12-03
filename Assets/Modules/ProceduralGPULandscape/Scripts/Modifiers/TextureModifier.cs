using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class TextureModifier : GPULandscapeModifier
{
    public struct TextureModifierData
    {
        public int priority;
        public int mode;
        public int textureID;
        public float zOffset;
        public Vector3 position;
        public Vector3 scale;
    }

    [System.NonSerialized]
    bool added = false;

    private static ModifierGPUArray<TextureModifier, TextureModifierData> gpuData = new ModifierGPUArray<TextureModifier, TextureModifierData>("TextureModifier");

    public TextureModifierData data;
    public Texture2D textureMask;
    [Range(0, 1)]
    public float zOffset = 0;
    private Texture2D internalTextureMask;

    private void OnEnable()
    {
        gpuData.TrackModifier(this);
        if (textureMask != null)
        {
            if (!added)
            {
                GPULandscapeTextureMask.AddTexture(textureMask);
                data.textureID = GPULandscapeTextureMask.GetTextureID(textureMask);
                added = true;
            }
        }
        GPULandscapeTextureMask.OnRebuildAtlas.AddListener(OnRebuildAtlas);
        OnUpdateData();
    }

    private void OnRebuildAtlas()
    {
        if (textureMask)
            data.textureID = GPULandscapeTextureMask.GetTextureID(textureMask);
    }

    private void OnDisable()
    {
        if (added)
        {
            GPULandscapeTextureMask.OnRebuildAtlas.RemoveListener(OnRebuildAtlas);
            GPULandscapeTextureMask.RemoveTexture(textureMask);
            gpuData.UntrackModifier(this);
            added = false;
        }
    }
    public override void OnUpdateData()
    {
        if (GPULandscape.Singleton)
            GPULandscape.Singleton.Reset = true;
        data.position = transform.position;
        data.scale = transform.localScale;
        data.zOffset = zOffset;
        if (internalTextureMask != textureMask)
        {
            if (internalTextureMask)
            {
                if (added)
                {
                    added = false;
                    GPULandscapeTextureMask.RemoveTexture(internalTextureMask);
                }
            }

            internalTextureMask = textureMask;

            if (internalTextureMask)
            {
                if (!added)
                {
                    added = true;
                    GPULandscapeTextureMask.AddTexture(internalTextureMask);
                    data.textureID = GPULandscapeTextureMask.GetTextureID(textureMask);
                }
            }
        }

        gpuData.UpdateValue(this, data);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(0.5f, 0.5f, 0, 0.2f);
        Gizmos.DrawCube(data.position + new Vector3(0, data.scale.y / 2, 0), data.scale);
    }
    
    // Update is called once per frame
    void Update()
    {
        if (data.scale != transform.localScale || data.position != transform.position || internalTextureMask != textureMask)
            OnUpdateData();
    }
}
