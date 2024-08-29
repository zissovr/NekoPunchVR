using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;

public class GameSceneManager : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI timeText;
    public Image progressBarImage;
    public GameObject timerUI_Gameobject;
    public GameObject restartButtonUI_Gameobject;
    //�X�R�A�\���؂�ւ��̃I�u�W�F�N�g�ݒ�
    public GameObject currentScoreUI_Gameobject;
    public GameObject finalScoreUI_Gameobject;
    public GameObject rankingUI_Gameobject;
    public GameObject nekopunchUI_Gameobject;
    //���Ƒ���p�l���̃I�u�W�F�N�g
    public GameObject mirror_Gameobject;
    public GameObject mirrorButton_Gameobject;
    public GameObject deckPanel_Gameobject;
    public TextMeshProUGUI textDeckNamePanel;
    //�X�R�A�^�[�Q�b�g�̃I�u�W�F�N�g
    public GameObject targets_Gameobject;

    [Header("Managers")]
    public GameObject fishSpawnManager;

    [Header("XRInteraction")]
    //RayInteractor�̎擾
    public GameObject rightRayInteractor;
    public GameObject leftRayInteractor;
    XRRayInteractor rightxrRay;
    XRRayInteractor leftxrRay;
    //Move�̎擾
    public GameObject moveLocomotion;

    //�I�[�f�B�I�̒����̕ϐ�
    float audioClipLength;

    //���݂̋���X�e�[�W�X�e�[�g
    public StageState currentState = 0;
    private Dictionary<StageState, int> stageIndices;

    //����X�e�[�W
    public enum StageState
    {
        NyarwayOffshore,
        NyanadaOffshore,
        NyankasanOffshore,
        NyalaskaOffshore,
        NyaruOffshore,
        NyankyokukaiOffshore,
        NyaringkaiOffshore,
        NyaltkaiOffshore,
        NyakkaiOffshore,
        GuestMode,
    }

    private void Awake()
    {
        //����X�e�[�W�̖��O�ƃC���f�b�N�X���}�b�s���O
        InitializeStageIndices();
    }

    private void Start()
    {
        //RayInteractor�̎擾�Ɣ�\��
        RayInteractorDeactivate();

        //�Ȃ̒������擾
        audioClipLength = AudioManager.instance.musicTheme.clip.length;

        //�Ȃ̃J�E���g�_�E�����X�^�[�g������
        StartCoroutine(StartCountdown(audioClipLength));

        //�v���O���X�o�[��������
        progressBarImage.fillAmount = Mathf.Clamp(0, 0, 1);

        //�m��X�R�A�E�����L���O��\���ƌ��݃X�R�A�\��
        finalScoreUI_Gameobject.SetActive(false);
        rankingUI_Gameobject.SetActive(false);
        nekopunchUI_Gameobject.SetActive(false);
        currentScoreUI_Gameobject.SetActive(true);
        restartButtonUI_Gameobject.SetActive(false);

        //���Ƒ���p�l���̔�\��
        mirror_Gameobject.SetActive(false);
        mirrorButton_Gameobject.SetActive(false);
        deckPanel_Gameobject.SetActive(false);

        //�^�[�Q�b�g�̕\��
        targets_Gameobject.SetActive(true);

        //�ړ��𐧌�
        moveLocomotion.SetActive(false);

        //����X�e�[�W���Z�b�g
        //SelectStage(currentState);
        SelectStage(StageState.NyarwayOffshore);
    }

    //RayInteractor�̎擾�Ɣ�\��
    public void RayInteractorDeactivate()
    {
        rightxrRay = rightRayInteractor.GetComponent<XRRayInteractor>();
        leftxrRay = leftRayInteractor.GetComponent<XRRayInteractor>();
        rightxrRay.enabled = false;
        leftxrRay.enabled = false;
    }

    public IEnumerator StartCountdown(float countdownValue)
    {
        while (countdownValue > 0)
        {
            yield return new WaitForSeconds(1.0f);
            countdownValue -= 1.0f;

            timeText.text = ConvertToMinAndSeconds(countdownValue);

            progressBarImage.fillAmount = (AudioManager.instance.musicTheme.time / audioClipLength);
        }
        TimeOver();
    }

    //�^�C���I�[�o�[����
    public void TimeOver()
    {
        Debug.Log("Time Over");
        timeText.text = ConvertToMinAndSeconds(0);

        //�X�|�[���I��
        fishSpawnManager.SetActive(false);

        //�m��X�R�A�E�����L���O�\���ƌ��݃X�R�A��\��
        finalScoreUI_Gameobject.SetActive(true);
        rankingUI_Gameobject.SetActive(true);
        nekopunchUI_Gameobject.SetActive(true);
        currentScoreUI_Gameobject.SetActive(false);
        restartButtonUI_Gameobject.SetActive(true);

        //���Ƒ���p�l���̕\��
        mirror_Gameobject.SetActive(true);
        mirrorButton_Gameobject.SetActive(true);
        deckPanel_Gameobject.SetActive(true);
        textDeckNamePanel.text = currentState.ToString();

        //�^�[�Q�b�g�̔�\��
        //targets_Gameobject.SetActive(false);

        //�^�[�Q�b�g�̕\��
        TargetsManager.instance.ActivateAllTargets();

        //���C�̕\��
        rightxrRay.enabled = true;
        leftxrRay.enabled = true;

        //�ړ����J��
        moveLocomotion.SetActive(true);

        //�����L���O�̎擾
        RankingManager.instance.RequestLeaderBoard();

        //�l�R�p���`�����̑��M
        PlayfabManager.instance.SubmitNumberOfNekopunch();

        //�l�R�p���`�����̎擾
        RankingManager.instance.RequestNekopunchLeaderBoard();

        //�e����X�e�[�W�̃^�C���I�[�o�[����
        TimeOverByStages(currentState);
    }

    //���ƕb�ɕϊ�����
    public string ConvertToMinAndSeconds(float totalTimeInSeconds)
    {
        string timeText = Mathf.Floor(totalTimeInSeconds / 60).ToString("00") + ":" + Mathf.FloorToInt(totalTimeInSeconds % 60).ToString("00");
        return timeText;
    }

    //���X�^�[�g�{�^��
    public void OnClickBackToScene()
    {
        SceneManager.LoadScene("SampleScene");
    }

    //�X�^�[�g�{�^��
    public void OnClickStart()
    {

    }

    //���̃e�N�X�`���ɕύX����{�^��
    public void OnClickChangeTexture()
    {
        TextureChanger.instance.ChangeNextTexture();
    }

    //�O�̃e�N�X�`���ɕύX����{�^��
    public void OnClickPreviousChangeTexture()
    {
        TextureChanger.instance.ChangePreviousTexture();
    }

    //����X�e�[�W�ύX�{�^��
    public void OnClickChangeGyojoStage()
    {
        int nextIndex = ((int)currentState + 1) % (stageIndices.Count - 1);
        currentState = (StageState)nextIndex;
        SelectStage(currentState);
    }

    //�O�̋���X�e�[�W�ύX�{�^��
    public void OnClickPreviousChangeGyojoStage()
    {
        int previousIndex = ((int)currentState - 1 + stageIndices.Count - 1) % (stageIndices.Count - 1);
        currentState = (StageState)previousIndex;
        SelectStage(currentState);
    }

    //�Q�X�g���[�h�{�^��
    public void OnClickGuestMode()
    {
        SelectStage(StageState.GuestMode);
    }

    //�X�e�[�W�̖��O�ƃC���f�b�N�X���}�b�s���O
    private void InitializeStageIndices()
    {
        stageIndices = new Dictionary<StageState, int>
        {
            { StageState.NyarwayOffshore, 0 },
            { StageState.NyanadaOffshore, 1 },
            { StageState.NyankasanOffshore, 2 },
            { StageState.NyalaskaOffshore, 3 },
            { StageState.NyaruOffshore, 4 },
            { StageState.NyankyokukaiOffshore, 5 },
            { StageState.NyaringkaiOffshore, 6 },
            { StageState.NyaltkaiOffshore, 7 },
            { StageState.NyakkaiOffshore, 8 },
            { StageState.GuestMode, 9 },
        };
    }

    //����X�e�[�W��I�����������s�����\�b�h
    private void SelectStage(StageState state)
    {
        //�X�e�[�W�̃^�[�Q�b�g��\��
        TargetsManager.instance.SelectStage(stageIndices[state]);

        //����X�e�[�W�̖��O���f�b�L�p�l���ɕ\��
        textDeckNamePanel.text = state.ToString();

        //����X�e�[�W���̏�����switch���Ŏ���
        switch (state)
        {
            case StageState.NyarwayOffshore:
                HandleNyarwayOffshore();
                break;

            case StageState.NyanadaOffshore:
                HandleNyanadaOffshore();
                break;

            case StageState.NyankasanOffshore:
                HandleNyankasanOffshore();
                break;

            case StageState.NyalaskaOffshore:
                HandleNyalaskaOffshore();
                break;

            case StageState.NyaruOffshore:
                HandleNyaruOffshore();
                break;

            case StageState.NyankyokukaiOffshore:
                HandleNyankyokukaiOffshore();
                break;

            case StageState.NyaringkaiOffshore:
                HandleNyaringkaiOffshore();
                break;

            case StageState.NyaltkaiOffshore:
                HandleNyaltkaiOffshore();
                break;

            case StageState.NyakkaiOffshore:
                HandleNyakkaiOffshore();
                break;

            case StageState.GuestMode:
                HandleGuestMode();
                break;
        }
    }

    //�j�����E�F�[���X�e�[�W�I���̏����������ɋL�q
    private void HandleNyarwayOffshore()
    {
        Debug.Log("�j�����E�F�[���X�e�[�W���I������܂����B");
        //�����L���OUI�̕\��
        //�ō��X�R�AUI�̕\��
        //�ō��q�b�g��UI�̕\��
        //DeckPanel�Ɏ��鋛�̕\��
        //����X�e�[�W���b�Z�[�W�̕\��
        //DeckPanel�Ɏ擾�����L�ʂƔL�p�����\��
    }

    //�j���i�_���X�e�[�W�I���̏����������ɋL�q
    private void HandleNyanadaOffshore()
    {
        Debug.Log("�j���i�_���X�e�[�W���I������܂����B");
    }

    //�j�����J�T�����X�e�[�W�I���̏����������ɋL�q
    private void HandleNyankasanOffshore()
    {
        Debug.Log("�j�����J�T�����X�e�[�W���I������܂����B");
    }

    //�j�����X�J���X�e�[�W�I���̏����������ɋL�q
    private void HandleNyalaskaOffshore()
    {
        Debug.Log("�j�����X�J���X�e�[�W���I������܂����B");
    }

    //�j�����[���X�e�[�W�I���̏����������ɋL�q
    private void HandleNyaruOffshore()
    {
        Debug.Log("�j�����[���X�e�[�W���I������܂����B");
    }

    //�j�����L���N�J�C���X�e�[�W�I���̏����������ɋL�q
    private void HandleNyankyokukaiOffshore()
    {
        Debug.Log("�j�����L���N�J�C���X�e�[�W���I������܂����B");
    }

    //�j���[�����O�J�C���X�e�[�W�I���̏����������ɋL�q
    private void HandleNyaringkaiOffshore()
    {
        Debug.Log("�j���[�����O�J�C���X�e�[�W���I������܂����B");
    }

    //�j�����g�J�C���X�e�[�W�I���̏����������ɋL�q
    private void HandleNyaltkaiOffshore()
    {
        Debug.Log("�j�����g�J�C���X�e�[�W���I������܂����B");
    }

    //�j���b�J�C���X�e�[�W�I���̏����������ɋL�q
    private void HandleNyakkaiOffshore()
    {
        Debug.Log("�j���b�J�C���X�e�[�W���I������܂����B");
    }

    //�Q�X�g���[�h�I���̏����������ɋL�q
    private void HandleGuestMode()
    {
        Debug.Log("�Q�X�g���[�h���I������܂����B");
    }


    //�e����X�e�[�W�̃^�C���I�[�o�[�������s�����\�b�h
    private void TimeOverByStages(StageState state)
    {
        //����X�e�[�W���̏�����switch���Ŏ���
        switch (state)
        {
            case StageState.NyarwayOffshore:
                TimeOverNyarwayOffshore(state);
                break;

            case StageState.NyanadaOffshore:
                
                break;

            case StageState.NyankasanOffshore:
                
                break;

            case StageState.NyalaskaOffshore:
                
                break;

            case StageState.NyaruOffshore:
                
                break;

            case StageState.NyankyokukaiOffshore:
                
                break;

            case StageState.NyaringkaiOffshore:
                
                break;

            case StageState.NyaltkaiOffshore:
                
                break;

            case StageState.NyakkaiOffshore:
                
                break;

            case StageState.GuestMode:
                
                break;
        }
    }

    //�j�����E�F�[���̃^�C���I�[�o�[���̏����������ɋL�q
    private void TimeOverNyarwayOffshore(StageState state)
    {
        Debug.Log($"{state}�X�e�[�W���I�����܂����B");

        //�q�b�g���̊m�F
        Debug.Log($"�X�|�[�����ꂽ���̐���{ScoreManager.instance.SpawnedFish}");
        Debug.Log($"{state}�X�e�[�W�̃q�b�g����{ScoreManager.instance.HitRate()}���ł��B");
    }
}
