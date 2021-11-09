using System;
using System.Collections.Generic;
#if (UNITY_EDITOR)
using UnityEditor;
#endif
using UnityEngine;


/*
 * Terrain procedural sur GPU : le maillage et le noise sont générés sur GPU évitant les lags de generation.
 * Permet une mise a jour du terrain en temps réel à très grande échelle et de faire de la modelisation par masques directement sur GPU (CF : GPULandscapeModifier)
 * 
 * TODO : passer la gestion du quadtree sur GPU (pas prioritaire ni necessaire, mais probleme interessant à traiter)
 */ 

[ExecuteInEditMode]
public class GPULandscape : MonoBehaviour
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

    // Largeur d'une section (km)
    public float SectionWidth = .1f;
    public float worldExponent = 4;
    public float minValue = 0;

    // Niveau de subdivision maximal d'une section
    [Header("LOD"), Range(0, 10)]
    public int maxLevel = 4;
    // Reglage du seuil de subdivision
    [Header("LOD"), Range(50, 5000)]
    public float quadtreeExponent = 500;

    // Distance / nombre de sections a charger (0 = 1 section à la fois)
    [Header("Rendering"), Range(0, 20)]
    public int ViewDistance = 4;

    // Subdivision du maillage de chaque section
    [Header("Rendering"), Range(2, 2000)]
    public int chunkSubdivision = 50;

    // Material du terrain (full GPU : doit generer les vertices)
    public Material landscape_material;

    [Header("Physics")]
    public ComputeShader landscapePhysicGetter;

    [Header("Developper features")] // Debug
    public bool Reset = false;

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

    // Liste des sections affichées
    private List<LandscapeSection> GeneratedSections = new List<LandscapeSection>();

    public void OnEnable()
    {
        // Mise a jour des parametrages
        if (PlayerPrefs.HasKey("LandscapeResolution"))
            chunkSubdivision = PlayerPrefs.GetInt("LandscapeResolution");

        IngamePlayerCamera = GameObject.FindGameObjectWithTag("MainCamera");
        UpdateCameraLocation();
        ResetLandscape();
        Refresh();
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
        PlayerPrefs.SetInt("LandscapeResolution", chunkSubdivision);
    }


    public void Update()
    {
        Refresh();

        // Met a jour la physique
        GPULandscapePhysic.Singleton.ProcessData();
    }

    void Refresh()
    {
        if (Reset) ResetLandscape();
        if (!landscape_material) return;

        IModifierGPUArray.UpdateMaterial(landscape_material);

        UpdateCameraLocation();

        // Remove out of range section
        int cameraX = (int)Math.Truncate(CameraCurrentLocation.x / SectionWidth);
        int cameraZ = (int)Math.Truncate(CameraCurrentLocation.z / SectionWidth);
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

        // Try load missing sections that are in range
        for (int x = cameraX - ViewDistance; x <= cameraX + ViewDistance; ++x)
            for (int y = cameraZ - ViewDistance; y <= cameraZ + ViewDistance; ++y)
                TryLoadSection(x, y);

        // For each section : update quadtree and draw mesh
        foreach (var section in GeneratedSections)
            section.root_node.Update();

    }

    public void OnDisable()
    {
        PlayerPrefs.Save();

        // Called on hot reload or when playing / returning back to editor ...
        ResetLandscape();
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
        foreach (var section in GeneratedSections)
        {
            section.root_node.destroy();
        }
        GeneratedSections.Clear();
    }
}
