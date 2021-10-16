using System;
using System.Collections.Generic;
using UnityEngine;
public class ProceduralLandscapeNode
{

    private MeshRenderer meshRenderer;
    private MeshFilter meshFilter;
    private GameObject container;

    private List<ProceduralLandscapeNode> ChildNodes = new List<ProceduralLandscapeNode>();
    private int NodeLevel;
    private Vector3 Position;
    private float Scale;
    private ProceduralLandscape Landscape;

    Bounds TreeBounds;
    ComputeBuffer TreeBuffer;

    public ProceduralLandscapeNode(ProceduralLandscape inLandscape, int inNodeLevel, Vector3 inPosition, float inScale)
    {
        NodeLevel = inNodeLevel;
        Position = inPosition;
        Scale = inScale;
        Landscape = inLandscape;

        createMesh();
        CreateTreeMatrices();
    }
    ~ProceduralLandscapeNode()
    {
        TreeBuffer.Release();
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

        Graphics.DrawMeshInstancedIndirect(Landscape.tree_mesh, 0, Landscape.tree_material, TreeBounds, TreeBuffer);
    }

    private void CreateTreeMatrices()
    {
        Matrix4x4[] TreeMatrices = new Matrix4x4[Landscape.per_section_tree_density * Landscape.per_section_tree_density];
        TreeBuffer = new ComputeBuffer(TreeMatrices.Length, sizeof(float) * 16, ComputeBufferType.IndirectArguments);
        TreeBounds = new Bounds(Position, new Vector3(Scale * 2, Scale * 2, Scale * 2));

        for (int x = 0; x < Landscape.per_section_tree_density; ++x)
        {
            for (int z = 0; z < Landscape.per_section_tree_density; ++z)
            {
                float posX = Position.x + (x / (float)Landscape.per_section_tree_density - 0.5f) * Scale;
                float posZ = Position.z + (z / (float)Landscape.per_section_tree_density - 0.5f) * Scale;

                float altitude = Landscape.GetAltitudeAtLocation(posX, posZ);

                Matrix4x4 matrix = Matrix4x4.Translate(Position + new Vector3(posX, altitude, posZ));
                matrix *= Matrix4x4.Scale(new Vector3(2000, 2000, 2000));
                TreeMatrices[x + z * Landscape.per_section_tree_density] = matrix;

            }
        }

        TreeBuffer.SetData(TreeMatrices);
        Landscape.tree_material.SetBuffer("positionBuffer", TreeBuffer);
    }


    private void subdivide()
    {

        if (ChildNodes.Count == 0)
        {

            ChildNodes.Add(new ProceduralLandscapeNode(
                Landscape,
                NodeLevel + 1,
                new Vector3(Position.x - Scale / 4, Position.y, Position.z - Scale / 4),
                Scale / 2)
            );
            ChildNodes.Add(new ProceduralLandscapeNode(
                Landscape,
                NodeLevel + 1,
                new Vector3(Position.x + Scale / 4, Position.y, Position.z - Scale / 4),
                this.Scale / 2)
            );
            ChildNodes.Add(new ProceduralLandscapeNode(
                Landscape,
                NodeLevel + 1,
                new Vector3(Position.x + Scale / 4, Position.y, Position.z + Scale / 4),
                Scale / 2)
            );
            ChildNodes.Add(new ProceduralLandscapeNode(
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
        if (NodeLevel == Landscape.maxLevel)
            container.AddComponent<MeshCollider>();
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