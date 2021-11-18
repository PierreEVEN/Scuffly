using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Rendering;


public interface GPULandscapePhysicInterface
{
    public Vector2[] Collectpoints();
    public void OnPointsProcessed(float[] processedPoints);
}


/*
 * Gestion de la physique du terrain
 * ### Principe de fonctionnement ###
 * Probleme : Le terrain est genere sur GPU, le CPU n'a donc aucune connaissance de la topologie pour le traitement de la physique.
 * Solution : On utilise un compute shader pour transferer les informations de topologie du GPU vers le CPU.
 * 
 * Implementation : Pour reduire au maximum les appels GPU, chaque frame on collecte une liste de points dont on veut connaitre l'altitude,
 * puis on traite ces points d'un bloc via un compute shaders, et enfin on retourne l'altitude pour chacun de ces points (CollisionItem)
 * 
 */
public class GPULandscapePhysic
{
    // Un group de points dont on veut connaitre l'altitude.
    List<GPULandscapePhysicInterface> collisionItems = new List<GPULandscapePhysicInterface>();

    // Compute buffer contenant l'ensemble des points a traiter (CPU -> GPU)
    ComputeBuffer sendDataBuffer;
    // Compute buffer contenant l'ensemble des points traites (GPU -> CPU)
    ComputeBuffer receiveDataBuffer;

    // Il ne peut y avoir qu'un seul terrain a la fois dans le moteur, on passe donc par un singleton pour simplifier l'architecture
    private static GPULandscapePhysic singleton;
    public static GPULandscapePhysic Singleton { get {
            if (singleton == null)
                singleton = new GPULandscapePhysic();
            return singleton;
        } 
    }

    // Compute shader qui sera charge de traiter les donnees
    private ComputeShader CSdataProcess;

    float[][] outputBlocks;
    NativeArray<float> dataResult;
    bool canProcessNext = true;

    private void FreeAllocations()
    {
        if (sendDataBuffer != null)
            sendDataBuffer.Release();
        if (receiveDataBuffer != null)
            receiveDataBuffer.Release();
        sendDataBuffer = null;
        receiveDataBuffer = null;

        AsyncGPUReadback.WaitAllRequests();

        if (dataResult.IsCreated)
            dataResult.Dispose();
    }



    // Enregistre une liste de points a tester
    // Retourne l'id du groupe de collision
    public void AddListener(GPULandscapePhysicInterface listener)
    {
        collisionItems.Add(listener);
    }

    // Libere le groupe de points correspondant a l'id
    public void RemoveListener(GPULandscapePhysicInterface listener)
    {
        collisionItems.Remove(listener);
        if (collisionItems.Count == 0)
            FreeAllocations();
    }

    // Traitement des donnees
    public void ProcessData()
    {
        if (!canProcessNext)
            return;

        // On cherche le compute shader a utiliser
        if (!CSdataProcess)
        {
            GameObject landscape = GameObject.FindGameObjectWithTag("GPULandscape");
            if (!landscape)
            {
                Debug.LogError("cannot find any landscape in current scene : required for physics");
                return;
            }


            CSdataProcess = landscape.GetComponentInChildren<GPULandscape>().landscapePhysicGetter;
            if (!CSdataProcess)
            {
                Debug.LogError("cannot find landscape shader physic getter");
                return;
            }
        }

        outputBlocks = new float[collisionItems.Count][];

        // On regroupe la liste de groupes a traiter dans un seul grand vecteur
        List<Vector2> points = new List<Vector2>();
        for (int i = 0; i < collisionItems.Count; ++i) {
            var collectedPoints = collisionItems[i].Collectpoints();
            points.AddRange(collectedPoints);
            outputBlocks[i] = new float[collectedPoints.Length];
        }

        // Rien a traiter
        if (points.Count == 0)
            return;


        // On resize les compute buffer pour qu'ils aient la bonne taille
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

        canProcessNext = false;

        // On met a jour les donnees a traiter
        sendDataBuffer.SetData(points.ToArray());

        // Traitement des donnees
        int kernelIndex = CSdataProcess.FindKernel("CSMain");
        IModifierGPUArray.UpdateCompute(CSdataProcess, kernelIndex);
        CSdataProcess.SetBuffer(kernelIndex, "Input", sendDataBuffer);
        CSdataProcess.SetBuffer(kernelIndex, "Output", receiveDataBuffer);
        CSdataProcess.Dispatch(kernelIndex, points.Count, 1, 1);
        if (dataResult.IsCreated)
            dataResult.Dispose();
        dataResult = new NativeArray<float>(receiveDataBuffer.count, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
        AsyncGPUReadback.RequestIntoNativeArray(ref dataResult, receiveDataBuffer, OnProcessedPhysics);
    }

    void OnProcessedPhysics(AsyncGPUReadbackRequest request)
    {
        NativeArray<float> data = request.GetData<float>();
        // On recupere les donnees traitees
        float[] outputData = new float[data.Length];
        data.CopyTo(outputData);

        // On copie les donnees traitees dans chaque groupe correspondant
        int indexCounter = 0;
        for (int i = 0; i < outputBlocks.Length; ++i)
        {
            for (int p = 0; p < outputBlocks[i].Length; ++p)
                outputBlocks[i][p] = outputData[indexCounter + p];

            indexCounter += outputBlocks[i].Length;

            if (collisionItems.Count == outputBlocks.Length)
                collisionItems[i].OnPointsProcessed(outputBlocks[i]);
        }
        canProcessNext = true;
        dataResult.Dispose();
    }
}
