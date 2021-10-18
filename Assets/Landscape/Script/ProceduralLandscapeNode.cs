using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
/**
 *  @Author : Pierre EVEN
 */

public class ProceduralLandscapeNode
{
    private MeshRenderer meshRenderer; // node mesh
    private GameObject gameObject; // Owner of node's components
    private List<ProceduralLandscapeNode> children = new List<ProceduralLandscapeNode>();
    private int subdivisionLevel; // Current subdivision level (+1 per subdivision)
    private Vector3 worldPosition;
    private float worldScale;
    private ProceduralLandscape owningLandscape; // Parent

    public ProceduralLandscapeNode(ProceduralLandscape inLandscape, int inNodeLevel, Vector3 inPosition, float inScale)
    {
        subdivisionLevel = inNodeLevel;
        worldPosition = inPosition;
        worldScale = inScale;
        owningLandscape = inLandscape;
        gameObject = new GameObject("ProceduralLandscapeNode");
        gameObject.hideFlags = HideFlags.DontSave;
        gameObject.transform.parent = owningLandscape.transform;

        // Start mesh generation operation (@TODO make generation async)
        createMesh();
    }

    // Called from Landscape
    public void CustomUpdate()
    {
        /*
        Either we wants to subdivide this node if he is to close, either we wants to display its mesh section
         */
        if (computeDesiredLODLevel() > subdivisionLevel)
            subdivide();
        else
            unSubdivide();

        foreach (var child in children)
            child.CustomUpdate();
    }

    // Subdivide this node into 4 nodes 2 times smaller
    private void subdivide()
    {

        if (children.Count == 0)
        {

            children.Add(new ProceduralLandscapeNode(
                owningLandscape,
                subdivisionLevel + 1,
                new Vector3(worldPosition.x - worldScale / 4, worldPosition.y, worldPosition.z - worldScale / 4),
                worldScale / 2)
            );
            children.Add(new ProceduralLandscapeNode(
                owningLandscape,
                subdivisionLevel + 1,
                new Vector3(worldPosition.x + worldScale / 4, worldPosition.y, worldPosition.z - worldScale / 4),
                this.worldScale / 2)
            );
            children.Add(new ProceduralLandscapeNode(
                owningLandscape,
                subdivisionLevel + 1,
                new Vector3(worldPosition.x + worldScale / 4, worldPosition.y, worldPosition.z + worldScale / 4),
                worldScale / 2)
            );
            children.Add(new ProceduralLandscapeNode(
                owningLandscape,
                subdivisionLevel + 1,
                new Vector3(worldPosition.x - worldScale / 4, worldPosition.y, worldPosition.z + worldScale / 4),
                worldScale / 2)
            );
        }

        bool childBuilt = true;
        foreach (var child in children)
        {
            if (child.meshRenderer == null)
            {
                childBuilt = false;
                return;
            }
        }
        // Wait children are built to hide geometry (avoid holes if children are not fully generated)
        if (childBuilt)
            hideGeometry();
    }

    private void unSubdivide()
    {
        showGeometry();

        /*
        Destroy child if not destroyed
         */
        if (children.Count == 0) return;
        foreach (var child in children)
            child.destroy();
        children.Clear();
    }

    private void hideGeometry()
    {
        if (meshRenderer)
            meshRenderer.enabled = false;
    }
    private void showGeometry()
    {
        if (meshRenderer)
            meshRenderer.enabled = true;
    }

    void createMesh()
    {
        int Density = owningLandscape.CellsPerChunk;

        int VerticeCount = (Density + 3) * (Density + 3) * 3;
        int IndiceCount = (Density + 2) * (Density + 2) * 6;

        Vector3[] v_position = new Vector3[VerticeCount];
        Vector3[] v_normals = new Vector3[VerticeCount];
        Vector2[] v_uvs = new Vector2[VerticeCount];
        Color[] v_colors = new Color[VerticeCount];

        int[] i_indices = new int[IndiceCount];

        float CellSize = worldScale / Density;

        int VerticesPerChunk = Density + 3;
        int FacesPerChunk = Density + 2;

        float OffsetX = worldPosition.x - worldScale / 2;
        float OffsetZ = worldPosition.z - worldScale / 2;

        /* Generate vertices */
        for (int x = 0; x < VerticesPerChunk; ++x)
        {
            for (int y = 0; y < VerticesPerChunk; ++y)
            {
                float l_PosX = (x - 1) * CellSize + OffsetX;
                float l_posZ = (y - 1) * CellSize + OffsetZ;

                int VertexIndex = (x + y * VerticesPerChunk);

                v_position[VertexIndex] = new Vector3(l_PosX, owningLandscape.GetAltitudeAtLocation(l_PosX, l_posZ), l_posZ);
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

        // Create mesh object
        Mesh mesh = new Mesh();
        mesh.vertices = v_position;
        mesh.colors = v_colors;
        mesh.normals = v_normals;
        mesh.uv = v_uvs;
        mesh.triangles = i_indices;
        mesh.RecalculateNormals(); // We recompute normal before moving seams down

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

        // Set mesh
        meshRenderer = gameObject.AddComponent<MeshRenderer>();
        meshRenderer.material = owningLandscape.landscape_material;
        meshRenderer.bounds = new Bounds(worldPosition, new Vector3(worldScale, worldScale, worldScale));
        MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
        meshFilter.mesh = mesh;

        // Enable collision on max level nodes
        if (subdivisionLevel == owningLandscape.maxLevel)
            gameObject.AddComponent<MeshCollider>();

        // Particle system for grass // @TODO improve system
        VisualEffect vfx = gameObject.AddComponent<VisualEffect>();
        vfx.visualEffectAsset = owningLandscape.GrassFX;
        vfx.SetMesh("NewMesh", mesh);
        vfx.SetVector3("BoundCenter", meshRenderer.bounds.center);
        vfx.SetVector3("BoundExtent", meshRenderer.bounds.size);
    }

    public void destroy()
    {
        unSubdivide();
        UnityEngine.Object.DestroyImmediate(gameObject);
        gameObject = null;
    }

    private int computeDesiredLODLevel()
    {
        // Height correction
        Vector3 cameraGroundLocation = owningLandscape.GetCameraPosition();
        cameraGroundLocation.y -= owningLandscape.GetAltitudeAtLocation(owningLandscape.GetCameraPosition().x, owningLandscape.GetCameraPosition().z);
        float Level = owningLandscape.maxLevel - Math.Min(owningLandscape.maxLevel, (Vector3.Distance(cameraGroundLocation, worldPosition) - worldScale) / owningLandscape.quadtreeExponent);
        return (int)Math.Truncate(Level);
    }
}