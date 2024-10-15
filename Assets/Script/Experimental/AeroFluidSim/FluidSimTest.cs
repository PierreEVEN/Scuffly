
using UnityEngine;

[ExecuteInEditMode]
public class FluidSimTest : MonoBehaviour
{
    public ComputeShader computeShader;
    private ComputeShader _computeShader;
    int kernelIndex;

    public bool reinitBase = false;

    public Material debugMaterial;

    RenderTexture computeTextureA;
    RenderTexture computeTextureB;
    RenderTexture debugTexture;

    GameObject child;

    // Start is called before the first frame update
    void OnEnable()
    {
        Init();
    }

    int CubeSize = 100;

    void Init()
    {
        kernelIndex = _computeShader.FindKernel("CSMain");
        computeTextureA = new RenderTexture(CubeSize, CubeSize, 0, RenderTextureFormat.ARGBFloat);
        computeTextureA.volumeDepth = CubeSize;
        computeTextureA.dimension = UnityEngine.Rendering.TextureDimension.Tex3D;
        computeTextureA.enableRandomWrite = true;
        computeTextureA.Create();

        computeTextureB = new RenderTexture(CubeSize, CubeSize, 0, RenderTextureFormat.ARGBFloat);
        computeTextureB.volumeDepth = CubeSize;
        computeTextureB.depthStencilFormat = UnityEngine.Experimental.Rendering.GraphicsFormat.None;
        computeTextureB.dimension = UnityEngine.Rendering.TextureDimension.Tex3D;
        computeTextureB.enableRandomWrite = true;
        computeTextureB.Create();


        debugTexture = new RenderTexture(CubeSize, CubeSize, 0, RenderTextureFormat.ARGB32);
        debugTexture.dimension = UnityEngine.Rendering.TextureDimension.Tex2D;
        debugTexture.enableRandomWrite = true;
        debugTexture.Create();
    }

    private void UpdateTexture()
    {
        if (!_computeShader)
            return;

        if (!debugTexture || !computeTextureA || !computeTextureB)
            Init();

        _computeShader.SetTexture(kernelIndex, "Debug", debugTexture);
        _computeShader.SetTexture(kernelIndex, "From", computeTextureA);
        _computeShader.SetTexture(kernelIndex, "To", computeTextureB);
        _computeShader.SetBool("INIT", reinitBase);
        _computeShader.SetFloat("Time", Time.time);

        _computeShader.Dispatch(kernelIndex, CubeSize, CubeSize, CubeSize);

        // Flip buffers
        var temp = computeTextureA;
        computeTextureA = computeTextureB;
        computeTextureB = temp;
    }


    void UpdateGlob()
    {
        if (computeShader != _computeShader)
        {
            _computeShader = computeShader;
            if (_computeShader)
                Init();
        }

        UpdateTexture();

        if (debugMaterial)
        {
            debugMaterial.SetTexture("_ResultTexture", debugTexture);

            if (!child)
            {
                child = new GameObject("child");
                child.transform.parent = transform;
                child.hideFlags = HideFlags.DontSave;
            }
            MeshRenderer mesh = child.GetComponent<MeshRenderer>();
            if (mesh)
            {
                mesh.material = debugMaterial;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        UpdateGlob();
    }

    private void OnDrawGizmos()
    {
        UpdateGlob();
    }
}
