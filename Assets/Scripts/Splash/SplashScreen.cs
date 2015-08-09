using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections;

public class SplashScreen : MonoBehaviour {

    public string NextLevel;
    public Image splashImage;

	void Start ()
    {
        splashImage.color = new Color(1, 1, 1, 0);

        DOTween.Sequence()
            .Append(splashImage.DOFade(1.0f, 1.0f)
            .SetEase(Ease.InSine))
            .AppendInterval(1.0f)
            .Append(splashImage.DOFade(0.0f, 0.5f)
            .SetEase(Ease.OutSine)
            .OnComplete(() => Application.LoadLevel(NextLevel)));
	}
	
	void Update ()
    {
	
	}
}
