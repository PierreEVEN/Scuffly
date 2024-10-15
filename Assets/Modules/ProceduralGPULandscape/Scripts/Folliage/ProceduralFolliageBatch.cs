
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.Rendering;

/// <summary>
/// For each foliage node, a foliage batch is created for each foliage asset used by the foliage system
/// </summary>
[ExecuteInEditMode]
public class ProceduralFolliageBatch : MonoBehaviour
{
    /// <summary>
    /// Foliage asset used
    /// </summary>
    public ProceduralFolliageAsset folliageAsset;

    /// <summary>
    /// Parent foliage node that is owning this batch
    /// </summary>
    public ProceduralFolliageNode folliageParent;

    /// <summary>
    /// Compute buffer containing the matrices of each instances
    /// </summary>
    ComputeBuffer treeMatrices = null;

    /// <summary>
    /// Compute buffer containing the number of instance to spawn
    /// </summary>
    ComputeBuffer proceduralDrawArgs = null;
    private uint[] matrixArgs = new uint[5] { 0, 0, 0, 0, 0 };

    /// <summary>
    /// Compute buffer containing the result of the tree generation
    /// </summary>
    ComputeBuffer shouldSpawnTreeBuffer = null;

    /// <summary>
    /// Current batch material custom data
    /// </summary>
    MaterialPropertyBlock InstanceMaterialProperties;

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
        // Ensure there is no configuration error
        if (folliageAsset == null || folliageAsset.spawnedMesh == null || folliageAsset.usedMaterial == null)
            return;

        if (InstanceMaterialProperties == null)
            InstanceMaterialProperties = new MaterialPropertyBlock();

        if (folliageParent == null)
            return;

        // The bound height is approximative because all the data is generated on the GPU
        Bounds bounds = new Bounds(folliageParent.nodePosition, new Vector3(folliageParent.nodeWidth, folliageParent.nodeWidth * 100f, folliageParent.nodeWidth));

        // Create the tree matrices
        if (proceduralDrawArgs == null)
            CreateOrRecreateMatrices();

        if (proceduralDrawArgs == null)
            return;

        // If everything is ok, draw the foliage batch
        Graphics.DrawMeshInstancedIndirect(folliageAsset.spawnedMesh, 0, folliageAsset.usedMaterial, bounds, proceduralDrawArgs, 0, InstanceMaterialProperties, ShadowCastingMode.On, true, 0, camera);
    }


#if UNITY_EDITOR
    public void DrawInEditor(SceneView sceneview)
    {
        if (EditorApplication.isPaused && EditorApplication.isPlaying)
            DrawSection(SceneView.currentDrawingSceneView.camera);
    }
#endif


    /// <summary>
    /// Generate the tree and build the matrice array
    /// </summary>
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

        // Step 1 : for each tree, we check if he can spawn, if it is the cas we store it's position into a buffer
        int kernelIndex = generationCS.FindKernel("CSMain");
        generationCS.SetBuffer(kernelIndex, "shouldSpawnTreeBuffer", shouldSpawnTreeBuffer);
        generationCS.SetVector("origin", folliageParent.nodePosition - new Vector3(folliageParent.nodeWidth / 2, 0, folliageParent.nodeWidth / 2));
        generationCS.SetFloat("scale", folliageParent.nodeWidth);
        generationCS.SetInt("width", finalDensity);

        generationCS.SetFloat("minNormal", folliageAsset.minNormal);
        generationCS.SetFloat("maxNormal", folliageAsset.maxNormal);
        generationCS.SetFloat("minAltitude", folliageAsset.minAltitude);
        generationCS.SetFloat("maxAltitude", folliageAsset.maxAltitude);

        IModifierGPUArray.ApplyToComputeBuffer(generationCS, kernelIndex);
        // Run compute shader
        generationCS.Dispatch(kernelIndex, finalDensity, finalDensity, 1);


        // Step 2 : we collect the previous generation data, and we generate a matrice for each tree that has spawned
        //@TODO : make it work in parallel !!!!!
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