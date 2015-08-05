﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class PostGame : MonoBehaviour
{
    public Text ScoreText, RecordText;
    public InputField NameInput;
    public Button SubmitButton;

    private int _currentScore;

    void Start()
    {
    
    }
    
    void Update()
    {
    
    }

    public void Init()
    {
        _currentScore = GameManager.Instance.Level.CurrentLevel;
        var bestScore = Mathf.Max(PlayerPrefs.GetInt("BestScore", 0), _currentScore);

        PlayerPrefs.SetInt("BestScore", bestScore);

        var playerName = PlayerPrefs.GetString("PlayerName", "");

        ScoreText.text = _currentScore.ToString();
        RecordText.text = string.Format("Your record: {0}", bestScore);
        
        NameInput.text = playerName;
        SubmitButton.interactable = playerName.Length > 0;
    }

    public void SubmitResult()
    {
        SubmitButton.interactable = false;
        NameInput.interactable = false;

        PlayerPrefs.SetString("PlayerName", NameInput.text);

        StartCoroutine(UploadResult());
    }

    IEnumerator UploadResult()
    {
        var url = string.Format(Constants.LeaderboardSubmitUrl, _currentScore, Constants.PlatformId, NameInput.text, Constants.AppId);
        var www = new WWW(url);

        yield return www;

        NameInput.interactable = true;
        SubmitButton.interactable = true;
    }

    public void OnPlayerNameChanged()
    {
        SubmitButton.interactable = NameInput.text.Length > 0;
    }
}
