using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
public class GPULandscapeCollision : MonoBehaviour
{

    private static GPULandscapeCollision singleton;

    public static GPULandscapeCollision Singleton
    {
        get
        {
            return singleton;
        }
    }

    public ComputeShader textureGridGetterShader;

    public GPULandscapeCollision()
    {
        singleton = this;
    }
    private void OnEnable()
    {
        singleton = this;
    }
    private void Start()
    {
        singleton = this;
    }
    public void GetAltitudeGrid(ref Vector3[] grid, int width, float scale)
    {
        if (textureGridGetterShader == null)
        {
            Debug.LogError("missing compute shader");
            return;
        }

        int kernelIndex = textureGridGetterShader.FindKernel("CSMain");

        RenderTexture gridTextureGetter = new RenderTexture(width, width, 0, RenderTextureFormat.RFloat);
        Texture2D textureGridGetterOutput = new Texture2D(width, width, TextureFormat.RFloat, false);
        gridTextureGetter.dimension = UnityEngine.Rendering.TextureDimension.Tex2D;
        gridTextureGetter.enableRandomWrite = true;
        gridTextureGetter.Create();

        IModifierGPUArray.UpdateCompute(textureGridGetterShader, kernelIndex);
        textureGridGetterShader.SetTexture(kernelIndex, "Output", gridTextureGetter);
        textureGridGetterShader.SetFloat("AreaScale", scale);
        textureGridGetterShader.SetVector("AreaOrigin", grid[0]);
        textureGridGetterShader.Dispatch(kernelIndex, width, width, 1);

        RenderTexture.active = gridTextureGetter;
        textureGridGetterOutput.ReadPixels(new Rect(0, 0, width, width), 0, 0);
        textureGridGetterOutput.Apply();
        RenderTexture.active = null;


        for (int i = 0; i < width; ++i)
        {
            for (int j = 0; j < width; ++j)
            {
                grid[i + j * width].y = textureGridGetterOutput.GetPixel(i, j).r;
            }
        }
    }
}
