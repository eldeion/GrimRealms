// DoorSound.cs
// Created by Alexander Ameye
// Version 1.1.0

using UnityEngine;

[RequireComponent(typeof(DoorRotation))]
public class DoorSound : MonoBehaviour
{
    public AudioClip OpeningClip;
    [Range(0f, 1f)]
    public float OpeningVolume;
    [Range(0.1f, 3f)]
    public float OpeningPitch;
    [HideInInspector]
    public AudioSource OpeningSource;
    [Range(0f, 1f)]
    public float OpeningOffset;

    public AudioClip OpenedClip;
    [Range(0f, 1f)]
    public float OpenedVolume;
    [Range(0.1f, 3f)]
    public float OpenedPitch;
    [HideInInspector]
    public AudioSource OpenedSource;
    [Range(0f, 1f)]
    public float OpenedOffset;

    public AudioClip ClosingClip;
    [Range(0f, 1f)]
    public float ClosingVolume;
    [Range(0.1f, 3f)]
    public float ClosingPitch;
    [HideInInspector]
    public AudioSource ClosingSource;
    [Range(0f, 1f)]
    public float ClosingOffset;

    public AudioClip ClosedClip;
    [Range(0f, 1f)]
    public float ClosedVolume;
    [Range(0.1f, 3f)]
    public float ClosedPitch;
    [HideInInspector]
    public AudioSource ClosedSource;
    [Range(0f, 1f)]
    public float ClosedOffset;

    public AudioClip LockedClip;
    [Range(0f, 1f)]
    public float LockedVolume;
    [Range(0.1f, 3f)]
    public float LockedPitch;
    [HideInInspector]
    public AudioSource LockedSource;
    [Range(0f, 1f)]
    public float LockedOffset;

    private void Awake()
    {
        #region Open
        OpeningSource = gameObject.AddComponent<AudioSource>();
        OpeningSource.clip = OpeningClip;
        OpeningSource.volume = OpeningVolume;
        OpeningSource.pitch = OpeningPitch;

        OpenedSource = gameObject.AddComponent<AudioSource>();
        OpenedSource.clip = OpenedClip;
        OpenedSource.volume = OpenedVolume;
        OpenedSource.pitch = OpenedPitch;
        #endregion

        #region Close
        ClosingSource = gameObject.AddComponent<AudioSource>();
        ClosingSource.clip = ClosingClip;
        ClosingSource.volume = ClosingVolume;
        ClosingSource.pitch = ClosingPitch;

        ClosedSource = gameObject.AddComponent<AudioSource>();
        ClosedSource.clip = ClosedClip;
        ClosedSource.volume = ClosedVolume;
        ClosedSource.pitch = ClosedPitch;
        #endregion

        #region Locked
        LockedSource = gameObject.AddComponent<AudioSource>();
        LockedSource.clip = LockedClip;
        LockedSource.volume = LockedVolume;
        LockedSource.pitch = LockedPitch;
        #endregion
    }

    public void Play(string name)
    {
        if (name == "opening") OpeningSource.PlayDelayed(OpeningOffset);
        else if (name == "opened") OpenedSource.PlayDelayed(OpenedOffset);
        else if (name == "closing") ClosingSource.PlayDelayed(ClosingOffset);
        else if (name == "closed") ClosedSource.PlayDelayed(ClosedOffset);
        else if (name == "locked") LockedSource.PlayDelayed(LockedOffset);
    }
}
