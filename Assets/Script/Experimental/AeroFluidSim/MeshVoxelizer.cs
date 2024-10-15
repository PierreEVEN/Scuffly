using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class MeshVoxelizer : MonoBehaviour
{

    public Mesh meshToAnalyze;
    [Range(0.01f, 1)]
    public float voxelStep = 0.1f;
    public bool rebuild = false;

    bool[] voxelData;

    public int xVoxels = 0;
    public int yVoxels = 0;
    public int zVoxels = 0;

    MeshRenderer mesh;
    MeshCollider col;
    MeshFilter filter;

    // Start is called before the first frame update
    void Start()
    {
    }

    int RoundToUpPowerOfTwo(int value)
    {
        value |= value >> 1;
        value |= value >> 2;
        value |= value >> 4;
        value |= value >> 8;
        value |= value >> 16;
        value++;
        return value;
    }

    void GenerateVoxelData()
    {
        voxelData = null;
        if (!meshToAnalyze)
            return;

        if (!mesh)
            mesh = gameObject.AddComponent<MeshRenderer>();
        if (!filter)
            filter = gameObject.AddComponent<MeshFilter>();
        if (!col)
            col = gameObject.AddComponent<MeshCollider>();
        mesh.hideFlags = HideFlags.DontSave;
        filter.hideFlags = HideFlags.DontSave;
        col.hideFlags = HideFlags.DontSave;

        filter.mesh = meshToAnalyze;
        col.sharedMesh = meshToAnalyze;

        xVoxels = Mathf.Max((int)(meshToAnalyze.bounds.size.x / voxelStep), 1);
        yVoxels = Mathf.Max((int)(meshToAnalyze.bounds.size.y / voxelStep), 1);
        zVoxels = Mathf.Max((int)(meshToAnalyze.bounds.size.z / voxelStep), 1);

        voxelData = new bool[xVoxels * yVoxels * zVoxels];

        VoxelizeNode(0, xVoxels - 1, 0, yVoxels - 1, 0, zVoxels - 1);
    }

    void setAreaStatus(int fromX, int toX, int fromY, int toY, int fromZ, int toZ, bool status)
    {
        for (int x = fromX; x <= toX; ++x)
        {
            for (int y = fromY; y <= toY; ++y)
            {
                for (int z = fromZ; z <= toZ; ++z)
                {
                    voxelData[x + y * xVoxels + z * xVoxels * yVoxels] = status;
                }
            }
        }
    }

    private void VoxelizeNode(int fromX, int toX, int fromY, int toY, int fromZ, int toZ)
    {
        bool subdivideX = toX - fromX > 0;
        bool subdivideY = toY - fromY > 0;
        bool subdivideZ = toZ - fromZ > 0;

        Vector3 offset = transform.position + meshToAnalyze.bounds.center - meshToAnalyze.bounds.size / 2 + new Vector3(voxelStep, voxelStep, voxelStep);

        Vector3 center = new Vector3((fromX + toX) / 2f, (fromY + toY) / 2f, (fromZ + toZ) / 2f) * voxelStep + offset;
        Vector3 extent = new Vector3((toX - fromX + 1) * voxelStep, (toY - fromY + 1) * voxelStep, (toZ - fromZ + 1) * voxelStep);

        bool collide = Physics.CheckBox(center, extent / 2.0f);

        if (!subdivideX && !subdivideY && !subdivideZ)
        {
            setAreaStatus(fromX, toX, fromY, toY, fromZ, toZ, collide);
            return;
        }


        int sepXlow = Mathf.FloorToInt((fromX + toX) / 2f);
        int sepYlow = Mathf.FloorToInt((fromY + toY) / 2f);
        int sepZlow = Mathf.FloorToInt((fromZ + toZ) / 2f);

        // (0, 0, 0)
        VoxelizeNode(
            fromX, subdivideX ? sepXlow : toX,
            fromY, subdivideY ? sepYlow : toY,
            fromZ, subdivideZ ? sepZlow : toZ
            );

        if (subdivideX)
        {
            // (1, 0, 0)
            VoxelizeNode(
                sepXlow + 1, toX,
                fromY, subdivideY ? sepYlow : toY,
                fromZ, subdivideZ ? sepZlow : toZ
                );

            // (1, 0, 1)
            if (subdivideZ)
            {
                VoxelizeNode(
                    sepXlow + 1, toX,
                    fromY, subdivideY ? sepYlow : toY,
                    sepZlow + 1, toZ
                    );
            }
            // (1, 1, 0)
            if (subdivideY)
            {
                VoxelizeNode(
                    sepXlow + 1, toX,
                        sepYlow + 1, toY,
                     fromZ, subdivideZ ? sepZlow : toZ
                    );

                // (1, 1, 1)
                if (subdivideZ)
                {
                    VoxelizeNode(
                        sepXlow + 1, toX,
                        sepYlow + 1, toY,
                        sepZlow + 1, toZ
                        );
                }
            }
        }
        if (subdivideY)
        {
            // (0, 1, 0)
            VoxelizeNode(
                fromX, subdivideX ? sepXlow : toX,
                sepYlow + 1, toY,
                fromZ, subdivideZ ? sepZlow : toZ
                );

            if (subdivideZ)
            {
                // (0, 1, 1)
                VoxelizeNode(
                    fromX, subdivideX ? sepXlow : toX,
                    sepYlow + 1, toY,
                        sepZlow + 1, toZ
                    );
            }
        }
        if (subdivideZ)
        {
            // (0, 0, 1)
            VoxelizeNode(
                fromX, subdivideX ? sepXlow : toX,
                fromY, subdivideY ? sepYlow : toY,
                sepZlow + 1, toZ
                );
        }
    }

    private void OnDrawGizmos()
    {
        Vector3 size = new Vector3(voxelStep, voxelStep, voxelStep);
        Vector3 offset = transform.position - meshToAnalyze.bounds.size / 2 + new Vector3(voxelStep, voxelStep, voxelStep);
        if (voxelData == null || xVoxels * yVoxels * zVoxels > voxelData.Length)
            return;
        for (int x = 0; x < xVoxels; ++x)
        {
            for (int y = 0; y < yVoxels; ++y)
            {
                for (int z = 0; z < zVoxels; ++z)
                {
                    Gizmos.color = Color.green;
                    if (voxelData[x + y * xVoxels + z * xVoxels * yVoxels])
                    {
                        Vector3 center = new Vector3(x, y, z) * voxelStep + offset;
                        Vector3 extent = new Vector3(voxelStep, voxelStep, voxelStep);
                        Gizmos.color = Color.red;
                        Gizmos.DrawCube(center, extent);
                    }
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (rebuild)
        {
            rebuild = false;
            GenerateVoxelData();
        }
    }
}
