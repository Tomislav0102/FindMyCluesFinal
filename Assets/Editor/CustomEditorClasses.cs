using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[CustomEditor(typeof(LocationMileStone))]
public class CustomLocationMilestone : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        LocationMileStone loc = (LocationMileStone)target;
        EditorUtility.SetDirty(loc);

        if (loc.hasCustomRange)
        {
            loc.customRange = EditorGUILayout.Vector2Field("Custom range", loc.customRange);
        }
    }
}

