
using UnityEngine;

[ExecuteInEditMode]
public class ProceduralFolliageBatch : MonoBehaviour
{
    public ProceduralFolliageAsset folliageAsset;
    public ProceduralFolliageNode folliageParent;

    ComputeBuffer matrixBuffer = null;
    ComputeBuffer matrixArgsBuffer = null;
    private uint[] matrixArgs = new uint[5] { 0, 0, 0, 0, 0 };
    MaterialPropertyBlock InstanceMaterialProperties;

    public void OnDisable()
    {
        if (matrixBuffer != null)
            matrixBuffer.Release();
        if (matrixArgsBuffer != null)
            matrixArgsBuffer.Release();

        matrixBuffer = null;
        matrixArgsBuffer = null;
    }

    private void Update()
    {
        if (folliageAsset == null || folliageAsset.spawnedMesh == null || folliageAsset.usedMaterial == null)
            return;

        if (InstanceMaterialProperties == null)
            InstanceMaterialProperties = new MaterialPropertyBlock();

        if (folliageParent == null)
            return;

        Bounds bounds = new Bounds(folliageParent.nodePosition, new Vector3(folliageParent.nodeWidth, folliageParent.nodeWidth * 0.1f, folliageParent.nodeWidth));

        if (matrixArgsBuffer == null)
            CreateOrRecreateMatrices();

        Graphics.DrawMeshInstancedIndirect(folliageAsset.spawnedMesh, 0, folliageAsset.usedMaterial, bounds, matrixArgsBuffer, 0, InstanceMaterialProperties);
    }

    void CreateOrRecreateMatrices()
    {
        // Recreate matrice buffer if needed
        int desiredCount = folliageAsset.DensityPerLevel * folliageAsset.DensityPerLevel;
        if (matrixBuffer == null || matrixBuffer.count != desiredCount)
        {
            if (matrixBuffer != null)
            {
                matrixBuffer.Release();
            }
            matrixBuffer = new ComputeBuffer(desiredCount, sizeof(float) * 16, ComputeBufferType.Structured);
        }

        if (matrixArgsBuffer != null) matrixArgsBuffer.Release();
        matrixArgsBuffer = new ComputeBuffer(1, matrixArgs.Length * sizeof(uint), ComputeBufferType.IndirectArguments);

        // reset instance count;
        matrixArgs[0] = folliageAsset.spawnedMesh.GetIndexCount(0);
        matrixArgs[1] = (uint)desiredCount;
        matrixArgs[2] = folliageAsset.spawnedMesh.GetIndexStart(0);
        matrixArgs[3] = folliageAsset.spawnedMesh.GetBaseVertex(0);
        matrixArgsBuffer.SetData(matrixArgs);

        ComputeShader generationCS = folliageParent.folliageSpawner.generationShader;
        if (!generationCS)
        {
            Debug.LogError("missing folliage generation compute shader");
            return;
        }

        int kernelIndex = generationCS.FindKernel("CSMain");
        generationCS.SetBuffer(kernelIndex, "outputBuffer", matrixBuffer);
        generationCS.SetVector("origin", folliageParent.nodePosition - new Vector3(folliageParent.nodeWidth / 2, 0, folliageParent.nodeWidth / 2));
        generationCS.SetFloat("scale", folliageParent.nodeWidth);
        generationCS.SetInt("width", folliageAsset.DensityPerLevel);
        IModifierGPUArray.UpdateCompute(generationCS, kernelIndex);
        // Run compute shader
        generationCS.Dispatch(kernelIndex, folliageAsset.DensityPerLevel, folliageAsset.DensityPerLevel, 1);
        InstanceMaterialProperties.SetBuffer("matrixBuffer", matrixBuffer);
    }
}