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

    //���[�_�[�{�[�h���ƃn�C�X�R�A�ϐ��̃}�b�s���O
    private Dictionary<string, string> highScoreMap = new Dictionary<string, string>();

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

        //�n�C�X�R�A���[�_�[�{�[�h�F�����̏�����
        InitializeLeaderboardDictionary();
    }

    //���O�C�����s
    private void Login()
    {
        _customID = LoadCustomID();
        var request = new LoginWithCustomIDRequest { CustomId = _customID, CreateAccount = _shouldCreateAccount };
        PlayFabClientAPI.LoginWithCustomID(request, OnLoginSuccess, OnLoginFailure);
    }

    //���O�C������
    private void OnLoginSuccess(LoginResult result)
    {
        //�A�J�E���g�쐬����ID�����łɎg���Ă����ꍇ
        if (_shouldCreateAccount && !result.NewlyCreated)
        {
            Debug.LogWarning($"CustomId : {_customID}�͂��łɎg���Ă��܂�");
            Login();
            return;
        }

        //�A�J�E���g�쐬����ID��ۑ�
        if (result.NewlyCreated)
        {
            SaveCustomID();
        }
        Debug.Log($"PlayFab�̃��O�C���ɐ���\nPlayFabId : {result.PlayFabId}, CustomId : {_customID}\n�A�J�E���g���쐬������ : {result.NewlyCreated}");

        //PlayFabId���Z�b�g
        masterPlayerAccountID = result.PlayFabId;

        //GetUserData();
    }

    //���O�C�����s
    private void OnLoginFailure(PlayFabError error)
    {
        Debug.LogError($"PlayFab�̃��O�C���Ɏ��s\n{error.GenerateErrorReport()}");
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
        _shouldCreateAccount = string.IsNullOrEmpty(id);
        return _shouldCreateAccount ? GenerateCustomID() : id;
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


    //[�����L���O�X�V]
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

    //���ׂẴn�C�X�R�A���[�_�[�{�[�h�̒l����C�Ɏ擾���郁�\�b�h
    public void GetAllLeaderboardValues()
    {
        foreach(string leaderboardName in leaderboardNames)
        {
            GetLeaderboardValue(leaderboardName);
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

    //�n�C�X�R�A���[�_�[�{�[�h�擾�ɐ��������ꍇ
    private void OnGetLeaderboardSuccess(GetLeaderboardAroundPlayerResult result, string leaderboardName)
    {
        if (result.Leaderboard.Count > 0)
        {
            var myEntry = result.Leaderboard[0];
            Debug.Log($"Leaderboard: {leaderboardName} - My Position: {myEntry.Position}, My Value: {myEntry.StatValue}");

            //�n�C�X�R�A���[�_�[�{�[�h���Ɉ�v����ϐ��Ƀn�C�X�R�A����
            UpdateHighScoreVariable(leaderboardName, myEntry.StatValue.ToString());
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
}
