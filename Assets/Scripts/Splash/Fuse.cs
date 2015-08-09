using System;
using UnityEngine;
using System.Collections;

public class Fuse : MonoBehaviour {

    IEnumerator Start()
    {
        var www = new WWW(Constants.ServerFuseUrl);
        
        yield return www;

        if (!string.IsNullOrEmpty(www.error))
        {
            Application.Quit();
        }

	    if (!PlayerPrefs.HasKey("FirstLaunchRegistered"))
	    {
	        var url = String.Format(Constants.ServerFirstUrl,
	            Application.systemLanguage, Constants.Version, Constants.BuildId, Constants.AppId);
	        www = new WWW(url);

            yield return www;

	        PlayerPrefs.SetInt("FirstLaunchRegistered", 1);
	    }
	}
	
	void Update()
    {
	
	}
}
