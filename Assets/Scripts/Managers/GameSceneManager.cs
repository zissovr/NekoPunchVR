using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;

public class GameSceneManager : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI timeText;
    public Image progressBarImage;
    public GameObject timerUI_Gameobject;
    public GameObject restartButtonUI_Gameobject;
    //スコア表示切り替えのオブジェクト設定
    public GameObject currentScoreUI_Gameobject;
    public GameObject finalScoreUI_Gameobject;
    public GameObject rankingUI_Gameobject;
    public GameObject nekopunchUI_Gameobject;
    //鏡のオブジェクト
    public GameObject mirror_Gameobject;

    [Header("Managers")]
    public GameObject fishSpawnManager;

    [Header("XRInteraction")]
    //RayInteractorの取得
    public GameObject rightRayInteractor;
    public GameObject leftRayInteractor;
    XRRayInteractor rightxrRay;
    XRRayInteractor leftxrRay;
    //Moveの取得
    public GameObject moveLocomotion;

    //オーディオの長さの変数
    float audioClipLength;

    private void Start()
    {
        //RayInteractorの取得と非表示
        RayInteractorDeactivate();

        //曲の長さを取得
        audioClipLength = AudioManager.instance.musicTheme.clip.length;

        //曲のカウントダウンをスタートさせる
        StartCoroutine(StartCountdown(audioClipLength));

        //プログレスバーを初期化
        progressBarImage.fillAmount = Mathf.Clamp(0, 0, 1);

        //確定スコア・ランキング非表示と現在スコア表示
        finalScoreUI_Gameobject.SetActive(false);
        rankingUI_Gameobject.SetActive(false);
        nekopunchUI_Gameobject.SetActive(false);
        currentScoreUI_Gameobject.SetActive(true);
        restartButtonUI_Gameobject.SetActive(false);

        //鏡の非表示
        mirror_Gameobject.SetActive(false);

        //移動を制限
        moveLocomotion.SetActive(false);
    }

    //RayInteractorの取得と非表示
    public void RayInteractorDeactivate()
    {
        rightxrRay = rightRayInteractor.GetComponent<XRRayInteractor>();
        leftxrRay = leftRayInteractor.GetComponent<XRRayInteractor>();
        rightxrRay.enabled = false;
        leftxrRay.enabled = false;
    }

    public IEnumerator StartCountdown(float countdownValue)
    {
        while (countdownValue > 0)
        {
            yield return new WaitForSeconds(1.0f);
            countdownValue -= 1.0f;

            timeText.text = ConvertToMinAndSeconds(countdownValue);

            progressBarImage.fillAmount = (AudioManager.instance.musicTheme.time / audioClipLength);
        }
        TimeOver();
    }

    //タイムオーバー処理
    public void TimeOver()
    {
        Debug.Log("Time Over");
        timeText.text = ConvertToMinAndSeconds(0);

        //スポーン終了
        fishSpawnManager.SetActive(false);

        //確定スコア・ランキング表示と現在スコア非表示
        finalScoreUI_Gameobject.SetActive(true);
        rankingUI_Gameobject.SetActive(true);
        nekopunchUI_Gameobject.SetActive(true);
        currentScoreUI_Gameobject.SetActive(false);
        restartButtonUI_Gameobject.SetActive(true);

        //鏡の非表示
        mirror_Gameobject.SetActive(true);

        //レイの表示
        rightxrRay.enabled = true;
        leftxrRay.enabled = true;

        //移動を開放
        moveLocomotion.SetActive(true);

        //ランキングの取得
        RankingManager.instance.RequestLeaderBoard();

        //ネコパンチ総数の送信
        PlayfabManager.instance.SubmitNumberOfNekopunch();

        //ネコパンチ総数の取得
        RankingManager.instance.RequestNekopunchLeaderBoard();
    }

    //分と秒に変換処理
    public string ConvertToMinAndSeconds(float totalTimeInSeconds)
    {
        string timeText = Mathf.Floor(totalTimeInSeconds / 60).ToString("00") + ":" + Mathf.FloorToInt(totalTimeInSeconds % 60).ToString("00");
        return timeText;
    }

    //リスタートボタン
    public void OnClickBackToScene()
    {
        SceneManager.LoadScene("SampleScene");
    }
}
