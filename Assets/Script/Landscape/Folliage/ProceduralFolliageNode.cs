
using System.Collections.Generic;
using UnityEngine;


public class ProceduralFolliageNode
{
    ProceduralFolliageNode[] children = new ProceduralFolliageNode[0];
    List<GameObject> batches = new List<GameObject>();

    public ProceduralFolliageSpawner folliageSpawner;
    int lodLevel;
    public float nodeWidth;
    public Vector3 nodePosition;
    
    public ProceduralFolliageNode(ProceduralFolliageSpawner spawner, Vector3 position, int level, float width)
    {
        folliageSpawner = spawner;
        lodLevel = level;
        nodeWidth = width;
        nodePosition = position;

        //@TODO don't generate all kind of foliage everywhere
        foreach (var asset in spawner.foliageAssets)
        {
            GameObject container = new GameObject("procedural_folliage_" + asset + "_level_" + lodLevel);
            container.transform.parent = spawner.transform;
            container.hideFlags = HideFlags.DontSave;

            ProceduralFolliageBatch batch = container.AddComponent<ProceduralFolliageBatch>();
            batch.folliageParent = this;
            batch.folliageAsset = asset;
            batches.Add(container);
        }
    }

    public void UpdateInternal()
    {
        if (lodLevel < folliageSpawner.LodLevels.Count)
        {
            float subdivisionDistance = folliageSpawner.LodLevels[lodLevel];
            float currentDistance = Vector3.Distance(folliageSpawner.CameraPosition, nodePosition + new Vector3(0, HeightGenerator.Singleton.GetAltitudeAtLocation(nodePosition.x, nodePosition.z), 0)) - nodeWidth / 2;

            if (currentDistance < subdivisionDistance)
                Subdivide();
            else
                Unsubdivide();
        }
        else
            Unsubdivide();

        foreach (var child in children)
            child.UpdateInternal();
    }

    void Subdivide()
    {
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


    public void DestroyNode()
    {
        foreach (var batch in batches)
            GameObject.DestroyImmediate(batch);

        foreach (var child in children)
            child.DestroyNode();
    }
}
