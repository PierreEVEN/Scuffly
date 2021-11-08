using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[ExecuteInEditMode]
public class GPULandscapePhysic : MonoBehaviour
{

    struct CollisionItem
    {
        public Vector2[] points;
        public float[] result;
        public bool processed;
        public int id;
    }

    ComputeBuffer sendDataBuffer;
    ComputeBuffer receiveDataBuffer;

    List<CollisionItem> collisionItems = new List<CollisionItem>();

    private static GPULandscapePhysic singleton;
    public static GPULandscapePhysic Singleton { get {
            return singleton;
        } 
    }

    public ComputeShader textureGridGetterShader;

    public UnityEvent OnPreProcess = new UnityEvent();
    public UnityEvent OnProcessed = new UnityEvent();


    private void OnEnable()
    {
        singleton = this;
    }
    private void Awake()
    {
        singleton = this;
    }
    private void Start()
    {
        singleton = this;
    }

    private void OnDisable()
    {
        if (sendDataBuffer != null)
            sendDataBuffer.Release();
        if (receiveDataBuffer != null)
            receiveDataBuffer.Release();
        sendDataBuffer = null;
        receiveDataBuffer = null;
        singleton = null;
    }

    public float[] GetPoints(int collisionId)
    {
        foreach (var item in collisionItems)
            if (item.id == collisionId)
                return item.result;
        return null;
    }

    bool DoesIdExists(int id)
    {
        foreach (var item in collisionItems)
            if (item.id == id)
                return true;
        return false;
    }

    public int AddCollisionItem(Vector2[] pointList)
    {
        int id = 0;
        while (DoesIdExists(id)) id++;

        collisionItems.Add(new CollisionItem()
        {
            points = pointList,
            result = new float[pointList.Length],
            processed = false,
            id = id
        });
        return id;
    }

    public float[] GetPhysicData(int id)
    {
        foreach (var item in collisionItems)
            if (item.id == id && item.processed)
                return item.result;
        return null;
    }

    public void RemoveCollisionItem(int id)
    {
        for (int i = collisionItems.Count - 1; i >= 0; --i)
            if (collisionItems[i].id == id)
                collisionItems.RemoveAt(i);
    }

    public void UpdateSourcePoints(int id, Vector2[] points)
    {
        for (int i = 0; i < collisionItems.Count; ++i)
        {
            if (collisionItems[i].id == id)
            {
                if (collisionItems[i].points.Length != points.Length)
                {
                    var data = collisionItems[i];
                    data.points = points;
                    collisionItems[i] = data;
                }
                else
                    points.CopyTo(collisionItems[i].points, 0);
            }
        }
    }

    private void Update()
    {
        ProcessData();
    }

    public void ProcessData()
    {
        if (!textureGridGetterShader)
            return;

        OnPreProcess.Invoke();
        // We collect an array of point to process
        List<Vector2> points = new List<Vector2>();
        foreach (var item in collisionItems)
            points.AddRange(item.points);

        if (points.Count == 0)
            return;


        // Update buffers
        if (sendDataBuffer == null || sendDataBuffer.count != points.Count)
        {
            if (sendDataBuffer != null) sendDataBuffer.Dispose();
            sendDataBuffer = new ComputeBuffer(points.Count, sizeof(float) * 2, ComputeBufferType.Structured);
        }

        if (receiveDataBuffer == null || receiveDataBuffer.count != points.Count)
        {
            if (receiveDataBuffer != null) receiveDataBuffer.Dispose();
            receiveDataBuffer = new ComputeBuffer(points.Count, sizeof(float), ComputeBufferType.Structured);
        }

        sendDataBuffer.SetData(points.ToArray());

        // Run compute
        int kernelIndex = textureGridGetterShader.FindKernel("CSMain");

        IModifierGPUArray.UpdateCompute(textureGridGetterShader, kernelIndex);
        textureGridGetterShader.SetBuffer(kernelIndex, "Input", sendDataBuffer);
        textureGridGetterShader.SetBuffer(kernelIndex, "Output", receiveDataBuffer);
        textureGridGetterShader.Dispatch(kernelIndex, points.Count, 1, 1);

        // Retrieve compute result
        float[] outputData = new float[points.Count];
        receiveDataBuffer.GetData(outputData);

        int indexCounter = 0;
        foreach (var item in collisionItems)
        {
            for (int i = 0; i < item.result.Length; ++i)
            {
                item.result[i] = outputData[indexCounter++];
            }
        }

        // Mark each item as processed
        for (int i = 0; i < collisionItems.Count; ++i)
        {
            // C# is trash -_-
            var value = collisionItems[i];
            value.processed = true;
            collisionItems[i] = value;
        }
        OnProcessed.Invoke();
    }
}
