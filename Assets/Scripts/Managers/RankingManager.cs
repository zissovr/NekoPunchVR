using PlayFab.ClientModels;
using PlayFab;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RankingManager : MonoBehaviour
{
    public int rankingScore;
    public int nekopunchScore;

    [Header("UI Fields")]
    public TextMeshProUGUI rankingScoreText;
    public TextMeshProUGUI nekopunchScoreText;

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
