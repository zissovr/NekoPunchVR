using System.Collections;
using System.Dynamic;
using UnityEngine;

public class StaticCoroutine : MonoBehaviour
{
    public static StaticCoroutine Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public static Coroutine Start(IEnumerator coroutine)
    {
        if (Instance == null)
        {
            CreateInstance();
        }

        return Instance.StartCoroutine(coroutine);
    }

    public static void Stop(Coroutine coroutine)
    {
        if (Instance == null)
        {
            Debug.LogError("StaticCoroutine instance is null when trying to stop a coroutine.");
            return;
        }

        Instance.StopCoroutine(coroutine);
    }

    private static void CreateInstance()
    {
        GameObject obj = new GameObject("StaticCoroutine");
        Instance = obj.AddComponent<StaticCoroutine>();
        DontDestroyOnLoad(obj);
    }
}
