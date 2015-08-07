﻿using System;
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
    private Enemy _stickTarget;

    private StateType _state;
    public StateType State
    {
        get { return _state; }
        set
        {
            _stickTarget = null;
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

                if (_stickTarget != null)
                {
                    Position = _stickTarget.Circle.AngleToPoint(_stickTarget.Data.Angle + 5);
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
        _stickTarget = enemy;
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
            renderer.material = VisualCircle == Circle.Level.Character.VisualCircle ? ActiveMaterial : InactiveMaterial;
        }
    }
}
