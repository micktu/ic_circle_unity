using System;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Runtime.InteropServices;

public class Circle : MonoBehaviour
{
    public float LayerZ;

    public Circle Previous;
    public Circle Next;

    public float AnimationPeriod;

    public float Radius = 1.0f;
    public float Width = 0.5f;
    public Color Color;

    private MeshFilter _meshFilter;
    private MeshRenderer _meshRenderer;

    public bool IsAnimating
    {
        get { return _animation != null; }
    }

    private Coroutine _animation;

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

                    Hole.Data.Radius = 1f;
                    Hole.Data.Angle = Data.HoleAngle;
                    Hole.UpdatePosition();
                    Hole.UpdateLayout();

                    foreach (var entity in Data.Entities)
                    {
                        var go = Level.EnemyPrefab.Spawn(Level.transform);
                        var enemy = go.GetComponent<Enemy>();
                        enemy.Circle = this;
                        enemy.Data = entity;
                        //enemy.Stop();
                        enemy.EnterIdle();
                        enemy.UpdatePosition();
                        enemy.UpdateLayout();

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

    public RingLayout Layout
    {
        set
        {
            var us = GameManager.Instance.ReferenceUnitScale;
            Radius = value.Radius * us;
            Width = value.Width * us;
            Color = value.Color;

            _isMeshDirty = true;
        }
    }

    /*
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
     * */
    
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
        if (_isMeshDirty)
        {
            var mesh = _meshFilter.sharedMesh;
            var isFresh = mesh == null;

            MeshFactory.GetRingMesh(Radius, Width, ref mesh, false);
            if (isFresh) _meshFilter.sharedMesh = mesh;

            var material = _meshRenderer.material;
            material.color = Color;
            //material.SetFloat("_Thickness", width * GameManager.Instance.ReferencePixelScale);

            if (Hole != null)
            {
                Hole.UpdateLayout();
            }

            _isMeshDirty = false;
        }

        //transform.localPosition = (Vector2)transform.localPosition;
        //transform.localRotation = Quaternion.identity;
        //transform.localScale = Vector3.one;
    }


    public void Animate(RingLayout targetLayout)
    {
        if (_animation != null)
        {
            StopCoroutine(_animation);
        }

        _animation = StartCoroutine(AnimationCoroutine(targetLayout));
    }

    private IEnumerator AnimationCoroutine(RingLayout targetLayout)
    {
        var sRadius = Radius;
        var sWidth = Width;
        var sColor = Color;

        var us = GameManager.Instance.ReferenceUnitScale;
        
        var tRadius = targetLayout.Radius * us;
        var tWidth = targetLayout.Width * us;
        var tColor = targetLayout.Color;

        float time = 0;

        while (time < AnimationPeriod)
        {
            var t = time / AnimationPeriod;
            Radius = Mathf.Lerp(sRadius, tRadius, t);
            Width = Mathf.Lerp(sWidth, tWidth, t);
            Color = Color.Lerp(sColor, tColor, t);

            _isMeshDirty = true;

            time += Time.deltaTime;
            yield return null;
        }

        Layout = targetLayout;

        _animation = null;
    }

    public void SetDirty()
    {
        _isMeshDirty = true;
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
