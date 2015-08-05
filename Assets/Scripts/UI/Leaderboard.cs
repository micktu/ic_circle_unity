using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine.UI;

public class Leaderboard : MonoBehaviour
{
    public RectTransform Daily, AllTime;

    public GameObject RowPrefab;
    
    private List<GameObject> _entries = new List<GameObject>();

    void Start()
    {
    
    }
    
    void Update()
    {
    
    }

    public void Init()
    {
        foreach (var entry in _entries.ToArray())
        {
            Destroy(entry);
        }
        _entries.Clear();

        var allTimeUrl = string.Format(Constants.LeaderboardAllTimeUrl, Constants.AppId);
        StartCoroutine(PopulateBoard(AllTime, allTimeUrl));

        var dailyUrl = string.Format(Constants.LeaderboardDailyUrl, Constants.AppId);
        StartCoroutine(PopulateBoard(Daily, dailyUrl));

    }

    private IEnumerator PopulateBoard(Transform board, string url)
    {
        var www = new WWW(url);
        yield return www;

        if (!string.IsNullOrEmpty(www.error)) yield break;

        var values = www.text.Split(',');
        var count = values.Length / 3;

        if (count < 1) yield break;

        for (var i = 0; i < count; i++)
        {
            var go = Instantiate(RowPrefab);
            _entries.Add(go);
            var text = go.GetComponent<Text>();

            var name = Encoding.UTF8.GetString(Encoding.Default.GetBytes(values[i * 3 + 1]));
            var score = values[i * 3];

            text.text = string.Format("{0}. {1} ({2})", i + 1, name, score);

            text.transform.SetParent(board);
            text.transform.localScale = Vector3.one;
        }
    }
}
