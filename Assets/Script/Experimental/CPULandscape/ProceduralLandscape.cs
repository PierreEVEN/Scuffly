using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.VFX;
/**
 *  @Author : Pierre EVEN
 */

/**
 * Procedural landscape class.
 * A procedural landscape contains multiple sections. Each section is subdivided into a quadtree of ProceduralLandscapeNode 
 */
[ExecuteInEditMode]
public class ProceduralLandscape : MonoBehaviour
{
    struct LandscapeSection
    {
        public int posX;
        public int posZ;
        public ProceduralLandscapeNode root_node;
    }

    /**
     * Parameters
     */

    [Header("Scale"), Range(1000, 100000)]
    public float SectionWidth = 15000;

    [Header("LOD"), Range(1, 10)]
    public int maxLevel = 4;
    [Header("LOD"), Range(50, 5000)]
    public float quadtreeExponent = 500;

    [Header("Rendering"), Range(0, 20)]
    public int ViewDistance = 4;
    [Header("Rendering"), Range(2, 500)]
    public int CellsPerChunk = 50;
    public Material landscape_material;

    [Header("Developper features")]
    public bool FreezeGeneration = false;
    public bool Reset = false;
    public bool showBounds = false;

    /**
     * Data
     */

    private GameObject IngamePlayerCamera;
    private Vector3 CameraCurrentLocation = new Vector3();
    private List<LandscapeSection> GeneratedSections = new List<LandscapeSection>();

    public void Awake()
    {
    }

    public void Start()
    {
        IngamePlayerCamera = GameObject.FindGameObjectWithTag("MainCamera");
        UpdateCameraLocation();
        ResetLandscape();
    }

    public void Update()
    {
        if (Reset) ResetLandscape();
        if (FreezeGeneration) return;

        UpdateCameraLocation();

        // Remove out of range section
        int cameraX = (int)Math.Truncate(GetCameraPosition().x / SectionWidth);
        int cameraZ = (int)Math.Truncate(GetCameraPosition().z / SectionWidth);
        for (int i = GeneratedSections.Count - 1; i >= 0; --i)
        {
            if (
                GeneratedSections[i].posX < cameraX - ViewDistance ||
                GeneratedSections[i].posX > cameraX + ViewDistance ||
                GeneratedSections[i].posZ < cameraZ - ViewDistance ||
                GeneratedSections[i].posZ > cameraZ + ViewDistance
            )
            {
                GeneratedSections[i].root_node.destroy();
                GeneratedSections.RemoveAt(i);
            }
        }

        // Try load missing in range sections
        for (int x = cameraX - ViewDistance; x <= cameraX + ViewDistance; ++x)
            for (int y = cameraZ - ViewDistance; y <= cameraZ + ViewDistance; ++y)
                tryLoadSection(x, y);

        // Refresh all sections
        foreach (var section in GeneratedSections)
            section.root_node.CustomUpdate();
    }

    private void OnDrawGizmos()
    {
        if (showBounds)
            foreach (var section in GeneratedSections)
                section.root_node.DrawGuizmo();

        if (Application.isEditor && !FreezeGeneration)
        {
            // Refresh all sections
            foreach (var section in GeneratedSections)
                section.root_node.CustomUpdate();
            UpdateCameraLocation();
        }
    }

    public void OnDisable()
    {
        // Called on hot reload or when playing / returning back to editor ...
        ResetLandscape();
    }

    public float GetAltitudeAtLocation(float x, float z)
    {
        return HeightGenerator.Singleton.GetAltitudeAtLocation(x, z);
    }

    public Vector3 GetCameraPosition()
    {
        return CameraCurrentLocation;
    }

    private void UpdateCameraLocation()
    {
        if (Application.isPlaying)
        {
            // Try get player camera
            if (!IngamePlayerCamera)
                IngamePlayerCamera = GameObject.FindGameObjectWithTag("MainCamera");
            if (IngamePlayerCamera)
                CameraCurrentLocation = IngamePlayerCamera.transform.position;
        }
        else
        {
            // Else get editor camera location
            var Cameras = SceneView.GetAllSceneCameras();
            foreach (var cam in Cameras)
                CameraCurrentLocation = cam.transform.position;
        }
    }

    private void tryLoadSection(int posX, int posZ)
    {
        // Find landscape section at x,z location. If it doesn't exist, add a new one
        bool exists = false;
        foreach (var section in GeneratedSections)
            if (section.posX == posX && section.posZ == posZ)
                exists = true;
        if (!exists)
        {
            GeneratedSections.Add(new LandscapeSection
            {
                posX = posX,
                posZ = posZ,
                root_node = new ProceduralLandscapeNode(this, 1, new Vector3(posX * SectionWidth, 0, posZ * SectionWidth), SectionWidth),
            });
        }
    }

    private void ResetLandscape()
    {
        Reset = false;
        foreach (var section in GeneratedSections)
        {
            section.root_node.destroy();
        }
        GeneratedSections.Clear();
    }
}