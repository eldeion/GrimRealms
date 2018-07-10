// DoorTrigger.cs
// Created by Alexander Ameye
// Version 1.1.0

using UnityEngine;
using RuntimeScriptField;
using AlexDoorSystem;

[RequireComponent(typeof(BoxCollider), typeof(SphereCollider))]
public class DoorTrigger : MonoBehaviour
{
    public int ID;

    #region Zone Settings
    public enum TypeOfCollider { Cubic, Spherical }
    public TypeOfCollider ColliderType;
    #endregion

    #region Player Requirements
    public bool CorrectTag, CorrectName, CorrectView, CorrectButton, CorrectScript, CorrectGameObject;
    public bool HasTag, HasName, IsLookingAt, HasPressed, HasScript, IsGameObject;
    [TagSelector]
    public string playerTag = "Untagged";
    public string playerName, character;
    public GameObject lookObject, isObject;
    public ComponentReference script = null;
    #endregion

    [HideInInspector]
    public bool allGood = true;

    #region Gizmo Settings
    public bool DrawGizmo = true;
    public bool DrawWire;
    public Color CustomGizmoColor;
    public Color CustomWireColor = Color.black;
    [Range(0.0f, 1.0f)]
    public float CustomGizmoColorAlpha = 0.5f;
    [Range(0.0f, 1.0f)]
    public float CustomWireColorAlpha = 1f;
    #endregion

    private DoorRotation doorrotation;
    private DoorDetection doordetection;
    private DoorSound doorsound;

    void OnDrawGizmos()
    {
        if (ColliderType == TypeOfCollider.Cubic)
        {
            GetComponent<BoxCollider>().enabled = true;
            GetComponent<SphereCollider>().enabled = false;
            Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.lossyScale);
            CustomGizmoColor.a = CustomGizmoColorAlpha;
            Gizmos.color = CustomGizmoColor;
            if (GetComponent<BoxCollider>() && DrawGizmo) Gizmos.DrawCube(Vector3.zero, Vector3.one);
            Gizmos.color = CustomWireColor;
            if (GetComponent<BoxCollider>() && DrawWire) Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
        }

        else
        {
            GetComponent<BoxCollider>().enabled = false;
            GetComponent<SphereCollider>().enabled = true;
            Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.lossyScale);
            CustomGizmoColor.a = CustomGizmoColorAlpha;
            Gizmos.color = CustomGizmoColor;
            if (GetComponent<SphereCollider>() && DrawGizmo) Gizmos.DrawSphere(Vector3.zero, 0.5f);
            Gizmos.color = CustomWireColor;
            if (GetComponent<SphereCollider>() && DrawWire) Gizmos.DrawWireSphere(Vector3.zero, 0.5f);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        //TODO: change this, transform.root is not always good
       // doorrotation = transform.root.GetComponentInChildren<DoorRotation>();
       //What's the best method here??
        doorrotation = transform.parent.transform.parent.transform.GetComponentInChildren<DoorRotation>();
        if (transform.root.GetComponentInChildren<DoorSound>() != null) doorsound = transform.root.GetComponentInChildren<DoorSound>();
        else doorsound = null;
        doordetection = GameObject.FindGameObjectWithTag("Player").GetComponent<DoorDetection>();
    }

    void OnTriggerStay(Collider other)
    {
        if (lookObject)
            doordetection.CheckUIPrefabs(lookObject);

        if (!doorrotation.RotationPending && doorrotation.CurrentRotationBlockIndex < doorrotation.RotationTimeline.Count && doorrotation.CurrentRotationBlockIndex == ID)
        {
            CorrectTag = (!HasTag || other.CompareTag(playerTag));
            CorrectName = (!HasName || other.name == playerName);
            CorrectView = (!IsLookingAt || doordetection.CheckIfLookingAt(lookObject));
            CorrectButton = (!HasPressed || Input.GetKey(character));
            CorrectScript = (!HasScript || other.gameObject.GetComponent(script.script.Name) != null);
            CorrectGameObject = (!IsGameObject || other.gameObject == isObject);

            allGood = CorrectTag && CorrectName && CorrectView && CorrectScript && CorrectGameObject;

            if (CorrectTag && CorrectName && CorrectView && CorrectScript && CorrectButton && CorrectGameObject)
            {
                if (transform.gameObject.name == "Move Trigger")
                    StartCoroutine(doorrotation.Rotate());

                else if (transform.gameObject.name == "Close Trigger")
                {
                    if (doorrotation.DoorIsClosing())
                    {
                        if (doorsound != null) PlaySoundStart();
                        StartCoroutine(doorrotation.Rotate());
                        if (doorsound != null) PlaySoundEnd();
                    }
                }

                else if (transform.gameObject.name == "Open Trigger")
                {
                    if (doorrotation.DoorIsOpening())
                    {
                        if (doorsound != null) PlaySoundStart();
                        StartCoroutine(doorrotation.Rotate());
                        if (doorsound != null) PlaySoundEnd();
                    }
                }

                else if (transform.gameObject.name == "Front Trigger")
                    StartCoroutine(doorrotation.Swing(true));

                else if (transform.gameObject.name == "Back Trigger")
                    StartCoroutine(doorrotation.Swing(false));
            }

            if (transform.gameObject.name == "Open Trigger" && doorsound != null)
            {
                if (!allGood && CorrectButton && doorrotation.DoorIsOpening())
                {
                    doorsound.Play("locked");
                    allGood = false;
                }
            }

            if (transform.gameObject.name == "Close Trigger" && doorsound != null)
            {
                if (!allGood && CorrectButton && doorrotation.DoorIsClosing())
                {
                    doorsound.Play("locked");
                    allGood = false;
                }
            }

            if (transform.gameObject.name == "Move Trigger" && doorsound != null)
            {
                if (!allGood && CorrectButton)
                {
                    doorsound.Play("locked");
                    allGood = false;
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (doorrotation.ResetOnLeave)
        {
            bool DoorWasMoved = doorrotation.transform.rotation == doorrotation.EndRotation * doorrotation.RotationOffset;
            bool HingeWasMoved = doorrotation.transform.parent.transform.rotation == doorrotation.EndRotation;

            if (doorrotation.PivotPosition == DoorRotation.PositionOfPivot.CorrectlyPositioned)
            {
                if (doorrotation.AngleConvention == 0 && DoorWasMoved)
                    StartCoroutine(RotationTools.Rotate(doorrotation.gameObject, -doorrotation.InitialYRotation + doorrotation.RotationTimeline[0].FinalAngle, doorrotation.RotationTimeline[0].InitialAngle - doorrotation.InitialYRotation, 2, false, 0, doorrotation.ShortestWay));
                else if (doorrotation.AngleConvention == 1 && DoorWasMoved)
                    StartCoroutine(RotationTools.Rotate(doorrotation.gameObject, -doorrotation.InitialYRotation - doorrotation.RotationTimeline[0].FinalAngle, -doorrotation.RotationTimeline[0].InitialAngle - doorrotation.InitialYRotation, 2, false, 0, doorrotation.ShortestWay));
            }

            if (doorrotation.PivotPosition == DoorRotation.PositionOfPivot.Centered)
            {
                if (doorrotation.AngleConvention == 0 && HingeWasMoved)
                    StartCoroutine(RotationTools.Rotate(doorrotation.transform.parent.gameObject, doorrotation.RotationTimeline[0].FinalAngle, doorrotation.RotationTimeline[0].InitialAngle, 2, false, 0, doorrotation.ShortestWay));
                else if (doorrotation.AngleConvention == 1 && HingeWasMoved)
                    StartCoroutine(RotationTools.Rotate(doorrotation.transform.parent.gameObject, -doorrotation.RotationTimeline[0].FinalAngle, -doorrotation.RotationTimeline[0].InitialAngle, 2, false, 0, doorrotation.ShortestWay));
            }

            doorrotation.CurrentRotationBlockIndex = 0;
            doorrotation.TimesRotated = 0;
        }
    }

    public void PlaySoundStart()
    {
        if (doorrotation.transform.GetComponent<DoorSound>() != null)
        {
            if (doorrotation.DoorIsOpening())
                doorrotation.transform.GetComponent<DoorSound>().Play("opening");
            else
                doorrotation.transform.GetComponent<DoorSound>().Play("closing");
        }
    }

    public void PlaySoundEnd()
    {
        if (doorrotation.transform.GetComponent<DoorSound>() != null)
        {
            if (doorrotation.DoorIsOpening())
                doorrotation.transform.GetComponent<DoorSound>().Play("opened");
            else
                doorrotation.transform.GetComponent<DoorSound>().Play("closed");
        }
    }
}

