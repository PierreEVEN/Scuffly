using System;
using System.Collections.Generic;
using UnityEngine;


public class ProceduralLandscape : MonoBehaviour
{

    public Material landscape_material;

    struct SectionReference
    {
        public int posx;
        public int posZ;
        public ProceduralLandscapeSection section;
    }

    GameObject scene_camera;
    List<SectionReference> Sections = new List<SectionReference>();

    public int ViewDistance = 4;
    public int CellsPerChunk = 20;
    public float SectionWidth = 20000;
    public float noiseScale = 1000;
    public float maxLevel = 8;
    public float quadtreeExponent = 500;
    public bool freeze = false;
    private HeightGenerator height_generator;

    public void Start()
    {
        scene_camera = GameObject.FindWithTag("MainCamera");
        height_generator = new HeightGenerator();
        //Update();
    }


    public float GetAltitudeAtLocation(float x, float z)
    {
        return height_generator.GetAltitudeAtLocation(x * 0.1f, z * 0.1f);
    }

    public Vector3 GetCameraPosition()
    {
        if (scene_camera == null)
        {
            scene_camera = GameObject.FindWithTag("MainCamera");
            Debug.LogError("failed to get current camera");
            return new Vector3();
        }
        return scene_camera.transform.position;
    }

    public void Update()
    {
        if (freeze) return;
        int cameraX = (int)Math.Truncate(GetCameraPosition().x / SectionWidth);
        int cameraZ = (int)Math.Truncate(GetCameraPosition().z / SectionWidth);
        for (int i = Sections.Count - 1; i >= 0; --i)
        {
            if (
                Sections[i].posx < cameraX - ViewDistance ||
                Sections[i].posx > cameraX + ViewDistance ||
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

    public void tryLoadSection(int posX, int posZ)
    {
        bool exists = false;
        foreach (var section in Sections)
        {
            if (section.posx == posX && section.posZ == posZ)
            {
                exists = true;
            }
        }
        if (!exists)
        {
            SectionReference new_section = new SectionReference();
            new_section.posx = posX;
            new_section.posZ = posZ;
            new_section.section = new ProceduralLandscapeSection(this, new Vector3(posX * SectionWidth, 0, posZ * SectionWidth), SectionWidth);
            Sections.Add(new_section);
        }
    }

    public void rebuildLandscape()
    {
        foreach (var section in Sections)
        {
            section.section.destroy();
        }

        Sections = null;
    }
}
