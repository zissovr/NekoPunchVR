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

        //������
        currentScore = 0;
        hitRate = 0;

        //10�b��Ƀn�C�X�R�A���X�V���Ă���
        StartCoroutine(UpdateHighScore());

        //10�b��Ƀn�C�q�b�g�����X�V���Ă���
        StartCoroutine(UpdateHighHitRate());
    }

    public void AddScore(int scorePoint)
    {
        currentScore = currentScore + scorePoint;
        PlayerPrefs.SetInt($"CurrentScore_{GameSceneManager.instance.currentState}", currentScore);

        //���݂̃X�R�A��UI�ɕ\������
        currentScoreText.text = currentScore.ToString();

        //�m��X�R�A���A�b�v�f�[�g
        finalScoreText.text = currentScore.ToString();

        //�n�C�X�R�A�X�V�Ȃ�A�b�v�f�[�g
        if (currentScore > PlayerPrefs.GetInt($"HighScore_{GameSceneManager.instance.currentState}", 0))
        {
            PlayerPrefs.SetInt($"HighScore_{GameSceneManager.instance.currentState}", currentScore);
            highScoreText.text = currentScore.ToString();
            //PlayfabManager.instance.SubmitScore(currentScore);
        }
    }

    //�q�b�g�����Z�o
    public void HitRate()
    {
        hitRate = ((float)HitFish / (float)SpawnedFish) * 100f;
        hitRate = (float)Math.Round(hitRate, 1, MidpointRounding.AwayFromZero);

        //����̃q�b�g�����Z�b�g
        PlayerPrefs.SetFloat($"CurrentHitRate_{GameSceneManager.instance.currentState}", hitRate);

        //�ō��̃q�b�g����UI�ɕ\��
        highHitRateText.text = $"{highHitRate:F1}%";

        //����̃q�b�g����UI�ɕ\��
        finalHitRateText.text = $"{hitRate:F1}%";

        //�q�b�g���X�V�Ȃ�A�b�v�f�[�g
        if (hitRate > PlayerPrefs.GetFloat($"HighHitRate_{GameSceneManager.instance.currentState}", 0))
        {
            PlayerPrefs.SetFloat($"HighHitRate_{GameSceneManager.instance.currentState}", hitRate);
            highHitRateText.text = $"{hitRate:F1}%";
            //PlayfabManager.instance.SubmitHitRate(hitRate);
        }
    }

    //�ŏI�̃X�R�A�ƃq�b�g����������
    public void InitializeCurrentScoreAndHitRate()
    {
        finalScoreText.text = "0";
        finalHitRateText.text = "0.0%";
    }

    //�n�C�X�R�A��Playfab����擾���郁�\�b�h
    public IEnumerator UpdateHighScore()
    {
        yield return new WaitForSeconds(10);

        highScore = PlayfabManager.instance.currentStagePlayerHighScore;

        highScoreText.text = highScore.ToString();
    }

    //�n�C�q�b�g����Playfab����擾���郁�\�b�h
    public IEnumerator UpdateHighHitRate()
    {
        yield return new WaitForSeconds(10);

        float currentStageHighHitRate = PlayfabManager.instance.currentStagePlayerHitRate;
        highHitRate = currentStageHighHitRate / 10;

        highHitRateText.text = $"{highHitRate:F1}%";
    }

    //�X�R�A�ƃq�b�g�����r���X�V���K�v���m�F���郁�\�b�h
    public void CompareAndSubmitScores()
    {
        //�n�C�X�R�A�`�F�b�N
        if (currentScore > highScore)
        {
            PlayfabManager.instance.SubmitHighScore(currentScore);

            //UI�\��
            highScoreText.text = currentScore.ToString();

            //���j�����o
        }

        //�n�C�q�b�g���`�F�b�N
        if (hitRate > highHitRate)
        {
            PlayfabManager.instance.submitHighHitRate(hitRate);

            //UI�\��
            highHitRateText.text = $"{hitRate:F1}%";

            //���j�����o
        }
    }
}
