// DoorSoundEditor.cs
// Created by Alexander Ameye
// Version 1.1.0

using UnityEngine;
using UnityEditor;
using StylesHelper;

[CustomEditor(typeof(DoorSound)), CanEditMultipleObjects]
public class DoorSoundEditor : Editor
{
    int ToolBarIndex;

    SerializedProperty OpeningClipProp, OpeningVolumeProp, OpeningPitchProp, OpeningOffsetProp;
    SerializedProperty OpenedClipProp, OpenedVolumeProp, OpenedPitchProp, OpenedOffsetProp;
    SerializedProperty ClosingClipProp, ClosingVolumeProp, ClosingPitchProp, ClosingOffsetProp;
    SerializedProperty ClosedClipProp, ClosedVolumeProp, ClosedPitchProp, ClosedOffsetProp;
    SerializedProperty LockedCLipProp, LockedVolumeProp, LockedPitchProp, LockedOffsetProp;

    public void OnEnable()
    {
        OpeningClipProp = serializedObject.FindProperty("OpeningClip");
        OpeningVolumeProp = serializedObject.FindProperty("OpeningVolume");
        OpeningPitchProp = serializedObject.FindProperty("OpeningPitch");
        OpeningOffsetProp = serializedObject.FindProperty("OpeningOffset");

        OpenedClipProp = serializedObject.FindProperty("OpenedClip");
        OpenedVolumeProp = serializedObject.FindProperty("OpenedVolume");
        OpenedPitchProp = serializedObject.FindProperty("OpenedPitch");
        OpenedOffsetProp = serializedObject.FindProperty("OpenedOffset");

        ClosingClipProp = serializedObject.FindProperty("ClosingClip");
        ClosingVolumeProp = serializedObject.FindProperty("ClosingVolume");
        ClosingPitchProp = serializedObject.FindProperty("ClosingPitch");
        ClosingOffsetProp = serializedObject.FindProperty("ClosingOffset");

        ClosedClipProp = serializedObject.FindProperty("ClosedClip");
        ClosedVolumeProp = serializedObject.FindProperty("ClosedVolume");
        ClosedPitchProp = serializedObject.FindProperty("ClosedPitch");
        ClosedOffsetProp = serializedObject.FindProperty("ClosedOffset");

        LockedCLipProp = serializedObject.FindProperty("LockedClip");
        LockedVolumeProp = serializedObject.FindProperty("LockedVolume");
        LockedPitchProp = serializedObject.FindProperty("LockedPitch");
        LockedOffsetProp = serializedObject.FindProperty("LockedOffset");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        GUIStyle style = new GUIStyle()
        {
            richText = true
        };

        string[] menuOptions = new string[3];
        menuOptions[0] = "Open";
        menuOptions[1] = "Close";
        menuOptions[2] = "Locked";

        EditorGUILayout.Space();
        ToolBarIndex = GUILayout.Toolbar(ToolBarIndex, menuOptions);

        switch (ToolBarIndex)
        {
            case 0:
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("<b>Opening</b>", style);
                EditorGUILayout.PropertyField(OpeningClipProp, new GUIContent("Clip"));
                EditorGUILayout.PropertyField(OpeningVolumeProp, new GUIContent("Volume"));
                EditorGUILayout.PropertyField(OpeningPitchProp, new GUIContent("Pitch"));
                EditorGUILayout.PropertyField(OpeningOffsetProp, new GUIContent("Delay"));

                EditorGUILayout.Space();
                EditorGUILayout.LabelField("<b>Opened</b>", style);
                EditorGUILayout.PropertyField(OpenedClipProp, new GUIContent("Clip"));
                EditorGUILayout.PropertyField(OpenedVolumeProp, new GUIContent("Volume"));
                EditorGUILayout.PropertyField(OpenedPitchProp, new GUIContent("Pitch"));
                EditorGUILayout.PropertyField(OpenedOffsetProp, new GUIContent("Delay"));

                EditorGUILayout.Space();
                EditorGUILayout.LabelField(Styles.VersionLabel, Styles.centeredVersionLabel);
                break;

            case 1:
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("<b>Closing</b>", style);
                EditorGUILayout.PropertyField(ClosingClipProp, new GUIContent("Clip"));
                EditorGUILayout.PropertyField(ClosingVolumeProp, new GUIContent("Volume"));
                EditorGUILayout.PropertyField(ClosingPitchProp, new GUIContent("Pitch"));
                EditorGUILayout.PropertyField(ClosingOffsetProp, new GUIContent("Delay"));

                EditorGUILayout.Space();
                EditorGUILayout.LabelField("<b>Closed</b>", style);
                EditorGUILayout.PropertyField(ClosedClipProp, new GUIContent("Clip"));
                EditorGUILayout.PropertyField(ClosedVolumeProp, new GUIContent("Volume"));
                EditorGUILayout.PropertyField(ClosedPitchProp, new GUIContent("Pitch"));
                EditorGUILayout.PropertyField(ClosedOffsetProp, new GUIContent("Delay"));

                EditorGUILayout.Space();
                EditorGUILayout.LabelField(Styles.VersionLabel, Styles.centeredVersionLabel);
                break;

            case 2:
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("<b>Locked</b>", style);
                EditorGUILayout.PropertyField(LockedCLipProp, new GUIContent("Clip"));
                EditorGUILayout.PropertyField(LockedVolumeProp, new GUIContent("Volume"));
                EditorGUILayout.PropertyField(LockedPitchProp, new GUIContent("Pitch"));
                EditorGUILayout.PropertyField(LockedOffsetProp, new GUIContent("Delay"));

                EditorGUILayout.Space();
                EditorGUILayout.LabelField(Styles.VersionLabel, Styles.centeredVersionLabel);
                break;
            default:
                break;
        }

        serializedObject.ApplyModifiedProperties();
    }
}