using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Runtime.InteropServices;

public class Circle : MonoBehaviour
{
    public struct CircleContext
    {
        public RingLayout From;
        public RingLayout To;
        public CircleData Data;
    }

    public float LayerZ;

    public Circle Previous;
    public Circle Next;

    public float AnimationPeriod;

    public float Radius = 1.0f;
    public float Width = 0.5f;

    public Texture Texture;

    private MeshFilter _meshFilter;
    private MeshRenderer _meshRenderer;

    public bool IsAnimating { get; private set; }
    private float _animationTime;

    private bool _isMeshDirty;

    private CircleData _data;
    public CircleData Data
    {
        get { return _data; }
        set
        {
            _data = value;
            _isDataDirty = true;

            if (_isDataDirty)
            {
                foreach (var enemy in Enemies)
                {
                    enemy.Data = null;
                    enemy.Circle = null;
                    enemy.Recycle();
                }

                Enemies.Clear();

                if (Data != null)
                {
                    if (Hole == null)
                    {
                        var go = Level.HolePrefab.Spawn(Level.transform);
                        Hole = go.GetComponent<Hole>();
                        Hole.Circle = this;
                        if (Hole.Data == null) Hole.Data = new EntityData();
                    }

                    Hole.Data.Angle = Data.HoleAngle;
                    Hole.UpdatePosition();

                    foreach (var entity in Data.Entities)
                    {
                        var go = Level.EnemyPrefab.Spawn(Level.transform);
                        var enemy = go.GetComponent<Enemy>();
                        enemy.Circle = this;
                        enemy.Data = entity;
                        //enemy.Stop();
                        enemy.EnterIdle();
                        enemy.UpdatePosition();

                        Enemies.Add(enemy);
                    }
                }
                else
                {
                    if (Hole != null)
                    {
                        Hole.Recycle();
                        Hole = null;
                    }
                }

                _isDataDirty = false;
            }
        }
    }

    private bool _isDataDirty;

    public Hole Hole;

    public List<Enemy> Enemies = new List<Enemy>();

    private RingLayout _layout;
    public RingLayout Layout
    {
        get { return _layout; }
        set
        {
            _layout = value;

            var us = GameManager.Instance.ReferenceUnitScale;
            Radius = _layout.Radius * us;
            Width = _layout.Width * us;
        }
    }

    private RingLayout _targetLayout;
    public RingLayout TargetLayout
    {
        get { return _targetLayout; }
        set
        {
            _targetLayout = value;
            IsAnimating = true;
        }
    }
    
    public Vector2 Position
    {
        get { return transform.localPosition; }
        set { transform.localPosition = new Vector3(value.x, value.y, LayerZ); }
    }

    public Level Level;

    public Circle()
    {

    }

    void Awake()
    {
        _meshFilter = GetComponent<MeshFilter>();
        _meshRenderer = GetComponent<MeshRenderer>();
        _meshRenderer.sharedMaterial.mainTexture = Texture;
    }

    void OnEnable()
    {
        _isMeshDirty = true;
    }

    void Start()
    {

    }

    void Update()
    {
        if (!_isMeshDirty && !IsAnimating) return;

        var radius = (float)Layout.Radius;
        var width = (float)Layout.Width;
        var color = Layout.Color;

        if (IsAnimating)
        {
            if (_animationTime < AnimationPeriod)
            {
                var t = _animationTime / AnimationPeriod;
                radius = Mathf.Lerp(radius, TargetLayout.Radius, t);
                width = Mathf.Lerp(width, TargetLayout.Width, t);
                color = Color.Lerp(color, TargetLayout.Color, t);

                _animationTime += Time.deltaTime;
            }
            else
            {
                radius = TargetLayout.Radius;
                width = TargetLayout.Width;
                color = TargetLayout.Color;

                Layout = TargetLayout;

                _animationTime = 0;
                IsAnimating = false;
            }

            _isMeshDirty = true;
        }

        var us = GameManager.Instance.ReferenceUnitScale;
        Radius = radius * us;
        Width = width * us;

        if (_isMeshDirty)
        {
            var mesh = _meshFilter.sharedMesh;
            var isFresh = mesh == null;

            MeshFactory.GetRingMesh(Radius, Width, ref mesh, false);
            if (isFresh) _meshFilter.sharedMesh = mesh;

            var material = _meshRenderer.material;
            material.color = color;
            //material.SetFloat("_Thickness", width * GameManager.Instance.ReferencePixelScale);

            _isMeshDirty = false;
        }

        //transform.localPosition = (Vector2)transform.localPosition;
        //transform.localRotation = Quaternion.identity;
        //transform.localScale = Vector3.one;
    }

    public Vector2 AngleToPoint(float angle, float distance = 0)
    {
        angle *= Mathf.Deg2Rad;
        return Position + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * (Radius + distance);
    }

    public float PointToAngle(Vector2 point)
    {
        return Mathf.Atan2(point.y - transform.position.y, point.x - transform.position.x) * Mathf.Rad2Deg;
    }
}
