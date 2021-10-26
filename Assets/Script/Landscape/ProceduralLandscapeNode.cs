using System;
using System.Collections.Generic;
using System.Threading;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.VFX;
/**
 *  @Author : Pierre EVEN
 */

struct SectionGenerationJob : IJob
{
    public int meshDensity;
    public float nodeWorldScale;
    public Vector3 nodeWorldPosition;
    public bool shouldAbort;

    public NativeArray<float> bounds;
    public NativeArray<int> i_indices;
    public NativeArray<Vector3> v_position;
    public NativeArray<Vector3> v_normals;
    public NativeArray<Vector2> v_uvs;
    public NativeArray<Color> v_colors;

    public void Execute()
    {
        if (shouldAbort) return;

        float CellSize = nodeWorldScale / meshDensity;

        int VerticesPerChunk = meshDensity + 3;
        int FacesPerChunk = meshDensity + 2;

        float OffsetX = nodeWorldPosition.x - nodeWorldScale / 2;
        float OffsetZ = nodeWorldPosition.z - nodeWorldScale / 2;
        bounds[0] = 0;
        bounds[1] = 0;
        /* Generate vertices */
        for (int x = 0; x < VerticesPerChunk && !shouldAbort; ++x)
        {
            for (int y = 0; y < VerticesPerChunk && !shouldAbort; ++y)
            {
                float l_PosX = (x - 1) * CellSize + OffsetX;
                float l_posZ = (y - 1) * CellSize + OffsetZ;
                float l_posY = HeightGenerator.Singleton.GetAltitudeAtLocation(l_PosX, l_posZ);

                if ((x == 0 && y == 0) || (l_posY < bounds[0])) bounds[0] = l_posY;
                if (((x == 0 && y == 0) || l_posY > bounds[1]) && l_posY > 0) bounds[1] = l_posY;

                int VertexIndex = (x + y * VerticesPerChunk);

                v_position[VertexIndex] = new Vector3(l_PosX, l_posY, l_posZ);
                v_colors[VertexIndex] = new Color32(0, 255, 0, 255);
                v_uvs[VertexIndex] = new Vector2(l_PosX / 100, l_posZ / 100);

                Vector3 normal;
                if (x == 0 || x == VerticesPerChunk - 1 || y == 0 || y == VerticesPerChunk - 1)
                    normal = new Vector3(0, 1, 0);
                else
                    normal = Vector3.Cross(v_position[(x - 1 + y * VerticesPerChunk)] - v_position[(x + y * VerticesPerChunk)], v_position[(x + y * VerticesPerChunk)] - v_position[(x + (y - 1) * VerticesPerChunk)]).normalized;

                v_normals[VertexIndex] = normal;
            }
        }

        /* Generate indices */
        for (int x = 0; x < FacesPerChunk && !shouldAbort; ++x)
        {
            for (int y = 0; y < FacesPerChunk && !shouldAbort; ++y)
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

        // Move the seams down to avoid holes in landscape

        // North seams
        for (int i = 0; i < VerticesPerChunk && !shouldAbort; ++i)
        {
            // Align Y to zero
            v_position[i] = v_position[i] + new Vector3(0, -CellSize, CellSize);
        }
        // South seams
        int maxSouth = VerticesPerChunk * VerticesPerChunk;
        for (int i = VerticesPerChunk * (VerticesPerChunk - 1); i < maxSouth && !shouldAbort; ++i)
        {
            // Align Y to zero
            v_position[i] = v_position[i] + new Vector3(0, -CellSize, -CellSize);
        }
        // West seams
        int maxWest = VerticesPerChunk * (VerticesPerChunk);
        for (int i = 0; i < maxWest && !shouldAbort; i += VerticesPerChunk)
        {
            // Align Y to zero
            v_position[i] = v_position[i] + new Vector3(CellSize, -CellSize, 0);
        }
        // East seams
        int maxEast = VerticesPerChunk * VerticesPerChunk - 1;
        for (int i = VerticesPerChunk - 1; i < maxEast && !shouldAbort; i += VerticesPerChunk)
        {
            // Align Y to zero
            v_position[i] = v_position[i] + new Vector3(-CellSize, -CellSize, 0);
        }
    }

    public void DisposeData()
    {
        if (bounds.IsCreated)
            bounds.Dispose();
        if (i_indices.IsCreated)
            i_indices.Dispose();
        if (v_position.IsCreated)
            v_position.Dispose();
        if (v_normals.IsCreated)
            v_normals.Dispose();
        if (v_uvs.IsCreated)
            v_uvs.Dispose();
        if (v_colors.IsCreated)
            v_colors.Dispose();
    }
}

public class ProceduralLandscapeNode
{
    private MeshRenderer meshRenderer; // node mesh
    private MeshFilter meshFilter;
    private GameObject gameObject; // Owner of node's components
    private List<ProceduralLandscapeNode> children = new List<ProceduralLandscapeNode>();
    private int subdivisionLevel; // Current subdivision level (+1 per subdivision)
    private Vector3 worldPosition;
    private float worldScale;
    private ProceduralLandscape owningLandscape; // Parent
    private SectionGenerationJob generationJob;
    private JobHandle generationJobHandle;
    private Bounds bounds;

    bool shouldDisplay = false;

    public ProceduralLandscapeNode(ProceduralLandscape inLandscape, int inNodeLevel, Vector3 inPosition, float inScale)
    {
        subdivisionLevel = inNodeLevel;
        worldPosition = inPosition;
        worldScale = inScale;
        owningLandscape = inLandscape;
        gameObject = new GameObject("ProceduralLandscapeNode");
        gameObject.hideFlags = HideFlags.DontSave | HideFlags.HideInHierarchy;
        gameObject.transform.parent = owningLandscape.transform;
        
        // Start mesh generation operation
        HeightGenerator.Singleton.OnUpdateRegion.AddListener(UpdateRegion);
        requestMeshUpdate();
    }

    // Event called when region is updated and need mesh update
    private void UpdateRegion(Rect region)
    {
        if (HeightGenerator.RectIntersect(region, new Rect(new Vector2(worldPosition.x - worldScale / 2, worldPosition.z - worldScale / 2), new Vector2(worldScale, worldScale))))
            requestMeshUpdate();
    }

    // Called from Landscape
    public void CustomUpdate()
    {
        /*
        Either we wants to subdivide this node if he is to close to the camera, either we wants to display its mesh section
         */
        if (ComputeDesiredLODLevel() > subdivisionLevel)
            SubdivideCurrentNode();
        else
            ShowCurrentNode();

        if (shouldDisplay && !meshRenderer)
            TryBuildMesh();

        if (meshRenderer)
            meshRenderer.enabled = shouldDisplay || !IsSubstageGenerationReady();

        // Propagate update through children
        foreach (var child in children)
            child.CustomUpdate();
    }

    public void DrawGuizmo()
    {
        if (bounds != null && shouldDisplay)
        {
            Gizmos.color = new Color((subdivisionLevel * 579 + 89) % 255 / 255.0f, (subdivisionLevel * 289 + 789) % 255 / 255.0f, (subdivisionLevel * 1587 + 89) % 255 / 255.0f, 0.5f);
            Gizmos.DrawCube(bounds.center, bounds.size + new Vector3(0, 5, 0));
        }
        foreach (var child in children)
            child.DrawGuizmo();
    }

    // Subdivide this node into 4 nodes 2 times smaller
    private void SubdivideCurrentNode()
    {
        // Test if already subdivided, or start subdivision
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

        shouldDisplay = false;
    }

    private void ShowCurrentNode()
    {
        shouldDisplay = true;

        /*
        Destroy child if not destroyed
         */
        if (children.Count == 0) return;
        foreach (var child in children)
            child.destroy();
        children.Clear();
    }

    public bool IsDisplayed()
    {
        return meshRenderer && meshRenderer.enabled;
    }

    public bool IsSubstageGenerationReady()
    {
        if (shouldDisplay)
            return meshRenderer;

        foreach(var child in children)
        {
            if (!child.IsSubstageGenerationReady())
                return false;
        }
        return true;
    }


    private void TryBuildMesh()
    {
        if (IsDisplayed())
            return;
        if (meshRenderer)
        {
            meshRenderer.enabled = true;
            return;
        }

        if (!generationJobHandle.IsCompleted) 
            return;

        generationJobHandle.Complete();

        Mesh resultingMesh = new Mesh();
        resultingMesh.vertices = generationJob.v_position.ToArray();
        resultingMesh.colors = generationJob.v_colors.ToArray();
        resultingMesh.normals = generationJob.v_normals.ToArray();
        resultingMesh.uv = generationJob.v_uvs.ToArray();
        resultingMesh.triangles = generationJob.i_indices.ToArray();

        bounds = new Bounds(new Vector3(worldPosition.x, (generationJob.bounds[0] + generationJob.bounds[1]) / 2, worldPosition.z), new Vector3(worldScale, generationJob.bounds[1] - generationJob.bounds[0], worldScale));

        meshRenderer = gameObject.AddComponent<MeshRenderer>();
        meshRenderer.material = owningLandscape.landscape_material;
        meshRenderer.bounds = bounds;

        // Set mesh
        if (!meshFilter)
            meshFilter = gameObject.AddComponent<MeshFilter>();
        meshFilter.mesh = resultingMesh;

        EndGenerationJob();
    }

    void EndGenerationJob()
    {
        generationJob.shouldAbort = true;
        generationJobHandle.Complete();
        generationJob.DisposeData();
    }

    void requestMeshUpdate()
    {
        // Cancel existing task and wait task completion
        EndGenerationJob();
        if (meshRenderer)
            GameObject.DestroyImmediate(meshRenderer);
        if (meshFilter)
            GameObject.DestroyImmediate(meshFilter);

        // Create and start a new task
        generationJob = new SectionGenerationJob();
        generationJob.meshDensity = owningLandscape.CellsPerChunk;
        generationJob.nodeWorldPosition = worldPosition;
        generationJob.nodeWorldScale = worldScale;
        generationJob.shouldAbort = false;
        generationJob.bounds = new NativeArray<float>(2, Allocator.Persistent);
        int VerticeCount = (owningLandscape.CellsPerChunk + 3) * (owningLandscape.CellsPerChunk + 3) * 3;
        int IndiceCount = (owningLandscape.CellsPerChunk + 2) * (owningLandscape.CellsPerChunk + 2) * 6;
        generationJob.i_indices = new NativeArray<int>(IndiceCount, Allocator.Persistent);
        generationJob.v_position = new NativeArray<Vector3>(VerticeCount, Allocator.Persistent);
        generationJob.v_normals = new NativeArray<Vector3>(VerticeCount, Allocator.Persistent);
        generationJob.v_uvs = new NativeArray<Vector2>(VerticeCount, Allocator.Persistent);
        generationJob.v_colors = new NativeArray<Color>(VerticeCount, Allocator.Persistent);
        generationJobHandle = generationJob.Schedule();
    }

    public void destroy()
    {
        EndGenerationJob();

        ShowCurrentNode();
        UnityEngine.Object.DestroyImmediate(gameObject);
        HeightGenerator.Singleton.OnUpdateRegion.RemoveListener(UpdateRegion);
    }

    private int ComputeDesiredLODLevel()
    {
        // Height correction
        Vector3 cameraGroundLocation = owningLandscape.GetCameraPosition();
        cameraGroundLocation.y -= owningLandscape.GetAltitudeAtLocation(owningLandscape.GetCameraPosition().x, owningLandscape.GetCameraPosition().z);
        float Level = owningLandscape.maxLevel - Math.Min(owningLandscape.maxLevel, (Vector3.Distance(cameraGroundLocation, worldPosition) - worldScale) / owningLandscape.quadtreeExponent);
        return (int)Math.Truncate(Level);
    }
}