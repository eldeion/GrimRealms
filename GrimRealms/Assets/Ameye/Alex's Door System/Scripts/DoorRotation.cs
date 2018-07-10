// DoorRotation.cs
// Created by Alexander Ameye
// Version 1.1.0

using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using System;
using AlexDoorSystem;

[RequireComponent(typeof(BoxCollider))]
public class DoorRotation : MonoBehaviour
{
    public List<RotationTimelineData> RotationTimeline;

    [Serializable]
    public class RotationTimelineData
    {
        public enum TypeOfRotation { SingleRotation, LoopedRotation, SwingRotation }
        public TypeOfRotation RotationType;

        public enum InitialSwingState { Front, Middle, Back }
        [ConditionalEnumHide("RotationType", 2)]
        public InitialSwingState InitialState;

        [ConditionalEnumHide("RotationType", 0, 1)]
        public float InitialAngle = 0f;
        [ConditionalEnumHide("RotationType", 0, 1)]
        public float FinalAngle = 90f;

        [ConditionalEnumHide("RotationType", 2)]
        public float FrontAngle;
        [ConditionalEnumHide("RotationType", 2)]
        public float MiddleAngle;
        [ConditionalEnumHide("RotationType", 2)]
        public float BackAngle;

        [StayPositive]
        public float Speed;
        [Curve(0, 0, 1f, 1f, true)]
        public AnimationCurve RotationCurve;

        [ConditionalEnumHide("RotationType", 1, 2)]
        public int TimesMoveable;
    }

    // Hinge
    private GameObject hinge;
    public enum PositionOfHinge { Left, Right }
    public PositionOfHinge HingePosition;

    // 3rd party compatibility
    public enum ScaleOfDoor { Unity3DUnits, Other }
    public ScaleOfDoor DoorScale;
    public enum PositionOfPivot { Centered, CorrectlyPositioned }
    public PositionOfPivot PivotPosition;

    // Rotations
    public Quaternion StartRotation, EndRotation;
    public int RotationState;
    public Quaternion RotationOffset;
    public float InitialYRotation;
    public Quaternion SwingStart, SwingMid, SwingEnd;

    // Rotation Progress
    public int CurrentRotationBlockIndex;
    public int SwingState;
    public int TimesRotated;
    public bool RotationPending;

    // Settings
    public bool ShortestWay = true;
    public enum WayOfRotation { Default, Shortest }
    public WayOfRotation RotationWay;
    public bool ResetOnLeave;
    public int AngleConvention = 1;
    public string[] AngleConventionNames = new string[] { "Alex's Door System", "Unity" };
    public int[] AngleConventionValues = { 0, 1 };

    void Start()
    {
        gameObject.tag = "Door";

        if (RotationTimeline[0].InitialState == RotationTimelineData.InitialSwingState.Front) SwingState = 1;
        if (RotationTimeline[0].InitialState == RotationTimelineData.InitialSwingState.Middle) SwingState = 2;
        if (RotationTimeline[0].InitialState == RotationTimelineData.InitialSwingState.Back) SwingState = 3;

        InitialYRotation = transform.localEulerAngles.y;
        RotationOffset = Quaternion.Euler(0, transform.localEulerAngles.y, 0);

        if (PivotPosition == PositionOfPivot.CorrectlyPositioned && AngleConvention == 0)
        {
            if (RotationTimeline[0].RotationType == RotationTimelineData.TypeOfRotation.LoopedRotation || RotationTimeline[0].RotationType == RotationTimelineData.TypeOfRotation.SingleRotation)
                transform.rotation = Quaternion.Euler(0, -RotationTimeline[0].InitialAngle, 0) * RotationOffset;

            if (RotationTimeline[0].RotationType == RotationTimelineData.TypeOfRotation.SwingRotation)
            {
                if (SwingState == 1) transform.rotation = Quaternion.Euler(0, -RotationTimeline[0].FrontAngle, 0) * RotationOffset;
                if (SwingState == 2) transform.rotation = Quaternion.Euler(0, -RotationTimeline[0].MiddleAngle, 0) * RotationOffset;
                if (SwingState == 3) transform.rotation = Quaternion.Euler(0, -RotationTimeline[0].BackAngle, 0) * RotationOffset;
            }
        }

        if (PivotPosition == PositionOfPivot.CorrectlyPositioned && AngleConvention == 1)
        {
            if (RotationTimeline[0].RotationType == RotationTimelineData.TypeOfRotation.LoopedRotation || RotationTimeline[0].RotationType == RotationTimelineData.TypeOfRotation.SingleRotation)
                transform.rotation = Quaternion.Euler(0, RotationTimeline[0].InitialAngle, 0) * RotationOffset;

            if (RotationTimeline[0].RotationType == RotationTimelineData.TypeOfRotation.SwingRotation)
            {
                if (SwingState == 1) transform.rotation = Quaternion.Euler(0, RotationTimeline[0].FrontAngle, 0) * RotationOffset;
                if (SwingState == 2) transform.rotation = Quaternion.Euler(0, RotationTimeline[0].MiddleAngle, 0) * RotationOffset;
                if (SwingState == 3) transform.rotation = Quaternion.Euler(0, RotationTimeline[0].BackAngle, 0) * RotationOffset;
            }
        }

        #region Hinge Algorithm

        if (DoorScale != ScaleOfDoor.Unity3DUnits || PivotPosition != PositionOfPivot.Centered) return;

        hinge = new GameObject("Hinge");

        ShortestWay = RotationWay == WayOfRotation.Shortest;

        var cosDeg = Mathf.Cos((transform.eulerAngles.y * Mathf.PI) / 180);
        var sinDeg = Mathf.Sin((transform.eulerAngles.y * Mathf.PI) / 180);

        //Read transform (position/rotation/scale) of the door.
        var posDoorX = transform.position.x;
        var posDoorY = transform.position.y;
        var posDoorZ = transform.position.z;

        var rotDoorX = transform.localEulerAngles.x;
        var rotDoorZ = transform.localEulerAngles.z;

        var scaleDoorX = transform.localScale.x;
        var scaleDoorZ = transform.localScale.z;

        //Create a placeholder of the hinge's position/rotation.
        var hingePosCopy = hinge.transform.position;
        var hingeRotCopy = hinge.transform.localEulerAngles;

        if (HingePosition == PositionOfHinge.Left && RotationTimeline.Count != 0)
        {
            if (transform.localScale.x > transform.localScale.z)
            {
                hingePosCopy.x = (posDoorX - (scaleDoorX / 2 * cosDeg));
                hingePosCopy.z = (posDoorZ + (scaleDoorX / 2 * sinDeg));
                hingePosCopy.y = posDoorY;

                hingeRotCopy.x = rotDoorX;
                if (AngleConvention == 0) hingeRotCopy.y = -RotationTimeline[0].InitialAngle;
                if (AngleConvention == 1) hingeRotCopy.y = RotationTimeline[0].InitialAngle;
                hingeRotCopy.z = rotDoorZ;
            }

            else
            {
                hingePosCopy.x = (posDoorX + (scaleDoorZ / 2 * sinDeg));
                hingePosCopy.z = (posDoorZ + (scaleDoorZ / 2 * cosDeg));
                hingePosCopy.y = posDoorY;

                hingeRotCopy.x = rotDoorX;

                if (RotationTimeline[0].RotationType == RotationTimelineData.TypeOfRotation.SingleRotation || RotationTimeline[0].RotationType == RotationTimelineData.TypeOfRotation.LoopedRotation)
                    if (AngleConvention == 0) // Alex's Door System Convention
                        hingeRotCopy.y = -RotationTimeline[0].InitialAngle;
                if (AngleConvention == 1) // Unity Convention
                    hingeRotCopy.y = RotationTimeline[0].InitialAngle;

                if (RotationTimeline[0].RotationType == RotationTimelineData.TypeOfRotation.SwingRotation)
                {
                    if (AngleConvention == 0) // Alex's Door System Convention
                    {
                        if (SwingState == 1) hingeRotCopy.y = -RotationTimeline[0].FrontAngle;
                        if (SwingState == 2) hingeRotCopy.y = -RotationTimeline[0].MiddleAngle;
                        if (SwingState == 3) hingeRotCopy.y = -RotationTimeline[0].BackAngle;
                    }

                    if (AngleConvention == 1) // Unity Convention
                    {
                        if (SwingState == 1) hingeRotCopy.y = RotationTimeline[0].FrontAngle;
                        if (SwingState == 2) hingeRotCopy.y = RotationTimeline[0].MiddleAngle;
                        if (SwingState == 3) hingeRotCopy.y = RotationTimeline[0].BackAngle;
                    }
                }
                hingeRotCopy.z = rotDoorZ;
            }
        }

        if (HingePosition == PositionOfHinge.Right && RotationTimeline.Count != 0)
        {
            if (transform.localScale.x > transform.localScale.z)
            {
                hingePosCopy.x = (posDoorX + (scaleDoorX / 2 * cosDeg));
                hingePosCopy.z = (posDoorZ - (scaleDoorX / 2 * sinDeg));
                hingePosCopy.y = posDoorY;

                hingeRotCopy.x = rotDoorX;

                if (RotationTimeline[0].RotationType == RotationTimelineData.TypeOfRotation.SingleRotation || RotationTimeline[0].RotationType == RotationTimelineData.TypeOfRotation.LoopedRotation)
                    if (AngleConvention == 0) // Alex's Door System Convention
                        hingeRotCopy.y = -RotationTimeline[0].InitialAngle;
                if (AngleConvention == 1) // Unity Convention
                    hingeRotCopy.y = RotationTimeline[0].InitialAngle;

                if (RotationTimeline[0].RotationType == RotationTimelineData.TypeOfRotation.SwingRotation)
                {
                    if (AngleConvention == 0) // Alex's Door System Convention
                    {
                        if (SwingState == 1) hingeRotCopy.y = -RotationTimeline[0].FrontAngle;
                        if (SwingState == 2) hingeRotCopy.y = -RotationTimeline[0].MiddleAngle;
                        if (SwingState == 3) hingeRotCopy.y = -RotationTimeline[0].BackAngle;
                    }

                    if (AngleConvention == 1) // Unity Convention
                    {
                        if (SwingState == 1) hingeRotCopy.y = RotationTimeline[0].FrontAngle;
                        if (SwingState == 2) hingeRotCopy.y = RotationTimeline[0].MiddleAngle;
                        if (SwingState == 3) hingeRotCopy.y = RotationTimeline[0].BackAngle;
                    }
                }
                hingeRotCopy.z = rotDoorZ;
            }

            else
            {
                hingePosCopy.x = (posDoorX - (scaleDoorZ / 2 * sinDeg));
                hingePosCopy.z = (posDoorZ - (scaleDoorZ / 2 * cosDeg));
                hingePosCopy.y = posDoorY;

                hingeRotCopy.x = rotDoorX;

                if (RotationTimeline[0].RotationType == RotationTimelineData.TypeOfRotation.SingleRotation || RotationTimeline[0].RotationType == RotationTimelineData.TypeOfRotation.LoopedRotation)
                    if (AngleConvention == 0) // Alex's Door System Convention
                        hingeRotCopy.y = -RotationTimeline[0].InitialAngle;
                if (AngleConvention == 1) // Unity Convention
                    hingeRotCopy.y = RotationTimeline[0].InitialAngle;


                if (RotationTimeline[0].RotationType == RotationTimelineData.TypeOfRotation.SwingRotation)
                {
                    if (AngleConvention == 0) // Alex's Door System Convention
                    {
                        if (SwingState == 1) hingeRotCopy.y = -RotationTimeline[0].FrontAngle;
                        if (SwingState == 2) hingeRotCopy.y = -RotationTimeline[0].MiddleAngle;
                        if (SwingState == 3) hingeRotCopy.y = -RotationTimeline[0].BackAngle;
                    }

                    if (AngleConvention == 1) // Unity Convention
                    {
                        if (SwingState == 1) hingeRotCopy.y = RotationTimeline[0].FrontAngle;
                        if (SwingState == 2) hingeRotCopy.y = RotationTimeline[0].MiddleAngle;
                        if (SwingState == 3) hingeRotCopy.y = RotationTimeline[0].BackAngle;
                    }
                }
                hingeRotCopy.z = rotDoorZ;
            }
        }

        hinge.transform.parent = transform.parent;
        hinge.transform.position = hingePosCopy;
        transform.SetParent(hinge.transform);
        hinge.transform.localEulerAngles = hingeRotCopy;
        #endregion
    }

    public IEnumerator Rotate()
    {
        RotationPending = true;

        Transform t = transform;

        RotationTimelineData CurrentRotationBlock = RotationTimeline[CurrentRotationBlockIndex];

        float TimeProgression = 0f;

        if (CurrentRotationBlock.RotationType == RotationTimelineData.TypeOfRotation.SingleRotation)
            CurrentRotationBlock.TimesMoveable = 1;

        if (AngleConvention == 0) // Alex's Door System Convention
        {
            StartRotation = Quaternion.Euler(0, -CurrentRotationBlock.InitialAngle, 0);
            EndRotation = Quaternion.Euler(0, -CurrentRotationBlock.FinalAngle, 0);
        }

        else if (AngleConvention == 1) // Unity Convention
        {
            StartRotation = Quaternion.Euler(0, CurrentRotationBlock.InitialAngle, 0);
            EndRotation = Quaternion.Euler(0, CurrentRotationBlock.FinalAngle, 0);
        }

        if (DoorScale == ScaleOfDoor.Unity3DUnits && PivotPosition == PositionOfPivot.Centered)
        {
            t = hinge.transform;
            RotationOffset = Quaternion.identity;
        }

        if (TimesRotated == 0) t.rotation = StartRotation * RotationOffset;

        if (TimesRotated < CurrentRotationBlock.TimesMoveable || CurrentRotationBlock.TimesMoveable == 0)
        {
            if (t.rotation == (RotationState == 0 ? EndRotation * RotationOffset : StartRotation * RotationOffset)) RotationState ^= 1;

            while (TimeProgression <= (1 / CurrentRotationBlock.Speed))
            {
                TimeProgression += Time.deltaTime;
                float RotationProgression = Mathf.Clamp01(TimeProgression / (1 / CurrentRotationBlock.Speed));
                float RotationCurveValue = CurrentRotationBlock.RotationCurve.Evaluate(RotationProgression);

                if (RotationState == 0) // Door is closed
                    t.rotation = RotationTools.Lerp(StartRotation * RotationOffset, EndRotation * RotationOffset, RotationCurveValue, ShortestWay);

                if (RotationState == 1) // Door is opened
                    t.rotation = RotationTools.Lerp(EndRotation * RotationOffset, StartRotation * RotationOffset, RotationCurveValue, ShortestWay);

                yield return null;
            }

            TimesRotated++;

            if (TimesRotated == CurrentRotationBlock.TimesMoveable && CurrentRotationBlock.TimesMoveable != 0)
            {
                TimesRotated = 0;
                CurrentRotationBlockIndex++;
            }
        }
        RotationPending = false;
    }

    public IEnumerator Swing(bool front)
    {
        RotationPending = true;

        Transform t = transform;

        RotationTimelineData CurrentRotationBlock = RotationTimeline[CurrentRotationBlockIndex];

        float TimeProgression = 0f;

        if (AngleConvention == 0) // Alex's Door System Convention
        {
            SwingStart = Quaternion.Euler(0, -CurrentRotationBlock.FrontAngle, 0);
            SwingMid = Quaternion.Euler(0, -CurrentRotationBlock.MiddleAngle, 0);
            SwingEnd = Quaternion.Euler(0, -CurrentRotationBlock.BackAngle, 0);
        }

        else if (AngleConvention == 1) // Unity Convention
        {
            SwingStart = Quaternion.Euler(0, CurrentRotationBlock.FrontAngle, 0);
            SwingMid = Quaternion.Euler(0, CurrentRotationBlock.MiddleAngle, 0);
            SwingEnd = Quaternion.Euler(0, CurrentRotationBlock.BackAngle, 0);
        }

        if (DoorScale == ScaleOfDoor.Unity3DUnits && PivotPosition == PositionOfPivot.Centered)
        {
            t = hinge.transform;

            if (TimesRotated == 0 && SwingState == 1)
                t.rotation = SwingStart;

            if (TimesRotated == 0 && SwingState == 2)
                t.rotation = SwingMid;

            if (TimesRotated == 0 && SwingState == 3)
                t.rotation = SwingEnd;
        }

        else if (PivotPosition == PositionOfPivot.CorrectlyPositioned)
        {
            if (TimesRotated == 0) t.rotation = StartRotation;
        }

        if (TimesRotated < CurrentRotationBlock.TimesMoveable || CurrentRotationBlock.TimesMoveable == 0)
        {
            while (TimeProgression <= (1 / CurrentRotationBlock.Speed))
            {
                TimeProgression += Time.deltaTime;
                float RotationProgression = Mathf.Clamp01(TimeProgression / (1 / CurrentRotationBlock.Speed));
                float RotationCurveValue = CurrentRotationBlock.RotationCurve.Evaluate(RotationProgression);

                if (front)
                {
                    if (SwingState == 1)
                        t.rotation = RotationTools.Lerp(SwingStart, SwingMid, RotationCurveValue, ShortestWay);
                    if (SwingState == 2)
                        t.rotation = RotationTools.Lerp(SwingMid, SwingEnd, RotationCurveValue, ShortestWay);
                    if (SwingState == 3)
                        t.rotation = RotationTools.Lerp(SwingEnd, SwingMid, RotationCurveValue, ShortestWay);
                }

                if (!front)
                {
                    if (SwingState == 3)
                        t.rotation = RotationTools.Lerp(SwingEnd, SwingMid, RotationCurveValue, ShortestWay);
                    if (SwingState == 2)
                        t.rotation = RotationTools.Lerp(SwingMid, SwingStart, RotationCurveValue, ShortestWay);
                    if (SwingState == 1)
                        t.rotation = RotationTools.Lerp(SwingStart, SwingMid, RotationCurveValue, ShortestWay);
                }

                yield return null;
            }

            if (front && SwingState == 1) SwingState++;
            else if (front && SwingState == 2) SwingState++;
            else if (front && SwingState == 3) SwingState--;

            else if (!front && SwingState == 1) SwingState++;
            else if (!front && SwingState == 2) SwingState--;
            else if (!front && SwingState == 3) SwingState--;

            TimesRotated++;

            if (TimesRotated == CurrentRotationBlock.TimesMoveable && CurrentRotationBlock.TimesMoveable != 0)
            {
                TimesRotated = 0;
                CurrentRotationBlockIndex++;
            }
        }
        RotationPending = false;
    }


    public bool DoorIsOpening()
    {
        if (TimesRotated % 2 == 0) return true;
        else return false;
    }

    public bool DoorIsClosing()
    {
        if (TimesRotated % 2 != 0) return true;
        else return false;
    }
}