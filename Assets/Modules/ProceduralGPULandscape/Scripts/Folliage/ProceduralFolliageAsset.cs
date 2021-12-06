
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A foliage asset is spawned using a procedural foliage spawner
/// </summary>
[CreateAssetMenu(fileName = "New folliage", order = 82)]
public class ProceduralFolliageAsset : ScriptableObject
{
    /// <summary>
    /// Min and max spawn LOD level
    /// </summary>
    public int MinSpawnLevel = 2;
    public int MaxSpawnLevel = 5;

    /// <summary>
    /// max number of spawned foliage instance = density x density
    /// </summary>
    public int DensityPerLevel = 20;

    /// <summary>
    /// Min and max spawn ground normal (y axis)
    /// </summary>
    public float minNormal = 0;
    public float maxNormal = 0.95f;

    /// <summary>
    /// Min and max spawn altitude
    /// </summary>
    public float minAltitude = 10;
    public float maxAltitude = 3000;

    /// <summary>
    /// The foliage instance mesh
    /// </summary>
    public Mesh spawnedMesh;

    /// <summary>
    /// The foliage rendering material (it should handle the instance matrix)
    /// </summary>
    public Material usedMaterial;

    private void OnValidate()
    {
        GameObject landscape = GameObject.FindGameObjectWithTag("GPULandscape");
        if (!landscape)
            return;
        ProceduralFolliageSpawner folliage = landscape.GetComponentInChildren<ProceduralFolliageSpawner>();
        if (!folliage)
            return;
        folliage.Reset = true;
    }
}
