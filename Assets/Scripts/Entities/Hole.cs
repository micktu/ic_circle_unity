using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

public class Hole : Entity
{
    protected override void Update()
    {
        UpdatePosition();

        GetComponent<SpriteRenderer>().enabled = IsInBounds;
    }

    public void UpdateLayout()
    {
        var scale = transform.localScale.x;
        var radius = Circle.Radius / scale;
        var offset = (Circle.Position - Circle.AngleToPoint(Data.Angle)) / scale;

        var material = GetComponent<SpriteRenderer>().material;
        material.SetColor("_Color", Circle.Layout.Color);
        material.SetColor("_CircleColor", Circle.Previous.Layout.Color);
        material.SetFloat("_Radius", radius);
        material.SetFloat("_X", offset.x);
        material.SetFloat("_Y", offset.y);
    }
}
