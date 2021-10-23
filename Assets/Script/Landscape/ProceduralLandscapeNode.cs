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

public struct SectionGenerationJob : IJob
{
    public int meshDensity;
    public float nodeWorldScale;
    public Vector3 nodeWorldPosition;
    public bool shouldAbort;

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

        /* Generate vertices */
        for (int x = 0; x < VerticesPerChunk && !shouldAbort; ++x)
        {
            for (int y = 0; y < VerticesPerChunk && !shouldAbort; ++y)
            {
                float l_PosX = (x - 1) * CellSize + OffsetX;
                float l_posZ = (y - 1) * CellSize + OffsetZ;

                int VertexIndex = (x + y * VerticesPerChunk);

                v_position[VertexIndex] = new Vector3(l_PosX, HeightGenerator.Singleton.GetAltitudeAtLocation(l_PosX, l_posZ), l_posZ);
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
    private VisualEffect vfx;
    private GameObject gameObject; // Owner of node's components
    private List<ProceduralLandscapeNode> children = new List<ProceduralLandscapeNode>();
    private int subdivisionLevel; // Current subdivision level (+1 per subdivision)
    private Vector3 worldPosition;
    private float worldScale;
    private ProceduralLandscape owningLandscape; // Parent
    private SectionGenerationJob generationJob;
    private JobHandle generationJobHandle;

    bool shouldDisplay = false;

    public ProceduralLandscapeNode(ProceduralLandscape inLandscape, int inNodeLevel, Vector3 inPosition, float inScale)
    {
        subdivisionLevel = inNodeLevel;
        worldPosition = inPosition;
        worldScale = inScale;
        owningLandscape = inLandscape;
        gameObject = new GameObject("ProceduralLandscapeNode");
        gameObject.hideFlags = HideFlags.DontSave;
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
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(worldPosition, new Vector3(worldScale, 100, worldScale));
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

        if (!generationJob.v_position.IsCreated)
            return;

        Mesh resultingMesh = new Mesh();
        resultingMesh.vertices = generationJob.v_position.ToArray();
        resultingMesh.colors = generationJob.v_colors.ToArray();
        resultingMesh.normals = generationJob.v_normals.ToArray();
        resultingMesh.uv = generationJob.v_uvs.ToArray();
        resultingMesh.triangles = generationJob.i_indices.ToArray();

        meshRenderer = gameObject.AddComponent<MeshRenderer>();
        meshRenderer.material = owningLandscape.landscape_material;
        meshRenderer.bounds = new Bounds(worldPosition, new Vector3(worldScale, 5000000, worldScale));

        if (owningLandscape.GrassFX)
        {
            vfx = gameObject.AddComponent<VisualEffect>();
            vfx.visualEffectAsset = owningLandscape.GrassFX;
            vfx.SetVector3("BoundCenter", meshRenderer.bounds.center);
            vfx.SetVector3("BoundExtent", meshRenderer.bounds.size);
        }

        // Set mesh
        if (!meshFilter)
            meshFilter = gameObject.AddComponent<MeshFilter>();
        meshFilter.mesh = resultingMesh;

        // Particle system for grass // @TODO improve with a cleaner system
        if (owningLandscape.GrassFX)
        {
            vfx.SetMesh("NewMesh", resultingMesh);
            vfx.Reinit();
        }
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
        if (vfx)
            GameObject.DestroyImmediate(vfx);

        // Create and start a new task
        generationJob = new SectionGenerationJob();
        generationJob.meshDensity = owningLandscape.CellsPerChunk;
        generationJob.nodeWorldPosition = worldPosition;
        generationJob.nodeWorldScale = worldScale;
        generationJob.shouldAbort = false;
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
        gameObject = null;
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