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
                    StatisticName = "HighScore",
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
