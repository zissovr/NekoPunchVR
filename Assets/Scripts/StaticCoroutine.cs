using System.Collections;
using UnityEngine;

public class StaticCoroutine : MonoBehaviour
{
    public static StaticCoroutine Instance { get; private set; }

    private void Start()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public static Coroutine Start(IEnumerator coroutine)
    {
        return Instance.StartCoroutine(coroutine);
    }

    public static void Stop(Coroutine coroutine)
    {
        Instance.StopCoroutine(coroutine);
    }
}
