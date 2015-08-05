using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Hole : Entity
{
    protected override void Start()
    {
        //Data.Side = EntityData.CircleSide.Outer;
        Data.Radius = 1f;
    }

    protected override void Update()
    {
        UpdatePosition();

        GetComponent<SpriteRenderer>().enabled = !IsOutOfBounds;
    }
}
