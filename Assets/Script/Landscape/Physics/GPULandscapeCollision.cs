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


    private void OnEnable()
    {
        singleton = this;
    }
    public void GetAltitudeGrid(ref Vector3[] grid, int resX, float scale)
    {
        if (textureGridGetterShader == null)
        {
            Debug.LogError("missing compute shader");
            return;
        }

        int kernelIndex = textureGridGetterShader.FindKernel("CSMain");

        int resY = grid.Length / resX;
        RenderTexture gridTextureGetter = new RenderTexture(resX, resY, 0, RenderTextureFormat.RFloat);
        Texture2D textureGridGetterOutput = new Texture2D(resX, resY, TextureFormat.RFloat, false);
        gridTextureGetter.dimension = UnityEngine.Rendering.TextureDimension.Tex2D;
        gridTextureGetter.enableRandomWrite = true;
        gridTextureGetter.Create();

        IModifierGPUArray.UpdateCompute(textureGridGetterShader, kernelIndex);
        textureGridGetterShader.SetTexture(kernelIndex, "Output", gridTextureGetter);
        textureGridGetterShader.SetFloat("AreaScale", scale / resX);
        textureGridGetterShader.SetVector("AreaOrigin", grid[0]);
        textureGridGetterShader.Dispatch(kernelIndex, resX, resY, 1);

        RenderTexture.active = gridTextureGetter;
        textureGridGetterOutput.ReadPixels(new Rect(0, 0, resX, resY), 0, 0);
        textureGridGetterOutput.Apply();
        RenderTexture.active = null;


        for (int i = 0; i < resX; ++i)
        {
            for (int j = 0; j < resY; ++j)
            {
                grid[i + j * resX].y = 0;// textureGridGetterOutput.GetPixel(i, j).r;
            }
        }
    }
}
