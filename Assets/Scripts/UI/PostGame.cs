using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class PostGame : MonoBehaviour
{
    public Text ScoreText, RecordText;
    public InputField NameInput;
    public Button SubmitButton;
    public Image Overlay;

    private int _currentScore;

    //private TouchScreenKeyboard _keyboard;

    void Start()
    {
    
    }
    
    void Update()
    {
        /*
        if (_keyboard != null && _keyboard.done)
        {
            SubmitResult();
            _keyboard = null;
        }
         * */
    }

    public void Init()
    {
        _currentScore = GameManager.Instance.Level.Character.Score;
        var bestScore = Mathf.Max(PlayerPrefs.GetInt("BestScore", 0), _currentScore);

        PlayerPrefs.SetInt("BestScore", bestScore);

        var playerName = PlayerPrefs.GetString("PlayerName", "");

        ScoreText.text = _currentScore.ToString();
        RecordText.text = string.Format("Your record: {0}", bestScore);
        
        NameInput.text = playerName;
        SubmitButton.interactable = playerName.Length > 0;

        //_keyboard = TouchScreenKeyboard.Open(playerName, TouchScreenKeyboardType.Default, false, false, false, true, "Type your name...");
    }

    public void SubmitResult()
    {
        //var playerName = _keyboard.text.Trim();
        var playerName = NameInput.text.Trim();
        if (playerName.Length < 1) return;

        SubmitButton.interactable = false;
        NameInput.interactable = false;

        PlayerPrefs.SetString("PlayerName", playerName);

        StartCoroutine(UploadResult());
    }

    IEnumerator UploadResult()
    {
        //var url = string.Format(Constants.LeaderboardSubmitUrl, _currentScore, Constants.PlatformId, _keyboard.text, Constants.AppId);
        var url = string.Format(Constants.LeaderboardSubmitUrl, _currentScore, Constants.PlatformId, NameInput.text.Trim(), Constants.AppId);
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
