using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    [Header("UI Fields")]
    public TextMeshProUGUI currentScoreText;
    public TextMeshProUGUI highScoreText;
    public TextMeshProUGUI highHitRateText;
    public TextMeshProUGUI finalScoreText;
    public TextMeshProUGUI finalHitRateText;

    public static ScoreManager instance;

    public int currentScore;
    public int highScore;

    public int SpawnedFish;
    public int HitFish;
    public int MissedFish;
    public float hitRate;
    public float highHitRate;
    public float Accuracy => HitFish / (float)(HitFish + MissedFish);

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        highScore = PlayerPrefs.GetInt($"HighScore_{GameSceneManager.instance.currentState}", 0);

        currentScoreText.text = "0";

        highHitRate = PlayerPrefs.GetFloat($"HighHitRate_{GameSceneManager.instance.currentState}", 0);

        //初期化
        currentScore = 0;
        hitRate = 0;

        //10秒後にハイスコアを更新しておく
        StartCoroutine(UpdateHighScore());

        //10秒後にハイヒット率を更新しておく
        StartCoroutine(UpdateHighHitRate());
    }

    public void AddScore(int scorePoint)
    {
        currentScore = currentScore + scorePoint;
        PlayerPrefs.SetInt($"CurrentScore_{GameSceneManager.instance.currentState}", currentScore);

        //現在のスコアをUIに表示する
        currentScoreText.text = currentScore.ToString();

        //確定スコアをアップデート
        finalScoreText.text = currentScore.ToString();

        //ハイスコア更新ならアップデート
        if (currentScore > PlayerPrefs.GetInt($"HighScore_{GameSceneManager.instance.currentState}", 0))
        {
            PlayerPrefs.SetInt($"HighScore_{GameSceneManager.instance.currentState}", currentScore);
            highScoreText.text = currentScore.ToString();
            //PlayfabManager.instance.SubmitScore(currentScore);
        }
    }

    //ヒット率を算出
    public void HitRate()
    {
        hitRate = ((float)HitFish / (float)SpawnedFish) * 100f;
        hitRate = (float)Math.Round(hitRate, 1, MidpointRounding.AwayFromZero);

        //今回のヒット率をセット
        PlayerPrefs.SetFloat($"CurrentHitRate_{GameSceneManager.instance.currentState}", hitRate);

        //最高のヒット率をUIに表示
        highHitRateText.text = $"{highHitRate:F1}%";

        //今回のヒット率をUIに表示
        finalHitRateText.text = $"{hitRate:F1}%";

        //ヒット率更新ならアップデート
        if (hitRate > PlayerPrefs.GetFloat($"HighHitRate_{GameSceneManager.instance.currentState}", 0))
        {
            PlayerPrefs.SetFloat($"HighHitRate_{GameSceneManager.instance.currentState}", hitRate);
            highHitRateText.text = $"{hitRate:F1}%";
            //PlayfabManager.instance.SubmitHitRate(hitRate);
        }
    }

    //最終のスコアとヒット率を初期化
    public void InitializeCurrentScoreAndHitRate()
    {
        finalScoreText.text = "0";
        finalHitRateText.text = "0.0%";
    }

    //ハイスコアをPlayfabから取得するメソッド
    public IEnumerator UpdateHighScore()
    {
        yield return new WaitForSeconds(10);

        highScore = PlayfabManager.instance.currentStagePlayerHighScore;

        highScoreText.text = highScore.ToString();
    }

    //ハイヒット率をPlayfabから取得するメソッド
    public IEnumerator UpdateHighHitRate()
    {
        yield return new WaitForSeconds(10);

        float currentStageHighHitRate = PlayfabManager.instance.currentStagePlayerHitRate;
        highHitRate = currentStageHighHitRate / 10;

        highHitRateText.text = $"{highHitRate:F1}%";
    }

    //スコアとヒット率を比較し更新が必要か確認するメソッド
    public void CompareAndSubmitScores()
    {
        //ハイスコアチェック
        if (currentScore > highScore)
        {
            PlayfabManager.instance.SubmitHighScore(currentScore);

            //UI表示
            highScoreText.text = currentScore.ToString();

            //お祝い演出
        }

        //ハイヒット率チェック
        if (hitRate > highHitRate)
        {
            PlayfabManager.instance.submitHighHitRate(hitRate);

            //UI表示
            highHitRateText.text = $"{hitRate:F1}%";

            //お祝い演出
        }
    }
}
