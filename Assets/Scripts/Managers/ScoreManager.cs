using System;
using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    [Header("UI Fields")]
    public TextMeshProUGUI currentScoreText;
    public TextMeshProUGUI highScoreText;
    public TextMeshProUGUI finalScoreText;

    public static ScoreManager instance;

    public int currentScore;
    public int highScore;

    public int SpawnedFish;
    public int HitFish;
    public int MissedFish;
    public float hitRate;
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
        highScore = PlayerPrefs.GetInt("HighScore", 0);
        highScoreText.text = highScore.ToString();

        currentScoreText.text = "0";
    }

    public void AddScore(int scorePoint)
    {
        currentScore = currentScore + scorePoint;
        PlayerPrefs.SetInt("CurrentScore", currentScore);

        //現在のスコアをUIに表示する
        currentScoreText.text = currentScore.ToString();

        //確定スコアをアップデート
        finalScoreText.text = currentScore.ToString();

        //ハイスコア更新ならアップデート
        if (currentScore > PlayerPrefs.GetInt("HighScore", 0))
        {
            PlayerPrefs.SetInt("HighScore", currentScore);
            highScoreText.text = currentScore.ToString();
            PlayfabManager.instance.SubmitScore(currentScore);
        }
    }

    //ヒット率を算出
    public float HitRate()
    {
        hitRate = ((float)HitFish / (float)SpawnedFish) * 100f;
        hitRate = (float)Math.Round(hitRate, 1, MidpointRounding.AwayFromZero);
        return hitRate;
    }
}
