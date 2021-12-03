#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public class GPULandscapeNode
{
    private GPULandscape owner;
    private bool shouldDisplay;

    // Section informations
    private int quadtreeLevel;
    private float width;
    private Vector3 worldPosition;

    // Delimitations de la section (@TODO : faire la gestion des bounds sur l'axe y)
    private Bounds bounds;

    //  Informations pour le material sur le node courrant a afficher
    private MaterialPropertyBlock MPB;

    // Nodes enfant du quadtree
    private GPULandscapeNode[] children;

    private int[] indirectArgs = { 0, 1, 0, 0, 0 };
    private ComputeBuffer indirectArgsBuffer;

    private ComputeBuffer indirectIndirectArgsBuffer;
    private ComputeBuffer nodeGeneratedLayers;

    public GPULandscapeNode(GPULandscape owner, int quadtreeLevel, Vector3 worldPosition, float width)
    {
        this.owner = owner;
        this.quadtreeLevel = quadtreeLevel;
        this.worldPosition = worldPosition;
        this.width = width;
        MPB = new MaterialPropertyBlock();
        bounds = new Bounds(worldPosition, new Vector3(this.width, 100000, this.width));

        int totalVerticeWidth = owner.meshDensity + 2;
        indirectArgsBuffer = new ComputeBuffer(5, sizeof(int), ComputeBufferType.IndirectArguments);
        nodeGeneratedLayers = new ComputeBuffer(totalVerticeWidth * totalVerticeWidth, sizeof(float) * 3, ComputeBufferType.Default);
        indirectIndirectArgsBuffer = new ComputeBuffer(3, sizeof(int), ComputeBufferType.IndirectArguments);

        indirectIndirectArgsBuffer.SetData(new int[] { owner.meshDensity, owner.meshDensity, 1});
        int kernel = owner.HeightMaskCompute.FindKernel("CSMain");

        owner.HeightMaskCompute.SetBuffer(kernel, "_Altitude", nodeGeneratedLayers);
        owner.HeightMaskCompute.SetInt("_Subdivision", totalVerticeWidth);
        owner.HeightMaskCompute.SetFloat("_Width", width / owner.meshDensity);
        owner.HeightMaskCompute.SetVector("_Offset", worldPosition);

        IModifierGPUArray.UpdateCompute(owner.HeightMaskCompute, kernel);
        owner.HeightMaskCompute.Dispatch(kernel, totalVerticeWidth * totalVerticeWidth, 1, 1);


#if UNITY_EDITOR
        SceneView.beforeSceneGui += DrawInEditor;
#endif
    }

    public void destroy()
    {
        MPB = null;
        indirectArgsBuffer.Release();
        nodeGeneratedLayers.Release();
        indirectIndirectArgsBuffer.Release();
        ShowCurrentNode();

#if UNITY_EDITOR
        SceneView.beforeSceneGui -= DrawInEditor;
#endif
    }


    void DrawSection(Camera camera)
    {
        if (shouldDisplay)
        {
            // Draw mesh using landscape material
            int totalVerticeWidth = owner.meshDensity + 2;

            MPB.SetInt("_Subdivision", totalVerticeWidth);
            MPB.SetFloat("_Width", width / owner.meshDensity);
            MPB.SetVector("_Offset", worldPosition);
            MPB.SetBuffer("_Altitude", nodeGeneratedLayers);

            if (indirectArgs[0] != totalVerticeWidth * totalVerticeWidth * 6)
            {
                indirectArgs[0] = totalVerticeWidth * totalVerticeWidth * 6;
                indirectArgsBuffer.SetData(indirectArgs);
            }

            Graphics.DrawProceduralIndirect(owner.landscape_material, bounds, MeshTopology.Triangles, indirectArgsBuffer, 0, camera, MPB);
        }
    }


#if UNITY_EDITOR
    public void DrawInEditor(SceneView sceneview)
    {
        if (EditorApplication.isPaused && EditorApplication.isPlaying)
            DrawSection(SceneView.currentDrawingSceneView.camera);
    }
#endif


    // Update is called once per frame
    public void Update()
    {
        /*
        Si le niveau de subdivision desire est superieur au niveau de subdivision courant : on subdivise, sinon on affiche le mesh courant et on detruit les nodes enfants
         */
        if (ComputeDesiredLODLevel() > quadtreeLevel)
            SubdivideCurrentNode();
        else
            ShowCurrentNode();

#if UNITY_EDITOR
        if (Application.isPlaying || !EditorApplication.isPaused)
#endif
            DrawSection(null);

        // Propagate update through children
        if (children != null)
            foreach (var child in children)
                child.Update();
    }


    // Detruit les nodes enfant et active le rendu du mesh courant.
    private void ShowCurrentNode()
    {
        shouldDisplay = true;

        /*
        Destroy child if not destroyed
         */
        if (children == null) return;
        foreach (var child in children)
            child.destroy();
        children = null;
    }

    // Subdivise le node courant en 4 nodes 2 fois plus petits. Desactive l'affichage du node courant
    private void SubdivideCurrentNode()
    {
        // Test if already subdivided, or start subdivision
        if (children == null)
        {
            children = new GPULandscapeNode[4];
            children[0] = new GPULandscapeNode(
                owner,
                quadtreeLevel + 1,
                new Vector3(worldPosition.x - width / 4, worldPosition.y, worldPosition.z - width / 4),
                width / 2);
            children[1] = new GPULandscapeNode(
                owner,
                quadtreeLevel + 1,
                new Vector3(worldPosition.x + width / 4, worldPosition.y, worldPosition.z - width / 4),
                width / 2);
            children[2] = new GPULandscapeNode(
                owner,
                quadtreeLevel + 1,
                new Vector3(worldPosition.x + width / 4, worldPosition.y, worldPosition.z + width / 4),
                width / 2);
            children[3] = new GPULandscapeNode(
                owner,
                quadtreeLevel + 1,
                new Vector3(worldPosition.x - width / 4, worldPosition.y, worldPosition.z + width / 4),
                width / 2);
        }

        shouldDisplay = false;
    }

    private int ComputeDesiredLODLevel()
    {
        // Height correction
        Vector3 cameraGroundLocation = owner.CameraCurrentLocation;
        cameraGroundLocation.y -= owner.currentGroundHeight; // substract landscape altitude at camera location
        float Level = owner.maxSubdivisionLevel - Mathf.Min(owner.maxSubdivisionLevel, (Vector3.Distance(cameraGroundLocation, worldPosition) - width) / owner.subdivisionThreshold);
        return (int)Level;
    }
}
