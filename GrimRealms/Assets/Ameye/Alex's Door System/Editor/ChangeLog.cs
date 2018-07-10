// ChangeLog.cs
// Created by Alexander Ameye
// Version 1.1.0

using UnityEngine;
using UnityEditor;
using StylesHelper;

public class ChangeLog : EditorWindow
{
    Vector2 scrollPos;
    static void ShowWindow()
    {
        ChangeLog myWindow = new ChangeLog();
        myWindow.ShowUtility();
        myWindow.titleContent = new GUIContent("Version Changes");
        GetWindow(typeof(SupportWindow)).Close();
    }

    public static bool OneDotOneFold
    {
        get { return EditorPrefs.GetBool("OneDotOneFold", false); }
        set { EditorPrefs.SetBool("OneDotOneFold", value); }
    }

    public static bool OneDotZeroFold
    {
        get { return EditorPrefs.GetBool("OneDotZeroFold", false); }
        set { EditorPrefs.SetBool("OneDotZeroFold", value); }
    }

  
    static GUIStyle _foldoutStyle;
    static GUIStyle FoldoutStyle
    {
        get
        {
            if (_foldoutStyle == null)
            {
                _foldoutStyle = new GUIStyle(EditorStyles.foldout)
                {
                    font = EditorStyles.boldFont
                };
            }
            return _foldoutStyle;
        }
    }

    static GUIStyle _boxStyle;
    public static GUIStyle BoxStyle
    {
        get
        {
            if (_boxStyle == null)
            {
                _boxStyle = new GUIStyle(EditorStyles.helpBox);
            }
            return _boxStyle;
        }
    }

    void OnGUI()
    {
        ChangeLog myWindow = (ChangeLog)GetWindow(typeof(ChangeLog));
        myWindow.minSize = new Vector2(350, 350);
        myWindow.maxSize = myWindow.minSize;
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos, false, false);

        EditorGUILayout.Space();
        GUILayout.Label(Styles.NewIcon, Styles.centeredVersionLabel);
        EditorGUILayout.Space();

        OneDotOneFold = BeginFold("New in version 1.1.0 (February 2017)", OneDotOneFold);
        if (OneDotOneFold)
        {
            EditorGUILayout.LabelField("• Sound Support");
            EditorGUILayout.LabelField("• New speed system");
            EditorGUILayout.LabelField("• Spherical trigger zones");
            EditorGUILayout.LabelField("• Reset on leave");
            EditorGUILayout.LabelField("• Completely re-written rotation code");
            EditorGUILayout.LabelField("• Swing doors");
            EditorGUILayout.LabelField("• Improved codebase");
            EditorGUILayout.LabelField("• Bug fixes and general improvements");
            EditorGUILayout.LabelField("• New demo scene");
            EditorGUILayout.LabelField("• Reworked asset and forum page");
            EditorGUILayout.LabelField("• Migrated documentation to MkDocs");
        }
        EndFold();

        OneDotZeroFold = BeginFold("New in version 1.0.0 (July 2017)", OneDotZeroFold);
        if (OneDotZeroFold)
        {
            EditorGUILayout.LabelField("• Rotation Timeline");
            EditorGUILayout.LabelField("• Trigger Zones");
            EditorGUILayout.LabelField("• Detection Script");
            EditorGUILayout.LabelField("• 2D Support");
            EditorGUILayout.LabelField("• Support for 3rd party doors");
            EditorGUILayout.LabelField("• Readable and organized source code");
            EditorGUILayout.LabelField("• Intuitive UI");
            EditorGUILayout.LabelField("• Easy set-up process");
            EditorGUILayout.LabelField("• Documentation");
            EditorGUILayout.LabelField("• Demo scene");
        }
        EndFold();

        EditorGUILayout.EndScrollView();
    }

    public static bool BeginFold(string foldName, bool foldState)
    {
        EditorGUILayout.BeginVertical(BoxStyle);
        GUILayout.Space(3);
        foldState = EditorGUI.Foldout(EditorGUILayout.GetControlRect(),
        foldState, foldName, true, FoldoutStyle);
        if (foldState) GUILayout.Space(3);
        return foldState;
    }

    public static void EndFold()
    {
        GUILayout.Space(3);
        EditorGUILayout.EndVertical();
        GUILayout.Space(0);
    }
}
