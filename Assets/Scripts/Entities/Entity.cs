using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Entity : MonoBehaviour
{
    public float LayerZ = 1;

    public Vector2 Position
    {
        get { return transform.position; }
        set { transform.position = new Vector3(value.x, value.y, LayerZ); }
    }

    //private Vector3 _scale = Vector3.one;

    private EntityData _data;
    public EntityData Data
    {
        get { return _data; }
        set
        {
            _data = value;
            _speed = value != null ? value.Speed : 0;
        }
    }

    [NonSerialized]
    public Circle Circle;

    public Circle VisualCircle
    {
        get
        {
            return Data.Side == EntityData.CircleSide.Inner ? Circle : Circle.Previous;
        }
    }

    public bool IsInBounds
    {
        get
        {
            /*
            var level = Circle.Level;
            return Circle == level.FirstCircle && Data.Side == EntityData.CircleSide.Outer ||
                   Circle == level.LastCircle && Data.Side == EntityData.CircleSide.Inner;
            */
            return Circle != Circle.Level.FirstCircle;
        }
    }

    public float FloorOffset;
    public float RotationOffset;
    public bool AlignRotation;

    public float Acceleration = 100f;
    public float Gravity = 5f;
    //public float VSpeed;

    protected float _speed;
    protected float _vspeed;
    protected float _height;

    protected bool _isinHole;

    protected virtual void Start()
    {

    
    }

    protected virtual void Update()
    {

    }

    public void UpdatePosition()
    {
        UpdatePosition(FloorOffset, RotationOffset);
    }

    protected void UpdatePosition(float distance)
    {
        UpdatePosition(distance, FloorOffset);
    }

    protected void UpdatePosition(float distance, float rotation)
    {
        var radius = VisualCircle.Width * Data.Radius;

        var offset = (int)Data.Side * GameManager.Instance.UnitsPerPixel;
        var verticalDirection = (int)Data.Side * 2 - 1;

        Position = Circle.AngleToPoint(Data.Angle, verticalDirection * (_height + distance * radius) - offset);

        if (AlignRotation)
        {
            transform.rotation = Quaternion.AngleAxis(rotation + Data.Angle, Vector3.forward);
        }

        transform.localScale = new Vector3(radius, radius, 1);
    }

    protected void UpdateOrbit()
    {
        _speed = Mathf.Clamp(_speed + Time.deltaTime * Acceleration, 0, Data.Speed);

        var direction = (int)Data.Direction * 2 - 1;
        //Data.Angle += direction * Time.deltaTime * _speed / Circle.Radius;
        Data.Angle += direction * Time.deltaTime * _speed;

        _height = Mathf.Clamp(_height + Time.deltaTime * _vspeed, 0, Circle.Radius);
        
        _vspeed -= Time.deltaTime * Gravity;

        UpdatePosition();
    }

    public void Stop()
    {
        _speed = 0;
    }

    protected virtual void HandleCollision()
    {

    }

    protected void SwitchSide()
    {
        Data.Side = (EntityData.CircleSide)(1 - (int)Data.Side);
        Data.Direction = (EntityData.CircleDirection)(1 - (int)Data.Direction);
    }

    protected bool CheckCollision(Entity entity)
    {
        return Data.Side == entity.Data.Side && (Position - entity.Position).magnitude <= (Data.Radius + entity.Data.Radius) * VisualCircle.Width / 2;
    }

    protected bool CheckHoleCollision()
    {
       //var hit = Circle.Hole != null && (Circle.Hole.Position - Position).magnitude <= Data.Radius * VisualCircle.Width / 2 / 0.95f;
        var direction = (int)Data.Direction * 2 - 1;
        var hit = Circle.Hole != null && Mathf.Repeat(direction * (Circle.Hole.Data.Angle - Data.Angle), 360f) <= 15f;

        if (hit && _isinHole) return false;

        _isinHole = hit;
        return hit;
    }

}
