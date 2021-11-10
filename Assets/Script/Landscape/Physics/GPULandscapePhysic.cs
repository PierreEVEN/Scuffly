using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

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
    struct CollisionItem
    {
        public Vector2[] points; // Ensemble des points a traiter
        public float[] result; // altitude des points correspondants
        public bool processed; // est-ce que les points ont ete traites
        public int id; // id du group de points
    }
    List<CollisionItem> collisionItems = new List<CollisionItem>();

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

    // Event appele lorsqu'on doit collecter la liste de points a traiter
    public UnityEvent OnPreProcess = new UnityEvent();
    // Event appele une fois que les points ont ete traites
    public UnityEvent OnProcessed = new UnityEvent();

    private void OnDisable()
    {
        if (sendDataBuffer != null)
            sendDataBuffer.Release();
        if (receiveDataBuffer != null)
            receiveDataBuffer.Release();
        sendDataBuffer = null;
        receiveDataBuffer = null;
    }

    public float[] GetPoints(int collisionId)
    {
        foreach (var item in collisionItems)
            if (item.id == collisionId)
                return item.result;
        return null;
    }

    // Regarde si un ID a deja ete utilise
    bool DoesIdExists(int id)
    {
        foreach (var item in collisionItems)
            if (item.id == id)
                return true;
        return false;
    }

    // Enregistre une liste de points a tester
    // Retourne l'id du groupe de collision
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

    // Recupere les donnees traites pour le groupe de points correspondant a l'id indique
    public float[] GetPhysicData(int id)
    {
        foreach (var item in collisionItems)
            if (item.id == id && item.processed)
                return item.result;
        return null;
    }

    // Libere le groupe de points correspondant a l'id
    public void RemoveCollisionItem(int id)
    {
        for (int i = collisionItems.Count - 1; i >= 0; --i)
            if (collisionItems[i].id == id)
                collisionItems.RemoveAt(i);

        if (collisionItems.Count == 0)
            OnDisable();
    }

    // Met a jour la liste des points dont on veut connaitre l'altitude
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

    // Traitement des donnees
    public void ProcessData()
    {
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

        // Mise a jour des donnees a traiter
        OnPreProcess.Invoke();

        // On regroupe la liste de groupes a traiter dans un seul grand vecteur
        List<Vector2> points = new List<Vector2>();
        foreach (var item in collisionItems)
            points.AddRange(item.points);

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

        // On met a jour les donnees a traiter
        sendDataBuffer.SetData(points.ToArray());

        // Traitement des donnees
        int kernelIndex = CSdataProcess.FindKernel("CSMain");
        IModifierGPUArray.UpdateCompute(CSdataProcess, kernelIndex);
        CSdataProcess.SetBuffer(kernelIndex, "Input", sendDataBuffer);
        CSdataProcess.SetBuffer(kernelIndex, "Output", receiveDataBuffer);
        CSdataProcess.Dispatch(kernelIndex, points.Count, 1, 1);

        // On recupere les donnees traitees
        float[] outputData = new float[points.Count];
        receiveDataBuffer.GetData(outputData);

        // On copie les donnees traitees dans chaque groupe correspondant
        int indexCounter = 0;
        foreach (var item in collisionItems)
            for (int i = 0; i < item.result.Length; ++i)
                item.result[i] = outputData[indexCounter++];

        // Mark each item as processed
        for (int i = 0; i < collisionItems.Count; ++i)
        {
            // C# is trash -_-
            var value = collisionItems[i];
            value.processed = true;
            collisionItems[i] = value;
        }

        // On notifie tout le monde que les donnees ont ete traitees
        OnProcessed.Invoke();
    }
}
