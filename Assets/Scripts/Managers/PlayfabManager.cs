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

    //�A�J�E���g���쐬���邩
    private bool _shouldCreateAccount;

    //���O�C�����Ɏg��ID
    private string _customID;

    // �擾����Master Player Account ID��ۑ�����ϐ�
    public string masterPlayerAccountID;

    //�n�C�X�R�A���[�_�[�{�[�h�̖��O���ƂɃn�C�X�R�A��ێ�����ϐ�
    public string HighScore_NyarwayOffshore;
    public string HighScore_NyanadaOffshore;
    public string HighScore_NyankasanOffshore;
    public string HighScore_NyalaskaOffshore;
    public string HighScore_NyaruOffshore;
    public string HighScore_NyankyokukaiOffshore;
    public string HighScore_NyaringkaiOffshore;
    public string HighScore_NyaltkaiOffshore;
    public string HighScore_NyakkaiOffshore;

    //�q�b�g�����[�_�[�{�[�h�̖��O���ƂɃq�b�g����ێ�����ϐ�
    public string HitRate_NyarwayOffshore;
    public string HitRate_NyanadaOffshore;
    public string HitRate_NyankasanOffshore;
    public string HitRate_NyalaskaOffshore;
    public string HitRate_NyaruOffshore;
    public string HitRate_NyankyokukaiOffshore;
    public string HitRate_NyaringkaiOffshore;
    public string HitRate_NyaltkaiOffshore;
    public string HitRate_NyakkaiOffshore;

    //���[�_�[�{�[�h���ƃn�C�X�R�A�ϐ��̃}�b�s���O
    private Dictionary<string, string> highScoreMap = new Dictionary<string, string>();

    //���[�_�[�{�[�h���ƃq�b�g���ϐ��̃}�b�s���O
    private Dictionary<string, string> hitRateMap = new Dictionary<string, string>();

    //�n�C�X�R�A���[�_�[�{�[�h�����i�[���郊�X�g
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

    //�q�b�g�����[�_�[�{�[�h�����i�[���郊�X�g
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
        //�N���G�C�g�A�J�E���g�t���O
        _customID = LoadCustomID();
        _shouldCreateAccount = string.IsNullOrEmpty(_customID);

        Login();

        //�n�C�X�R�A���[�_�[�{�[�h�F�����̏�����
        InitializeLeaderboardDictionary();

        //�q�b�g�����[�_�[�{�[�h�F�����̏�����
        InitializeHitRateLeaderboardDictionary();
    }

    //���O�C�����s
    private void Login()
    {
        //�A�J�E���g���쐬���邩�ǂ���
        var request = new LoginWithCustomIDRequest { CustomId = _customID, CreateAccount = _shouldCreateAccount };

        //���O�C��
        PlayFabClientAPI.LoginWithCustomID(request, OnLoginSuccess, OnLoginFailure);
    }

    //���O�C������
    private void OnLoginSuccess(LoginResult result)
    {
        //�A�J�E���g�쐬����ID�����łɎg���Ă����ꍇ
        if (_shouldCreateAccount && !result.NewlyCreated)
        {
            Debug.LogWarning($"CustomId : {_customID}�͂��łɎg���Ă��܂�");
            DebugToUI.Log($"CustomId : {_customID}is already in use");
            Login();
            return;
        }

        //�A�J�E���g�쐬����ID��ۑ�
        if (result.NewlyCreated)
        {
            SaveCustomID();
        }
        Debug.Log($"PlayFab�̃��O�C���ɐ���\nPlayFabId : {result.PlayFabId}, CustomId : {_customID}\n�A�J�E���g���쐬������ : {result.NewlyCreated}");
        DebugToUI.Log($"Successful PlayFab login\nPlayFabId : {result.PlayFabId}, CustomId : {_customID}\nHave you created an account : {result.NewlyCreated}");

        //PlayFabId���Z�b�g
        masterPlayerAccountID = result.PlayFabId;

        //���O�C�����Ƀf�C���[�p���`�����擾
        GetDailyPunchCount();

        //���O�C�����Ƀn�C�X�R�A�ƃq�b�g���̃��[�_�[�{�[�h���擾
        GetAllLeaderboardValues();
        GetAllHitRateLeaderboardValues();
    }

    //���O�C�����s
    private void OnLoginFailure(PlayFabError error)
    {
        Debug.LogError($"PlayFab�̃��O�C���Ɏ��s\n�G���[�R�[�h: {error.Error} - �ڍ�: {error.ErrorDetails}\n{error.GenerateErrorReport()}");
        DebugToUI.Log($"PlayFab login failedError code: {error.Error} - Details: {error.ErrorDetails}\n{error.GenerateErrorReport()}");

        if (error.Error == PlayFabErrorCode.AccountNotFound)
        {
            DebugToUI.Log($"Account not found. Create a new account.");
            _shouldCreateAccount = true;
            // �V�����A�J�E���g���쐬���邽�߂ɍĎ��s
            Login();
        }
    }


    //[�J�X�^��ID�̎擾]
    //ID��ۑ����鎞��KEY
    private static readonly string CUSTOM_ID_SAVE_KEY = "CUSTOM_ID_SAVE_KEY";

    //ID���擾
    private string LoadCustomID()
    {
        //ID���擾
        string id = PlayerPrefs.GetString(CUSTOM_ID_SAVE_KEY);

        //�ۑ�����Ă��Ȃ���ΐV�K�쐬
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

    //ID�̕ۑ�
    private void SaveCustomID()
    {
        PlayerPrefs.SetString(CUSTOM_ID_SAVE_KEY, _customID);
    }

    //[�J�X�^��ID�̐���]
    //ID�Ɏg�p���镶����
    private static readonly string ID_CHARACTERS = "0123456789abcdefghijklmnopqrstuvwxyz";

    //ID�𐶐�����
    private string GenerateCustomID()
    {
        //ID�̒���
        int idLength = 32;
        StringBuilder stringBuilder = new StringBuilder(idLength);
        var random = new System.Random();

        //�����_����ID�𐶐�
        for (int i = 0; i < idLength; i++)
        {
            stringBuilder.Append(ID_CHARACTERS[random.Next(ID_CHARACTERS.Length)]);
        }

        return stringBuilder.ToString();
    }


    //[�X�R�A�����L���O�X�V]
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
            //Debug.Log("�X�R�A���M�����I");
        }, error =>
        {
            Debug.Log(error.GenerateErrorReport());
        });
    }

    //[�X�R�A�����L���O�X�V]
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
            //Debug.Log("�q�b�g�����M�����I");
        }, error =>
        {
            Debug.Log(error.GenerateErrorReport());
        });
    }

    //�����̏������F�n�C�X�R�A���[�_�[�{�[�h�����L�[�ɕϐ����Q�Ƃ���ݒ�
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

    //�����̏������F�q�b�g�����[�_�[�{�[�h�����L�[�ɕϐ����Q�Ƃ���ݒ�
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

    //���ׂẴn�C�X�R�A���[�_�[�{�[�h�̒l����C�Ɏ擾���郁�\�b�h
    public void GetAllLeaderboardValues()
    {
        foreach(string leaderboardName in GetLeaderboardNames())
        {
            GetLeaderboardValue(leaderboardName);
        }
    }

    //���ׂẴq�b�g�����[�_�[�{�[�h�̒l����C�Ɏ擾���郁�\�b�h
    public void GetAllHitRateLeaderboardValues()
    {
        foreach (string hitRateLeaderboardName in GetHitRateLeaderboardNames())
        {
            GetHitRateLeaderboardValue(hitRateLeaderboardName);
        }
    }

    //�e�n�C�X�R�A���[�_�[�{�[�h�̒l���擾���郁�\�b�h
    public void GetLeaderboardValue(string leaderboardName)
    {
        var request = new GetLeaderboardAroundPlayerRequest
        {
            StatisticName = leaderboardName,
            MaxResultsCount = 1 // �����݂̂��擾
        };

        PlayFabClientAPI.GetLeaderboardAroundPlayer(request, result => OnGetLeaderboardSuccess(result, leaderboardName), OnGetLeaderboardError);
    }

    //�e�q�b�g�����[�_�[�{�[�h�̒l���擾���郁�\�b�h
    public void GetHitRateLeaderboardValue(string hitRateLeaderboardName)
    {
        var request = new GetLeaderboardAroundPlayerRequest
        {
            StatisticName = hitRateLeaderboardName,
            MaxResultsCount = 1 // �����݂̂��擾
        };

        PlayFabClientAPI.GetLeaderboardAroundPlayer(request, result => OnGetHitRateLeaderboardSuccess(result, hitRateLeaderboardName), OnGetHitRateLeaderboardError);
    }

    //�n�C�X�R�A���[�_�[�{�[�h�擾�ɐ��������ꍇ
    private void OnGetLeaderboardSuccess(GetLeaderboardAroundPlayerResult result, string leaderboardName)
    {
        if (result.Leaderboard.Count > 0)
        {
            var myEntry = result.Leaderboard[0];
            //Debug.Log($"Leaderboard: {leaderboardName} - My Position: {myEntry.Position}, My Value: {myEntry.StatValue}");

            //�n�C�X�R�A���[�_�[�{�[�h���Ɉ�v����ϐ��Ƀn�C�X�R�A����
            UpdateHighScoreVariable(leaderboardName, myEntry.StatValue.ToString());
        }
        else
        {
            Debug.Log("No entries found for the player in the leaderboard.");
        }
    }

    //�q�b�g�����[�_�[�{�[�h�擾�ɐ��������ꍇ
    private void OnGetHitRateLeaderboardSuccess(GetLeaderboardAroundPlayerResult result, string hitRateLeaderboardName)
    {
        if (result.Leaderboard.Count > 0)
        {
            var myEntry = result.Leaderboard[0];
            float hitRateMyEntry = myEntry.StatValue;
            float hitRateF1 = hitRateMyEntry / 10;
            //Debug.Log($"HitRate Leaderboard: {hitRateLeaderboardName} - My Position: {myEntry.Position}, My Value: {hitRateF1}%");

            //�n�C�X�R�A���[�_�[�{�[�h���Ɉ�v����ϐ��Ƀn�C�X�R�A����
            UpdateHitRateVariable(hitRateLeaderboardName, hitRateF1.ToString());
        }
        else
        {
            Debug.Log("No entries found for the player in the leaderboard.");
        }
    }

    //�n�C�X�R�A���[�_�[�{�[�h�擾�Ɏ��s�����ꍇ
    private void OnGetLeaderboardError(PlayFabError error)
    {
        Debug.LogError($"Error retrieving leaderboard: {error.GenerateErrorReport()}");
    }

    //�q�b�g�����[�_�[�{�[�h�擾�Ɏ��s�����ꍇ
    private void OnGetHitRateLeaderboardError(PlayFabError error)
    {
        Debug.LogError($"Error retrieving leaderboard: {error.GenerateErrorReport()}");
    }

    //�n�C�X�R�A���[�_�[�{�[�h���ɑΉ�����ϐ��ɃX�R�A�������郁�\�b�h
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

    //�q�b�g�����[�_�[�{�[�h���ɑΉ�����ϐ��Ƀq�b�g���������郁�\�b�h
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

    //[�l�R�p���`�����L���O�X�V]
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
            //Debug.Log("�l�R�p���`�����L���O���M����");
        }, error =>
        {
            Debug.Log(error.GenerateErrorReport());
        });
    }

    //���O�C�����Ƀf�C���[�p���`�����擾
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
            //�f�[�^�����݂��Ȃ��ꍇ��0�ŏ�����
            NekopunchManager.instance.dailyNekoPunchCount = 0;
        }
    }

    private void OnError(PlayFabError error)
    {
        Debug.LogError("Error while accessing PlayFab: " + error.GenerateErrorReport());
    }

    //�񓯊������Ńf�C���[�p���`�𑗐M
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

        //�����L���O�擾
        GetDailyNekoPunchRanking();
    }

    private void OnStatisticsUpdate(UpdatePlayerStatisticsResult result)
    {
        Debug.Log("Successfully updated player statistics.");
    }

    //�f�C���[�p���`�����L���O�̎擾
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
            //DisplayName��Null�łȂ����m�F��null�Ȃ�f�t�H���g���ɂ���
            string displayName = string.IsNullOrEmpty(item.DisplayName) ? "Unknown Player" : item.DisplayName;

            NekopunchManager.instance.dailyNekoPunchText.text += $"\n{item.Position + 1}. {displayName} : {item.StatValue}";
            Debug.Log($"\n{item.Position + 1}. {displayName} : {item.StatValue}");
        }
    }
}
