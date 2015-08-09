﻿using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;

public class Character : Entity
{
    public float Speed;
    public int SpeedIncreaseLevel;
    public float SpeedPerLevel;

    public float StickDistanceToCharacter = 120f;
    public float StickDistanceBetweenEnemies = 45f;
    public float StickDangerDistance = 45f;

    public bool Invulnerable;

    public int Score;

    private bool _isDead;

    protected override void Start()
    {
    
    }

    protected override void Update()
    {
        if (GameManager.Instance.IsPaused)
        {
            UpdatePosition();

            if (_isDead) return;

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
        Score = 0;
        _isDead = false;
    }

    public void Die()
    {
        _isDead = true;
        var gm = GameManager.Instance;
        gm.IsPaused = true;

        var overlay = gm.ContainerHUD.GetComponent<HUD>().Overlay;
        var color = new Color(0.8f, 0.8f, 0.8f, 0f);
        overlay.color = color;
        overlay.DOFade(1f, 1f).SetEase(Ease.Linear).OnComplete(() =>
        {
            overlay.color = color;
            gm.EnterPostGame();
        });
    }

    protected override void HandleCollision()
    {
        if (CheckHoleCollision())
        {
            var levelDifference = 1 + Circle.Level.CurrentLevel - SpeedIncreaseLevel;
            Data.Speed = levelDifference > 0 ? Speed + levelDifference * SpeedPerLevel : Speed;

            Circle.Level.Animate(1 - (int) Data.Side * 2);
            SwitchSide();

            var currentLevel = Circle.Level.CurrentLevel;
            if (Score < currentLevel)
            {
                Score = currentLevel;
            }

            return;
        }

        if (!Invulnerable && Circle.Enemies.Any(CheckCollision))
        {
            Die();
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
