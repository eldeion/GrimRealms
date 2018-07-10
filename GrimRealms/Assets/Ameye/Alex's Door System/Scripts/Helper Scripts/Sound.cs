// Sound.cs
// Created by Alexander Ameye
// Version 1.1.0

using UnityEngine;

[System.Serializable, System.Obsolete("This is an obsolete class.")]
public class Sound
{
    public string name;
    public AudioClip clip;
    [Range(0f, 1f)]
    public float volume;
    [Range(0.1f, 3f)]
    public float pitch;
    [HideInInspector]
    public AudioSource source;
}
