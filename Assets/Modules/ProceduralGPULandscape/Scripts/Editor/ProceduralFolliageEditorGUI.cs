#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;

/// <summary>
/// Custom UI for the LOD configuration of the procedural folliage component
/// </summary>
[CustomEditor(typeof(ProceduralFolliageSpawner))]
[CanEditMultipleObjects]
public class ProceduralFolliageEditorGUI : Editor
{
    SerializedProperty LodLevels;
    float maxDistance;
    void OnEnable()
    {
        LodLevels = serializedObject.FindProperty("LodLevels");
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

            // if desired array size > current array size : add a new default element to the array
            while (newSize > LodLevels.arraySize)
            {
                LodLevels.InsertArrayElementAtIndex(LodLevels.arraySize);
                LodLevels.GetArrayElementAtIndex(LodLevels.arraySize - 1).floatValue = 0;
            }
            // if desired array size < current array size : revemove the last element from the array
            while (newSize < LodLevels.arraySize)
            {
                LodLevels.DeleteArrayElementAtIndex(LodLevels.arraySize - 1);
            }
            // Draw a slider for each LOD level
            for (int i = 0; i < LodLevels.arraySize; ++i)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PrefixLabel("#" + i);
                LodLevels.GetArrayElementAtIndex(i).floatValue = EditorGUILayout.Slider(LodLevels.GetArrayElementAtIndex(i).floatValue, 0, maxDistance);
                EditorGUILayout.EndHorizontal();
            }

            // ensure the next LOD level value is less than the previous one
            for (int i = 0; i < LodLevels.arraySize - 1; ++i)
            {
                if (LodLevels.GetArrayElementAtIndex(i).floatValue < LodLevels.GetArrayElementAtIndex(i + 1).floatValue)
                    LodLevels.GetArrayElementAtIndex(i + 1).floatValue = LodLevels.GetArrayElementAtIndex(i).floatValue;
            }
        }
        serializedObject.ApplyModifiedProperties();
    }
}
#endif