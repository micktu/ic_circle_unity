﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using DG.Tweening;
using UnityEngine.Assertions;
using UnityEngine.UI;
using UnityEngineInternal;

public class GameManager : MonoBehaviour
{
    public CanvasScaler CanvasScaler;

    public Level Level;

    public GameObject ContainerMainMenu, ContainerHUD, ContainerPostGame, ContainerHelp, ContainerLeaderboard;

    public static GameManager Instance { get; private set; }

    public float GameStartTime { get; private set; }

    public float ReferenceWidth;

    public float UnitsPerPixel { get; private set; }
    public float ReferenceUnitScale { get; private set; }
    //public float ReferencePixelScale { get; private set; }

    private ScreenOrientation _oldOrientation;
    private int _oldScreenWidth, _oldScreenHeight;

    public enum StateType
    {
        MainMenu,
        Help,
        InGame,
        PostGame,
        Leaderboard
    }

    private StateType _state;
    public StateType State
    {
        get
        {
            return _state;
        }
        private set
        {
            ContainerMainMenu.SetActive(false);
            ContainerHUD.SetActive(false);
            ContainerHelp.SetActive(false);
            ContainerPostGame.SetActive(false);
            ContainerLeaderboard.SetActive(false);
            Level.Clear();
            Level.IsMenuMode = true;

            _lastState = _state;
            _state = value;
        }
    }

    private StateType _lastState;

    public bool IsPaused;

    public GameManager()
    {
        Debug.Assert(Instance == null);
        Instance = this;
    }

	void OnEnable()
	{
#if UNITY_IOS
		Application.targetFrameRate = 60;
#endif

        CheckResize();
    }

    void Start()
    {
        CanvasScaler = FindObjectOfType<Canvas>().GetComponent<CanvasScaler>();

        GameStartTime = Time.realtimeSinceStartup;

        EnterMainMenu();
    }
    
    void Update()
    {
        //CheckResize();

        var backPressed = Input.GetKeyDown(KeyCode.Escape);
        var tapped = Input.GetButtonDown("Fire1");
        var inHelp = State == StateType.Help;

        if (backPressed || tapped && inHelp)
        {
            EnterMainMenu();
        }
    }

    public void EnterMainMenu()
    {
        State = StateType.MainMenu;

        ContainerMainMenu.SetActive(true);
    }

    public void EnterHelp()
    {
        State = StateType.Help;

        ContainerHelp.SetActive(true);
    }

    public void EnterLeaderboard()
    {
        State = StateType.Leaderboard;

        ContainerLeaderboard.SetActive(true);

        var leaderboard = ContainerLeaderboard.GetComponent<Leaderboard>();
        leaderboard.Init();
    }

    public void EnterGame()
    {
        State = StateType.InGame;
        
        ContainerHUD.SetActive(true);
        Level.Init();
        Level.IsMenuMode = false;
        IsPaused = true;
    }

    public void EnterPostGame()
    {
        State = StateType.PostGame;

        var postGame = ContainerPostGame.GetComponent<PostGame>();
        postGame.Init();
        
        ContainerPostGame.SetActive(true);
    }

    public void CheckResize()
    {
        var orientation = Screen.orientation;
        var width = Screen.width;
        var height = Screen.height;

        if (orientation != _oldOrientation ||
            width != _oldScreenWidth ||
            height != _oldScreenHeight)
        {
            var isWide = width > height;

            UnitsPerPixel = Camera.main.orthographicSize * 2 / height;
            ReferenceUnitScale = Camera.main.orthographicSize * 2 * (isWide ? Camera.main.aspect : 1) / ReferenceWidth;
            //ReferencePixelScale = width / ReferenceWidth;

            _oldOrientation = orientation;
            _oldScreenWidth = width;
            _oldScreenHeight = height;
            /*
            CanvasScaler.matchWidthOrHeight = isWide ? 0 : 1;

            foreach (var circleObject in Level.CircleObjects)
            {
                circleObject.SetDirty();
            }
             * */
        }
    }
}
