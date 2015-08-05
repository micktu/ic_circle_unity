using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class EntityData
{
    public enum CircleDirection
    {
        Clockwise,
        Counterclockwise
    }

    public enum CircleSide
    {
        Inner,
        Outer
    }

    public float StartAngle;
    public float StartTime;
    public float Speed;

    public CircleDirection Direction;
    public CircleSide Side;

    public float Radius;

    private float _angle;
    public float Angle
    {
        get { return _angle; }
        set { _angle = Mathf.Repeat(value, 360f); }
    }

    public CircleData Circle;
}
