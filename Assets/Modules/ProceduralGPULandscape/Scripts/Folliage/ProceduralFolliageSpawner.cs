using System.Collections;
using System.Collections.Generic;
#if (UNITY_EDITOR)
using UnityEditor;
#endif
using UnityEngine;

/// <summary>
/// Generate the foliage on a procedural landscape.
/// On the CPU side it's a basic quadtree, but the trees of each node are generated on the GPU to avoid GPU-CPU data transferts.
/// </summary>
[ExecuteInEditMode]
public class ProceduralFolliageSpawner : MonoBehaviour
{
    /// <summary>
    /// Lod configuration (one max distance per lod level)
    /// </summary>
    [HideInInspector]
    public List<float> LodLevels = new List<float>();

    /// <summary>
    /// The foliage assets used by this foliage system
    /// </summary>
    public List<ProceduralFolliageAsset> foliageAssets = new List<ProceduralFolliageAsset>();

    /// <summary>
    /// Quality settings
    /// </summary>
    [Header("Quality"), Range(0, 5)]
    public int sectionLoadDistance = 2;
    [Range(0.1f, 2.0f)]
    public float densityMultiplier = 1.0f;
    [Header("LodSettings"), Range(100, 20000)]
    public float SectionWidth = 2000;

    /// <summary>
    /// The shader used to determine if a tree spawn at each position
    /// </summary>
    public ComputeShader generationShader;
    /// <summary>
    /// The shader used to group the generated data into a big matrix buffer
    /// </summary>
    public ComputeShader matrixbuildShader;

    /// <summary>
    /// Rebuild everyhting
    /// </summary>
    public bool Reset = false;

    // Debug
    public bool FreezeGeneration = false;
    public bool DrawDebugBounds = false;

    [HideInInspector]
    public Vector3 CameraPosition = new Vector3();

    /// <summary>
    /// A list of spawned section (the number of section depend on the sectionLoadDistance parameter)
    /// </summary>
    private Dictionary<Vector2Int, ProceduralFolliageNode> nodes = new Dictionary<Vector2Int, ProceduralFolliageNode>();

    // Start is called before the first frame update
    void Start()
    {
    }

    public void Update()
    {
        if (Reset)
        {
            Reset = false;
            ResetFolliage();
        }
        if (FreezeGeneration || densityMultiplier == 0)
            return;
        UpdateCameraLocation();

        int cameraX = (int)(CameraPosition.x / SectionWidth);
        int cameraZ = (int)(CameraPosition.z / SectionWidth);

        // Get a list of node that are out of loading range
        List<Vector2Int> removedNodes = new List<Vector2Int>();
        foreach (var node in nodes)
        {
            if (
                node.Key.x < cameraX - sectionLoadDistance ||
                node.Key.x > cameraX + sectionLoadDistance ||
                node.Key.y < cameraZ - sectionLoadDistance ||
                node.Key.y > cameraZ + sectionLoadDistance
            )
            {
                node.Value.DestroyNode();
                removedNodes.Add(node.Key);
            }
        }
        // Destroy them
        foreach (var node in removedNodes)
            nodes.Remove(node);

        // Find which node need to be added
        for (int x = -sectionLoadDistance; x <= sectionLoadDistance; ++x)
        {
            for (int z = -sectionLoadDistance; z <= sectionLoadDistance; ++z)
            {
                Vector2Int key = new Vector2Int(cameraX + x, cameraZ + z);

                if (!nodes.ContainsKey(key))
                    nodes.Add(key, new ProceduralFolliageNode(this, new Vector3(key.x * SectionWidth, 0, key.y * SectionWidth), 0, SectionWidth));
            }
        }

        // Update all the node quadtree
        foreach (var node in nodes)
            node.Value.UpdateInternal();
    }

    private void OnValidate()
    {
        Reset = true;
    }


    /// <summary>
    /// Update the positino of the camera (use editor or Main camera depending on the context)
    /// </summary>
    private void UpdateCameraLocation()
    {
        if (Application.isPlaying)
        {
            // Try get player camera
            GameObject IngamePlayerCamera = GameObject.FindGameObjectWithTag("MainCamera");
            if (IngamePlayerCamera)
                CameraPosition = IngamePlayerCamera.transform.position;
        }
#if (UNITY_EDITOR)
        else
        {
            // Else get editor camera location
            var Cameras = SceneView.GetAllSceneCameras();
            foreach (var cam in Cameras)
                CameraPosition = cam.transform.position;
        }
#endif
    }

    private void OnDrawGizmos()
    {
        if (!DrawDebugBounds)
            return;
        foreach (var child in nodes)
        {
            child.Value.DrawDebug();
        }
    }
    public void OnDisable()
    {
        // Called on hot reload or when playing / returning back to editor ...
        ResetFolliage();
    }

    /// <summary>
    /// Destroy everything
    /// </summary>
    void ResetFolliage()
    {
        foreach (var child in nodes)
            child.Value.DestroyNode();
        nodes.Clear();
    }
}