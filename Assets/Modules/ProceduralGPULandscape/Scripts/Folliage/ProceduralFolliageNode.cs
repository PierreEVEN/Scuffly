
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A foliage quadtree node
/// </summary>
public class ProceduralFolliageNode
{
    ProceduralFolliageNode[] children = new ProceduralFolliageNode[0];
    List<GameObject> batches = new List<GameObject>();

    /// <summary>
    /// The owning procedural foliage
    /// </summary>
    public ProceduralFolliageSpawner folliageSpawner;

    /// <summary>
    /// The subdivision level of this node
    /// </summary>
    int lodLevel;

    /// <summary>
    /// The world width in m
    /// </summary>
    public float nodeWidth;

    /// <summary>
    /// The position in the world of this node
    /// </summary>
    public Vector3 nodePosition;
    
    public ProceduralFolliageNode(ProceduralFolliageSpawner spawner, Vector3 position, int level, float width)
    {
        folliageSpawner = spawner;
        lodLevel = level;
        nodeWidth = width;
        nodePosition = position;

        //@TODO don't generate all kind of foliage everywhere
        // Spawn one foliage batch for this node level (one per foliage asset)
        foreach (var asset in spawner.foliageAssets)
        {
            if (lodLevel < asset.MinSpawnLevel || lodLevel > asset.MaxSpawnLevel)
                continue;

            GameObject container = new GameObject("procedural_folliage_" + asset + "_level_" + lodLevel);
            container.transform.parent = spawner.transform;
            container.hideFlags = HideFlags.DontSave | HideFlags.HideInHierarchy;

            ProceduralFolliageBatch batch = container.AddComponent<ProceduralFolliageBatch>();
            batch.folliageParent = this;
            batch.folliageAsset = asset;
            batches.Add(container);
        }
    }

    public void UpdateInternal()
    {
        // If we can still subdivide into sublevels
        if (lodLevel < folliageSpawner.LodLevels.Count)
        {
            // Get the next subdivision threshold
            float subdivisionDistance = folliageSpawner.LodLevels[lodLevel]; //@TODO Use camera relative altitude
            float currentDistance = Vector3.Distance(folliageSpawner.CameraPosition, nodePosition + new Vector3(0, 0, 0)) - nodeWidth / 2;

            // Check if we should subdivide this node
            if (currentDistance < subdivisionDistance)
                Subdivide();
            else
                Unsubdivide();
        }
        else
            Unsubdivide();

        // Update children
        foreach (var child in children)
            child.UpdateInternal();
    }

    void Subdivide()
    {
        // Spawn children LOD levels if not already created
        if (children.Length != 0)
            return;

        children = new ProceduralFolliageNode[] {
            new ProceduralFolliageNode(folliageSpawner, new Vector3(nodePosition.x - nodeWidth / 4, 0, nodePosition.z - nodeWidth / 4), lodLevel + 1, nodeWidth / 2),
            new ProceduralFolliageNode(folliageSpawner, new Vector3(nodePosition.x + nodeWidth / 4, 0, nodePosition.z - nodeWidth / 4), lodLevel + 1, nodeWidth / 2),
            new ProceduralFolliageNode(folliageSpawner, new Vector3(nodePosition.x + nodeWidth / 4, 0, nodePosition.z + nodeWidth / 4), lodLevel + 1, nodeWidth / 2),
            new ProceduralFolliageNode(folliageSpawner, new Vector3(nodePosition.x - nodeWidth / 4, 0, nodePosition.z + nodeWidth / 4), lodLevel + 1, nodeWidth / 2),
        };
    }

    void Unsubdivide()
    {
        //Destroy all the children nodes recursivly
        if (children.Length == 0)
            return;

        foreach (var child in children)
            child.DestroyNode();
        children = new ProceduralFolliageNode[0];
    }


    public void DrawDebug()
    {
        if (children.Length == 0)
        {
            Gizmos.color = new Color((lodLevel * 10.897f + 5.4f) % 1.0f, (lodLevel * 8.4287f + 3.77f) % 1.0f, (lodLevel * 3.879f + 1.2f) % 1.0f, 0.5f);
            Gizmos.DrawCube(nodePosition, new Vector3(nodeWidth, nodeWidth, nodeWidth));
        }
        foreach (var child in children)
            child.DrawDebug();
    }

    /// <summary>
    /// Destroy the current quadtree node with it's children
    /// </summary>

    public void DestroyNode()
    {
        foreach (var batch in batches)
            GameObject.DestroyImmediate(batch);

        foreach (var child in children)
            child.DestroyNode();
    }
}
