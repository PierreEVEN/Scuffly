using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class TextureModifier : GPULandscapeModifier
{
    public struct TextureModifierData
    {
        public int textureID;
        public Vector3 position;
        public Vector3 scale;
    }

    private static ModifierGPUArray<TextureModifier, TextureModifierData> gpuData = new ModifierGPUArray<TextureModifier, TextureModifierData>("TextureModifier");

    public TextureModifierData data;
    public Texture2D textureMask;
    private Texture2D internalTextureMask;

    private void OnEnable()
    {
        gpuData.TrackModifier(this);
        if (textureMask != null)
        {
            GPULandscapeTextureMask.AddTexture(textureMask);
            data.textureID = GPULandscapeTextureMask.GetTextureID(textureMask);
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
        GPULandscapeTextureMask.OnRebuildAtlas.RemoveListener(OnRebuildAtlas);
        GPULandscapeTextureMask.RemoveTexture(textureMask);
        gpuData.UntrackModifier(this);
    }

    public override void OnUpdateData()
    {
        data.position = transform.position;
        data.scale = transform.localScale;

        if (internalTextureMask)
        {
            GPULandscapeTextureMask.RemoveTexture(internalTextureMask);
        }

        internalTextureMask = textureMask;

        if (internalTextureMask)
        {
            GPULandscapeTextureMask.AddTexture(internalTextureMask);
            data.textureID = GPULandscapeTextureMask.GetTextureID(textureMask);
        }

        gpuData.UpdateValue(this, data);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(0.5f, 0.5f, 0, 0.2f);
        Gizmos.DrawCube(data.position, data.scale);
    }
    
    // Update is called once per frame
    void Update()
    {
        if (data.scale != transform.localScale || data.position != transform.position || internalTextureMask != textureMask)
            OnUpdateData();
    }
}
