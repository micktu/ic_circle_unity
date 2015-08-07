using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class HUD : MonoBehaviour
{
    public Text LevelText;

    void Start()
    {
    
    }
    
    void Update()
    {
        LevelText.text = GameManager.Instance.Level.Character.Score.ToString();
    }
}
