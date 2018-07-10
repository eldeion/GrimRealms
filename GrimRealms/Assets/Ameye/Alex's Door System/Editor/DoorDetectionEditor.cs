// DoorDetectionEditor.cs
// Created by Alexander Ameye
// Version 1.1.0

using UnityEngine;
using UnityEditor;
using StylesHelper;

[CustomEditor(typeof(DoorDetection))]
public class DoorDetectionEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DoorDetection DoorDetection = target as DoorDetection;

        GUIStyle style = new GUIStyle()
        {
            richText = true
        };

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("<b>UI Settings</b>", style);
        DoorDetection.LookingAtPrefab = (GameObject)EditorGUILayout.ObjectField("Looking at", DoorDetection.LookingAtPrefab, typeof(GameObject), true);
        DoorDetection.InTriggerZoneLookingAtPrefab = (GameObject)EditorGUILayout.ObjectField("In zone", DoorDetection.InTriggerZoneLookingAtPrefab, typeof(GameObject), true);
        DoorDetection.CrosshairPrefab = (GameObject)EditorGUILayout.ObjectField("Crosshair Prefab", DoorDetection.CrosshairPrefab, typeof(GameObject), true);

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("<b>Raycast Settings</b>", style);
        DoorDetection.Reach = EditorGUILayout.FloatField("Reach", DoorDetection.Reach);
        DoorDetection.DebugRay = EditorGUILayout.Toggle("Debug Ray", DoorDetection.DebugRay);
        if (DoorDetection.DebugRay)
        {
            DoorDetection.DebugRayColor = EditorGUILayout.ColorField("Color", DoorDetection.DebugRayColor);
            DoorDetection.DebugRayColorAlpha = EditorGUILayout.Slider("Opacity", DoorDetection.DebugRayColorAlpha, 0, 1);
            DoorDetection.DebugRayColor.a = DoorDetection.DebugRayColorAlpha;
        }

        EditorGUILayout.Space();
        EditorGUILayout.LabelField(Styles.VersionLabel, Styles.centeredVersionLabel);
    }
}
