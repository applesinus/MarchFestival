using UnityEditor;
using UnityEngine;

public class AnchorSnapper
{
    [MenuItem("CONTEXT/RectTransform/Snap Anchors To Bounds")]
    static void SnapAnchors(MenuCommand command)
    {
        RectTransform rect = (RectTransform)command.context;
        if (rect == null || rect.parent == null) return;

        Undo.RecordObject(rect, "Snap Anchors");

        RectTransform parent = rect.parent.GetComponent<RectTransform>();

        Vector2 offsetMin = rect.offsetMin;
        Vector2 offsetMax = rect.offsetMax;
        Vector2 anchorMin = rect.anchorMin;
        Vector2 anchorMax = rect.anchorMax;
        Vector2 parentSize = parent.rect.size;

        anchorMin.x += offsetMin.x / parentSize.x;
        anchorMin.y += offsetMin.y / parentSize.y;
        anchorMax.x += offsetMax.x / parentSize.x;
        anchorMax.y += offsetMax.y / parentSize.y;

        rect.anchorMin = anchorMin;
        rect.anchorMax = anchorMax;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
        
        EditorUtility.SetDirty(rect);
    }
}