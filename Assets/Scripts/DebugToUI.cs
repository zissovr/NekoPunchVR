using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DebugToUI : MonoBehaviour
{
    public TextMeshProUGUI debugText;

    private static DebugToUI instance;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        instance = this;
    }

    public static void Log(string message)
    {
        //Debug.Log(message);
        if (instance != null && instance.debugText != null)
        {
            instance.debugText.text += message + "\n";
        }
    }
}
