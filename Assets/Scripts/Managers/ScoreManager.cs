using System.Collections;
using System.Collections.Generic;
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
}
