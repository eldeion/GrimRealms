// DoorRotationEditor.cs
// Created by Alexander Ameye
// Version 1.1.0

using UnityEngine;
using UnityEditor;
using Malee.Editor;
using StylesHelper;

[CustomEditor(typeof(DoorRotation)), CanEditMultipleObjects]
public class DoorRotationEditor : Editor
{
    private static bool prefsLoaded = false;

    [Tooltip("Automatically add a sound component when adding a rotation component to a door.")]
    private static bool AutoAddSound = false;

    private ReorderableList RotationTimeline;

    int NumberOfTriggers, NumberOfRotationBlocks, ToolBarIndex;

    SerializedProperty hingePositionProp, doorScaleProp, pivotPositionProp, rotationWayProp, angleConventionProp, resetOnLeaveProp;

    public GameObject MoveTrigger, OpenTrigger, CloseTrigger, FrontTrigger, BackTrigger;
    public GameObject DoorParent, RotationParent;

    // Colors
    Color moveTriggerColor = new Color32(0, 128, 255, 191);
    Color openTriggerColor = new Color32(0, 255, 44, 191);
    Color closeTriggerColor = new Color32(255, 0, 58, 191);
    Color frontTriggerColor = new Color32(220, 0, 255, 191);
    Color backTriggerColor = new Color32(255, 237, 0, 191);
    Color orange = new Color32(255, 139, 0, 255);

    public void OnEnable()
    {
        ToolBarIndex = 1;

        DoorRotation DoorRotation = target as DoorRotation;

        RotationTimeline = new ReorderableList(serializedObject.FindProperty("RotationTimeline"), true, true, true, ReorderableList.ElementDisplayType.Expandable, "RotationType", null);

        hingePositionProp = serializedObject.FindProperty("HingePosition");
        doorScaleProp = serializedObject.FindProperty("DoorScale");
        pivotPositionProp = serializedObject.FindProperty("PivotPosition");
        rotationWayProp = serializedObject.FindProperty("RotationWay");
        resetOnLeaveProp = serializedObject.FindProperty("ResetOnLeave");

        if (DoorRotation.transform.parent == null)
        {
            // Create a parent with the same name as the door itself and reset it
            DoorParent = new GameObject(DoorRotation.gameObject.name);
            DoorParent.transform.localRotation = Quaternion.identity;
            DoorParent.transform.localPosition = Vector3.zero;
            DoorParent.transform.localScale = Vector3.one;
            DoorRotation.transform.SetParent(DoorParent.transform);
        }

        if (DoorRotation.transform.parent != null && DoorRotation.transform.parent.transform.name != DoorRotation.gameObject.name)
        {
            int siblingIndex = DoorRotation.transform.GetSiblingIndex();

            DoorParent = new GameObject(DoorRotation.gameObject.name);
            DoorParent.transform.localRotation = Quaternion.identity;
            DoorParent.transform.localPosition = Vector3.zero;
            DoorParent.transform.localScale = Vector3.one;
            DoorParent.transform.SetParent(DoorRotation.transform.parent);
            DoorParent.transform.SetSiblingIndex(siblingIndex);
            DoorRotation.transform.SetParent(DoorParent.transform);
        }

        // Loop through all the children of the parent object and check for triggers
        NumberOfTriggers = 0;

        for (int x = 0; x < DoorRotation.transform.parent.childCount; x++)
        {
            if (DoorRotation.transform.parent.Find("Rotation " + x + " (Single)") != null || DoorRotation.transform.parent.Find("Rotation " + x + " (Looped)") != null || DoorRotation.transform.parent.Find("Rotation " + x + " (Swing)") != null) NumberOfTriggers += 1;
        }
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        DoorRotation DoorRotation = target as DoorRotation;

        string[] menuOptions = new string[2];
        menuOptions[0] = "Door";
        menuOptions[1] = "Rotations";

        EditorGUILayout.Space();

        ToolBarIndex = GUILayout.Toolbar(ToolBarIndex, menuOptions);

        GUIStyle style = new GUIStyle()
        {
            richText = true
        };

        switch (ToolBarIndex)
        {
            //Door and hinge settings
            case 0:
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("<b>Door Settings</b>", style);
                EditorGUILayout.PropertyField(doorScaleProp, new GUIContent("Scale"));
                EditorGUILayout.PropertyField(pivotPositionProp, new GUIContent("Pivot Position"));
                EditorGUILayout.Space();
                if (DoorRotation.DoorScale == DoorRotation.ScaleOfDoor.Unity3DUnits && DoorRotation.PivotPosition == DoorRotation.PositionOfPivot.Centered)
                {
                    EditorGUILayout.LabelField("<b>Hinge Settings</b>", style);
                    EditorGUILayout.PropertyField(hingePositionProp, new GUIContent("Position"));
                    EditorGUILayout.Space();
                }

                if (DoorRotation.DoorScale == DoorRotation.ScaleOfDoor.Other && DoorRotation.PivotPosition == DoorRotation.PositionOfPivot.Centered)
                    EditorGUILayout.HelpBox("If your door is not scaled in Unity3D units and the pivot position is not already positioned correctly, the hinge algorithm will not work as expected.", MessageType.Error);

                else if (Tools.pivotMode == PivotMode.Center)
                    EditorGUILayout.HelpBox("Make sure the tool handle is placed at the active object's pivot point.", MessageType.Warning);

                EditorGUILayout.Space();
                EditorGUILayout.LabelField(Styles.VersionLabel, Styles.centeredVersionLabel);
                serializedObject.ApplyModifiedProperties();
                break;

            case 1:
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("<b>Rotation Settings</b>", style);
                EditorGUILayout.PropertyField(rotationWayProp, new GUIContent("Rotation"));
                DoorRotation.AngleConvention = EditorGUILayout.IntPopup("Convention", DoorRotation.AngleConvention, DoorRotation.AngleConventionNames, DoorRotation.AngleConventionValues);
                EditorGUILayout.PropertyField(resetOnLeaveProp, new GUIContent("Reset On Leave"));

                RotationTimeline.DoLayoutList();

                if (DoorRotation.transform.gameObject.GetComponent(typeof(DoorSound)) == null)
                {
                    GUI.color = Color.green;
                    if (GUILayout.Button("Add Audio Component"))
                        DoorRotation.transform.gameObject.AddComponent(typeof(DoorSound));
                    GUI.color = Color.white;
                }

                else
                {
                    GUI.color = orange;
                    if (GUILayout.Button("Remove Audio Component"))
                        DestroyImmediate(DoorRotation.transform.gameObject.GetComponent(typeof(DoorSound)));
                    GUI.color = Color.white;
                }

                if (DoorRotation.RotationTimeline != null)
                {
                    for (int x = 0; x < DoorRotation.RotationTimeline.Count; x++)
                    {
                        if ((DoorRotation.RotationTimeline[x].FinalAngle - DoorRotation.RotationTimeline[x].InitialAngle) >= 360)
                            EditorGUILayout.HelpBox("The difference between FinalAngle and InitialAngle should not exceed 360°. (See rotation " + (x + 1) + ")", MessageType.Warning);
                    }
                }

                EditorGUILayout.LabelField(Styles.VersionLabel, Styles.centeredVersionLabel);
                serializedObject.ApplyModifiedProperties();
                break;
            default: break;
        }

        NumberOfRotationBlocks = 0;
        if (DoorRotation.RotationTimeline != null) NumberOfRotationBlocks = DoorRotation.RotationTimeline.Count;

        if (Application.isPlaying) return;

        // Adding rotation blocks
        if (NumberOfTriggers < NumberOfRotationBlocks)
        {
            for (int index = 0; index < NumberOfRotationBlocks; index++)
            {
                if (DoorRotation.transform.parent.Find("Rotation " + (index + 1) + " (Single)") == null && DoorRotation.RotationTimeline[index].RotationType == DoorRotation.RotationTimelineData.TypeOfRotation.SingleRotation)
                    CreateMoveTrigger(true, index);

                if (DoorRotation.transform.parent.Find("Rotation " + (index + 1) + " (Looped)") == null && DoorRotation.RotationTimeline[index].RotationType == DoorRotation.RotationTimelineData.TypeOfRotation.LoopedRotation)
                    CreateOpenCloseTriggers(true, index);

                if (DoorRotation.transform.parent.Find("Rotation " + (index + 1) + " (Swing)") == null && DoorRotation.RotationTimeline[index].RotationType == DoorRotation.RotationTimelineData.TypeOfRotation.SwingRotation)
                    CreateFrontBackTriggers(true, index);
            }
        }

        // Changing rotation blocks
        for (int index = 0; index < NumberOfRotationBlocks; index++)
        {
            if (DoorRotation.RotationTimeline[index].RotationType == DoorRotation.RotationTimelineData.TypeOfRotation.LoopedRotation)
            {
                if (DoorRotation.transform.parent.transform.Find("Rotation " + (index + 1) + " (Single)") && !Application.isPlaying)
                {
                    DestroyImmediate(DoorRotation.transform.parent.Find("Rotation " + (index + 1) + " (Single)").gameObject);
                    CreateOpenCloseTriggers(false, index);
                }

                if (DoorRotation.transform.parent.transform.Find("Rotation " + (index + 1) + " (Swing)") && !Application.isPlaying)
                {
                    DestroyImmediate(DoorRotation.transform.parent.Find("Rotation " + (index + 1) + " (Swing)").gameObject);
                    CreateOpenCloseTriggers(false, index);
                }
            }

            if (DoorRotation.RotationTimeline[index].RotationType == DoorRotation.RotationTimelineData.TypeOfRotation.SingleRotation)
            {
                if (DoorRotation.transform.parent.transform.Find("Rotation " + (index + 1) + " (Looped)") && !Application.isPlaying)
                {
                    DestroyImmediate(DoorRotation.transform.parent.Find("Rotation " + (index + 1) + " (Looped)").gameObject);
                    CreateMoveTrigger(false, index);
                }

                if (DoorRotation.transform.parent.transform.Find("Rotation " + (index + 1) + " (Swing)") && !Application.isPlaying)
                {
                    DestroyImmediate(DoorRotation.transform.parent.Find("Rotation " + (index + 1) + " (Swing)").gameObject);
                    CreateMoveTrigger(false, index);
                }
            }

            if (DoorRotation.RotationTimeline[index].RotationType == DoorRotation.RotationTimelineData.TypeOfRotation.SwingRotation)
            {
                if (DoorRotation.transform.parent.transform.Find("Rotation " + (index + 1) + " (Single)") && !Application.isPlaying)
                {
                    DestroyImmediate(DoorRotation.transform.parent.Find("Rotation " + (index + 1) + " (Single)").gameObject);
                    CreateFrontBackTriggers(false, index);
                }

                if (DoorRotation.transform.parent.transform.Find("Rotation " + (index + 1) + " (Looped)") && !Application.isPlaying)
                {
                    DestroyImmediate(DoorRotation.transform.parent.Find("Rotation " + (index + 1) + " (Looped)").gameObject);
                    CreateFrontBackTriggers(false, index);
                }
            }
        }

        if (NumberOfTriggers <= NumberOfRotationBlocks) return;

        // Removing Rotation blocks
        while (NumberOfTriggers > NumberOfRotationBlocks)
        {
            if (DoorRotation.transform.parent.Find("Rotation " + NumberOfTriggers + " (Single)") != null)
            {
                DestroyImmediate(DoorRotation.transform.parent.Find("Rotation " + NumberOfTriggers + " (Single)").gameObject);
                NumberOfTriggers--;
            }

            else if (DoorRotation.transform.parent.Find("Rotation " + NumberOfTriggers + " (Swing)") != null)
            {
                DestroyImmediate(DoorRotation.transform.parent.Find("Rotation " + NumberOfTriggers + " (Swing)").gameObject);
                NumberOfTriggers--;
            }

            else if (DoorRotation.transform.parent.Find("Rotation " + NumberOfTriggers + " (Looped)") != null)
            {
                DestroyImmediate(DoorRotation.transform.parent.Find("Rotation " + NumberOfTriggers + " (Looped)").gameObject);
                NumberOfTriggers--;
            }

            else if (DoorRotation.transform.parent.Find("Rotation " + NumberOfTriggers + " (Looped)") == null || DoorRotation.transform.parent.Find("Rotation " + NumberOfTriggers + " (Single)") == null || DoorRotation.transform.parent.Find("Rotation " + NumberOfTriggers + " (Swing)") == null) continue;
        }

        serializedObject.ApplyModifiedProperties();
    }

    public void CreateMoveTrigger(bool AddToNumberOfTriggers, int index)
    {
        DoorRotation DoorRotation = target as DoorRotation;

        MoveTrigger = new GameObject("Move Trigger");
        RotationParent = new GameObject("Rotation " + (index + 1) + " (Single)");
        ResetTransform(MoveTrigger, DoorRotation);
        SetParentChild(RotationParent, MoveTrigger);
        AddTriggerScript(MoveTrigger, index);
        MoveTrigger.GetComponent<DoorTrigger>().CustomGizmoColor = moveTriggerColor;
        if (AddToNumberOfTriggers) NumberOfTriggers += 1;
    }

    public void CreateOpenCloseTriggers(bool AddToNumberOfTriggers, int index)
    {
        DoorRotation DoorRotation = target as DoorRotation;

        OpenTrigger = new GameObject("Open Trigger");
        CloseTrigger = new GameObject("Close Trigger");
        RotationParent = new GameObject("Rotation " + (index + 1) + " (Looped)");

        ResetTransform(OpenTrigger, DoorRotation);
        SetParentChild(RotationParent, OpenTrigger);
        AddTriggerScript(OpenTrigger, index);
        OpenTrigger.GetComponent<DoorTrigger>().CustomGizmoColor = openTriggerColor;

        ResetTransform(CloseTrigger, DoorRotation);
        SetParentChild(RotationParent, CloseTrigger);
        AddTriggerScript(CloseTrigger, index);
        CloseTrigger.GetComponent<DoorTrigger>().CustomGizmoColor = closeTriggerColor;

        if (AddToNumberOfTriggers) NumberOfTriggers += 1;
    }

    public void CreateFrontBackTriggers(bool AddToNumberOfTriggers, int index)
    {
        DoorRotation DoorRotation = target as DoorRotation;

        FrontTrigger = new GameObject("Front Trigger");
        BackTrigger = new GameObject("Back Trigger");
        RotationParent = new GameObject("Rotation " + (index + 1) + " (Swing)");

        ResetTransform(FrontTrigger, DoorRotation);
        SetParentChild(RotationParent, FrontTrigger);
        AddTriggerScript(FrontTrigger, index);
        FrontTrigger.GetComponent<DoorTrigger>().CustomGizmoColor = frontTriggerColor;

        ResetTransform(BackTrigger, DoorRotation);
        SetParentChild(RotationParent, BackTrigger);
        AddTriggerScript(BackTrigger, index);
        BackTrigger.GetComponent<DoorTrigger>().CustomGizmoColor = backTriggerColor;

        if (AddToNumberOfTriggers) NumberOfTriggers += 1;
    }

    public static void SetParentChild(GameObject Parent, GameObject Trigger)
    {
        Parent.transform.parent = Selection.activeGameObject.transform.parent.transform;
        Trigger.transform.parent = Parent.transform;
    }

    public static void ResetTransform(GameObject obj, DoorRotation DoorRotation)
    {
        if (obj.name == "Move Trigger")
        {
            obj.transform.position = DoorRotation.gameObject.transform.position + new Vector3(0, 0.125f, 0);
            obj.transform.localScale = new Vector3(2, DoorRotation.gameObject.transform.localScale.y + 0.25f, 2);
            obj.transform.rotation = DoorRotation.gameObject.transform.rotation;
        }

        if (obj.name == "Open Trigger" || obj.name == "Front Trigger")
        {
            obj.transform.position = DoorRotation.gameObject.transform.position - new Vector3(1, 0, 0) + new Vector3(0, 0.125f, 0);
            obj.transform.localScale = new Vector3(2, DoorRotation.gameObject.transform.localScale.y + 0.25f, 2);
            obj.transform.rotation = DoorRotation.gameObject.transform.rotation;
        }

        if (obj.name == "Close Trigger" || obj.name == "Back Trigger")
        {
            obj.transform.position = DoorRotation.gameObject.transform.position + new Vector3(1, 0, 0) + new Vector3(0, 0.125f, 0);
            obj.transform.localScale = new Vector3(2, DoorRotation.gameObject.transform.localScale.y + 0.25f, 2);
            obj.transform.rotation = DoorRotation.gameObject.transform.rotation;
        }
    }

    public static void AddTriggerScript(GameObject Trigger, int index)
    {
        Trigger.AddComponent<DoorTrigger>();
        Trigger.GetComponent<DoorTrigger>().ID = index;
    }

    [PreferenceItem("Alex's Door System")]
    public static void PreferencesGUI()
    {
        if (!prefsLoaded)
        {
            AutoAddSound = EditorPrefs.GetBool("BoolPreferenceKey", false);
            prefsLoaded = true;
        }

        //AutoAddSound = EditorGUILayout.Toggle(new GUIContent("Auto add sound component", "Automatically add a sound component when adding a rotation component to a door."), AutoAddSound);

        EditorGUILayout.LabelField("Coming soon!", EditorStyles.centeredGreyMiniLabel);

        if (GUI.changed)
            EditorPrefs.SetBool("BoolPreferenceKey", AutoAddSound);

        GUILayout.FlexibleSpace();
        EditorGUILayout.LabelField(Styles.VersionLabel, Styles.centeredVersionLabel);
    }
}