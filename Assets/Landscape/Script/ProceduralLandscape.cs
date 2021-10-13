using System;
using System.Collections.Generic;
using UnityEngine;




class HeightGenerator
{
	public HeightGenerator()
    {
    }

    public float GetAltitudeAtLocation(float posX, float posY)
    {

        posX += 100000;
        posY += 10000;

        float mountainLevel = getMountainLevel(posX, posY);

        float alt = mountainLevel * 800;

        float scale = 0.01f;
        float mountainNoise = (float)Math.Pow(Mathf.PerlinNoise(posX * scale, posY * scale), 2) * 3000;

        alt += mountainLevel * mountainNoise;

        alt += getHillsLevel(posX, posY, mountainLevel) * 200;

        alt = addBeaches(posX, posY, alt);

        return alt;
    }

    float getMountainLevel(float posX, float posY)
    {
        float scale = 0.001f;
        float level = 1.5f - Mathf.PerlinNoise(posX * scale, posY * scale) * 1.5f;
        level -= 0.5f;

        return level;
    }

    float getHillsLevel(float posX, float posY, float mountainLevel)
    {
        float scale = 0.01f;
        return Mathf.PerlinNoise(posX * scale, posY * scale) * (1 - (float)Math.Pow(Math.Abs(mountainLevel), 1));
    }

    float addBeaches(float posX, float posY, float currentAltitude)
    {

        return currentAltitude;
    }



};


class LandscapeNode
{

    private MeshRenderer meshRenderer;
    private MeshFilter meshFilter;
    private GameObject container;

    private List<LandscapeNode> ChildNodes = new List<LandscapeNode>();
    private int NodeLevel;
    private Vector3 Position;
    private float Scale;
    private ProceduralLandscape Landscape;


    public LandscapeNode(ProceduralLandscape inLandscape, int inNodeLevel, Vector3 inPosition, float inScale)
    {
        NodeLevel = inNodeLevel;
        Position = inPosition;
        Scale = inScale;
        Landscape = inLandscape;

        createMesh();
    }

    public void update()
    {
        /*
        Either we wants to subdivide this node if he is to close, either we wants to display its mesh section
         */
        if (computeDesiredLODLevel() > NodeLevel)
            subdivide();
        else
            unSubdivide();

        foreach (var child in ChildNodes)
            child.update();
    }

    private void subdivide()
    {

        if (ChildNodes.Count == 0)
        {

            ChildNodes.Add(new LandscapeNode(
                Landscape,
                NodeLevel + 1,
                new Vector3(Position.x - Scale / 4, Position.y, Position.z - Scale / 4),
                Scale / 2)
            );
            ChildNodes.Add(new LandscapeNode(
                Landscape,
                NodeLevel + 1,
                new Vector3(Position.x + Scale / 4, Position.y, Position.z - Scale / 4),
                this.Scale / 2)
            );
            ChildNodes.Add(new LandscapeNode(
                Landscape,
                NodeLevel + 1,
                new Vector3(Position.x + Scale / 4, Position.y, Position.z + Scale / 4),
                Scale / 2)
            );
            ChildNodes.Add(new LandscapeNode(
                Landscape,
                NodeLevel + 1,
                new Vector3(Position.x - Scale / 4, Position.y, Position.z + Scale / 4),
                Scale / 2)
            );
        }

        bool childBuilt = true;
        foreach (var child in ChildNodes)
        {
            if (child.meshFilter == null)
            {
                childBuilt = false;
                return;
            }
        }
        if (childBuilt)
        {
            hideGeometry();
        }
        else
        {
        }
    }

    private void unSubdivide()
    {
        showGeometry();

        /*
        Destroy child if not destroyed
         */
        if (ChildNodes.Count == 0) return;
        foreach (var child in ChildNodes) 
            child.destroy();
        ChildNodes.Clear();
    }

    private void hideGeometry()
    {
        meshRenderer.enabled = false;
    }
    private void showGeometry()
    {
        meshRenderer.enabled = true;
    }

    void createMesh()
    {
        int Density = Landscape.CellsPerChunk;
        float PosX = Position.x;
        float PosZ = Position.z;
        float Size = Scale;

        int VerticeCount = (Density + 3) * (Density + 3) * 3;
        int IndiceCount = (Density + 2) * (Density + 2) * 6;

        Vector3[] v_position = new Vector3[VerticeCount];
        Vector3[] v_normals = new Vector3[VerticeCount];
        Vector2[] v_uvs = new Vector2[VerticeCount];
        Color[] v_colors = new Color[VerticeCount];

        int[] i_indices = new int[IndiceCount];

        float CellSize = Size / Density;

        int VerticesPerChunk = Density + 3;
        int FacesPerChunk = Density + 2;

        float OffsetX = PosX - Size / 2;
        float OffsetZ = PosZ - Size / 2;

        /* Generate vertices */
        for (int x = 0; x < VerticesPerChunk; ++x)
        {
            for (int y = 0; y < VerticesPerChunk; ++y)
            {
                float l_PosX = (x - 1) * CellSize + OffsetX;
                float l_posZ = (y - 1) * CellSize + OffsetZ;

                int VertexIndex = (x + y * VerticesPerChunk);

                v_position[VertexIndex] = new Vector3(l_PosX, Landscape.GetAltitudeAtLocation(l_PosX, l_posZ), l_posZ);
                v_normals[VertexIndex] = new Vector3(0, 1, 0);
                v_colors[VertexIndex] = new Color32(0, 255, 0, 255);
                v_uvs[VertexIndex] = new Vector2(l_PosX / 100, l_posZ / 100);
            }
        }

        /* Generate indices */
        for (int x = 0; x < FacesPerChunk; ++x)
        {
            for (int y = 0; y < FacesPerChunk; ++y)
            {

                int IndiceIndex = (x + y * FacesPerChunk) * 6;

                i_indices[IndiceIndex] = (x + y * VerticesPerChunk);
                i_indices[IndiceIndex + 2] = (x + 1 + y * VerticesPerChunk);
                i_indices[IndiceIndex + 1] = (x + 1 + (y + 1) * VerticesPerChunk);

                i_indices[IndiceIndex + 3] = (x + y * VerticesPerChunk);
                i_indices[IndiceIndex + 5] = (x + 1 + (y + 1) * VerticesPerChunk);
                i_indices[IndiceIndex + 4] = (x + (y + 1) * VerticesPerChunk);
            }
        }

        // @TODO COMPUTE NORMAL

        // North seams
        for (int i = 0; i < VerticesPerChunk; ++i)
        {
            // Align Y to zero
            v_position[i].z += CellSize;
            v_position[i].y -= CellSize;
        }
        // South seams
        int maxSouth = VerticesPerChunk * VerticesPerChunk;
        for (int i = VerticesPerChunk * (VerticesPerChunk - 1); i < maxSouth; ++i)
        {
            // Align Y to zero
            v_position[i].z -= CellSize;
            v_position[i].y -= CellSize;
        }
        // West seams
        int maxWest = VerticesPerChunk * (VerticesPerChunk);
        for (int i = 0; i < maxWest; i += VerticesPerChunk)
        {
            // Align Y to zero
            v_position[i].x += CellSize;
            v_position[i].y -= CellSize;
        }
        // East seams
        int maxEast = VerticesPerChunk * VerticesPerChunk - 1;
        for (int i = VerticesPerChunk - 1; i < maxEast; i += VerticesPerChunk)
        {
            // Align Y to zero
            v_position[i].x -= CellSize;
            v_position[i].y -= CellSize;
        }

        container = new GameObject("Cool GameObject made from Code");

        meshRenderer = container.AddComponent<MeshRenderer>();

        if (meshRenderer == null)
        {
            Debug.LogError("failed to create mesh renderer");
        }
        meshRenderer.material = Landscape.landscape_material;

        meshFilter = container.AddComponent<MeshFilter>();

        if (meshFilter == null)
        {
            Debug.LogError("failed to create mesh filter");
        }
        Mesh mesh = new Mesh();

        mesh.vertices = v_position;
        mesh.colors = v_colors;
        mesh.normals = v_normals;
        mesh.uv = v_uvs;

        mesh.triangles = i_indices;

        meshFilter.mesh = mesh;
    }

    public void destroy()
    {
        unSubdivide();
        hideGeometry();
        meshFilter = null;
        meshRenderer = null;
    }

    private int computeDesiredLODLevel()
    {
        // Height correction
        Vector3 cameraGroundLocation = Landscape.GetCameraPosition();
        cameraGroundLocation.y -= Landscape.GetAltitudeAtLocation(Landscape.GetCameraPosition().x, Landscape.GetCameraPosition().z);
        float Level = Landscape.maxLevel - Math.Min(Landscape.maxLevel, (Vector3.Distance(cameraGroundLocation, Position) - Scale) / Landscape.quadtreeExponent);
        return (int)Math.Truncate(Level);
    }
}

class LandscapeSection
{
    public Vector3 Pos;
    ProceduralLandscape Landscape;
    float Scale;
    LandscapeNode RootNode;
    public LandscapeSection(ProceduralLandscape inLandscape, Vector3 inPos, float inScale)
    {
        Landscape = inLandscape;
        Pos = inPos;
        Scale = inScale;
        RootNode = new LandscapeNode(Landscape, 1, Pos, Scale);
    }

    public void update()
    {
        RootNode.update();
    }

    /**
     * Destructor
     */
    public void destroy()
    {
        RootNode.destroy();
        RootNode = null;
    }

}


public class ProceduralLandscape : MonoBehaviour
{

    public Material landscape_material;

    struct SectionReference
    {
        public int posx;
        public int posZ;
        public LandscapeSection section;
    }

    Camera scene_camera;
    List<SectionReference> Sections = new List<SectionReference>();

    public int ViewDistance = 4;
    public int CellsPerChunk = 20;
    public float SectionWidth = 20000;
    public float noiseScale = 1000;
    public float maxLevel = 8;
    public float quadtreeExponent = 500;
    private HeightGenerator height_generator;

    public void Start()
    {
        scene_camera = Camera.main;
        height_generator = new HeightGenerator();
        Update();
    }


    public float GetAltitudeAtLocation(float x, float z)
    {
        return height_generator.GetAltitudeAtLocation(x * 0.1f, z * 0.1f);
    }

    public Vector3 GetCameraPosition()
    {
        if (scene_camera == null)
        {
            scene_camera = Camera.main;
            Debug.LogError("failed to get current camera");
            return new Vector3();
        }
        return scene_camera.transform.position;
    }

    public void Update()
    {
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
            new_section.section = new LandscapeSection(this, new Vector3(posX * SectionWidth, 0, posZ * SectionWidth), SectionWidth);
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
