using PlayFab.ClientModels;
using PlayFab;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using PlayFab.Json;

public class PlayfabManager : MonoBehaviour
{
    public static PlayfabManager instance;

    //アカウントを作成するか
    private bool _shouldCreateAccount;

    //ログイン時に使うID
    private string _customID;

    // 取得したMaster Player Account IDを保存する変数
    public string masterPlayerAccountID;

    //ハイスコアリーダーボードの名前ごとにハイスコアを保持する変数
    public string HighScore_NyarwayOffshore;
    public string HighScore_NyanadaOffshore;
    public string HighScore_NyankasanOffshore;
    public string HighScore_NyalaskaOffshore;
    public string HighScore_NyaruOffshore;
    public string HighScore_NyankyokukaiOffshore;
    public string HighScore_NyaringkaiOffshore;
    public string HighScore_NyaltkaiOffshore;
    public string HighScore_NyakkaiOffshore;

    //ヒット率リーダーボードの名前ごとにヒット率を保持する変数
    public string HitRate_NyarwayOffshore;
    public string HitRate_NyanadaOffshore;
    public string HitRate_NyankasanOffshore;
    public string HitRate_NyalaskaOffshore;
    public string HitRate_NyaruOffshore;
    public string HitRate_NyankyokukaiOffshore;
    public string HitRate_NyaringkaiOffshore;
    public string HitRate_NyaltkaiOffshore;
    public string HitRate_NyakkaiOffshore;

    //リーダーボード名とハイスコア変数のマッピング
    private Dictionary<string, string> highScoreMap = new Dictionary<string, string>();

    //リーダーボード名とヒット率変数のマッピング
    private Dictionary<string, string> hitRateMap = new Dictionary<string, string>();

    //ハイスコアリーダーボード名を格納するリスト
    private List<string> leaderboardNames = new List<string>
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

    //ヒット率リーダーボード名を格納するリスト
    private List<string> leaderboardHitRateNames = new List<string>
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

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        instance = this;
    }
    private void Start()
    {
        //クリエイトアカウントフラグ
        _customID = LoadCustomID();
        _shouldCreateAccount = string.IsNullOrEmpty(_customID);

        Login();

        //ハイスコアリーダーボード：辞書の初期化
        InitializeLeaderboardDictionary();

        //ヒット率リーダーボード：辞書の初期化
        InitializeHitRateLeaderboardDictionary();
    }

    //ログイン実行
    private void Login()
    {
        //アカウントを作成するかどうか
        var request = new LoginWithCustomIDRequest { CustomId = _customID, CreateAccount = _shouldCreateAccount };

        //ログイン
        PlayFabClientAPI.LoginWithCustomID(request, OnLoginSuccess, OnLoginFailure);
    }

    //ログイン成功
    private void OnLoginSuccess(LoginResult result)
    {
        //アカウント作成時にIDがすでに使われていた場合
        if (_shouldCreateAccount && !result.NewlyCreated)
        {
            Debug.LogWarning($"CustomId : {_customID}はすでに使われています");
            DebugToUI.Log($"CustomId : {_customID}is already in use");
            Login();
            return;
        }

        //アカウント作成時にIDを保存
        if (result.NewlyCreated)
        {
            SaveCustomID();
        }
        Debug.Log($"PlayFabのログインに成功\nPlayFabId : {result.PlayFabId}, CustomId : {_customID}\nアカウントを作成したか : {result.NewlyCreated}");
        DebugToUI.Log($"Successful PlayFab login\nPlayFabId : {result.PlayFabId}, CustomId : {_customID}\nHave you created an account : {result.NewlyCreated}");

        //PlayFabIdをセット
        masterPlayerAccountID = result.PlayFabId;

        //ログイン時にデイリーパンチ数を取得
        GetDailyPunchCount();

        //ログイン時にハイスコアとヒット率のリーダーボードを取得
        GetAllLeaderboardValues();
        GetAllHitRateLeaderboardValues();
    }

    //ログイン失敗
    private void OnLoginFailure(PlayFabError error)
    {
        Debug.LogError($"PlayFabのログインに失敗\nエラーコード: {error.Error} - 詳細: {error.ErrorDetails}\n{error.GenerateErrorReport()}");
        DebugToUI.Log($"PlayFab login failedError code: {error.Error} - Details: {error.ErrorDetails}\n{error.GenerateErrorReport()}");

        if (error.Error == PlayFabErrorCode.AccountNotFound)
        {
            DebugToUI.Log($"Account not found. Create a new account.");
            _shouldCreateAccount = true;
            // 新しいアカウントを作成するために再試行
            Login();
        }
    }


    //[カスタムIDの取得]
    //IDを保存する時のKEY
    private static readonly string CUSTOM_ID_SAVE_KEY = "CUSTOM_ID_SAVE_KEY";

    //IDを取得
    private string LoadCustomID()
    {
        //IDを取得
        string id = PlayerPrefs.GetString(CUSTOM_ID_SAVE_KEY);

        //保存されていなければ新規作成
        if (string.IsNullOrEmpty(id))
        {
            _shouldCreateAccount = true;
            return GenerateCustomID();
        } else
        {
            _shouldCreateAccount = false;
            return id;
        }
    }

    //IDの保存
    private void SaveCustomID()
    {
        PlayerPrefs.SetString(CUSTOM_ID_SAVE_KEY, _customID);
    }

    //[カスタムIDの生成]
    //IDに使用する文字列
    private static readonly string ID_CHARACTERS = "0123456789abcdefghijklmnopqrstuvwxyz";

    //IDを生成する
    private string GenerateCustomID()
    {
        //IDの長さ
        int idLength = 32;
        StringBuilder stringBuilder = new StringBuilder(idLength);
        var random = new System.Random();

        //ランダムにIDを生成
        for (int i = 0; i < idLength; i++)
        {
            stringBuilder.Append(ID_CHARACTERS[random.Next(ID_CHARACTERS.Length)]);
        }

        return stringBuilder.ToString();
    }


    //[スコアランキング更新]
    public void SubmitScore(int playerScore)
    {
        PlayFabClientAPI.UpdatePlayerStatistics(new UpdatePlayerStatisticsRequest
        {
            Statistics = new List<StatisticUpdate>
            {
                new StatisticUpdate
                {
                    StatisticName = $"HighScore_{GameSceneManager.instance.currentState}",
                    Value = playerScore
                }
            }
        }, result =>
        {
            //Debug.Log("スコア送信完了！");
        }, error =>
        {
            Debug.Log(error.GenerateErrorReport());
        });
    }

    //[スコアランキング更新]
    public void SubmitHitRate(float hitRate)
    {
        PlayFabClientAPI.UpdatePlayerStatistics(new UpdatePlayerStatisticsRequest
        {
            Statistics = new List<StatisticUpdate>
            {
                new StatisticUpdate
                {
                    StatisticName = $"HitRate_{GameSceneManager.instance.currentState}",
                    Value = (int)(hitRate * 10f)
                }
            }
        }, result =>
        {
            //Debug.Log("ヒット率送信完了！");
        }, error =>
        {
            Debug.Log(error.GenerateErrorReport());
        });
    }

    //辞書の初期化：ハイスコアリーダーボード名をキーに変数を参照する設定
    private void InitializeLeaderboardDictionary()
    {
        highScoreMap.Add("HighScore_NyarwayOffshore", nameof(HighScore_NyarwayOffshore));
        highScoreMap.Add("HighScore_NyanadaOffshore", nameof(HighScore_NyanadaOffshore));
        highScoreMap.Add("HighScore_NyankasanOffshore", nameof(HighScore_NyankasanOffshore));
        highScoreMap.Add("HighScore_NyalaskaOffshore", nameof(HighScore_NyalaskaOffshore));
        highScoreMap.Add("HighScore_NyaruOffshore", nameof(HighScore_NyaruOffshore));
        highScoreMap.Add("HighScore_NyankyokukaiOffshore", nameof(HighScore_NyankyokukaiOffshore));
        highScoreMap.Add("HighScore_NyaringkaiOffshore", nameof(HighScore_NyaringkaiOffshore));
        highScoreMap.Add("HighScore_NyaltkaiOffshore", nameof(HighScore_NyaltkaiOffshore));
        highScoreMap.Add("HighScore_NyakkaiOffshore", nameof(HighScore_NyakkaiOffshore));
    }

    //辞書の初期化：ヒット率リーダーボード名をキーに変数を参照する設定
    private void InitializeHitRateLeaderboardDictionary()
    {
        hitRateMap.Add("HitRate_NyarwayOffshore", nameof(HitRate_NyarwayOffshore));
        hitRateMap.Add("HitRate_NyanadaOffshore", nameof(HitRate_NyanadaOffshore));
        hitRateMap.Add("HitRate_NyankasanOffshore", nameof(HitRate_NyankasanOffshore));
        hitRateMap.Add("HitRate_NyalaskaOffshore", nameof(HitRate_NyalaskaOffshore));
        hitRateMap.Add("HitRate_NyaruOffshore", nameof(HitRate_NyaruOffshore));
        hitRateMap.Add("HitRate_NyankyokukaiOffshore", nameof(HitRate_NyankyokukaiOffshore));
        hitRateMap.Add("HitRate_NyaringkaiOffshore", nameof(HitRate_NyaringkaiOffshore));
        hitRateMap.Add("HitRate_NyaltkaiOffshore", nameof(HitRate_NyaltkaiOffshore));
        hitRateMap.Add("HitRate_NyakkaiOffshore", nameof(HitRate_NyakkaiOffshore));
    }

    //すべてのハイスコアリーダーボードの値を一気に取得するメソッド
    public void GetAllLeaderboardValues()
    {
        foreach(string leaderboardName in GetLeaderboardNames())
        {
            GetLeaderboardValue(leaderboardName);
        }
    }

    //すべてのヒット率リーダーボードの値を一気に取得するメソッド
    public void GetAllHitRateLeaderboardValues()
    {
        foreach (string hitRateLeaderboardName in GetHitRateLeaderboardNames())
        {
            GetHitRateLeaderboardValue(hitRateLeaderboardName);
        }
    }

    //各ハイスコアリーダーボードの値を取得するメソッド
    public void GetLeaderboardValue(string leaderboardName)
    {
        var request = new GetLeaderboardAroundPlayerRequest
        {
            StatisticName = leaderboardName,
            MaxResultsCount = 1 // 自分のみを取得
        };

        PlayFabClientAPI.GetLeaderboardAroundPlayer(request, result => OnGetLeaderboardSuccess(result, leaderboardName), OnGetLeaderboardError);
    }

    //各ヒット率リーダーボードの値を取得するメソッド
    public void GetHitRateLeaderboardValue(string hitRateLeaderboardName)
    {
        var request = new GetLeaderboardAroundPlayerRequest
        {
            StatisticName = hitRateLeaderboardName,
            MaxResultsCount = 1 // 自分のみを取得
        };

        PlayFabClientAPI.GetLeaderboardAroundPlayer(request, result => OnGetHitRateLeaderboardSuccess(result, hitRateLeaderboardName), OnGetHitRateLeaderboardError);
    }

    //ハイスコアリーダーボード取得に成功した場合
    private void OnGetLeaderboardSuccess(GetLeaderboardAroundPlayerResult result, string leaderboardName)
    {
        if (result.Leaderboard.Count > 0)
        {
            var myEntry = result.Leaderboard[0];
            //Debug.Log($"Leaderboard: {leaderboardName} - My Position: {myEntry.Position}, My Value: {myEntry.StatValue}");

            //ハイスコアリーダーボード名に一致する変数にハイスコアを代入
            UpdateHighScoreVariable(leaderboardName, myEntry.StatValue.ToString());
        }
        else
        {
            Debug.Log("No entries found for the player in the leaderboard.");
        }
    }

    //ヒット率リーダーボード取得に成功した場合
    private void OnGetHitRateLeaderboardSuccess(GetLeaderboardAroundPlayerResult result, string hitRateLeaderboardName)
    {
        if (result.Leaderboard.Count > 0)
        {
            var myEntry = result.Leaderboard[0];
            float hitRateMyEntry = myEntry.StatValue;
            float hitRateF1 = hitRateMyEntry / 10;
            //Debug.Log($"HitRate Leaderboard: {hitRateLeaderboardName} - My Position: {myEntry.Position}, My Value: {hitRateF1}%");

            //ハイスコアリーダーボード名に一致する変数にハイスコアを代入
            UpdateHitRateVariable(hitRateLeaderboardName, hitRateF1.ToString());
        }
        else
        {
            Debug.Log("No entries found for the player in the leaderboard.");
        }
    }

    //ハイスコアリーダーボード取得に失敗した場合
    private void OnGetLeaderboardError(PlayFabError error)
    {
        Debug.LogError($"Error retrieving leaderboard: {error.GenerateErrorReport()}");
    }

    //ヒット率リーダーボード取得に失敗した場合
    private void OnGetHitRateLeaderboardError(PlayFabError error)
    {
        Debug.LogError($"Error retrieving leaderboard: {error.GenerateErrorReport()}");
    }

    //ハイスコアリーダーボード名に対応する変数にスコアを代入するメソッド
    private void UpdateHighScoreVariable(string leaderboardName, string value)
    {
        switch (leaderboardName)
        {
            case "HighScore_NyarwayOffshore":
                HighScore_NyarwayOffshore = value;
                break;
            case "HighScore_NyanadaOffshore":
                HighScore_NyanadaOffshore = value;
                break;
            case "HighScore_NyankasanOffshore":
                HighScore_NyankasanOffshore = value;
                break;
            case "HighScore_NyalaskaOffshore":
                HighScore_NyalaskaOffshore = value;
                break;
            case "HighScore_NyaruOffshore":
                HighScore_NyaruOffshore = value;
                break;
            case "HighScore_NyankyokukaiOffshore":
                HighScore_NyankyokukaiOffshore = value;
                break;
            case "HighScore_NyaringkaiOffshore":
                HighScore_NyaringkaiOffshore = value;
                break;
            case "HighScore_NyaltkaiOffshore":
                HighScore_NyaltkaiOffshore = value;
                break;
            case "HighScore_NyakkaiOffshore":
                HighScore_NyakkaiOffshore = value;
                break;
        }
    }

    //ヒット率リーダーボード名に対応する変数にヒット率を代入するメソッド
    private void UpdateHitRateVariable(string hitRateLeaderboardName, string value)
    {
        switch (hitRateLeaderboardName)
        {
            case "HitRate_NyarwayOffshore":
                HitRate_NyarwayOffshore = value;
                break;
            case "HitRate_NyanadaOffshore":
                HitRate_NyanadaOffshore = value;
                break;
            case "HitRate_NyankasanOffshore":
                HitRate_NyankasanOffshore = value;
                break;
            case "HitRate_NyalaskaOffshore":
                HitRate_NyalaskaOffshore = value;
                break;
            case "HitRate_NyaruOffshore":
                HitRate_NyaruOffshore = value;
                break;
            case "HitRate_NyankyokukaiOffshore":
                HitRate_NyankyokukaiOffshore = value;
                break;
            case "HitRate_NyaringkaiOffshore":
                HitRate_NyaringkaiOffshore = value;
                break;
            case "HitRate_NyaltkaiOffshore":
                HitRate_NyaltkaiOffshore = value;
                break;
            case "HitRate_NyakkaiOffshore":
                HitRate_NyakkaiOffshore = value;
                break;
        }
    }

    private List<string> GetLeaderboardNames()
    {
        return new List<string>
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
    }

    private List<string> GetHitRateLeaderboardNames()
    {
        return new List<string>
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
    }

    //[ネコパンチランキング更新]
    public void SubmitNumberOfNekopunch()
    {
        PlayFabClientAPI.UpdatePlayerStatistics(new UpdatePlayerStatisticsRequest
        {
            Statistics = new List<StatisticUpdate>
            {
                new StatisticUpdate
                {
                    StatisticName = "NekoPunch",
                    Value = NekopunchManager.instance.nekoPunch
                }
            }
        }, result =>
        {
            //Debug.Log("ネコパンチランキング送信完了");
        }, error =>
        {
            Debug.Log(error.GenerateErrorReport());
        });
    }

    //ログイン時にデイリーパンチ数を取得
    public void GetDailyPunchCount()
    {
        var request = new GetPlayerStatisticsRequest();

        PlayFabClientAPI.GetPlayerStatistics(request, OnGetPlayerStatisticsSuccess, OnError);
    }

    private void OnGetPlayerStatisticsSuccess(GetPlayerStatisticsResult result)
    {
        Debug.Log("Player statistics retrieved successfully.");

        var dailyPunchStat = result.Statistics.Find(stat => stat.StatisticName == "DailyNekoPunch");

        if (dailyPunchStat != null)
        {
            NekopunchManager.instance.dailyNekoPunchCount = dailyPunchStat.Value;
            Debug.Log("Current Daily Punch Count: " + NekopunchManager.instance.dailyNekoPunchCount);
        }
        else
        {
            Debug.Log("No data found for DailyNekoPunch. Initializing to 0.");
            //データが存在しない場合は0で初期化
            NekopunchManager.instance.dailyNekoPunchCount = 0;
        }
    }

    private void OnError(PlayFabError error)
    {
        Debug.LogError("Error while accessing PlayFab: " + error.GenerateErrorReport());
    }

    //非同期処理でデイリーパンチを送信
    public IEnumerator SendDailyPunchCountToPlayFab()
    {
        var request = new UpdatePlayerStatisticsRequest
        {
            Statistics = new List<StatisticUpdate>
            {
                new StatisticUpdate { StatisticName  = "DailyNekoPunch", Value = NekopunchManager.instance.dailyNekoPunchCount }
            }
        };

        PlayFabClientAPI.UpdatePlayerStatistics(request, OnStatisticsUpdate, OnError);

        yield return new WaitForSeconds(10);

        //ランキング取得
        GetDailyNekoPunchRanking();
    }

    private void OnStatisticsUpdate(UpdatePlayerStatisticsResult result)
    {
        Debug.Log("Successfully updated player statistics.");
    }

    //デイリーパンチランキングの取得
    private void GetDailyNekoPunchRanking()
    {
        var request = new GetLeaderboardRequest
        {
            StatisticName = "DailyNekoPunch",
            StartPosition = 0,
            MaxResultsCount = 20
        };

        PlayFabClientAPI.GetLeaderboard(request, OnDailyPunchGet, OnError);
    }

    private void OnDailyPunchGet(GetLeaderboardResult result)
    {
        NekopunchManager.instance.dailyNekoPunchText.text = "";

        foreach (var item in result.Leaderboard)
        {
            Debug.Log("Get Neko Punch Ranking.");
            //DisplayNameがNullでないか確認しnullならデフォルト名にする
            string displayName = string.IsNullOrEmpty(item.DisplayName) ? "Unknown Player" : item.DisplayName;

            NekopunchManager.instance.dailyNekoPunchText.text += $"\n{item.Position + 1}. {displayName} : {item.StatValue}";
            Debug.Log($"\n{item.Position + 1}. {displayName} : {item.StatValue}");
        }
    }
}
