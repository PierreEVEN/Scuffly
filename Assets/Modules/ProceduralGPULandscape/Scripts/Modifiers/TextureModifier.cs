using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class TextureModifier : GPULandscapeModifier
{
    public struct TextureModifierData
    {
        public int textureID;
        public float zOffset;
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
        gpuData.UpdateValue(this);
    }

    private void OnDrawGizmos()
    {
#if UNITY_EDITOR
        Gizmos.color = new Color(affectAltitude ? 1 : 0.5f, affectFoliage ? 1 : 0.5f, affectGrass ? 1 : 0.5f, UnityEditor.Selection.activeGameObject == this.gameObject ? 0.5f : 0.1f);
        Gizmos.DrawCube(transform.position + new Vector3(0, transform.localScale.y / 2, 0), transform.localScale);
#endif
    }
    
    // Update is called once per frame
    public override void InternalUpdate()
    {
        if (internalTextureMask != textureMask)
           UpdateData();
    }

    public override byte[] GetCustomData()
    {
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
        return StructToBytes(data);
    }
}
