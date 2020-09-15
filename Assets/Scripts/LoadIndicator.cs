using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadIndicator : MonoBehaviour
{
    public bool IsPop { get; private set; }

    [SerializeField]
    private float _SpinSpeed = 2;
    [SerializeField]
    private float _SpinRound = 5;

    private void Start()
    {        
        TransitionSceneManager.Instance.OnEnterNewScene += StartVanish;
        TransitionSceneManager.Instance.OnPreTransition += StartPop;
        //gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        TransitionSceneManager.Instance.OnEnterNewScene -= StartVanish;
        TransitionSceneManager.Instance.OnPreTransition -= StartPop;
    }

    public void StartPop()
    {
        gameObject.SetActive(true);
        StartCoroutine(Pop());
    }

    private IEnumerator Pop()
    {
        float t = 0;
        transform.localScale = new Vector3(0, 0, 0);

        while (t < _SpinSpeed + 0.01)
        {            
            yield return null;
            Mathf.Clamp(t = t + Time.deltaTime, 0, 2);
            float s = Mathf.Sin(t / _SpinSpeed * Mathf.PI / 2);

            float rotation = s * _SpinRound * 360;
            transform.localScale = new Vector3(s, s, s);
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, rotation, transform.eulerAngles.z);
        }
        IsPop = true;
    }

    public void StartVanish()
    {
        StartCoroutine(Vanish());
    }

    private IEnumerator Vanish()
    {
        float t = 0;
        float initAngle = transform.eulerAngles.y;
        while (t < _SpinSpeed + 0.01)
        {
            yield return null;
            Mathf.Clamp(t = t + Time.deltaTime, 0, 2);
            float s = Mathf.Sin((1 - t / _SpinSpeed) * Mathf.PI / 2);

            float rotation = (1 - s) * _SpinRound * 360;
            transform.localScale = new Vector3(s, s, s);
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, initAngle + rotation, transform.eulerAngles.z);
        }
        IsPop = false;
        gameObject.SetActive(false);
    }
}
