using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Script used to render a scale
/// Every graduation of the scale are generated using a procedural mesh
/// Like the metterRenderer, you can spawn text for each graduation
/// </summary>
[RequireComponent(typeof(CanvasRenderer))]
public class ScaleRenderer : MaskableGraphic
{
    /// <summary>
    /// Number of graduations
    /// </summary>
    [Header("Global Parameters")]
    public int SubdivisionCount = 20;

    /// <summary>
    /// Width of each graduation
    /// </summary>
    [Range(0, 1)]
    public float Width = 0.1f;

    /// <summary>
    /// Does we use value text for each graduation
    /// </summary>
    [Header("Graduations")]
    public bool useTextScale = false;
    public float GraduationStart = 0;
    public float GraduationIncrement = 1;
    public float TextOffset = 0;
    public int fontSize = 14;
    public TextAnchor textAnchor = TextAnchor.MiddleCenter;
    public float TextRotation = 0;

    [SerializeField]
    Texture m_Texture;

    GameObject graduationContainer;

    protected override void OnEnable()
    {
        base.OnEnable();

        BuildGraduations();
    }

    protected override void OnDisable()
    {
        base.OnDisable();

        if (graduationContainer)
            if (Application.isPlaying)
                Destroy(graduationContainer);
            else
                DestroyImmediate(graduationContainer);
    }

    public Texture texture
    {
        get
        {
            return m_Texture;
        }
        set
        {
            if (m_Texture == value)
                return;

            m_Texture = value;
            SetVerticesDirty();
            SetMaterialDirty();
        }
    }
    public override Texture mainTexture
    {
        get
        {
            return m_Texture == null ? s_WhiteTexture : m_Texture;
        }
    }

    /// <summary>
    /// Generate the graduation mesh
    /// </summary>
    /// <param name="vh"></param>
    protected override void OnPopulateMesh(VertexHelper vh)
    {
        // Let's make sure we don't enter infinite loops
        if (SubdivisionCount <= 0)
        {
            SubdivisionCount = 1;
            Debug.LogWarning("SubdivisionCount must be positive number. Setting to 1 to avoid problems.");
        }

        // Clear vertex helper to reset vertices, indices etc.
        vh.Clear();

        Vector2 basePos = -rectTransform.pivot - new Vector2(rectTransform.rect.width / 2, rectTransform.rect.height / 2);

        float relativeWidth = Width * rectTransform.rect.height / SubdivisionCount;

        for (float i = 0; i < rectTransform.rect.height; i += rectTransform.rect.height / SubdivisionCount)
        {
            // Add one rectangle per graduation. i is the y offset of the graduation
            Vector2 pos = basePos + new Vector2(0, i);

            int triangle = vh.currentVertCount;

            UIVertex vert = new UIVertex();
            vert.color = color;

            vert.position = pos + new Vector2(0, -relativeWidth / 2);
            vert.uv0 = vert.position;
            vh.AddVert(vert);

            vert.position = pos + new Vector2(rectTransform.rect.width, -relativeWidth / 2);
            vert.uv0 = vert.position;
            vh.AddVert(vert);

            vert.position = pos + new Vector2(rectTransform.rect.width, relativeWidth / 2);
            vert.uv0 = vert.position;
            vh.AddVert(vert);

            vert.position = pos + new Vector2(0, relativeWidth / 2);
            vert.uv0 = vert.position;
            vh.AddVert(vert);

            vh.AddTriangle(triangle + 0, triangle + 2, triangle + 1);
            vh.AddTriangle(triangle + 3, triangle + 2, triangle + 0);

        }
    }

    bool shouldUpdate = false;
#if UNITY_EDITOR
    protected override void OnValidate()
    {
        base.OnValidate();
        shouldUpdate = true;
    }
#endif

    private void Update()
    {
        if (shouldUpdate)
            BuildGraduations();
    }

    /// <summary>
    /// Generate graduation value text
    /// </summary>
    void BuildGraduations()
    {
        shouldUpdate = false;
        if (graduationContainer)
            if (Application.isPlaying)
                Destroy(graduationContainer);
            else
                DestroyImmediate(graduationContainer);
        graduationContainer = null;

        if (!useTextScale)
            return;

        graduationContainer = new GameObject("graduation container");
        graduationContainer.hideFlags = HideFlags.DontSave | HideFlags.HideInHierarchy;
        graduationContainer.transform.parent = transform;
        graduationContainer.transform.localPosition = Vector3.zero;
        graduationContainer.transform.localRotation = Quaternion.identity;
        graduationContainer.transform.localScale = Vector3.one;

        float gradValue = GraduationStart;

        Vector2 basePos = -rectTransform.pivot - new Vector2(rectTransform.rect.width / 2 + TextOffset, rectTransform.rect.height / 2);

        for (float i = 0; i < rectTransform.rect.height; i += rectTransform.rect.height / SubdivisionCount)
        {
            Vector2 pos = basePos + new Vector2(0, i);

            GameObject gradContainer = new GameObject("graduation_container_" + gradValue);
            gradContainer.hideFlags = HideFlags.DontSave | HideFlags.HideInHierarchy;
            gradContainer.transform.parent = graduationContainer.transform;
            gradContainer.transform.localScale = Vector3.one;
            Text gradText = gradContainer.AddComponent<Text>();
            gradText.text = gradValue.ToString();
            gradText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            gradValue += GraduationIncrement;
            gradText.GetComponent<RectTransform>().localPosition = pos;
            gradText.GetComponent<RectTransform>().sizeDelta = new Vector2(50, 50);
            gradText.fontSize = fontSize;
            gradText.alignment = textAnchor;
            gradText.transform.localRotation = Quaternion.Euler(0, 0, TextRotation);
        }
    }

    protected override void OnRectTransformDimensionsChange()
    {
        base.OnRectTransformDimensionsChange();
        SetVerticesDirty();
        SetMaterialDirty();
    }
}
