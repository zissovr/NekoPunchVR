using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NekopunchManager : MonoBehaviour
{
    public static NekopunchManager instance;

    [Header("UI Fields")]
    public TextMeshProUGUI nekoPunchText;
    public TextMeshProUGUI dailyNekoPunchText;

    public int nekoPunch;
    public int dailyNekoPunchCount = 0;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        instance = this;
    }

    void Start()
    {
        //今までの総猫パンチ数を取得し更新
        nekoPunch = PlayerPrefs.GetInt("TotalNekoPunch", 0);
        nekoPunchText.text = nekoPunch.ToString();
    }

    public void AddNekoPunch(int nekoPunchPoint)
    {
        nekoPunch = nekoPunch + nekoPunchPoint;
        PlayerPrefs.SetInt("TotalNekoPunch", nekoPunch);

        //ネコパンチ数をアップデート
        nekoPunchText.text = nekoPunch.ToString();

        //デイリー猫パンチ数を加算
        dailyNekoPunchCount += nekoPunchPoint;
    }
    
}
