using PlayFab.ClientModels;
using PlayFab;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Unity.VisualScripting;

public class RankingManager : MonoBehaviour
{
    public int rankingScore;
    public int nekopunchScore;

    [Header("UI Fields")]
    public TextMeshProUGUI rankingScoreText;
    public TextMeshProUGUI nekopunchScoreText;
    //�e�q�b�g�����[�_�[�{�[�h��UI�\���p
    public TextMeshProUGUI[] rankingHitRateTexts;
    //�e�n�C�X�R�A���[�_�[�{�[�h��UI�\���p
    public TextMeshProUGUI[] rankingHighScoreTexts;

    //�e���[�_�[�{�[�h�̌��ʂ��i�[���郊�X�g
    private List<Dictionary<string, int>> rankingHitRateList = new List<Dictionary<string, int>>();

    //�e�n�C�X�R�A���[�_�[�{�[�h�̌��ʂ��i�[���郊�X�g
    private List<Dictionary<string, int>> rankingHighScoreList = new List<Dictionary<string, int>>();

    public static RankingManager instance;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        instance = this;
    }
    //�q�b�g�����[�_�[�{�[�h�̎擾���J�n
    public void RequestAllHitRateRanking()
    {
        string[] HitRateRankingNames = new string[]
        {
            "HitRate_NyarwayOffshore",
            "HitRate_NyanadaOffshore",
            "HitRate_NyankasanOffshore",
            "HitRate_NyalaskaOffshore",
            "HitRate_NyaruOffshore",
            "HitRate_NyankyokukaiOffshore",
            "HitRate_NyaringkaiOffshore",
            "HitRate_NyaltkaiOffshore",
            "HitRate_NyakkaiOffshore"
        };

        //���X�g���N���A
        rankingHitRateList.Clear();

        for (int i = 0; i < HitRateRankingNames.Length; i++)
        {
            RequestHitRateRanking(HitRateRankingNames[i], i);
        }
    }

    //�n�C�X�R�A���[�_�[�{�[�h�̎擾���J�n
    public void RequestAllHighScoreRanking()
    {
        string[] HighScoreRankingNames = new string[]
        {
            "HighScore_NyarwayOffshore",
            "HighScore_NyanadaOffshore",
            "HighScore_NyankasanOffshore",
            "HighScore_NyalaskaOffshore",
            "HighScore_NyaruOffshore",
            "HighScore_NyankyokukaiOffshore",
            "HighScore_NyaringkaiOffshore",
            "HighScore_NyaltkaiOffshore",
            "HighScore_NyakkaiOffshore"
        };

        //���X�g���N���A
        rankingHighScoreList.Clear();

        for (int i = 0; i < HighScoreRankingNames.Length; i++)
        {
            RequestHighScoreRanking(HighScoreRankingNames[i], i);
        }
    }

    //�e�q�b�g�����[�_�[�{�[�h�̎擾
    private void RequestHitRateRanking(string statisticName, int index)
    {
        PlayFabClientAPI.GetLeaderboard(new GetLeaderboardRequest
        {
            StatisticName = statisticName,
            StartPosition = 0,
            MaxResultsCount = 10
        }, result =>
        {
            //�e�q�b�g�����[�_�[�{�[�h�̎���
            Dictionary<string, int> hitRateRankingDictionary = new Dictionary<string, int>();

            result.Leaderboard.ForEach(x => { 
                if (!string.IsNullOrEmpty(x.DisplayName))
                {
                    //�����Ɍ��ʂ��i�[
                    hitRateRankingDictionary[x.DisplayName] = x.StatValue;
                }
                else
                {
                    //null�̏ꍇ
                    Debug.LogWarning($"DisplayName is null for player with StatValue: {x.StatValue}");
                    hitRateRankingDictionary["Unknown Player"] = x.StatValue;   
                }
            });

            //���X�g�Ɏ�����ǉ�
            rankingHitRateList.Add(hitRateRankingDictionary);

            //UI�̍X�V
            if (index < rankingHitRateTexts.Length)
            {
                UpdateHitRateRankingUI(result.Leaderboard, rankingHitRateTexts[index]);
            }
        }, error =>
        {
            Debug.LogError(error.GenerateErrorReport());
        });
    }

    //�e�n�C�X�R�A���[�_�[�{�[�h�̎擾
    private void RequestHighScoreRanking(string statisticName, int index)
    {
        PlayFabClientAPI.GetLeaderboard(new GetLeaderboardRequest
        {
            StatisticName = statisticName,
            StartPosition = 0,
            MaxResultsCount = 10
        }, result =>
        {
            //�e�n�C�X�R�A���[�_�[�{�[�h�̎���
            Dictionary<string, int> highScoreRankingDictionary = new Dictionary<string, int>();

            result.Leaderboard.ForEach(x =>
            {
                if (!string.IsNullOrEmpty(x.DisplayName))
                {
                    //�����Ɍ��ʂ��i�[
                    highScoreRankingDictionary[x.DisplayName] = x.StatValue;
                }
                else
                {
                    //null�̏ꍇ
                    Debug.LogWarning($"DisplayName is null for player with StatValue: {x.StatValue}");
                    highScoreRankingDictionary["Unknown Player"] = x.StatValue;
                }
            });

            //���X�g�Ɏ�����ǉ�
            rankingHighScoreList.Add(highScoreRankingDictionary);

            //UI�̍X�V
            if (index < rankingHighScoreTexts.Length)
            {
                UpdateHighScoreRankingUI(result.Leaderboard, rankingHighScoreTexts[index]);
            }
        }, error =>
        {
            Debug.LogError(error.GenerateErrorReport());
        });
    }

    //�q�b�g��UI���X�V���郁�\�b�h
    private void UpdateHitRateRankingUI(List<PlayerLeaderboardEntry> leaderboard, TextMeshProUGUI textComponent)
    {
        textComponent.text = "";
        leaderboard.ForEach(x =>
        {
            float hitRateRaw = x.StatValue;
            float hitRate = hitRateRaw / 10;
            textComponent.text += $"\n{x.Position + 1}. {x.DisplayName} : {hitRate}%";
        });
    }

    //�n�C�X�R�AUI���X�V���郁�\�b�h
    private void UpdateHighScoreRankingUI(List<PlayerLeaderboardEntry> leaderboard, TextMeshProUGUI textComponent)
    {
        textComponent.text = "";
        leaderboard.ForEach(x =>
        {
            textComponent.text += $"\n{x.Position + 1}. {x.DisplayName} : {x.StatValue}";
        });
    }


    //�q�b�g�����[�_�[�{�[�h�̐؂�ւ�
    public void ShowHitRateRanking(int index)
    {
        //�S�Ẵq�b�g�����[�_�[�{�[�h���\���ɂ���
        for (int i = 0; i < rankingHitRateTexts.Length; i++)
        {
            rankingHitRateTexts[i].gameObject.SetActive(false);
        }

        //�I�����ꂽ�q�b�g�����[�_�[�{�[�h�̃e�L�X�g�̂ݕ\������
        if (index >= 0 && index < rankingHitRateTexts.Length)
        {
            rankingHitRateTexts[index].gameObject.SetActive(true);
        }
    }

    //�n�C�X�R�A���[�_�[�{�[�h�̐؂�ւ�
    public void ShowHighScoreRanking(int index)
    {
        //�S�Ẵn�C�X�R�A���[�_�[�{�[�h���\���ɂ���
        for (int i = 0; i < rankingHighScoreTexts.Length; i++)
        {
            rankingHighScoreTexts[i].gameObject.SetActive(false);
        }

        //�I�����ꂽ�n�C�X�R�A���[�_�[�{�[�h�̃e�L�X�g�̂ݕ\������
        if (index >= 0 && index < rankingHighScoreTexts.Length)
        {
            rankingHighScoreTexts[index].gameObject.SetActive(true);
        }
    }

    //[�X�R�A�����L���O�擾]
    public void RequestLeaderBoard()
    {
        PlayFabClientAPI.GetLeaderboard(new GetLeaderboardRequest
        {
            StatisticName = "HighScore",
            StartPosition = 0,
            MaxResultsCount = 10
        }, result =>
        {
            result.Leaderboard.ForEach(x => {
                //Debug.Log($"{x.Position + 1}. {x.DisplayName} " + $" : {x.StatValue}");
                rankingScoreText.text += $"\n{x.Position + 1}. {x.DisplayName} " + $" : {x.StatValue}";
            });
        }, error =>
        {
            Debug.Log(error.GenerateErrorReport());
        }
        );
    }

    //[�l�R�p���`�����L���O�擾]
    public void RequestNekopunchLeaderBoard()
    {
        PlayFabClientAPI.GetLeaderboard(new GetLeaderboardRequest
        {
            StatisticName = "NekoPunch",
            StartPosition = 0,
            MaxResultsCount = 10
        }, result =>
        {
            result.Leaderboard.ForEach(x => {
                //Debug.Log($"{x.Position + 1}��: {x.DisplayName} " + $"���l�R�p���`�� {x.StatValue}");
                nekopunchScoreText.text += $"\n{x.Position + 1}. {x.DisplayName} " + $" : {x.StatValue}punch";
            });
        }, error =>
        {
            Debug.Log(error.GenerateErrorReport());
        }
        );
    }
}
