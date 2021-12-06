
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Script osed for dial rendering
/// It use a shader material to render graduation, and optionnaly
/// add text values
/// </summary>
[ExecuteInEditMode]
public class MetterRenderer : MonoBehaviour
{
    // The material used
    public Material metterMaterial;

    /// <summary>
    /// Custom per instance material parameters
    /// </summary>
    MaterialPropertyBlock mpb;

    //@TODO : use face primitive instead
    public static Mesh sharedMesh;

    /// <summary>
    /// The number of graduations
    /// </summary>
    [Header("Parameters")]
    public float SubdivisionCount = 20;

    /// <summary>
    /// Min
    /// </summary>
    [Range(0, 360)]
    public float FromDegrees = 0;

    /// <summary>
    /// and Max angles
    /// </summary>
    [Range(0, 360)]
    public float ToDegrees = 360;

    /// <summary>
    /// Width of each graduation (from 0 to 1)
    /// </summary>
    [Range(0, 1)]
    public float Width = 0.1f;

    /// <summary>
    /// Min
    /// </summary>
    [Range(0, 0.5f)]
    public float MinDistance = 0.4f;

    /// <summary>
    /// And max length of each graduation
    /// </summary>
    [Range(0, 0.5f)]
    public float MaxDistance = 0.5f;

    /// <summary>
    /// Global rotation of the dial
    /// </summary>
    [Range(0, 360)]
    public float AngleOffset = 360;

    /// <summary>
    /// Dial graduation color
    /// </summary>
    [ColorUsage(true, true)]
    public Color LineColor = Color.white;

    /// <summary>
    /// optionnaly, you can display values for each graduation
    /// </summary>
    [Header("graduations")]
    public bool RenderGraduations;
    public float initialValue = 0;
    public float GraduationStep = 1;
    public float TextDistance = 30;
    public float TextScale = 1;

    /// <summary>
    /// One gameObject per graduation text
    /// </summary>
    List<GameObject> graduations = new List<GameObject>();

#if UNITY_EDITOR
    bool shouldUpdate = false;
#endif

    // Start is called before the first frame update
    void OnEnable()
    {
        if (!sharedMesh)
        {
            // Generate the mesh where the material is rendered on
            sharedMesh = new Mesh();

            sharedMesh.vertices = new Vector3[]
            {
                new Vector3(-.5f, -.5f, 0),
                new Vector3(.5f, -.5f, 0),
                new Vector3(.5f, .5f, 0),
                new Vector3(-.5f, .5f, 0)
            };

            sharedMesh.uv = new Vector2[]
            {
                new Vector2(0, 0),
                new Vector2(1, 0),
                new Vector2(1, 1),
                new Vector2(0, 1)
            };

            sharedMesh.triangles = new int[]
            {
                0, 2, 1,
                0, 3, 2
            };
        }
        UpdateMaterialProperties();

        SpawnGraduations();

#if UNITY_EDITOR
        SceneView.duringSceneGui += DrawInEditor;
#endif
    }


    private void OnDisable()
    {
#if UNITY_EDITOR
        SceneView.duringSceneGui -= DrawInEditor;
#endif
        // Clean graduations
        foreach (var item in graduations)
            if (Application.isPlaying)
                Destroy(item);
            else
                DestroyImmediate(item);
        graduations.Clear();
    }

    /// <summary>
    /// Create the text graduations
    /// </summary>
    public void SpawnGraduations()
    {
#if UNITY_EDITOR
        shouldUpdate = false;
#endif
        // Destroy the current one
        foreach (var item in graduations)
            DestroyImmediate(item);
        graduations.Clear();

        if (!RenderGraduations)
            return;

        Canvas canvas = GetComponentInChildren<Canvas>();
        if (!canvas) return;

        // Add one text per graduation
        float step = initialValue;
        for (int i = 0; i < SubdivisionCount; ++i)
        {
            float graduation = step;
            step += GraduationStep;
            GameObject grad = new GameObject("SubdivisionText_" + graduation);
            grad.transform.parent = canvas.transform;
            grad.hideFlags = HideFlags.DontSave | HideFlags.HideInHierarchy;

            grad.transform.localScale = new Vector3(TextScale, TextScale, TextScale);
            grad.transform.localPosition = new Vector3(0, 0, 0);
            grad.transform.localRotation = Quaternion.identity;

            Text txt = grad.AddComponent<Text>();
            txt.text = graduation.ToString();
            txt.alignment = TextAnchor.MiddleCenter;
            txt.font = Resources.GetBuiltinResource<Font>("Arial.ttf");

            RectTransform rect = txt.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(30, 30);

            float delta = (i + 1) / ((float)SubdivisionCount);

            delta *= ((ToDegrees - FromDegrees) / 360);
            delta += FromDegrees / 360;

            delta += 0.25f - AngleOffset / 360;
            rect.localPosition = new Vector3(Mathf.Cos(delta * 2 * Mathf.PI) * TextDistance, -Mathf.Sin(delta * 2 * Mathf.PI) * TextDistance, 0);


            graduations.Add(grad);
        }
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        UpdateMaterialProperties();
        shouldUpdate = true;
    }
#endif

    /// <summary>
    /// Update the material properties (just copy this component's parameter to the material)
    /// </summary>
    public void UpdateMaterialProperties()
    {
        if (mpb == null)
            mpb = new MaterialPropertyBlock();

        mpb.SetFloat("_SubdivisionCount", SubdivisionCount);
        mpb.SetFloat("_FromDegrees", FromDegrees);
        mpb.SetFloat("_ToDegrees", ToDegrees);
        mpb.SetFloat("_Width", Width);
        mpb.SetFloat("_MinDistance", MinDistance);
        mpb.SetFloat("_MaxDistance", MaxDistance);
        mpb.SetFloat("_AngleOffset", AngleOffset);
        mpb.SetFloat("_MaxDistance", MaxDistance);
        mpb.SetColor("_Color", LineColor);

    }
#if UNITY_EDITOR
    public void DrawInEditor(SceneView sceneview)
    {
        Graphics.DrawMesh(sharedMesh, transform.localToWorldMatrix, metterMaterial, 0, SceneView.lastActiveSceneView.camera, 0, mpb);
    }
#endif

    // Update is called once per frame
    void Update()
    {
        if (!metterMaterial)
        {
#if UNITY_EDITOR
            // Automatically load the material if not set
            foreach (var asset in AssetDatabase.FindAssets("MetterMaterial"))
            {
                metterMaterial = (Material)AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(asset), typeof(Material));
                break;
            }
            if (!metterMaterial)
#endif
                return;
        }

#if !UNITY_EDITOR
        Graphics.DrawMesh(sharedMesh, transform.localToWorldMatrix, metterMaterial, 0, null, 0, mpb);
#else
        if (Application.isPlaying)
        {
            // Draw mesh manually to use custom material data per instance
            Graphics.DrawMesh(sharedMesh, transform.localToWorldMatrix, metterMaterial, 0, null, 0, mpb);
        }
        if (shouldUpdate)
            SpawnGraduations();
#endif
    }
}
