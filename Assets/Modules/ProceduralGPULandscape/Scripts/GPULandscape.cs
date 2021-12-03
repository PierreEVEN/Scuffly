using System;
using System.Collections.Generic;
#if (UNITY_EDITOR)
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.Profiling;


/*
 * Terrain procedural sur GPU : le maillage et le noise sont générés sur GPU évitant les lags de generation.
 * Permet une mise a jour du terrain en temps réel à très grande échelle et de faire de la modelisation par masques directement sur GPU (CF : GPULandscapeModifier)
 * 
 * TODO : passer la gestion du quadtree sur GPU (pas prioritaire ni necessaire, mais probleme interessant à traiter)
 */

[ExecuteAlways]
public class GPULandscape : MonoBehaviour, GPULandscapePhysicInterface
{
    /*
     * Le terrain est découpé en N x N sections (N = ViewDistance * 2 + 1)
     * chaque section est un quadtree indépendant subdivisé au besoin.
     */
    struct LandscapeSection
    {
        public int posX;
        public int posZ;
        public GPULandscapeNode root_node;
    }

    /**
     * Parameters
     */

    [Header("Quality"), Range(0, 20)]
    public int sectionLoadDistance = 4; // Distance / nombre de sections a charger (0 = 1 section à la fois)
    [Range(2, 100)]
    public int meshDensity = 50; // Subdivision du maillage de chaque section

    [Header("LodSettings"), Range(1000, 100000)]
    public float SectionWidth = 15000; // Largeur d'une section (km)
    [Range(1, 10)]
    public int maxSubdivisionLevel = 4; // Niveau de subdivision maximal d'une section
    [Range(50, 5000)]
    public float subdivisionThreshold = 500; // Reglage du seuil de subdivision


    // Material du terrain (full GPU : doit generer les vertices)
    public Material landscape_material;

    public ComputeShader HeightMaskCompute;

    [Header("Physics")]
    public ComputeShader landscapePhysicGetter;
    public bool enablePhysicUpdates = true;

    [Header("Developper features")] // Debug
    public bool Reset = false;

    public static GPULandscape Singleton;

    /**
     * Data
     */

    // Camera used in play mode
    private GameObject IngamePlayerCamera;
    private Vector3 cameraCurrentLocation = new Vector3();
    public Vector3 CameraCurrentLocation
    {
        get { return cameraCurrentLocation; }
    }
    [HideInInspector]
    public float currentGroundHeight = 0;

    // Liste des sections affichées
    private List<LandscapeSection> GeneratedSections = new List<LandscapeSection>();

    public void OnEnable()
    {
        Singleton = this;
        // Mise a jour des parametrages
        if (PlayerPrefs.HasKey("LandscapeResolution"))
            meshDensity = PlayerPrefs.GetInt("LandscapeResolution");

        IngamePlayerCamera = GameObject.FindGameObjectWithTag("MainCamera");
        UpdateCameraLocation();
        ResetLandscape();
        Refresh();

        GPULandscapePhysic.Singleton.AddListener(this);
    }

    private void Start()
    {
        if (Application.isPlaying)
        {
            DontDestroyOnLoad(this);
        }
    }

    public static void OnUpdateProperties()
    {
        foreach (var landscape in GameObject.FindGameObjectsWithTag("GPULandscape"))
        {
            landscape.SetActive(false);
            landscape.SetActive(true);
        }
    }

    public void OnValidate()
    {
        // Sauvegarde les parametres du landscape dans les preferences du UnityPlayer
        PlayerPrefs.SetInt("LandscapeResolution", meshDensity);
        generatedMesh = null;
    }


    public void Update()
    {
        Refresh();
    }

    void Refresh()
    {
        if (Reset) ResetLandscape();
        if (!landscape_material) return;

        Profiler.BeginSample("Update camera and materials");
        UpdateCameraLocation();
        IModifierGPUArray.UpdateMaterial(landscape_material);
        Profiler.EndSample();


        Profiler.BeginSample("Update sections");
        // Remove out of range section
        int cameraX = (int)Math.Truncate(CameraCurrentLocation.x / SectionWidth);
        int cameraZ = (int)Math.Truncate(CameraCurrentLocation.z / SectionWidth);
        for (int i = GeneratedSections.Count - 1; i >= 0; --i)
        {
            if (
                GeneratedSections[i].posX < cameraX - sectionLoadDistance ||
                GeneratedSections[i].posX > cameraX + sectionLoadDistance ||
                GeneratedSections[i].posZ < cameraZ - sectionLoadDistance ||
                GeneratedSections[i].posZ > cameraZ + sectionLoadDistance
            )
            {
                GeneratedSections[i].root_node.destroy();
                GeneratedSections.RemoveAt(i);
            }
        }

        // Try load missing sections that are in range
        for (int x = cameraX - sectionLoadDistance; x <= cameraX + sectionLoadDistance; ++x)
            for (int y = cameraZ - sectionLoadDistance; y <= cameraZ + sectionLoadDistance; ++y)
                TryLoadSection(x, y);

        Profiler.EndSample();

        // For each section : update quadtree and draw mesh
        Profiler.BeginSample("Update landscape quadtree");
        foreach (var section in GeneratedSections)
            section.root_node.Update();
        Profiler.EndSample();

        

        // Met a jour la physique
        if (enablePhysicUpdates)
        {

            Profiler.BeginSample("Update landscape Physics");
            GPULandscapePhysic.Singleton.ProcessData();
            Profiler.EndSample();
        }
    }

    public void OnDisable()
    {
        Singleton = null;

        GPULandscapePhysic.Singleton.RemoveListener(this);

        PlayerPrefs.Save();

        // Called on hot reload or when playing / returning back to editor ...
        ResetLandscape();
    }


    Mesh generatedMesh;
    public Mesh LandscapeMesh
    {
        get
        {
            if (generatedMesh != null)
                return generatedMesh;

            generatedMesh = new Mesh();

            int verticeWidth = meshDensity + 2;

            Vector3[] vertices = new Vector3[verticeWidth * verticeWidth];

            int[] triangles = new int[verticeWidth * verticeWidth * 6];

            for (int x = 0; x < verticeWidth; ++x)
            {
                for (int y = 0; y < verticeWidth; ++y)
                {
                    vertices[x + y * verticeWidth] = new Vector3((x - 1) / (float)(meshDensity - 1), 0, (y - 1) / (float)(meshDensity - 1)) + new Vector3(-0.5f, 0, -0.5f);

                    if (x == 0)
                        vertices[x + y * verticeWidth] += new Vector3(1 / (float)meshDensity, -1 / (float)meshDensity, 0);
                    if (x == verticeWidth - 1)
                        vertices[x + y * verticeWidth] += new Vector3(-1 / (float)meshDensity, -1 / (float)meshDensity, 0);
                    if (y == 0)
                        vertices[x + y * verticeWidth] += new Vector3(0, -1 / (float)meshDensity, 1 / (float)meshDensity);
                    if (y == verticeWidth - 1)
                        vertices[x + y * verticeWidth] += new Vector3(0, -1 / (float)meshDensity, -1 / (float)meshDensity);
                }
            }

            for (int x = 0; x < verticeWidth - 1; ++x)
            {
                for (int y = 0; y < verticeWidth - 1; ++y)
                {
                    int triangleIndex = (x + y * (verticeWidth - 1)) * 6;

                    triangles[triangleIndex] = (x + y * verticeWidth);
                    triangles[triangleIndex + 2] = (x + 1 + y * verticeWidth);
                    triangles[triangleIndex + 1] = (x + 1 + (y + 1) * verticeWidth);

                    triangles[triangleIndex + 4] = (x + y * verticeWidth);
                    triangles[triangleIndex + 3] = (x + 1 + (y + 1) * verticeWidth);
                    triangles[triangleIndex + 5] = (x + (y + 1) * verticeWidth);
                }
            }

            generatedMesh.vertices = vertices;
            generatedMesh.triangles = triangles;
            return generatedMesh;
        }
    }


    private void UpdateCameraLocation()
    {
        if (Application.isPlaying)
        {
            // Try get player camera
            if (!IngamePlayerCamera)
                IngamePlayerCamera = GameObject.FindGameObjectWithTag("MainCamera");
            if (IngamePlayerCamera)
                cameraCurrentLocation = IngamePlayerCamera.transform.position;
        }
#if (UNITY_EDITOR)
        else
        {
            // Else get editor camera location
            var Cameras = SceneView.GetAllSceneCameras();
            foreach (var cam in Cameras)
                cameraCurrentLocation = cam.transform.position;
        }
#endif
    }

    private void TryLoadSection(int posX, int posZ)
    {
        // Find landscape section at x,z location. If it doesn't exist, add a new one
        bool exists = false;
        foreach (var section in GeneratedSections)
            if (section.posX == posX && section.posZ == posZ)
                exists = true;
        if (!exists)
        {
            // Si la section n'est pas encore chargee : on la crée
            GeneratedSections.Add(new LandscapeSection
            {
                posX = posX,
                posZ = posZ,
                root_node = new GPULandscapeNode(this, 1, new Vector3(posX * SectionWidth, 0, posZ * SectionWidth), SectionWidth),
            });
        }
    }

    private void ResetLandscape()
    {
        Reset = false;
        generatedMesh = null;
        foreach (var section in GeneratedSections)
        {
            section.root_node.destroy();
        }
        GeneratedSections.Clear();
    }

    public Vector2[] Collectpoints()
    {
        return new Vector2[] { new Vector2(CameraCurrentLocation.x, CameraCurrentLocation.z) };
    }

    public void OnPointsProcessed(float[] processedPoints)
    {
        currentGroundHeight = processedPoints[0];
    }
}
