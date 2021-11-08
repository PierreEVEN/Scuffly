using UnityEngine;

[ExecuteInEditMode]
public class TempFoliageSpawner : MonoBehaviour
{
    public AmplifyImpostors.AmplifyImpostorAsset impostorAsset;
    public Bounds bounds = new Bounds(new Vector3(0, 0, 0), new Vector3(10000, 10000, 10000));
    private ComputeBuffer positionBuffer;
    private ComputeBuffer argsBuffer;
    public bool rebuild = false;

    public float spacing = 20;
    public int width = 100;

    private uint[] args = new uint[5] { 0, 0, 0, 0, 0 };

    // Start is called before the first frame update
    void Start()
    {
        UpdateBfr();
    }

    void UpdateBfr()
    {
        if (!impostorAsset)
            return;

        if (positionBuffer != null)
            positionBuffer.Release();

        int inst_count = width * width;

        positionBuffer = new ComputeBuffer(inst_count, 16);

        Vector4[] positions = new Vector4[inst_count];
        for (int x = 0; x < width; ++x)
        {
            for (int y = 0; y < width; ++y)
            {
                float posx = x * spacing + Random.Range(-spacing / 2, spacing / 2);
                float posy = y * spacing + Random.Range(-spacing / 2, spacing / 2);


                positions[x + y * width] = new Vector4(posx, HeightGenerator.Singleton.GetAltitudeAtLocation(posx, posy), posy, 0);
            }
        }
        positionBuffer.SetData(positions);
        impostorAsset.Material.SetBuffer("positionBuffer", positionBuffer);




        if (argsBuffer != null)
            argsBuffer.Release();

        argsBuffer = new ComputeBuffer(1, args.Length * sizeof(uint), ComputeBufferType.IndirectArguments);
        args[0] = (uint)impostorAsset.Mesh.GetIndexCount(0);
        args[1] = (uint)inst_count;
        args[2] = (uint)impostorAsset.Mesh.GetIndexStart(0);
        args[3] = (uint)impostorAsset.Mesh.GetBaseVertex(0);

        argsBuffer.SetData(args);

        Debug.Log("created buffer with " + inst_count + " instances");
    }

    // Update is called once per frame
    void Update()
    {
        if (positionBuffer == null || rebuild)
        {
            rebuild = false;
            UpdateBfr();
        }
        Graphics.DrawMeshInstancedIndirect(impostorAsset.Mesh, 0, impostorAsset.Material, bounds, argsBuffer);
    }

    private void OnDisable()
    {
        if (positionBuffer != null)
            positionBuffer.Release();
        if (argsBuffer != null)
            argsBuffer.Release();

        positionBuffer = null;
        argsBuffer = null;
    }
}