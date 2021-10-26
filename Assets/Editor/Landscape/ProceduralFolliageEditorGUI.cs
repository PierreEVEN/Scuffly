using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ProceduralFolliageSpawner))]
[CanEditMultipleObjects]

public class ProceduralFolliageEditorGUI : Editor
{
    SerializedProperty LodLevels;
    SerializedProperty ShowBounds;
    SerializedProperty SectionWidth;
    SerializedProperty Radius;
    SerializedProperty Reset;

    float maxDistance;
    void OnEnable()
    {
        Reset = serializedObject.FindProperty("Reset");
        LodLevels = serializedObject.FindProperty("LodLevels");
        ShowBounds = serializedObject.FindProperty("DrawDebugBounds");
        SectionWidth = serializedObject.FindProperty("SectionWidth");
        Radius = serializedObject.FindProperty("Radius");
        if (LodLevels.arraySize != 0)
            maxDistance = (LodLevels.GetArrayElementAtIndex(0).floatValue + 1) * 1.5f;
        else 
            maxDistance = 1000;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        serializedObject.Update();

        EditorGUILayout.Separator();
        EditorGUILayout.LabelField("LOD groups", EditorStyles.boldLabel);

        if (LodLevels.isArray)
        {
            int newSize = EditorGUILayout.IntSlider(LodLevels.arraySize, 0, 10);

            EditorGUILayout.Separator();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("max distance");
            maxDistance = EditorGUILayout.FloatField(maxDistance);
            EditorGUILayout.EndHorizontal();

            while (newSize > LodLevels.arraySize)
            {
                LodLevels.InsertArrayElementAtIndex(LodLevels.arraySize);
                LodLevels.GetArrayElementAtIndex(LodLevels.arraySize - 1).floatValue = 0;
            }
            while (newSize < LodLevels.arraySize)
            {
                LodLevels.DeleteArrayElementAtIndex(LodLevels.arraySize - 1);
            }
            for (int i = 0; i < LodLevels.arraySize; ++i)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PrefixLabel("#" + i);
                LodLevels.GetArrayElementAtIndex(i).floatValue = EditorGUILayout.Slider(LodLevels.GetArrayElementAtIndex(i).floatValue, 0, maxDistance);
                EditorGUILayout.EndHorizontal();
            }


            for (int i = 0; i < LodLevels.arraySize - 1; ++i)
            {
                if (LodLevels.GetArrayElementAtIndex(i).floatValue < LodLevels.GetArrayElementAtIndex(i + 1).floatValue)
                    LodLevels.GetArrayElementAtIndex(i + 1).floatValue = LodLevels.GetArrayElementAtIndex(i).floatValue;
            }
        }
        serializedObject.ApplyModifiedProperties();
    }
}
