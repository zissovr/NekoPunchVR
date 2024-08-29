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

    //リーダーボード名とハイスコア変数のマッピング
    private Dictionary<string, string> highScoreMap = new Dictionary<string, string>();

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
        Login();

        //ハイスコアリーダーボード：辞書の初期化
        InitializeLeaderboardDictionary();
    }

    //ログイン実行
    private void Login()
    {
        _customID = LoadCustomID();
        var request = new LoginWithCustomIDRequest { CustomId = _customID, CreateAccount = _shouldCreateAccount };
        PlayFabClientAPI.LoginWithCustomID(request, OnLoginSuccess, OnLoginFailure);
    }

    //ログイン成功
    private void OnLoginSuccess(LoginResult result)
    {
        //アカウント作成時にIDがすでに使われていた場合
        if (_shouldCreateAccount && !result.NewlyCreated)
        {
            Debug.LogWarning($"CustomId : {_customID}はすでに使われています");
            Login();
            return;
        }

        //アカウント作成時にIDを保存
        if (result.NewlyCreated)
        {
            SaveCustomID();
        }
        Debug.Log($"PlayFabのログインに成功\nPlayFabId : {result.PlayFabId}, CustomId : {_customID}\nアカウントを作成したか : {result.NewlyCreated}");

        //PlayFabIdをセット
        masterPlayerAccountID = result.PlayFabId;

        //GetUserData();
    }

    //ログイン失敗
    private void OnLoginFailure(PlayFabError error)
    {
        Debug.LogError($"PlayFabのログインに失敗\n{error.GenerateErrorReport()}");
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
        _shouldCreateAccount = string.IsNullOrEmpty(id);
        return _shouldCreateAccount ? GenerateCustomID() : id;
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


    //[ランキング更新]
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

    //すべてのハイスコアリーダーボードの値を一気に取得するメソッド
    public void GetAllLeaderboardValues()
    {
        foreach(string leaderboardName in leaderboardNames)
        {
            GetLeaderboardValue(leaderboardName);
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

    //ハイスコアリーダーボード取得に成功した場合
    private void OnGetLeaderboardSuccess(GetLeaderboardAroundPlayerResult result, string leaderboardName)
    {
        if (result.Leaderboard.Count > 0)
        {
            var myEntry = result.Leaderboard[0];
            Debug.Log($"Leaderboard: {leaderboardName} - My Position: {myEntry.Position}, My Value: {myEntry.StatValue}");

            //ハイスコアリーダーボード名に一致する変数にハイスコアを代入
            UpdateHighScoreVariable(leaderboardName, myEntry.StatValue.ToString());
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
}
