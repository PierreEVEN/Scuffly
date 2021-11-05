using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class LandscapeCollider : MonoBehaviour
{
    // Start is called before the first frame update
    GameObject collisionPrefab;
    MeshCollider generatedCollider;

    public int resolutionRadius = 2;
    public float cellWidth = 5.0f;

    void Start()
    {
        CreateMesh();
    }

    private void OnDestroy()
    {
        if (collisionPrefab)
            Destroy(collisionPrefab);
    }

    void CreateMesh()
    {
        if (!collisionPrefab)
        {
            collisionPrefab = new GameObject(gameObject.name + "_landscape_collision");
            collisionPrefab.hideFlags = HideFlags.DontSave | HideFlags.HideInHierarchy;
        }
        if (!generatedCollider)
            generatedCollider = collisionPrefab.AddComponent<MeshCollider>();
    }


    // Update is called once per frame
    void Update()
    {
        int verticeWidth = resolutionRadius * 2 + 1;

        Mesh new_mesh = new Mesh();
        Vector3[] vertices = new Vector3[verticeWidth * verticeWidth];
        int[] triangles = new int[verticeWidth * verticeWidth * 6];

        for (int x = 0; x < verticeWidth; ++x)
        {
            for (int y = 0; y < verticeWidth; ++y)
            {
                vertices[x + y * verticeWidth] = new Vector3(cellWidth * x, 0, cellWidth * y) + new Vector3(transform.position.x - cellWidth * resolutionRadius, 0, transform.position.z - cellWidth * resolutionRadius);
            }
        }

        for (int x = 0; x < verticeWidth - 1; ++x)
        {
            for (int y = 0; y < verticeWidth - 1; ++y)
            {

                int IndiceIndex = (x + y * verticeWidth) * 6;

                triangles[IndiceIndex] = (x + y * verticeWidth);
                triangles[IndiceIndex + 2] = (x + 1 + y * verticeWidth);
                triangles[IndiceIndex + 1] = (x + 1 + (y + 1) * verticeWidth);

                triangles[IndiceIndex + 3] = (x + y * verticeWidth);
                triangles[IndiceIndex + 5] = (x + 1 + (y + 1) * verticeWidth);
                triangles[IndiceIndex + 4] = (x + (y + 1) * verticeWidth);
            }
        }


        if (GPULandscapeCollision.Singleton)
        {
            GPULandscapeCollision.Singleton.GetAltitudeGrid(ref vertices, verticeWidth, cellWidth);
        }
        else
            Debug.LogError("missing landscape collision");



        new_mesh.vertices = vertices;
        new_mesh.triangles = triangles;
        if (!generatedCollider)
            CreateMesh();
        generatedCollider.sharedMesh = new_mesh;
    }
}
