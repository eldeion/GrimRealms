// DoorTriggerEditor.cs
// Created by Alexander Ameye
// Version 1.1.0

using UnityEngine;
using UnityEditor;
using StylesHelper;

[CustomEditor(typeof(DoorTrigger)), CanEditMultipleObjects]
public class DoorTriggerEditor : Editor
{
    int ToolBarIndex;

    private DoorDetection doordetection;
    private DoorTrigger doortrigger;

    SerializedProperty colliderTypeProp;

    SerializedProperty hasTagProp, hasNameProp, isLookingAtProp, hasPressedProp, hasScriptProp, isGameObjectProp;
    SerializedProperty tagProp, nameProp, charProp;
    SerializedProperty lookObjectProp, objectProp;
    SerializedProperty scriptProp;

    SerializedProperty drawGizmoProp, drawWireProp;
    SerializedProperty customGizmoColorProp, customWireColorProp;
    SerializedProperty customGizmoAlphaProp, customWireAlphaProp;

    void OnEnable()
    {
        colliderTypeProp = serializedObject.FindProperty("ColliderType");

        hasTagProp = serializedObject.FindProperty("HasTag");
        hasNameProp = serializedObject.FindProperty("HasName");
        isLookingAtProp = serializedObject.FindProperty("IsLookingAt");
        hasPressedProp = serializedObject.FindProperty("HasPressed");
        hasScriptProp = serializedObject.FindProperty("HasScript");
        isGameObjectProp = serializedObject.FindProperty("IsGameObject");

        tagProp = serializedObject.FindProperty("playerTag");
        nameProp = serializedObject.FindProperty("playerName");
        charProp = serializedObject.FindProperty("character");
        lookObjectProp = serializedObject.FindProperty("lookObject");
        scriptProp = serializedObject.FindProperty("script");
        objectProp = serializedObject.FindProperty("isObject");

        drawGizmoProp = serializedObject.FindProperty("DrawGizmo");
        drawWireProp = serializedObject.FindProperty("DrawWire");
        customGizmoColorProp = serializedObject.FindProperty("CustomGizmoColor");
        customWireColorProp = serializedObject.FindProperty("CustomWireColor");
        customGizmoAlphaProp = serializedObject.FindProperty("CustomGizmoColorAlpha");
        customWireAlphaProp = serializedObject.FindProperty("CustomWireColorAlpha");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        doortrigger = target as DoorTrigger;
        doordetection = GameObject.FindGameObjectWithTag("Player").GetComponent<DoorDetection>();

        GUIStyle style = new GUIStyle()
        {
            richText = true
        };
        string[] menuOptions = new string[2];
        menuOptions[0] = "Trigger";
        menuOptions[1] = "Gizmo";

        EditorGUILayout.Space();
        ToolBarIndex = GUILayout.Toolbar(ToolBarIndex, menuOptions);

        switch (ToolBarIndex)
        {
            case 0:
                EditorGUIUtility.labelWidth = 70;

                EditorGUILayout.Space();
                EditorGUILayout.LabelField("<b>Trigger Zone</b>", style);
                EditorGUILayout.PropertyField(colliderTypeProp, new GUIContent("Shape"));

                EditorGUILayout.Space();
                EditorGUILayout.LabelField("<b>Player Requirements</b>", style);

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PropertyField(hasTagProp, new GUIContent("Tag"));
                if (doortrigger.HasTag)
                    EditorGUILayout.PropertyField(tagProp, GUIContent.none);
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PropertyField(hasNameProp, new GUIContent("Name"));
                if (doortrigger.HasName)
                    EditorGUILayout.PropertyField(nameProp, GUIContent.none);
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PropertyField(isLookingAtProp, new GUIContent("Looking At"));
                if (doortrigger.IsLookingAt)
                    EditorGUILayout.PropertyField(lookObjectProp, GUIContent.none);
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PropertyField(hasPressedProp, new GUIContent("Pressed"));
                if (doortrigger.HasPressed)
                    EditorGUILayout.PropertyField(charProp, GUIContent.none);
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PropertyField(hasScriptProp, new GUIContent("Script"));
                if (doortrigger.HasScript)
                    EditorGUILayout.PropertyField(scriptProp, GUIContent.none);
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PropertyField(isGameObjectProp, new GUIContent("Is Object"));
                if (doortrigger.IsGameObject)
                    EditorGUILayout.PropertyField(objectProp, GUIContent.none);
                EditorGUILayout.EndHorizontal();

                if (AnyEmptyFields())
                    EditorGUILayout.HelpBox("One or more fields have been left empty.", MessageType.Warning);

                if (doortrigger.IsLookingAt && doordetection.Reach == 0 && doortrigger.IsLookingAt && doortrigger.lookObject != null)
                    EditorGUILayout.HelpBox("The reach of your player is zero.", MessageType.Warning);

                EditorGUILayout.Space();
                GUI.color = Color.green;

                if (GUILayout.Button("Add " + doortrigger.transform.gameObject.name))
                {
                    GameObject Trigger = new GameObject(doortrigger.transform.gameObject.name);
                    GameObject RotationParent = doortrigger.transform.parent.gameObject;

                    Trigger.transform.position = doortrigger.transform.position;
                    Trigger.transform.localScale = doortrigger.transform.localScale;
                    Trigger.transform.rotation = doortrigger.transform.rotation;

                    SetParentChild(RotationParent, Trigger);
                    Trigger.AddComponent<DoorTrigger>();
                    Trigger.GetComponent<DoorTrigger>().ID = doortrigger.ID;
                    Trigger.GetComponent<DoorTrigger>().CustomGizmoColor = doortrigger.CustomGizmoColor;
                    Trigger.GetComponent<DoorTrigger>().ColliderType = doortrigger.ColliderType;
                }
                GUI.color = Color.white;

                EditorGUILayout.Space();
                EditorGUILayout.LabelField(Styles.VersionLabel, Styles.centeredVersionLabel);
                break;

            case 1:
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("<b>Trigger Zone</b>", style);
                EditorGUILayout.PropertyField(drawGizmoProp, new GUIContent("Draw Trigger Zone"));

                if (doortrigger.DrawGizmo)
                {
                    EditorGUILayout.PropertyField(customGizmoColorProp, new GUIContent("Color"));
                    EditorGUILayout.PropertyField(customGizmoAlphaProp, new GUIContent("Opacity"));
                    doortrigger.CustomGizmoColor.a = doortrigger.CustomGizmoColorAlpha;
                }

                EditorGUILayout.Space();
                EditorGUILayout.LabelField("<b>Wireframe</b>", style);
                EditorGUILayout.PropertyField(drawWireProp, new GUIContent("Draw Wireframe"));

                if (doortrigger.DrawWire)
                {
                    EditorGUILayout.PropertyField(customWireColorProp, new GUIContent("Color"));
                    EditorGUILayout.PropertyField(customWireAlphaProp, new GUIContent("Opacity"));
                    doortrigger.CustomWireColor.a = doortrigger.CustomWireColorAlpha;
                }

                if (!doortrigger.DrawWire)
                    EditorGUILayout.HelpBox("Collapse collider components in order to hide their wireframes.", MessageType.None);

                EditorGUILayout.Space();
                EditorGUILayout.LabelField(Styles.VersionLabel, Styles.centeredVersionLabel);
                break;
            default: break;
        }

        if (Event.current.type == EventType.Repaint)
        {
            if (doortrigger.ColliderType == DoorTrigger.TypeOfCollider.Cubic)
            {
                if (doortrigger.gameObject.GetComponent<BoxCollider>() == null) doortrigger.gameObject.AddComponent<BoxCollider>();
                doortrigger.gameObject.GetComponent<BoxCollider>().isTrigger = true;
            }

            else if (doortrigger.ColliderType == DoorTrigger.TypeOfCollider.Spherical)
            {
                if (doortrigger.gameObject.GetComponent<SphereCollider>() == null) doortrigger.gameObject.AddComponent<SphereCollider>();
                doortrigger.gameObject.GetComponent<SphereCollider>().isTrigger = true;
            }
        }

        serializedObject.ApplyModifiedProperties();
    }

    public static void SetParentChild(GameObject Parent, GameObject Trigger)
    {
        Parent.transform.parent = Selection.activeGameObject.transform.parent.transform;
        Trigger.transform.parent = Parent.transform;
    }

    public bool AnyEmptyFields()
    {
        DoorTrigger doortrigger = target as DoorTrigger;

        if (doortrigger.HasName && doortrigger.playerName == "")
            return true;
        else if (doortrigger.IsLookingAt && doortrigger.lookObject == null)
            return true;
        else if (doortrigger.HasPressed && doortrigger.character == "")
            return true;
        else if (doortrigger.HasScript && doortrigger.script.script.Name == "")
            return true;
        else if (doortrigger.IsGameObject && doortrigger.isObject == null)
            return true;
        else
            return false;
    }
}
