using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Character : Entity
{
    public float Speed;
    public int SpeedIncreaseLevel;
    public float SpeedPerLevel;

    public float StickDistanceToCharacter = 120f;
    public float StickDistanceBetweenEnemies = 45f;
    public float StickDangerDistance = 45f;

    public bool Invulnerable;

    protected override void Start()
    {
    
    }

    protected override void Update()
    {
        if (GameManager.Instance.IsPaused)
        {
            UpdatePosition();

            if (Input.GetButtonDown("Fire1"))
            {
                GameManager.Instance.IsPaused = false;
            }
            
            return;
        }

        UpdateOrbit();
        
        HandleCollision();

        if (Input.GetButtonDown("Fire1"))
        {
            if (Data.Side == EntityData.CircleSide.Outer)
            {
                Data.Side = EntityData.CircleSide.Inner;
                Circle = Circle.Previous;
            }
            else
            {
                Circle = Circle.Next;
                Data.Side = EntityData.CircleSide.Outer;
            }
        }
    }

    public void Reset()
    {
        Data = new EntityData { Radius = 0.5f, Speed = Speed, Angle = 90f };
    }

    protected override void HandleCollision()
    {
        if (CheckHoleCollision())
        {
            var levelDifference = 1 + Circle.Level.CurrentLevel - SpeedIncreaseLevel;
            Data.Speed = levelDifference > 0 ? Speed + levelDifference * SpeedPerLevel : Speed;

            Circle.Level.Animate(1 - (int) Data.Side * 2);
            SwitchSide();
            return;
        }

        if (!Invulnerable && Circle.Enemies.Any(CheckCollision))
        {
            GameManager.Instance.EnterPostGame();
            return;
        }

        HandleSticking();
    }

    void HandleSticking()
    {
        var direction = (int)Data.Direction * 2 - 1;
        
        var enemy = Circle.Enemies.FirstOrDefault(e =>
            e.State == Enemy.StateType.Idle &&
            e.Data.Side == Data.Side &&
            Mathf.Repeat(direction * (e.Data.Angle - Data.Angle), 360f) <= StickDistanceToCharacter
        );

        if (enemy == null) return;

        var oppositeSide = (EntityData.CircleSide)(1 - (int)Data.Side);
        var nextCircle = (Data.Side == EntityData.CircleSide.Inner) ? Circle.Next : Circle.Previous;
        
        var otherEnemy = nextCircle.Enemies.FirstOrDefault(e =>
            e.State == Enemy.StateType.Idle &&
            e.Data.Side == oppositeSide &&
            Mathf.Abs(e.Data.Angle - enemy.Data.Angle) <= StickDistanceBetweenEnemies
        );

        if (otherEnemy == null) return;

        if (Mathf.Abs(Data.Angle - enemy.Data.Angle) <= StickDangerDistance)
        {
            enemy.EnterStuck(otherEnemy);
            otherEnemy.EnterStuck();
        }
        else
        {
            otherEnemy.EnterStuck(enemy);
            enemy.EnterStuck();
        }
    }
}
