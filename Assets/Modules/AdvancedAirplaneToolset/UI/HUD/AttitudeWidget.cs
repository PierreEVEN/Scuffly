using UnityEngine;
using UnityEngine.UI;

public class AttitudeWidget : MaskableGraphic
{
    [SerializeField]
    Texture m_Texture;

    GameObject graduationContainer;

    protected override void OnEnable()
    {
        base.OnEnable();

        PopulateTexts();
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

    void AddRectangle(VertexHelper vh, Vector2 posA, Vector2 posB, Vector2 PosC, Vector2 posD)
    {
        int triangle = vh.currentVertCount;

        UIVertex vert = new UIVertex();
        vert.color = color;

        vert.position = posA + rectTransform.pivot ;
        vert.uv0 = vert.position;
        vh.AddVert(vert);

        vert.position = posB + rectTransform.pivot;
        vert.uv0 = vert.position;
        vh.AddVert(vert);

        vert.position = PosC + rectTransform.pivot;
        vert.uv0 = vert.position;
        vh.AddVert(vert);

        vert.position = posD + rectTransform.pivot;
        vert.uv0 = vert.position;
        vh.AddVert(vert);

        vh.AddTriangle(triangle + 0, triangle + 2, triangle + 1);
        vh.AddTriangle(triangle + 3, triangle + 2, triangle + 0);
    }

    const float innerWidth = 30;
    const float outterWidth = 75;
    const float bottomOffset = -30;
    const float spacing = 150;
    protected override void OnPopulateMesh(VertexHelper vh)
    {
        // Clear vertex helper to reset vertices, indices etc.
        vh.Clear();

        AddRectangle(vh, new Vector2(-1000, 1), new Vector2(-innerWidth, 1), new Vector2(-innerWidth, -1), new Vector2(-1000, -1));
        AddRectangle(vh, new Vector2(1000, 1), new Vector2(innerWidth, 1), new Vector2(innerWidth, -1), new Vector2(1000, -1));

        for (int i = 1; i < 18; ++i)
        {
            float y = i * spacing;
            AddRectangle(vh, new Vector2(-outterWidth, 1 + y), new Vector2(-innerWidth, 1 + y), new Vector2(-innerWidth, -1 + y), new Vector2(-outterWidth, -1 + y));
            AddRectangle(vh, new Vector2(-innerWidth - 2, -1 + y), new Vector2(-innerWidth, -1 + y), new Vector2(-innerWidth, -6 + y), new Vector2(-innerWidth - 2, -6 + y));

            AddRectangle(vh, new Vector2(outterWidth, 1 + y), new Vector2(innerWidth, 1 + y), new Vector2(innerWidth, -1 + y), new Vector2(outterWidth, -1 + y));
            AddRectangle(vh, new Vector2(innerWidth + 2, -1 + y), new Vector2(innerWidth, -1 + y), new Vector2(innerWidth, -6 + y), new Vector2(innerWidth + 2, -6 + y));
        }

        for (int i = 1; i < 18; ++i)
        {
            float y = i * -spacing;
            float bottomOffsetDelta = bottomOffset * i / 20.0f;

            AddRectangle(vh, new Vector2(-outterWidth, 1 + y + bottomOffsetDelta), new Vector2(-innerWidth, 1 + y), new Vector2(-innerWidth, -1 + y), new Vector2(-outterWidth, -1 + y + bottomOffsetDelta));
            AddRectangle(vh, new Vector2(-innerWidth - 2, 1 + y), new Vector2(-innerWidth, 1 + y), new Vector2(-innerWidth, 6 + y), new Vector2(-innerWidth - 2, 6 + y));

            AddRectangle(vh, new Vector2(outterWidth, 1 + y + bottomOffsetDelta), new Vector2(innerWidth, 1 + y), new Vector2(innerWidth, -1 + y), new Vector2(outterWidth, -1 + y + bottomOffsetDelta));
            AddRectangle(vh, new Vector2(innerWidth + 2, 1 + y), new Vector2(innerWidth, 1 + y), new Vector2(innerWidth, 6 + y), new Vector2(innerWidth + 2, 6 + y));
        }
    }

    void PopulateTexts()
    {
        if (graduationContainer)
            if (Application.isPlaying)
                Destroy(graduationContainer);
            else
                DestroyImmediate(graduationContainer);
        graduationContainer = null;

        graduationContainer = new GameObject("graduation container");
        graduationContainer.hideFlags = HideFlags.DontSave | HideFlags.HideInHierarchy;
        graduationContainer.transform.parent = transform;
        graduationContainer.transform.localPosition = Vector3.zero;
        graduationContainer.transform.localRotation = Quaternion.identity;
        graduationContainer.transform.localScale = Vector3.one;
        
        for (int i = 1; i < 18; ++i)
        {
            int value = i * 5;

            GameObject gradContainerLeft = new GameObject("graduation_container_left" + value);
            gradContainerLeft.hideFlags = HideFlags.DontSave | HideFlags.HideInHierarchy;
            gradContainerLeft.transform.parent = graduationContainer.transform;
            gradContainerLeft.transform.localPosition = Vector3.zero;
            gradContainerLeft.transform.localRotation = Quaternion.identity;
            gradContainerLeft.transform.localScale = Vector3.one;
            Text gradTextLeft = gradContainerLeft.AddComponent<Text>();
            gradTextLeft.text = value.ToString();
            gradTextLeft.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            gradTextLeft.GetComponent<RectTransform>().localPosition = new Vector2(-outterWidth + 5, spacing * i -6);
            gradTextLeft.GetComponent<RectTransform>().sizeDelta = new Vector2(50, 50);
            gradTextLeft.fontSize = 12;
            gradTextLeft.alignment = TextAnchor.MiddleCenter;


            GameObject gradContainerRight = new GameObject("graduation_container_right" + value);
            gradContainerRight.hideFlags = HideFlags.DontSave | HideFlags.HideInHierarchy;
            gradContainerRight.transform.parent = graduationContainer.transform;
            gradContainerRight.transform.localPosition = Vector3.zero;
            gradContainerRight.transform.localRotation = Quaternion.identity;
            gradContainerRight.transform.localScale = Vector3.one;
            Text gradTextRight = gradContainerRight.AddComponent<Text>();
            gradTextRight.text = value.ToString();
            gradTextRight.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            gradTextRight.GetComponent<RectTransform>().localPosition = new Vector2(outterWidth - 5, spacing * i - 6);
            gradTextRight.GetComponent<RectTransform>().sizeDelta = new Vector2(50, 50);
            gradTextRight.fontSize = 12;
            gradTextRight.alignment = TextAnchor.MiddleCenter;
        }


        for (int i = 1; i < 18; ++i)
        {
            int value = i * 5;

            float bottomOffsetDelta = bottomOffset * i / 20.0f;

            GameObject gradContainerLeft = new GameObject("graduation_container_bottom_left" + value);
            gradContainerLeft.hideFlags = HideFlags.DontSave | HideFlags.HideInHierarchy;
            gradContainerLeft.transform.parent = graduationContainer.transform;
            gradContainerLeft.transform.localPosition = Vector3.zero;
            gradContainerLeft.transform.localRotation = Quaternion.identity;
            gradContainerLeft.transform.localScale = Vector3.one;
            Text gradTextLeft = gradContainerLeft.AddComponent<Text>();
            gradTextLeft.text = value.ToString();
            gradTextLeft.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            gradTextLeft.GetComponent<RectTransform>().localPosition = new Vector2(-outterWidth + 5, -spacing * i + 7 + bottomOffsetDelta);
            gradTextLeft.GetComponent<RectTransform>().sizeDelta = new Vector2(50, 50);
            gradTextLeft.fontSize = 12;
            gradTextLeft.alignment = TextAnchor.MiddleCenter;


            GameObject gradContainerRight = new GameObject("graduation_container_bottom_right" + value);
            gradContainerRight.hideFlags = HideFlags.DontSave | HideFlags.HideInHierarchy;
            gradContainerRight.transform.parent = graduationContainer.transform;
            gradContainerRight.transform.localPosition = Vector3.zero;
            gradContainerRight.transform.localRotation = Quaternion.identity;
            gradContainerRight.transform.localScale = Vector3.one;
            Text gradTextRight = gradContainerRight.AddComponent<Text>();
            gradTextRight.text = value.ToString();
            gradTextRight.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            gradTextRight.GetComponent<RectTransform>().localPosition = new Vector2(outterWidth - 5, -spacing * i + 7 + bottomOffsetDelta);
            gradTextRight.GetComponent<RectTransform>().sizeDelta = new Vector2(50, 50);
            gradTextRight.fontSize = 12;
            gradTextRight.alignment = TextAnchor.MiddleCenter;
        }
    }

    protected override void OnRectTransformDimensionsChange()
    {
        base.OnRectTransformDimensionsChange();
        SetVerticesDirty();
        SetMaterialDirty();
    }
}
