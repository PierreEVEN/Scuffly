using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class LandscapeCollider : MonoBehaviour, GPULandscapePhysicInterface
{
    // Start is called before the first frame update
    GameObject collisionPrefab;
    MeshCollider generatedCollider;

    public int resolutionRadius = 2;
    public float cellWidth = 5.0f;

    private int internalRadius;
    private float internalWidth;

    private void OnEnable()
    {
        GPULandscapePhysic.Singleton.AddListener(this);
        CreateMesh();
    }

    private void OnDisable()
    {
        GPULandscapePhysic.Singleton.RemoveListener(this);

        if (collisionPrefab)
            DestroyImmediate(collisionPrefab);
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

    private void Update()
    {
        if (internalRadius != resolutionRadius || internalWidth != cellWidth)
        {
            OnDisable();
            internalRadius = resolutionRadius;
            internalWidth = cellWidth;
            OnEnable();
        }
    }

    public Vector2[] Collectpoints()
    {
        int verticeWidth = internalRadius * 2 + 1;

        Vector2[] points = new Vector2[verticeWidth * verticeWidth];

        for (int x = 0; x < verticeWidth; ++x)
            for (int y = 0; y < verticeWidth; ++y)
                points[x + y * verticeWidth] = new Vector2(internalWidth * x, internalWidth * y) + new Vector2(transform.position.x - internalWidth * internalRadius, transform.position.z - internalWidth * internalRadius);

        return points;
    }

    public void OnPointsProcessed(float[] processedPoints)
    {

        int verticeWidth = internalRadius * 2 + 1;

        Mesh new_mesh = new Mesh();
        Vector3[] vertices = new Vector3[verticeWidth * verticeWidth];
        int[] triangles = new int[verticeWidth * verticeWidth * 6];


        if (processedPoints.Length != verticeWidth * verticeWidth)
            return;

            for (int x = 0; x < verticeWidth; ++x)
            for (int y = 0; y < verticeWidth; ++y)
                vertices[x + y * verticeWidth] = new Vector3(
                    internalWidth * x + transform.position.x - internalWidth * internalRadius,
                    processedPoints[x + y * verticeWidth],
                    internalWidth * y + transform.position.z - internalWidth * internalRadius
                    );

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


        if (vertices.Length < 3)
            return;
        if (vertices[0] == vertices[1] || vertices[1] == vertices[2] || vertices[0] == vertices[2])
            return;

        new_mesh.vertices = vertices;
        new_mesh.triangles = triangles;
        if (!generatedCollider)
            CreateMesh();
        generatedCollider.sharedMesh = new_mesh;
    }
}
