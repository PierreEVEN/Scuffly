
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
        if (folliageAsset == null || folliageAsset.Impostor == null)
            return;

        if (InstanceMaterialProperties == null)
            InstanceMaterialProperties = new MaterialPropertyBlock();

        Bounds bounds = new Bounds(folliageParent.nodePosition, new Vector3(folliageParent.nodeWidth, folliageParent.nodeWidth * 0.1f, folliageParent.nodeWidth));

        if (matrixArgsBuffer == null)
            CreateOrRecreateMatrices();

        Graphics.DrawMeshInstancedIndirect(folliageAsset.Impostor.Mesh, 0, folliageAsset.Impostor.Material, bounds, matrixArgsBuffer, 0, InstanceMaterialProperties);
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
            matrixBuffer = new ComputeBuffer(desiredCount, sizeof(float) * 16, ComputeBufferType.IndirectArguments);
        }

        if (matrixArgsBuffer != null) matrixArgsBuffer.Release();
        matrixArgsBuffer = new ComputeBuffer(1, matrixArgs.Length * sizeof(uint), ComputeBufferType.IndirectArguments);

        // reset instance count;
        matrixArgs[0] = folliageAsset.Impostor.Mesh.GetIndexCount(0);
        matrixArgs[1] = (uint)desiredCount;
        matrixArgs[2] = folliageAsset.Impostor.Mesh.GetIndexStart(0);
        matrixArgs[3] = folliageAsset.Impostor.Mesh.GetBaseVertex(0);
        matrixArgsBuffer.SetData(matrixArgs);

        ComputeShader generationCS = folliageParent.folliageSpawner.generationShader;
        int kernelIndex = generationCS.FindKernel("CSMain");
        generationCS.SetBuffer(kernelIndex, "outputBuffer", matrixBuffer);
        generationCS.SetVector("origin", folliageParent.nodePosition - new Vector3(folliageParent.nodeWidth / 2, 0, folliageParent.nodeWidth / 2));
        generationCS.SetFloat("scale", folliageParent.nodeWidth);
        generationCS.SetInt("width", folliageAsset.DensityPerLevel);
        IModifierGPUArray.UpdateCompute(generationCS, kernelIndex);
        // Run compute shader
        InstanceMaterialProperties.SetBuffer("matrixBuffer", matrixBuffer);
        generationCS.Dispatch(kernelIndex, folliageAsset.DensityPerLevel, folliageAsset.DensityPerLevel, 1);


    }
}