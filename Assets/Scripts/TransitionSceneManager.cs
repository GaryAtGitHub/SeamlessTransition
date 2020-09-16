using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TransitionSceneManager : MonoBehaviour
{
    public string NextSceneName;
    public bool IsStartFadeout { get; private set; } = true;
    public Action OnPreTransition;
    public Action OnEnterNewScene;
    public bool _IsLoading { get; private set; }

    [SerializeField]
    private Camera _TransitionCamera;
    [SerializeField]
    private Camera _BackgroundCamera;
    [SerializeField]
    private LoadIndicator _LoadIndicator;

    [SerializeField]
    private GameObject[] _Gos;

    public static TransitionSceneManager Instance;

    public void StartLoadNextScene()
    {
        if (!_IsLoading)
        {
            StartCoroutine(LoadScene(NextSceneName));
        }
    }

    public void PreTransition()
    {
        foreach (var go in _Gos)
        {
            if (go != gameObject)
            {
                go?.SetActive(true);
            }
        }
        EnableCameras();
        OnPreTransition?.Invoke();
    }

    private void Awake()
    {
        //Singleton
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }

        //Store all gameobjects of TransitionScene
        _Gos = SceneManager.GetActiveScene().GetRootGameObjects();
        SceneManager.sceneLoaded += SetLoadedSceneActive;
        StartCoroutine(InitCameras());
    }

    private void SetLoadedSceneActive(Scene scene, LoadSceneMode arg1)
    {
        SceneManager.SetActiveScene(scene);
    }

    //Turn off Camera at the End of Frame to avoid a camera flicker bug when first enabled during runtime
    IEnumerator InitCameras()
    {        
        yield return new WaitForEndOfFrame();
        EnableCameras(false);
    }

    // Start is called before the first frame update
    void Start()
    {
        //Load the Scene with index 1 in build
        StartCoroutine(LoadAdditively(1));
        StartCoroutine(DisableGameObjects());
    }

    IEnumerator DisableGameObjects()
    {
        yield return new WaitForEndOfFrame();
        foreach (var go in _Gos)
        {
            if (go != gameObject)
            {
                go?.SetActive(false);
            }
        }
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }

    private void EnableCameras(bool value = true)
    {
        _TransitionCamera.enabled = value;
        _BackgroundCamera.enabled = value;
    }

    private IEnumerator LoadScene(string nextSceneName, string sceneToUnload = null)
    {
        _IsLoading = true;
        IsStartFadeout = false;

        foreach (GameObject go in _Gos)
        {
            if (go != gameObject)
            {
                go?.SetActive(true);
            }
        }


        // if no scene is provided in argument, unload the current active scene
        if (sceneToUnload == null)
        {
            sceneToUnload = SceneManager.GetActiveScene().name;
        }

        if (sceneToUnload == gameObject.scene.name)
        {
            Debug.LogWarning("Try to unload TransitionScene.");
            yield break;
        }

        yield return StartCoroutine(UnLoadScene(sceneToUnload));

        yield return StartCoroutine(LoadAdditively());

        if (_LoadIndicator != null)
        {
            while (!_LoadIndicator.IsPop)
            {
                yield return null;
            }
        }

        _BackgroundCamera.enabled = false;
        IsStartFadeout = true;
        OnEnterNewScene.Invoke();

        if (_LoadIndicator != null)
        {
            while (_LoadIndicator.IsPop)
            {
                yield return null;
            }
        }

        foreach (var go in _Gos)
        {
            if (go != gameObject)
            {
                go?.SetActive(false);
            }
        }

        _IsLoading = false;
    }

    private IEnumerator LoadAdditively(int index = -1)
    {
        if (NextSceneName == null && index < 0)
        {
            yield break;
        }

        AsyncOperation asyncLoad;

        if (index < 0)
        {
            asyncLoad = SceneManager.LoadSceneAsync(NextSceneName, LoadSceneMode.Additive);
        }
        else
        {
            asyncLoad = SceneManager.LoadSceneAsync(index, LoadSceneMode.Additive);
        }

        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        NextSceneName = null;
    }

    private IEnumerator UnLoadScene(string sceneName)
    {
        if (SceneManager.sceneCount == 1)
        {
            Debug.LogError("Erro: try to unload the last remaining loaded scene");
            yield break;
        }

        AsyncOperation asyncUnLoad = SceneManager.UnloadSceneAsync(sceneName);

        while (!asyncUnLoad.isDone)
        {
            yield return null;
        }
    }
}
