using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[ExecuteInEditMode]
public class ProceduralLandscape : MonoBehaviour
{

    public Material landscape_material;

    // TREES
    public Mesh tree_mesh;
    public Material tree_material;
    public int per_section_tree_density = 50;

    struct SectionReference
    {
        public int posX;
        public int posZ;
        public ProceduralLandscapeSection section;
    }

    Vector3 cameraLocation = new Vector3();
    List<SectionReference> Sections = new List<SectionReference>();

    public int ViewDistance = 4;
    public int CellsPerChunk = 50;
    public float SectionWidth = 15000;
    public float noiseScale = 1000;
    public float maxLevel = 4;
    public float quadtreeExponent = 500;
    public bool freeze = false;

    GameObject PlayerCamera;

    public void Start()
    {
        UpdateCameraLocation();
        PlayerCamera = GameObject.FindGameObjectWithTag("MainCamera");
        ClearSections();
    }

    public float GetAltitudeAtLocation(float x, float z)
    {
        return HeightGenerator.Get().GetAltitudeAtLocation(x * 0.04f, z * 0.04f);
    }

    public Vector3 GetCameraPosition()
    {
        return cameraLocation;
    }

    public void Update()
    {
        if (freeze) return;

        UpdateCameraLocation();

        int cameraX = (int)Math.Truncate(GetCameraPosition().x / SectionWidth);
        int cameraZ = (int)Math.Truncate(GetCameraPosition().z / SectionWidth);
        for (int i = Sections.Count - 1; i >= 0; --i)
        {
            if (
                Sections[i].posX < cameraX - ViewDistance ||
                Sections[i].posX > cameraX + ViewDistance ||
                Sections[i].posZ < cameraZ - ViewDistance ||
                Sections[i].posZ > cameraZ + ViewDistance
            )

            {
                Sections[i].section.destroy();
                Sections.RemoveAt(i);
            }
        }

        for (int x = cameraX - ViewDistance; x <= cameraX + ViewDistance; ++x)
        {
            for (int y = cameraZ - ViewDistance; y <= cameraZ + ViewDistance; ++y)
            {
                tryLoadSection(x, y);
            }
        }

        foreach (var section in Sections)
        {
            section.section.update();
        }
    }

    private void UpdateCameraLocation()
    {
        if (Application.isPlaying)
        {
            if (!PlayerCamera)
                PlayerCamera = GameObject.FindGameObjectWithTag("MainCamera");
            if (PlayerCamera)
            {
                cameraLocation = PlayerCamera.transform.position;
            }
        }
        else
        {
            var Cameras = SceneView.GetAllSceneCameras();
            foreach (var cam in Cameras)
            {
                cameraLocation = cam.transform.position;
            }
        }
    }

    public void tryLoadSection(int posX, int posZ)
    {
        bool exists = false;
        foreach (var section in Sections)
        {
            if (section.posX == posX && section.posZ == posZ)
            {
                exists = true;
            }
        }
        if (!exists)
        {
            SectionReference new_section = new SectionReference();
            new_section.posX = posX;
            new_section.posZ = posZ;
            new_section.section = new ProceduralLandscapeSection(this, new Vector3(posX * SectionWidth, 0, posZ * SectionWidth), SectionWidth);
            Sections.Add(new_section);
        }
    }

    private void ClearSections()
    {
        foreach (var section in Sections)
        {
            section.section.destroy();
        }
        Sections.Clear();
    }

    public void OnDisable()
    {
        ClearSections();
    }

}
