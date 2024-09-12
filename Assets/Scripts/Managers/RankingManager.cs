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
    //各ヒット率リーダーボードのUI表示用
    public TextMeshProUGUI[] rankingHitRateTexts;
    //各ハイスコアリーダーボードのUI表示用
    public TextMeshProUGUI[] rankingHighScoreTexts;

    //各リーダーボードの結果を格納するリスト
    private List<Dictionary<string, int>> rankingHitRateList = new List<Dictionary<string, int>>();

    //各ハイスコアリーダーボードの結果を格納するリスト
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
    //ヒット率リーダーボードの取得を開始
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

        //リストをクリア
        rankingHitRateList.Clear();

        for (int i = 0; i < HitRateRankingNames.Length; i++)
        {
            RequestHitRateRanking(HitRateRankingNames[i], i);
        }
    }

    //ハイスコアリーダーボードの取得を開始
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

        //リストをクリア
        rankingHighScoreList.Clear();

        for (int i = 0; i < HighScoreRankingNames.Length; i++)
        {
            RequestHighScoreRanking(HighScoreRankingNames[i], i);
        }
    }

    //各ヒット率リーダーボードの取得
    private void RequestHitRateRanking(string statisticName, int index)
    {
        PlayFabClientAPI.GetLeaderboard(new GetLeaderboardRequest
        {
            StatisticName = statisticName,
            StartPosition = 0,
            MaxResultsCount = 10
        }, result =>
        {
            //各ヒット率リーダーボードの辞書
            Dictionary<string, int> hitRateRankingDictionary = new Dictionary<string, int>();

            result.Leaderboard.ForEach(x => { 
                if (!string.IsNullOrEmpty(x.DisplayName))
                {
                    //辞書に結果を格納
                    hitRateRankingDictionary[x.DisplayName] = x.StatValue;
                }
                else
                {
                    //nullの場合
                    Debug.LogWarning($"DisplayName is null for player with StatValue: {x.StatValue}");
                    hitRateRankingDictionary["Unknown Player"] = x.StatValue;   
                }
            });

            //リストに辞書を追加
            rankingHitRateList.Add(hitRateRankingDictionary);

            //UIの更新
            if (index < rankingHitRateTexts.Length)
            {
                UpdateHitRateRankingUI(result.Leaderboard, rankingHitRateTexts[index]);
            }
        }, error =>
        {
            Debug.LogError(error.GenerateErrorReport());
        });
    }

    //各ハイスコアリーダーボードの取得
    private void RequestHighScoreRanking(string statisticName, int index)
    {
        PlayFabClientAPI.GetLeaderboard(new GetLeaderboardRequest
        {
            StatisticName = statisticName,
            StartPosition = 0,
            MaxResultsCount = 10
        }, result =>
        {
            //各ハイスコアリーダーボードの辞書
            Dictionary<string, int> highScoreRankingDictionary = new Dictionary<string, int>();

            result.Leaderboard.ForEach(x =>
            {
                if (!string.IsNullOrEmpty(x.DisplayName))
                {
                    //辞書に結果を格納
                    highScoreRankingDictionary[x.DisplayName] = x.StatValue;
                }
                else
                {
                    //nullの場合
                    Debug.LogWarning($"DisplayName is null for player with StatValue: {x.StatValue}");
                    highScoreRankingDictionary["Unknown Player"] = x.StatValue;
                }
            });

            //リストに辞書を追加
            rankingHighScoreList.Add(highScoreRankingDictionary);

            //UIの更新
            if (index < rankingHighScoreTexts.Length)
            {
                UpdateHighScoreRankingUI(result.Leaderboard, rankingHighScoreTexts[index]);
            }
        }, error =>
        {
            Debug.LogError(error.GenerateErrorReport());
        });
    }

    //ヒット率UIを更新するメソッド
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

    //ハイスコアUIを更新するメソッド
    private void UpdateHighScoreRankingUI(List<PlayerLeaderboardEntry> leaderboard, TextMeshProUGUI textComponent)
    {
        textComponent.text = "";
        leaderboard.ForEach(x =>
        {
            textComponent.text += $"\n{x.Position + 1}. {x.DisplayName} : {x.StatValue}";
        });
    }


    //ヒット率リーダーボードの切り替え
    public void ShowHitRateRanking(int index)
    {
        //全てのヒット率リーダーボードを非表示にする
        for (int i = 0; i < rankingHitRateTexts.Length; i++)
        {
            rankingHitRateTexts[i].gameObject.SetActive(false);
        }

        //選択されたヒット率リーダーボードのテキストのみ表示する
        if (index >= 0 && index < rankingHitRateTexts.Length)
        {
            rankingHitRateTexts[index].gameObject.SetActive(true);
        }
    }

    //ハイスコアリーダーボードの切り替え
    public void ShowHighScoreRanking(int index)
    {
        //全てのハイスコアリーダーボードを非表示にする
        for (int i = 0; i < rankingHighScoreTexts.Length; i++)
        {
            rankingHighScoreTexts[i].gameObject.SetActive(false);
        }

        //選択されたハイスコアリーダーボードのテキストのみ表示する
        if (index >= 0 && index < rankingHighScoreTexts.Length)
        {
            rankingHighScoreTexts[index].gameObject.SetActive(true);
        }
    }

    //[スコアランキング取得]
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

    //[ネコパンチランキング取得]
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
                //Debug.Log($"{x.Position + 1}位: {x.DisplayName} " + $"総ネコパンチ数 {x.StatValue}");
                nekopunchScoreText.text += $"\n{x.Position + 1}. {x.DisplayName} " + $" : {x.StatValue}punch";
            });
        }, error =>
        {
            Debug.Log(error.GenerateErrorReport());
        }
        );
    }
}
