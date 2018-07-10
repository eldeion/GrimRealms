// SupportWindow.cs
// Created by Alexander Ameye
// Version 1.1.0

using UnityEngine;
using UnityEditor;
using StylesHelper;

public class SupportWindow : EditorWindow
{
    [MenuItem("Tools/Alex's Door System/Support")]
    public static void ShowWindow()
    {
        GetWindow(typeof(SupportWindow));
        SupportWindow myWindow = (SupportWindow)GetWindow(typeof(SupportWindow));
        myWindow.titleContent = new GUIContent("Support");
        GetWindow(typeof(ChangeLog)).Close();
    }

    public static void Init()
    {
        SupportWindow myWindow = (SupportWindow)GetWindow(typeof(SupportWindow));
        myWindow.Show();
    }

    void OnGUI()
    {
        SupportWindow myWindow = (SupportWindow)GetWindow(typeof(SupportWindow));
        myWindow.minSize = new Vector2(300, 260);
        myWindow.maxSize = myWindow.minSize;

        if (GUILayout.Button(Styles.Forum, Styles.helpbox))
            Application.OpenURL("https://forum.unity3d.com/threads/wip-doors-pro-a-powerful-door-system.459866/");

        if (GUILayout.Button(Styles.Documentation, Styles.helpbox))
            Application.OpenURL("https://alexdoorsystem.github.io/");

        if (GUILayout.Button(Styles.Contact, Styles.helpbox))
            Application.OpenURL("mailto:alexanderameye@gmail.com?");

        if (GUILayout.Button(Styles.Twitter, Styles.helpbox))
            Application.OpenURL("https://twitter.com/alexanderameye");

        if (GUILayout.Button(Styles.Review, Styles.helpbox))
            Application.OpenURL("https://www.assetstore.unity3d.com/en/#!/account/downloads/search=Alex%20s%20Door%20System");

        if (GUILayout.Button(Styles.Changelog, Styles.helpbox))
        {
            GetWindow(typeof(ChangeLog));
            Close();
        }
    }
}
