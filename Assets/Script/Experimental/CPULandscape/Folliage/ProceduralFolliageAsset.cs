
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New folliage", order = 82)]
public class ProceduralFolliageAsset : ScriptableObject
{
    public int MinSpawnLevel = 2;
    public int MaxSpawnLevel = 5;

    public int DensityPerLevel = 20;

    public AmplifyImpostors.AmplifyImpostorAsset Impostor;
}
