
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

struct FoliageNodeGenerationJob : IJob
{

    public int max_instance_count;
    public NativeArray<int> generatedElements;
    public NativeArray<float> yBounds;
    public NativeArray<Matrix4x4> instanceTransform;
    public bool shouldAbort;
    public Vector3 nodePosition;
    public float nodeWidth;
    public void Execute()
    {
        bool yBoundInit = false;
        generatedElements[0] = 0;
        uint seed = (uint)(nodePosition.x * nodePosition.y + nodePosition.y * 5.8f);
        Unity.Mathematics.Random rand = new Unity.Mathematics.Random(seed == 0 ? 1 : seed);
        for (int i = 0; i < max_instance_count; ++i)
        {
            float x = nodePosition.x + rand.NextFloat(-nodeWidth / 2, nodeWidth / 2);
            float z = nodePosition.z + rand.NextFloat(-nodeWidth / 2, nodeWidth / 2);
            float y = HeightGenerator.Singleton.GetAltitudeAtLocation(x, z);
            if (y < 10)
                continue;
            if (x > -1500 && x < 1700 && z > -150 && z < 500)
                continue;

            if (!yBoundInit || y < yBounds[0])
                yBounds[0] = y;

            if (!yBoundInit || y > yBounds[1])
                yBounds[1] = y;

            yBoundInit = true;

            float scale = 1.0f;// rand.NextFloat(0.5f, 2.0f);
            instanceTransform[generatedElements[0]] = Matrix4x4.TRS(new Vector3(x, y, z), Quaternion.identity, new Vector3(scale, scale, scale));
            generatedElements[0]++;
        }
    }

    public void DisposeData()
    {
        if (instanceTransform.IsCreated)
            instanceTransform.Dispose();
        if (generatedElements.IsCreated)
            generatedElements.Dispose();
        if (yBounds.IsCreated)
            yBounds.Dispose();
    }
}

[ExecuteInEditMode]
public class ProceduralFolliageBatch : MonoBehaviour
{
    private FoliageNodeGenerationJob generationJob;
    private JobHandle generationJobHandle;

    private ProceduralFolliageAsset folliageAsset;
    Bounds bounds;

    ComputeBuffer bufferWithArgs;
    ComputeBuffer matrixBuffer;
    private uint[] args = new uint[5] { 0, 0, 0, 0, 0 };
    ProceduralFolliageNode folliageParent;

    MaterialPropertyBlock MPB;
    public void Start()
    {
    }

    public void Init(ProceduralFolliageNode parent, ProceduralFolliageAsset asset)
    {
        folliageParent = parent;
        folliageAsset = asset;
        args[0] = (uint)folliageAsset.Impostor.Mesh.GetIndexCount(0);
        args[2] = (uint)folliageAsset.Impostor.Mesh.GetIndexStart(0);
        args[3] = (uint)folliageAsset.Impostor.Mesh.GetBaseVertex(0);
        GenerateBatch();
    }

    private void Update()
    {
        if (folliageAsset == null || folliageAsset.Impostor == null)
            return;

        if (generationJobHandle.IsCompleted && matrixBuffer == null)
        {
            generationJobHandle.Complete();
            if (generationJob.generatedElements[0] == 0)
                return;

            matrixBuffer = new ComputeBuffer(generationJob.generatedElements[0], sizeof(float) * 16);
            matrixBuffer.SetData(generationJob.instanceTransform.GetSubArray(0, generationJob.generatedElements[0]));
            bounds = new Bounds(new Vector3(folliageParent.nodePosition.x, (generationJob.yBounds[0] + generationJob.yBounds[1]) / 2, folliageParent.nodePosition.z), new Vector3(folliageParent.nodeWidth, generationJob.yBounds[1] - generationJob.yBounds[0], folliageParent.nodeWidth));
            args[1] = (uint)generationJob.generatedElements[0];
            MPB = new MaterialPropertyBlock();
            MPB.SetBuffer("matrixBuffer", matrixBuffer);
            EndBatchGeneration();
        }

        if (bufferWithArgs == null)
        {
            bufferWithArgs = new ComputeBuffer(1, args.Length * sizeof(uint), ComputeBufferType.IndirectArguments);
            bufferWithArgs.SetData(args);
        }

        Graphics.DrawMeshInstancedIndirect(folliageAsset.Impostor.Mesh, 0, folliageAsset.Impostor.Material, bounds, bufferWithArgs, 0, MPB);
    }

    public void OnDisable()
    {
        if (matrixBuffer != null)
            matrixBuffer.Dispose();
        if (bufferWithArgs != null)
            bufferWithArgs.Dispose();
        EndBatchGeneration();
    }

    void GenerateBatch()
    {
        EndBatchGeneration();

        generationJob = new FoliageNodeGenerationJob();
        generationJob.max_instance_count = folliageAsset.DensityPerLevel;
        generationJob.nodePosition = folliageParent.nodePosition;
        generationJob.nodeWidth = folliageParent.nodeWidth;
        generationJob.generatedElements = new NativeArray<int>(1, Allocator.Persistent);
        generationJob.yBounds = new NativeArray<float>(2, Allocator.Persistent);
        generationJob.instanceTransform = new NativeArray<Matrix4x4>(folliageAsset.DensityPerLevel, Allocator.Persistent);
        generationJob.shouldAbort = false;
        generationJobHandle = generationJob.Schedule();
    }

    void EndBatchGeneration()
    {
        generationJob.shouldAbort = true;
        generationJobHandle.Complete();
        generationJob.DisposeData();
    }
}