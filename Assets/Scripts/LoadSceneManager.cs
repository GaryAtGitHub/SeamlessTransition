using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadSceneManager : MonoBehaviour
{
    public Fader ScreenFader = null;
    public string NextSceneName;

    private float _AnimationSpeed = 0.5f;
    private bool _IsLoading = false;

    public void StartTransitToNextScene()
    {
        StartCoroutine(TransitToNextScene());
    }

    private IEnumerator TransitToNextScene()
    {
        if (!TransitionSceneManager.Instance._IsLoading)
        {
            if (!_IsLoading)
            {
                _IsLoading = true;
                yield return StartCoroutine(Shrink());
                TransitionSceneManager.Instance.PreTransition();
                yield return ScreenFader ? ScreenFader.StartFadeIn() : null;
                TransitionSceneManager.Instance.NextSceneName = NextSceneName;
                TransitionSceneManager.Instance.StartLoadNextScene();
            }
        }
    }

    private IEnumerator Shrink()
    {
        float t = 0;
        Vector3 originalScel = gameObject.transform.localScale;
        while (t < _AnimationSpeed)
        {
            yield return null;
            t += Time.deltaTime;
            Vector3 newScale = new Vector3(originalScel.x * (1 - t / _AnimationSpeed), originalScel.y * (1 - t / _AnimationSpeed), originalScel.z * (1 - t / _AnimationSpeed));
        gameObject.transform.localScale = newScale;
        }        
    }
}
