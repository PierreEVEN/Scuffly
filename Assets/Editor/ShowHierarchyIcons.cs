using UnityEditor;
using UnityEngine;

/// <summary>     
/// Hierarchy window game object icon.
/// http://diegogiacomelli.com.br/unitytips-hierarchy-window-gameobject-icon/ 
/// </summary> 
[InitializeOnLoad]
public static class HierarchyWindowGameObjectIcon
{
    static HierarchyWindowGameObjectIcon()
    {
        EditorApplication.hierarchyWindowItemOnGUI += HandleHierarchyWindowItemOnGUI;
    }

    static void HandleHierarchyWindowItemOnGUI(int instanceID, Rect selectionRect)
    {
        var content = EditorGUIUtility.ObjectContent(EditorUtility.InstanceIDToObject(instanceID), null);

        if (content.image != null && !content.image.name.Contains("d_"))
            GUI.DrawTexture(new Rect(selectionRect.xMax - 16, selectionRect.yMin, 16, 16), content.image);
    }
}