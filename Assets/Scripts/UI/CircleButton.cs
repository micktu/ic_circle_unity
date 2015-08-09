using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class CircleButton : MonoBehaviour
{
    public Color Color, BackgroundColor;

    void Start()
    {
        var rectTransform = GetComponent<RectTransform>();
        var buttonWidth = rectTransform.sizeDelta.x;
        var radius = rectTransform.localPosition.magnitude / buttonWidth;
        var offset = -rectTransform.localPosition / buttonWidth;

        var image = GetComponent<Graphic>();
        var material = new Material(image.material);

        material.SetFloat("_Radius", radius);
        material.SetFloat("_X", offset.x);
        material.SetFloat("_Y", offset.y);
        material.SetColor("_CircleColor", Color);
        material.SetColor("_Color", BackgroundColor);

        image.material = material;
    }

    void Update()
    {

    }
}
