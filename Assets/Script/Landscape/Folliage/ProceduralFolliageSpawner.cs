using System.Collections;
using System.Collections.Generic;
#if (UNITY_EDITOR)
using UnityEditor;
#endif
using UnityEngine;

[ExecuteInEditMode]
public class ProceduralFolliageSpawner : MonoBehaviour
{
    [HideInInspector]
    public List<float> LodLevels = new List<float>();

    public List<ProceduralFolliageAsset> foliageAssets = new List<ProceduralFolliageAsset>();
    [Range(0, 5)]
    public int Radius = 2;
    [Range(100, 20000)]
    public float SectionWidth = 2000;
    [Range(0.1f, 2.0f)]
    public float foliageDensityMultiplier = 1.0f;

    public ComputeShader generationShader;
    public ComputeShader matrixbuildShader;

    public bool Reset = false;
    public bool FreezeGeneration = false;

    public bool DrawDebugBounds = false;

    [HideInInspector]
    public Vector3 CameraPosition = new Vector3();

    private Dictionary<Vector2Int, ProceduralFolliageNode> nodes = new Dictionary<Vector2Int, ProceduralFolliageNode>();

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    public void Update()
    {
        if (Reset)
        {
            Reset = false;
            ResetFolliage();
        }
        if (FreezeGeneration)
            return;
        UpdateCameraLocation();

        int cameraX = (int)(CameraPosition.x / SectionWidth);
        int cameraZ = (int)(CameraPosition.z / SectionWidth);

        List<Vector2Int> removedNodes = new List<Vector2Int>();
        foreach (var node in nodes)
        {
            if (
                node.Key.x < cameraX - Radius ||
                node.Key.x > cameraX + Radius ||
                node.Key.y < cameraZ - Radius ||
                node.Key.y > cameraZ + Radius
            )
            {
                node.Value.DestroyNode();
                removedNodes.Add(node.Key);
            }
        }
        foreach (var node in removedNodes)
            nodes.Remove(node);


        for (int x = -Radius; x <= Radius; ++x)
        {
            for (int z = -Radius; z <= Radius; ++z)
            {
                Vector2Int key = new Vector2Int(cameraX + x, cameraZ + z);

                if (!nodes.ContainsKey(key))
                    nodes.Add(key, new ProceduralFolliageNode(this, new Vector3(key.x * SectionWidth, 0, key.y * SectionWidth), 0, SectionWidth));
            }
        }

        foreach (var node in nodes)
            node.Value.UpdateInternal();
    }

    private void OnValidate()
    {
        Reset = true;
    }

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

    void ResetFolliage()
    {
        foreach (var child in nodes)
            child.Value.DestroyNode();
        nodes.Clear();
    }
}