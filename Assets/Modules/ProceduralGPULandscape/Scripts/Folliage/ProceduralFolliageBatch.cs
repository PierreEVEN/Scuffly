
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.Rendering;

[ExecuteInEditMode]
public class ProceduralFolliageBatch : MonoBehaviour
{
    public ProceduralFolliageAsset folliageAsset;
    public ProceduralFolliageNode folliageParent;

    ComputeBuffer treeMatrices = null;
    ComputeBuffer proceduralDrawArgs = null;
    ComputeBuffer shouldSpawnTreeBuffer = null;
    private uint[] matrixArgs = new uint[5] { 0, 0, 0, 0, 0 };
    MaterialPropertyBlock InstanceMaterialProperties;
    struct ShouldSpawnTreeStruct
    {
        Vector3 spawnPosition;
        int shouldSpawn;
    };

    private void OnEnable()
    {
#if UNITY_EDITOR
        SceneView.beforeSceneGui += DrawInEditor;
#endif
    }

    public void OnDisable()
    {
        if (treeMatrices != null)
            treeMatrices.Release();
        if (shouldSpawnTreeBuffer != null)
            shouldSpawnTreeBuffer.Release();
        if (proceduralDrawArgs != null)
            proceduralDrawArgs.Release();

        shouldSpawnTreeBuffer = null;
        treeMatrices = null;
        proceduralDrawArgs = null;

#if UNITY_EDITOR
        SceneView.beforeSceneGui -= DrawInEditor;
#endif
    }

    private void Update()
    {
#if UNITY_EDITOR
        if (Application.isPlaying || !EditorApplication.isPaused)
#endif
            DrawSection(null);
    }

    void DrawSection(Camera camera)
    {
        if (folliageAsset == null || folliageAsset.spawnedMesh == null || folliageAsset.usedMaterial == null)
            return;

        if (InstanceMaterialProperties == null)
            InstanceMaterialProperties = new MaterialPropertyBlock();

        if (folliageParent == null)
            return;

        Bounds bounds = new Bounds(folliageParent.nodePosition, new Vector3(folliageParent.nodeWidth, folliageParent.nodeWidth * 100f, folliageParent.nodeWidth));

        if (proceduralDrawArgs == null)
            CreateOrRecreateMatrices();

        if (proceduralDrawArgs == null)
            return;

        Graphics.DrawMeshInstancedIndirect(folliageAsset.spawnedMesh, 0, folliageAsset.usedMaterial, bounds, proceduralDrawArgs, 0, InstanceMaterialProperties, ShadowCastingMode.On, true, 0, camera);
    }


#if UNITY_EDITOR
    public void DrawInEditor(SceneView sceneview)
    {
        if (EditorApplication.isPaused && EditorApplication.isPlaying)
            DrawSection(SceneView.currentDrawingSceneView.camera);
    }
#endif



    void CreateOrRecreateMatrices()
    {
        // Recreate matrice buffer if needed
        int finalDensity = (int)(folliageAsset.DensityPerLevel * folliageParent.folliageSpawner.densityMultiplier);

        int desiredCount = finalDensity * finalDensity;
        if (treeMatrices == null || treeMatrices.count != desiredCount)
        {
            if (treeMatrices != null)
            {
                treeMatrices.Release();
            }
            if (desiredCount != 0)
                treeMatrices = new ComputeBuffer(desiredCount, sizeof(float) * 16, ComputeBufferType.Default);
            if (shouldSpawnTreeBuffer != null)
                shouldSpawnTreeBuffer.Dispose();
            if (desiredCount != 0)
                shouldSpawnTreeBuffer = new ComputeBuffer(desiredCount, sizeof(float) * 3 + sizeof(int), ComputeBufferType.Default);
        }

        if (proceduralDrawArgs != null) proceduralDrawArgs.Release();
        if (desiredCount != 0)
            proceduralDrawArgs = new ComputeBuffer(1, matrixArgs.Length * sizeof(uint), ComputeBufferType.IndirectArguments);

        if (desiredCount == 0)
            return;

        // reset instance count;
        matrixArgs[0] = folliageAsset.spawnedMesh.GetIndexCount(0);
        matrixArgs[1] = 0;
        matrixArgs[2] = folliageAsset.spawnedMesh.GetIndexStart(0);
        matrixArgs[3] = folliageAsset.spawnedMesh.GetBaseVertex(0);
        proceduralDrawArgs.SetData(matrixArgs);

        ComputeShader generationCS = folliageParent.folliageSpawner.generationShader; 
        ComputeShader matrixbuildShader = folliageParent.folliageSpawner.matrixbuildShader;
        if (!generationCS)
        {
            Debug.LogError("missing folliage generation compute shader");
            return;
        }

        int kernelIndex = generationCS.FindKernel("CSMain");
        generationCS.SetBuffer(kernelIndex, "shouldSpawnTreeBuffer", shouldSpawnTreeBuffer);
        generationCS.SetVector("origin", folliageParent.nodePosition - new Vector3(folliageParent.nodeWidth / 2, 0, folliageParent.nodeWidth / 2));
        generationCS.SetFloat("scale", folliageParent.nodeWidth);
        generationCS.SetInt("width", finalDensity);

        generationCS.SetFloat("minNormal", folliageAsset.minNormal);
        generationCS.SetFloat("maxNormal", folliageAsset.maxNormal);
        generationCS.SetFloat("minAltitude", folliageAsset.minAltitude);
        generationCS.SetFloat("maxAltitude", folliageAsset.maxAltitude);

        IModifierGPUArray.UpdateCompute(generationCS, kernelIndex);
        // Run compute shader
        generationCS.Dispatch(kernelIndex, finalDensity, finalDensity, 1);

        int kernel2Index = matrixbuildShader.FindKernel("CSMain");
        matrixbuildShader.SetBuffer(kernel2Index, "treeMatrices", treeMatrices);
        matrixbuildShader.SetBuffer(kernel2Index, "proceduralDrawArgs", proceduralDrawArgs);
        matrixbuildShader.SetBuffer(kernel2Index, "shouldSpawnTreeBuffer", shouldSpawnTreeBuffer);
        matrixbuildShader.SetInt("elemCount", desiredCount);
        matrixbuildShader.Dispatch(kernel2Index, 1, 1, 1);
        proceduralDrawArgs.GetData(matrixArgs);

        InstanceMaterialProperties.SetBuffer("matrixBuffer", treeMatrices);

        shouldSpawnTreeBuffer.Dispose();
        shouldSpawnTreeBuffer = null;
    }
}