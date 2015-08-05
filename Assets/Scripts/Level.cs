using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Level : MonoBehaviour
{
    public GameObject RingPrefab, HolePrefab, CharacterPrefab, EnemyPrefab;

    List<CircleData> Circles = new List<CircleData>();
    public Circle[] CircleObjects { get; private set; }

    public Circle FirstCircle { get; private set; }
    public Circle LastCircle { get; private set; }

    public int CurrentLevel { get; private set; }

    public RingLayout[] RingLayout;

    public Character Character;

    void Awake()
    {
        SetupCircles();
    }

    void Update()
    {
    
    }

    void SetupCircles()
    {
        Debug.Assert(CircleObjects == null);

        var count = RingLayout.Length;
        CircleObjects = new Circle[count];

        Circle first, last;

        for (var i = 0; i < count; i++)
        {
            var go = Instantiate(RingPrefab);
            go.transform.SetParent(transform);
            var circle = go.GetComponent<Circle>();
            circle.LayerZ = -0.1f * (i + 1);
            circle.Position = Vector2.zero;
            circle.Level = this;

            if (i > 0)
            {
                last = CircleObjects[i - 1];
                circle.Previous = last;
                last.Next = circle;
            }

            CircleObjects[i] = circle;

            circle.Layout = RingLayout[i];
        }

        FirstCircle = CircleObjects[0];
        LastCircle = CircleObjects.Last();
        
        FirstCircle.Previous = LastCircle;
        LastCircle.Next = FirstCircle;
    }

    public void Init()
    {
        UpdateData();
        AddCharacter();
    }

    public void Clear()
    {
        foreach (var circle in CircleObjects)
        {
            circle.Enemies.ForEach(enemy => enemy.Recycle());
            circle.Enemies.Clear();
            circle.Data = null;
        }

        Circles.Clear();

        CurrentLevel = 0;

        if (Character != null)
        {
            Character.gameObject.SetActive(false);
        }
    }

    void AddCharacter()
    {
        if (Character == null)
        {
            var go = Instantiate(CharacterPrefab);
            Character = go.GetComponent<Character>();
        }

        Character.Circle = CircleObjects[3];
        Character.Reset();

        Character.gameObject.SetActive(true);
    }

    public void Animate(int direction)
    {
        CurrentLevel -= direction;

        var isIn = direction > 0;

        var indexOffset = 0;
        var sourceOffset = 1;

        if (isIn)
        {
            indexOffset = 1;
            sourceOffset = -1;
        }

        Character.Circle = isIn ? Character.Circle.Next : Character.Circle.Previous;
        Character.Stop();

        for (var i = 0; i < RingLayout.Length - 1; i++)
        {
            CircleObjects[i + indexOffset].Layout = RingLayout[i + indexOffset + sourceOffset];
            CircleObjects[i + indexOffset].TargetLayout = RingLayout[i + indexOffset];
        }

        UpdateData();
    }

    void UpdateData()
    {
        for (var i = 0; i < CircleObjects.Length; i++)
        {
            CircleData cd = null;
            var circle = CircleObjects[i];
            var index = i + CurrentLevel - 4;

            if (index > -1)
            {
                if (index < Circles.Count)
                {
                    cd = Circles[index];
                }
                else
                {
                    cd = GetNewCircleData();
                    cd.Entities.Add(GetNewEnemyData());
                    Circles.Add(cd);
                }
            }

            circle.Data = cd;
        }
    }

    CircleData GetNewCircleData()
    {
        return new CircleData
        {
            HoleAngle = Random.value * 360
        };
    }

    EntityData GetNewEnemyData()
    {
        return new EntityData
        {
            Angle = Random.value * 360,
            Speed = Random.value * 50 + 50,
            StartTime = Time.realtimeSinceStartup,
            Radius = 0.5f
        };
    }
}
