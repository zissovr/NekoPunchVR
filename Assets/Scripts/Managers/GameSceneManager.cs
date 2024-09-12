using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;

public class GameSceneManager : MonoBehaviour
{
    public static GameSceneManager instance;

    [Header("UI")]
    public TextMeshProUGUI timeText;
    public Image progressBarImage;
    public GameObject timerUI_Gameobject;
    //�X�R�A�\���؂�ւ��̃I�u�W�F�N�g�ݒ�
    public GameObject currentScoreUI_Gameobject;
    public GameObject finalScoreUI_Gameobject;
    public GameObject rankingUI_Gameobject;
    public GameObject nekopunchUI_Gameobject;
    public GameObject hitRateUI_Gameobject;
    public GameObject daylypunchUI_Gameobject;
    //���Ƒ���p�l���̃I�u�W�F�N�g
    public GameObject mirror_Gameobject;
    public GameObject mirrorButton_Gameobject;
    public GameObject deckPanel_Gameobject;
    public TextMeshProUGUI textDeckNamePanel;
    public TextMeshProUGUI textDeckNyancoin;
    public TextMeshProUGUI textPlayfabID;
    //�X�R�A�^�[�Q�b�g�̃I�u�W�F�N�g
    public GameObject targets_Gameobject;
    //����^�C�g��
    public GameObject gyojyoTitle;
    public TextMeshProUGUI textGyojyoTital;

    [Header("SpawnProfile")]
    public SpawnProfile[] profiles;


    [Header("Managers")]
    public FishSpawnManager fishSpawnManager;

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
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        instance = this;

        DontDestroyOnLoad(this.gameObject);

        // �V�[�������[�h���ꂽ�Ƃ��̃C�x���g�Ƀ��X�i�[��ǉ�
        SceneManager.sceneLoaded += OnSceneLoaded;

        //����X�e�[�W�̖��O�ƃC���f�b�N�X���}�b�s���O
        InitializeStageIndices();
    }

    private void OnDestroy()
    {
        // �V�[���̃��[�h�C�x���g�̃��X�i�[������
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // �V�[�������[�h���ꂽ�Ƃ��̏���
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (TargetsManager.instance != null)
        {
            // �^�[�Q�b�g�̏�Ԃ����Z�b�g
            ActivateSelectedStageTargets();
        }
        else
        {
            Debug.LogError("TargetsManager instance is null after scene load.");
        }
    }

    private void Start()
    {
        //RayInteractor�̎擾�Ɣ�\��
        RayInteractorDeactivate();

        // �V�[�����[�h��̏�����
        if (TargetsManager.instance != null)
        {
            // �V�[�����[�h��ɃX�e�[�W�̃^�[�Q�b�g���A�N�e�B�u��
            TargetsManager.instance.DeactivateAllStagesOnSceneLoad();
        }
        // 0.5�b��Ƀ^�[�Q�b�g�O���[�v�̕\�����s��
        StartCoroutine(SelectStageAfterDelay(0.5f));

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
        hitRateUI_Gameobject.SetActive(false);
        daylypunchUI_Gameobject.SetActive(false);

        //����^�C�g���̔�\��
        gyojyoTitle.SetActive(false);

        //���Ƒ���p�l���̔�\��
        mirror_Gameobject.SetActive(false);
        mirrorButton_Gameobject.SetActive(false);
        deckPanel_Gameobject.SetActive(false);

        //�^�[�Q�b�g�̔�\��
        //targets_Gameobject.SetActive(false);

        //�ړ��𐧌�
        moveLocomotion.SetActive(false);

        //PlayerPrefs����ۑ����ꂽ�X�e�[�W�X�e�[�g��ǂݍ���
        if (PlayerPrefs.HasKey("SelectedStageState"))
        {
            currentState = (StageState)PlayerPrefs.GetInt("SelectedStageState");
        }
        else
        {
            //�f�t�H���g�X�e�[�W
            currentState = StageState.NyarwayOffshore;
        }

        //����X�e�[�W���Z�b�g
        //SelectStage(currentState);
        SelectStageForSceneLoad(currentState);
        //SelectStage(StageState.NyarwayOffshore);

        //�I�����ꂽ�X�e�[�W�̃^�[�Q�b�g�I�u�W�F�N�g���A�N�e�B�u��
        ActivateSelectedStageTargets();

        //�X�|�[���J�n
        fishSpawnManager.gameObject.SetActive(true);
    }

    private IEnumerator SelectStageAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        // ���݂̃X�e�[�W�̃^�[�Q�b�g�O���[�v��I������
        int stageIndex = stageIndices[currentState];
        TargetsManager.instance.SelectStageForSceneLoad(stageIndex);
    }

    //�I�����ꂽ�X�e�[�W�̃^�[�Q�b�g�I�u�W�F�N�g���A�N�e�B�u�����郁�\�b�h
    private void ActivateSelectedStageTargets() 
    {
        if (TargetsManager.instance == null)
        {
            Debug.LogError("TargetsManager.instance is null in ActivateSelectedStageTargets.");
            return;
        }

        // �e�I�u�W�F�N�g�iTargets�j����A�N�e�B�u�̏ꍇ�A�A�N�e�B�u������
        if (!TargetsManager.instance.gameObject.activeInHierarchy)
        {
            TargetsManager.instance.gameObject.SetActive(true);
            Debug.Log("Parent object 'Targets' has been activated.");
        }

        // �I�����ꂽ����X�e�[�W�̃C���f�b�N�X
        if (!stageIndices.ContainsKey(currentState))
        {
            Debug.LogError($"StageState {currentState} not found in stageIndices.");
            return;
        }

        int stageIndex = stageIndices[currentState];

        // ���ׂẴX�e�[�W���A�N�e�B�u�ɂ���
        foreach (GameObject targetGroup in TargetsManager.instance.targetGroups)
        {
            if (targetGroup != null)
            {
                targetGroup.SetActive(false);
            }
        }

        // �Ώۂ̋���X�e�[�W���A�N�e�B�u��
        if (TargetsManager.instance.targetGroups[stageIndex] != null)
        {
            TargetsManager.instance.targetGroups[stageIndex].SetActive(true);
            Debug.Log($"Activated target group: {TargetsManager.instance.targetGroups[stageIndex].name}");
        }
        else
        {
            Debug.LogWarning($"Target group for stage index {stageIndex} is null.");
        }
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
        fishSpawnManager.gameObject.SetActive(false);

        //�m��X�R�A�E�����L���O�\���ƌ��݃X�R�A��\��
        finalScoreUI_Gameobject.SetActive(true);
        rankingUI_Gameobject.SetActive(true);
        nekopunchUI_Gameobject.SetActive(true);
        currentScoreUI_Gameobject.SetActive(false);
        hitRateUI_Gameobject.SetActive(true);
        daylypunchUI_Gameobject.SetActive(true);

        //����^�C�g���̕\��
        gyojyoTitle.SetActive(true);
        textGyojyoTital.text = currentState.ToString();

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
        //RankingManager.instance.RequestLeaderBoard();

        //�l�R�p���`�����̑��M
        PlayfabManager.instance.SubmitNumberOfNekopunch();

        //�l�R�p���`�����̎擾
        RankingManager.instance.RequestNekopunchLeaderBoard();

        //�j�����R�C���̎擾�ƕ\��
        GetNyancoin();

        //�e����X�e�[�W�̃^�C���I�[�o�[����
        TimeOverByStages(currentState);

        //�񓯊������Ńf�C���[�p���`�𑗐M
        StartCoroutine(PlayfabManager.instance.SendDailyPunchCountToPlayFab());

        //�f�b�L�p�l���̃V�����_�[��������
        DeckPanelManager.instance.DeckPanelCylinderRenderer();

        //�f�b�L�p�l����̃V�����_�[��_��
        DeckPanelManager.instance.DeckPanelCylinderLightOn((int)currentState);

        //�q�b�g�������L���OUI�̕\��
        RankingManager.instance.ShowHitRateRanking((int)currentState);

        //�n�C�X�R�A�����L���OUI�̕\��
        RankingManager.instance.ShowHighScoreRanking((int)currentState);
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
        SceneManager.LoadScene("SampleScene");
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

        //����X�e�[�W�I����PlayerPrefs�ɕۑ�
        PlayerPrefs.SetInt("SelectedStageState", (int)currentState);
        PlayerPrefs.Save();

        //�O��̃X�R�A�ƃq�b�g����������
        ScoreManager.instance.InitializeCurrentScoreAndHitRate();
    }

    //�O�̋���X�e�[�W�ύX�{�^��
    public void OnClickPreviousChangeGyojoStage()
    {
        int previousIndex = ((int)currentState - 1 + stageIndices.Count - 1) % (stageIndices.Count - 1);
        currentState = (StageState)previousIndex;
        SelectStage(currentState);

        //����X�e�[�W�I����PlayerPrefs�ɕۑ�
        PlayerPrefs.SetInt("SelectedStageState", (int)currentState);
        PlayerPrefs.Save();

        //�O��̃X�R�A�ƃq�b�g����������
        ScoreManager.instance.InitializeCurrentScoreAndHitRate();
    }

    // �X�e�[�W��I�����A�K�؂ȃ��\�b�h���Ăяo��
    private void SelectStageForUI(StageState state)
    {
        int stageIndex = stageIndices[state];

        // UI���쎞�ɂ�SelectStageFor...���Ăяo��
        if (TargetsManager.instance != null)
        {
            TargetsManager.instance.SelectStageForUI(stageIndex);
            DeckPanelManager.instance.SelectStageForDeckPanel(stageIndex);
        }
        else
        {
            Debug.LogError("TargetsManager.instance is null when trying to select stage for UI.");
        }

        currentState = state;
    }

    // �V�[�����[�h���ɃX�e�[�W��I�����郁�\�b�h
    private void SelectStageForSceneLoad(StageState state)
    {
        int stageIndex = stageIndices[state];

        // �V�[�����[�h���ɂ�SelectStageForSceneLoad���Ăяo��
        if (TargetsManager.instance != null)
        {
            TargetsManager.instance.SelectStageForSceneLoad(stageIndex);
        }
        else
        {
            Debug.LogError("TargetsManager.instance is null when trying to select stage for scene load.");
        }

        currentState = state;
    }

    //�Q�X�g���[�h�{�^��
    public void OnClickGuestMode()
    {
        SelectStage(StageState.GuestMode);
    }

    //�j�����R�C���̎擾�ƕ\��
    private void GetNyancoin()
    {
        int coin = PlayerPrefs.GetInt("TotalNekoPunch", 0);
        textDeckNyancoin.text = coin.ToString();
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
        if (TargetsManager.instance == null)
        {
            Debug.LogError("TargetsManager.instance is null. Ensure TargetsManager is in the scene and initialized.");
            return;
        }

        if (!stageIndices.ContainsKey(state))
        {
            Debug.LogError($"StageState {state} not found in stageIndices.");
        }

        int stageIndex = stageIndices[state];

        //�X�e�[�W�̃^�[�Q�b�g��\��
        SelectStageForUI(state);


        //���݂̃X�e�[�W���X�V
        currentState = state;

        //����X�e�[�W�̖��O���f�b�L�p�l���ɕ\��
        textDeckNamePanel.text = state.ToString();

        //����X�e�[�W�̖��O��O��UI�ɕ\��
        textGyojyoTital.text = state.ToString();

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
        //�X�|�[���v���t�B�[���̐ݒ�
        fishSpawnManager.SetSpawnProfile(profiles[0]);

        //�q�b�g�������L���OUI�̕\��
        RankingManager.instance.ShowHitRateRanking(0);
        //�n�C�X�R�A�����L���OUI�̕\��
        RankingManager.instance.ShowHighScoreRanking(0);
        //�����̃n�C�X�R�AUI�̕\��
        ScoreManager.instance.highScoreText.text = PlayfabManager.instance.HighScore_NyarwayOffshore;
        //�ō��q�b�g��UI�̕\��
        ScoreManager.instance.highHitRateText.text = $"{PlayfabManager.instance.HitRate_NyarwayOffshore}%";
        //DeckPanel�Ɏ��鋛�̕\��
        //����X�e�[�W���b�Z�[�W�̕\��
        //DeckPanel�Ɏ擾�����L�ʂƔL�p�����\��
    }

    //�j���i�_���X�e�[�W�I���̏����������ɋL�q
    private void HandleNyanadaOffshore()
    {
        //�X�|�[���v���t�B�[���̐ݒ�
        fishSpawnManager.SetSpawnProfile(profiles[1]);

        //�q�b�g�������L���OUI�̕\��
        RankingManager.instance.ShowHitRateRanking(1);
        //�n�C�X�R�A�����L���OUI�̕\��
        RankingManager.instance.ShowHighScoreRanking(1);
        //�ō��X�R�AUI�̕\��
        ScoreManager.instance.highScoreText.text = PlayfabManager.instance.HighScore_NyanadaOffshore;
        //�ō��q�b�g��UI�̕\��
        ScoreManager.instance.highHitRateText.text = $"{PlayfabManager.instance.HitRate_NyanadaOffshore}%";
    }

    //�j�����J�T�����X�e�[�W�I���̏����������ɋL�q
    private void HandleNyankasanOffshore()
    {
        //�X�|�[���v���t�B�[���̐ݒ�
        fishSpawnManager.SetSpawnProfile(profiles[2]);

        //�q�b�g�������L���OUI�̕\��
        RankingManager.instance.ShowHitRateRanking(2);
        //�n�C�X�R�A�����L���OUI�̕\��
        RankingManager.instance.ShowHighScoreRanking(2);
        //�ō��X�R�AUI�̕\��
        ScoreManager.instance.highScoreText.text = PlayfabManager.instance.HighScore_NyankasanOffshore;
        //�ō��q�b�g��UI�̕\��
        ScoreManager.instance.highHitRateText.text = $"{PlayfabManager.instance.HitRate_NyankasanOffshore}%";
    }

    //�j�����X�J���X�e�[�W�I���̏����������ɋL�q
    private void HandleNyalaskaOffshore()
    {
        //�X�|�[���v���t�B�[���̐ݒ�
        fishSpawnManager.SetSpawnProfile(profiles[3]);

        //�q�b�g�������L���OUI�̕\��
        RankingManager.instance.ShowHitRateRanking(3);
        //�n�C�X�R�A�����L���OUI�̕\��
        RankingManager.instance.ShowHighScoreRanking(3);
        //�ō��X�R�AUI�̕\��
        ScoreManager.instance.highScoreText.text = PlayfabManager.instance.HighScore_NyalaskaOffshore;
        //�ō��q�b�g��UI�̕\��
        ScoreManager.instance.highHitRateText.text = $"{PlayfabManager.instance.HitRate_NyalaskaOffshore}%";
    }

    //�j�����[���X�e�[�W�I���̏����������ɋL�q
    private void HandleNyaruOffshore()
    {
        //�X�|�[���v���t�B�[���̐ݒ�
        fishSpawnManager.SetSpawnProfile(profiles[4]);

        //�q�b�g�������L���OUI�̕\��
        RankingManager.instance.ShowHitRateRanking(4);
        //�n�C�X�R�A�����L���OUI�̕\��
        RankingManager.instance.ShowHighScoreRanking(4);
        //�ō��X�R�AUI�̕\��
        ScoreManager.instance.highScoreText.text = PlayfabManager.instance.HighScore_NyaruOffshore;
        //�ō��q�b�g��UI�̕\��
        ScoreManager.instance.highHitRateText.text = $"{PlayfabManager.instance.HitRate_NyaruOffshore}%";
    }

    //�j�����L���N�J�C���X�e�[�W�I���̏����������ɋL�q
    private void HandleNyankyokukaiOffshore()
    {
        //�X�|�[���v���t�B�[���̐ݒ�
        fishSpawnManager.SetSpawnProfile(profiles[5]);

        //�q�b�g�������L���OUI�̕\��
        RankingManager.instance.ShowHitRateRanking(5);
        //�n�C�X�R�A�����L���OUI�̕\��
        RankingManager.instance.ShowHighScoreRanking(5);
        //�ō��X�R�AUI�̕\��
        ScoreManager.instance.highScoreText.text = PlayfabManager.instance.HighScore_NyankyokukaiOffshore;
        //�ō��q�b�g��UI�̕\��
        ScoreManager.instance.highHitRateText.text = $"{PlayfabManager.instance.HitRate_NyankyokukaiOffshore}%";
    }

    //�j���[�����O�J�C���X�e�[�W�I���̏����������ɋL�q
    private void HandleNyaringkaiOffshore()
    {
        //�X�|�[���v���t�B�[���̐ݒ�
        fishSpawnManager.SetSpawnProfile(profiles[6]);

        //�q�b�g�������L���OUI�̕\��
        RankingManager.instance.ShowHitRateRanking(6);
        //�n�C�X�R�A�����L���OUI�̕\��
        RankingManager.instance.ShowHighScoreRanking(6);
        //�ō��X�R�AUI�̕\��
        ScoreManager.instance.highScoreText.text = PlayfabManager.instance.HighScore_NyaringkaiOffshore;
        //�ō��q�b�g��UI�̕\��
        ScoreManager.instance.highHitRateText.text = $"{PlayfabManager.instance.HitRate_NyaringkaiOffshore}%";
    }

    //�j�����g�J�C���X�e�[�W�I���̏����������ɋL�q
    private void HandleNyaltkaiOffshore()
    {
        //�X�|�[���v���t�B�[���̐ݒ�
        fishSpawnManager.SetSpawnProfile(profiles[7]);

        //�q�b�g�������L���OUI�̕\��
        RankingManager.instance.ShowHitRateRanking(7);
        //�n�C�X�R�A�����L���OUI�̕\��
        RankingManager.instance.ShowHighScoreRanking(7);
        //�ō��X�R�AUI�̕\��
        ScoreManager.instance.highScoreText.text = PlayfabManager.instance.HighScore_NyaltkaiOffshore;
        //�ō��q�b�g��UI�̕\��
        ScoreManager.instance.highHitRateText.text = $"{PlayfabManager.instance.HitRate_NyaltkaiOffshore}%";
    }

    //�j���b�J�C���X�e�[�W�I���̏����������ɋL�q
    private void HandleNyakkaiOffshore()
    {
        //�X�|�[���v���t�B�[���̐ݒ�
        fishSpawnManager.SetSpawnProfile(profiles[8]);

        //�q�b�g�������L���OUI�̕\��
        RankingManager.instance.ShowHitRateRanking(8);
        //�n�C�X�R�A�����L���OUI�̕\��
        RankingManager.instance.ShowHighScoreRanking(8);
        //�ō��X�R�AUI�̕\��
        ScoreManager.instance.highScoreText.text = PlayfabManager.instance.HighScore_NyakkaiOffshore;
        //�ō��q�b�g��UI�̕\��
        ScoreManager.instance.highHitRateText.text = $"{PlayfabManager.instance.HitRate_NyakkaiOffshore}%";
    }

    //�Q�X�g���[�h�I���̏����������ɋL�q
    private void HandleGuestMode()
    {
        //�X�|�[���v���t�B�[���̐ݒ�
        fishSpawnManager.SetSpawnProfile(profiles[9]);

        Debug.Log("�Q�X�g���[�h���I������܂����B");
    }


    //�e����X�e�[�W�̃^�C���I�[�o�[�������s�����\�b�h
    private void TimeOverByStages(StageState state)
    {
        //���݂̋���X�e�[�W�̎����̃n�C�X�R�A�Ə��ʂ��擾
        PlayfabManager.instance.GetAllLeaderboardValues();

        //�n�C�X�R�A�̕\��
        //ScoreManager.instance.highScoreText.text = ScoreManager.instance.highScore.ToString();
        RankingManager.instance.RequestAllHighScoreRanking();

        //�q�b�g���̑��M
        ScoreManager.instance.HitRate();

        //�q�b�g�������L���O�̎擾�ƕ\��
        RankingManager.instance.RequestAllHitRateRanking();

        //���݂̋���X�e�[�W�̎����̃q�b�g���Ə��ʂ��擾
        PlayfabManager.instance.GetAllHitRateLeaderboardValues();

        //DeckPanel��PlayfabID��ݒ�
        textPlayfabID.text = PlayfabManager.instance.masterPlayerAccountID;

        //����X�e�[�W���̏�����switch���Ŏ���
        switch (state)
        {
            case StageState.NyarwayOffshore:
                TimeOverNyarwayOffshore(state);
                break;

            case StageState.NyanadaOffshore:
                TimeOverNyanadaOffshore(state);
                break;

            case StageState.NyankasanOffshore:
                TimeOverNyankasanOffshore(state);
                break;

            case StageState.NyalaskaOffshore:
                TimeOverNyalaskaOffshore(state);
                break;

            case StageState.NyaruOffshore:
                TimeOverNyaruOffshore(state);
                break;

            case StageState.NyankyokukaiOffshore:
                TimeOverNyankyokukaiOffshore(state);
                break;

            case StageState.NyaringkaiOffshore:
                TimeOverNyaringkaiOffshore(state);
                break;

            case StageState.NyaltkaiOffshore:
                TimeOverNyaltkaiOffshore(state);
                break;

            case StageState.NyakkaiOffshore:
                TimeOverNyakkaiOffshore(state);
                break;

            case StageState.GuestMode:

                break;
        }
    }

    //�j�����E�F�[���̃^�C���I�[�o�[���̏����������ɋL�q
    private void TimeOverNyarwayOffshore(StageState state)
    {
        Debug.Log($"{state}�X�e�[�W���I�����܂����B");
    }

    //�j���i�_���̃^�C���I�[�o�[���̏����������ɋL�q
    private void TimeOverNyanadaOffshore(StageState state)
    {
        Debug.Log($"{state}�X�e�[�W���I�����܂����B");
    }

    //�j���i�_���̃^�C���I�[�o�[���̏����������ɋL�q
    private void TimeOverNyankasanOffshore(StageState state)
    {
        Debug.Log($"{state}�X�e�[�W���I�����܂����B");
    }

    //�j���i�_���̃^�C���I�[�o�[���̏����������ɋL�q
    private void TimeOverNyalaskaOffshore(StageState state)
    {
        Debug.Log($"{state}�X�e�[�W���I�����܂����B");
    }

    //�j���i�_���̃^�C���I�[�o�[���̏����������ɋL�q
    private void TimeOverNyaruOffshore(StageState state)
    {
        Debug.Log($"{state}�X�e�[�W���I�����܂����B");
    }

    //�j���i�_���̃^�C���I�[�o�[���̏����������ɋL�q
    private void TimeOverNyankyokukaiOffshore(StageState state)
    {
        Debug.Log($"{state}�X�e�[�W���I�����܂����B");
    }

    //�j���i�_���̃^�C���I�[�o�[���̏����������ɋL�q
    private void TimeOverNyaringkaiOffshore(StageState state)
    {
        Debug.Log($"{state}�X�e�[�W���I�����܂����B");
    }

    //�j���i�_���̃^�C���I�[�o�[���̏����������ɋL�q
    private void TimeOverNyaltkaiOffshore(StageState state)
    {
        Debug.Log($"{state}�X�e�[�W���I�����܂����B");
    }

    //�j���i�_���̃^�C���I�[�o�[���̏����������ɋL�q
    private void TimeOverNyakkaiOffshore(StageState state)
    {
        Debug.Log($"{state}�X�e�[�W���I�����܂����B");
    }
}
