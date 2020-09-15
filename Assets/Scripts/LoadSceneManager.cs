using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadSceneManager : MonoBehaviour
{
    public Fader ScreenFader = null;
    public string NextSceneName;

    private bool _IsLoading = false;

    public void StartTransitToNextScene()
    {
        StartCoroutine(TransitToNextScene());
    }

    private IEnumerator TransitToNextScene()
    {
        if (!_IsLoading)
        {
            _IsLoading = true;
            TransitionSceneManager.Instance.PreTransition();
            yield return ScreenFader ? ScreenFader.StartFadeIn() : null;
            TransitionSceneManager.Instance.NextSceneName = NextSceneName;
            TransitionSceneManager.Instance.StartLoadNextScene();           
        }
    }
}
