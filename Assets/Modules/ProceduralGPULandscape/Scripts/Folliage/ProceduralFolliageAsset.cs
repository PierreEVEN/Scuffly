
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New folliage", order = 82)]
public class ProceduralFolliageAsset : ScriptableObject
{
    public int MinSpawnLevel = 2;
    public int MaxSpawnLevel = 5;

    public int DensityPerLevel = 20;


    public float minNormal = 0;
    public float maxNormal = 0.95f;

    public float minAltitude = 10;
    public float maxAltitude = 3000;

    public Mesh spawnedMesh;
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
