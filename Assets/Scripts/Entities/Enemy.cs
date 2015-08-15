using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Enemy : Entity
{
    public enum StateType
    {
        Idle,
        Stuck
    }

    public Material ActiveMaterial, InactiveMaterial;

    public float MinSpeed;
    public float MaxSpeed;

    public float StickTime;

    private float _stateTime;
    public Enemy StickTarget { get; private set; }

    private StateType _state;
    public StateType State
    {
        get { return _state; }
        set
        {
            StickTarget = null;
            _stateTime = 0;
            _state = value;
        }
    }

    protected override void Update()
    {
        switch (State)
        {
            case StateType.Idle:
                if (GameManager.Instance.IsPaused)
                {
                    UpdatePosition();
                    break;
                }

                UpdateOrbit();
                HandleCollision();
                break;
            case StateType.Stuck:
                if (_stateTime >= StickTime)
                {
                    EnterIdle();
                    return;
                }

                if (StickTarget != null)
                {
                    var side = (int) StickTarget.Data.Side;
                    Position = StickTarget.Circle.AngleToPoint(StickTarget.Data.Angle + 5,
                        (side * 2 - 1) * FloorOffset * VisualCircle.Width * Data.Radius - side * GameManager.Instance.UnitsPerPixel);
                }
                else
                {
                    UpdatePosition();
                }

                break;
        }

        UpdateLayout();

        _stateTime += Time.deltaTime;
    }

    public void EnterIdle()
    {
        State = StateType.Idle;
    }

    public void EnterStuck(Enemy enemy = null)
    {
        State = StateType.Stuck;
        StickTarget = enemy;
    }

    protected override void HandleCollision()
    {
        if (CheckHoleCollision())
        {
            SwitchSide();
        }
    }

    public void UpdateLayout()
    {
        var renderer = GetComponent<SpriteRenderer>();
        
        renderer.enabled = IsInBounds;
        
        if (IsInBounds)
        {
            renderer.sharedMaterial = VisualCircle == Circle.Level.Character.VisualCircle ? ActiveMaterial : InactiveMaterial;
        }
    }
}
