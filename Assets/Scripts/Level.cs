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

    private bool _isMenuMode;
    public bool IsMenuMode
    {
        get { return _isMenuMode; }
        set
        {
            _isMenuMode = value;
            CircleObjects[4].GetComponent<Renderer>().enabled =
                CircleObjects[5].GetComponent<Renderer>().enabled = !value;
        }
    }

    public RingLayout[] RingLayout;

    public Character Character;

    private RingLayout _centerLayout;

    private Queue<int> _animationQueue = new Queue<int>();
    private int _currentAnimationDirection;

    void Awake()
    {
       SetupCircles();
    }

    void Update()
    {
        if (Character == null) return;

        var isAnimating = Character.Circle.IsAnimating;

        if (!isAnimating) _currentAnimationDirection = 0;

        if (_animationQueue.Count > 0)
        {
            if (isAnimating)
            {
                if (_animationQueue.Peek() != _currentAnimationDirection)
                {
                    
                }
            }
            else
            {
                _currentAnimationDirection = _animationQueue.Dequeue();
                Animate(_currentAnimationDirection);
            }
        }
    }

    void SetupCircles()
    {
        Debug.Assert(CircleObjects == null);

        var count = RingLayout.Length;

        CircleObjects = new Circle[count];

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
                var last = CircleObjects[i - 1];
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

        _centerLayout = new RingLayout { Color = RingLayout[count - 1].Color };
    }

    public void Init()
    {
        AddCharacter();
        UpdateData();
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

    public void EnqueueAnimation(int direction)
    {
        _animationQueue.Enqueue(direction);
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
        //Character.Stop();

        var count = RingLayout.Length - 1;
        for (var i = 0; i < count; i++)
        {
            var circle = CircleObjects[i + indexOffset];

            if (!circle.IsAnimating)
            {
                circle.Layout = RingLayout[i + indexOffset + sourceOffset];
            }

            circle.Animate(RingLayout[i + indexOffset]);
        }
        
        // FIXME
        if (!isIn)
        {
            var circle = CircleObjects.Last();
            var lastLayout = RingLayout.Last();
            circle.Layout = _centerLayout;
            circle.Animate(lastLayout);
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
        var enemy = EnemyPrefab.GetComponent<Enemy>();

        return new EntityData
        {
            Angle = Random.value * 360,
            Speed = Random.Range(enemy.MinSpeed, enemy.MaxSpeed),
            StartTime = Time.realtimeSinceStartup,
            Radius = 0.5f
        };
    }
}
