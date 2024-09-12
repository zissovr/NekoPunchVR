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
    //スコア表示切り替えのオブジェクト設定
    public GameObject currentScoreUI_Gameobject;
    public GameObject finalScoreUI_Gameobject;
    public GameObject rankingUI_Gameobject;
    public GameObject nekopunchUI_Gameobject;
    public GameObject hitRateUI_Gameobject;
    public GameObject daylypunchUI_Gameobject;
    //鏡と操作パネルのオブジェクト
    public GameObject mirror_Gameobject;
    public GameObject mirrorButton_Gameobject;
    public GameObject deckPanel_Gameobject;
    public TextMeshProUGUI textDeckNamePanel;
    public TextMeshProUGUI textDeckNyancoin;
    public TextMeshProUGUI textPlayfabID;
    //スコアターゲットのオブジェクト
    public GameObject targets_Gameobject;
    //漁場タイトル
    public GameObject gyojyoTitle;
    public TextMeshProUGUI textGyojyoTital;

    [Header("SpawnProfile")]
    public SpawnProfile[] profiles;


    [Header("Managers")]
    public FishSpawnManager fishSpawnManager;

    [Header("XRInteraction")]
    //RayInteractorの取得
    public GameObject rightRayInteractor;
    public GameObject leftRayInteractor;
    XRRayInteractor rightxrRay;
    XRRayInteractor leftxrRay;
    //Moveの取得
    public GameObject moveLocomotion;

    //オーディオの長さの変数
    float audioClipLength;

    //現在の漁場ステージステート
    public StageState currentState = 0;
    private Dictionary<StageState, int> stageIndices;

    //漁場ステージ
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

        // シーンがロードされたときのイベントにリスナーを追加
        SceneManager.sceneLoaded += OnSceneLoaded;

        //漁場ステージの名前とインデックスをマッピング
        InitializeStageIndices();
    }

    private void OnDestroy()
    {
        // シーンのロードイベントのリスナーを解除
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // シーンがロードされたときの処理
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (TargetsManager.instance != null)
        {
            // ターゲットの状態をリセット
            ActivateSelectedStageTargets();
        }
        else
        {
            Debug.LogError("TargetsManager instance is null after scene load.");
        }
    }

    private void Start()
    {
        //RayInteractorの取得と非表示
        RayInteractorDeactivate();

        // シーンロード後の初期化
        if (TargetsManager.instance != null)
        {
            // シーンロード後にステージのターゲットを非アクティブ化
            TargetsManager.instance.DeactivateAllStagesOnSceneLoad();
        }
        // 0.5秒後にターゲットグループの表示を行う
        StartCoroutine(SelectStageAfterDelay(0.5f));

        //曲の長さを取得
        audioClipLength = AudioManager.instance.musicTheme.clip.length;

        //曲のカウントダウンをスタートさせる
        StartCoroutine(StartCountdown(audioClipLength));

        //プログレスバーを初期化
        progressBarImage.fillAmount = Mathf.Clamp(0, 0, 1);

        //確定スコア・ランキング非表示と現在スコア表示
        finalScoreUI_Gameobject.SetActive(false);
        rankingUI_Gameobject.SetActive(false);
        nekopunchUI_Gameobject.SetActive(false);
        currentScoreUI_Gameobject.SetActive(true);
        hitRateUI_Gameobject.SetActive(false);
        daylypunchUI_Gameobject.SetActive(false);

        //漁場タイトルの非表示
        gyojyoTitle.SetActive(false);

        //鏡と操作パネルの非表示
        mirror_Gameobject.SetActive(false);
        mirrorButton_Gameobject.SetActive(false);
        deckPanel_Gameobject.SetActive(false);

        //ターゲットの非表示
        //targets_Gameobject.SetActive(false);

        //移動を制限
        moveLocomotion.SetActive(false);

        //PlayerPrefsから保存されたステージステートを読み込む
        if (PlayerPrefs.HasKey("SelectedStageState"))
        {
            currentState = (StageState)PlayerPrefs.GetInt("SelectedStageState");
        }
        else
        {
            //デフォルトステージ
            currentState = StageState.NyarwayOffshore;
        }

        //漁場ステージをセット
        //SelectStage(currentState);
        SelectStageForSceneLoad(currentState);
        //SelectStage(StageState.NyarwayOffshore);

        //選択されたステージのターゲットオブジェクトをアクティブ化
        ActivateSelectedStageTargets();

        //スポーン開始
        fishSpawnManager.gameObject.SetActive(true);
    }

    private IEnumerator SelectStageAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        // 現在のステージのターゲットグループを選択する
        int stageIndex = stageIndices[currentState];
        TargetsManager.instance.SelectStageForSceneLoad(stageIndex);
    }

    //選択されたステージのターゲットオブジェクトをアクティブ化するメソッド
    private void ActivateSelectedStageTargets() 
    {
        if (TargetsManager.instance == null)
        {
            Debug.LogError("TargetsManager.instance is null in ActivateSelectedStageTargets.");
            return;
        }

        // 親オブジェクト（Targets）が非アクティブの場合、アクティブ化する
        if (!TargetsManager.instance.gameObject.activeInHierarchy)
        {
            TargetsManager.instance.gameObject.SetActive(true);
            Debug.Log("Parent object 'Targets' has been activated.");
        }

        // 選択された漁場ステージのインデックス
        if (!stageIndices.ContainsKey(currentState))
        {
            Debug.LogError($"StageState {currentState} not found in stageIndices.");
            return;
        }

        int stageIndex = stageIndices[currentState];

        // すべてのステージを非アクティブにする
        foreach (GameObject targetGroup in TargetsManager.instance.targetGroups)
        {
            if (targetGroup != null)
            {
                targetGroup.SetActive(false);
            }
        }

        // 対象の漁場ステージをアクティブ化
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

    //RayInteractorの取得と非表示
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

    //タイムオーバー処理
    public void TimeOver()
    {
        Debug.Log("Time Over");
        timeText.text = ConvertToMinAndSeconds(0);

        //スポーン終了
        fishSpawnManager.gameObject.SetActive(false);

        //確定スコア・ランキング表示と現在スコア非表示
        finalScoreUI_Gameobject.SetActive(true);
        rankingUI_Gameobject.SetActive(true);
        nekopunchUI_Gameobject.SetActive(true);
        currentScoreUI_Gameobject.SetActive(false);
        hitRateUI_Gameobject.SetActive(true);
        daylypunchUI_Gameobject.SetActive(true);

        //漁場タイトルの表示
        gyojyoTitle.SetActive(true);
        textGyojyoTital.text = currentState.ToString();

        //鏡と操作パネルの表示
        mirror_Gameobject.SetActive(true);
        mirrorButton_Gameobject.SetActive(true);
        deckPanel_Gameobject.SetActive(true);
        textDeckNamePanel.text = currentState.ToString();

        //ターゲットの非表示
        //targets_Gameobject.SetActive(false);

        //ターゲットの表示
        TargetsManager.instance.ActivateAllTargets();

        //レイの表示
        rightxrRay.enabled = true;
        leftxrRay.enabled = true;

        //移動を開放
        moveLocomotion.SetActive(true);

        //ランキングの取得
        //RankingManager.instance.RequestLeaderBoard();

        //ネコパンチ総数の送信
        PlayfabManager.instance.SubmitNumberOfNekopunch();

        //ネコパンチ総数の取得
        RankingManager.instance.RequestNekopunchLeaderBoard();

        //ニャンコインの取得と表示
        GetNyancoin();

        //各漁場ステージのタイムオーバー処理
        TimeOverByStages(currentState);

        //非同期処理でデイリーパンチを送信
        StartCoroutine(PlayfabManager.instance.SendDailyPunchCountToPlayFab());

        //デッキパネルのシリンダーを初期化
        DeckPanelManager.instance.DeckPanelCylinderRenderer();

        //デッキパネル上のシリンダーを点灯
        DeckPanelManager.instance.DeckPanelCylinderLightOn((int)currentState);

        //ヒット率ランキングUIの表示
        RankingManager.instance.ShowHitRateRanking((int)currentState);

        //ハイスコアランキングUIの表示
        RankingManager.instance.ShowHighScoreRanking((int)currentState);
    }

    //分と秒に変換処理
    public string ConvertToMinAndSeconds(float totalTimeInSeconds)
    {
        string timeText = Mathf.Floor(totalTimeInSeconds / 60).ToString("00") + ":" + Mathf.FloorToInt(totalTimeInSeconds % 60).ToString("00");
        return timeText;
    }

    //リスタートボタン
    public void OnClickBackToScene()
    {
        SceneManager.LoadScene("SampleScene");
    }

    //スタートボタン
    public void OnClickStart()
    {
        SceneManager.LoadScene("SampleScene");
    }

    //次のテクスチャに変更するボタン
    public void OnClickChangeTexture()
    {
        TextureChanger.instance.ChangeNextTexture();
    }

    //前のテクスチャに変更するボタン
    public void OnClickPreviousChangeTexture()
    {
        TextureChanger.instance.ChangePreviousTexture();
    }

    //漁場ステージ変更ボタン
    public void OnClickChangeGyojoStage()
    {
        int nextIndex = ((int)currentState + 1) % (stageIndices.Count - 1);
        currentState = (StageState)nextIndex;
        SelectStage(currentState);

        //漁場ステージ選択をPlayerPrefsに保存
        PlayerPrefs.SetInt("SelectedStageState", (int)currentState);
        PlayerPrefs.Save();

        //前回のスコアとヒット率を初期化
        ScoreManager.instance.InitializeCurrentScoreAndHitRate();
    }

    //前の漁場ステージ変更ボタン
    public void OnClickPreviousChangeGyojoStage()
    {
        int previousIndex = ((int)currentState - 1 + stageIndices.Count - 1) % (stageIndices.Count - 1);
        currentState = (StageState)previousIndex;
        SelectStage(currentState);

        //漁場ステージ選択をPlayerPrefsに保存
        PlayerPrefs.SetInt("SelectedStageState", (int)currentState);
        PlayerPrefs.Save();

        //前回のスコアとヒット率を初期化
        ScoreManager.instance.InitializeCurrentScoreAndHitRate();
    }

    // ステージを選択し、適切なメソッドを呼び出す
    private void SelectStageForUI(StageState state)
    {
        int stageIndex = stageIndices[state];

        // UI操作時にはSelectStageFor...を呼び出す
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

    // シーンロード時にステージを選択するメソッド
    private void SelectStageForSceneLoad(StageState state)
    {
        int stageIndex = stageIndices[state];

        // シーンロード時にはSelectStageForSceneLoadを呼び出す
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

    //ゲストモードボタン
    public void OnClickGuestMode()
    {
        SelectStage(StageState.GuestMode);
    }

    //ニャンコインの取得と表示
    private void GetNyancoin()
    {
        int coin = PlayerPrefs.GetInt("TotalNekoPunch", 0);
        textDeckNyancoin.text = coin.ToString();
    }

    //ステージの名前とインデックスをマッピング
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

    //漁場ステージを選択し処理を行うメソッド
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

        //ステージのターゲットを表示
        SelectStageForUI(state);


        //現在のステージを更新
        currentState = state;

        //漁場ステージの名前をデッキパネルに表示
        textDeckNamePanel.text = state.ToString();

        //漁場ステージの名前を前面UIに表示
        textGyojyoTital.text = state.ToString();

        //漁場ステージ毎の処理をswitch文で実装
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

    //ニャルウェー沖ステージ選択の処理をここに記述
    private void HandleNyarwayOffshore()
    {
        //スポーンプロフィールの設定
        fishSpawnManager.SetSpawnProfile(profiles[0]);

        //ヒット率ランキングUIの表示
        RankingManager.instance.ShowHitRateRanking(0);
        //ハイスコアランキングUIの表示
        RankingManager.instance.ShowHighScoreRanking(0);
        //自分のハイスコアUIの表示
        ScoreManager.instance.highScoreText.text = PlayfabManager.instance.HighScore_NyarwayOffshore;
        //最高ヒット率UIの表示
        ScoreManager.instance.highHitRateText.text = $"{PlayfabManager.instance.HitRate_NyarwayOffshore}%";
        //DeckPanelに取れる魚の表示
        //漁場ステージメッセージの表示
        //DeckPanelに取得した猫缶と猫用おやつを表示
    }

    //ニャナダ沖ステージ選択の処理をここに記述
    private void HandleNyanadaOffshore()
    {
        //スポーンプロフィールの設定
        fishSpawnManager.SetSpawnProfile(profiles[1]);

        //ヒット率ランキングUIの表示
        RankingManager.instance.ShowHitRateRanking(1);
        //ハイスコアランキングUIの表示
        RankingManager.instance.ShowHighScoreRanking(1);
        //最高スコアUIの表示
        ScoreManager.instance.highScoreText.text = PlayfabManager.instance.HighScore_NyanadaOffshore;
        //最高ヒット率UIの表示
        ScoreManager.instance.highHitRateText.text = $"{PlayfabManager.instance.HitRate_NyanadaOffshore}%";
    }

    //ニャンカサン沖ステージ選択の処理をここに記述
    private void HandleNyankasanOffshore()
    {
        //スポーンプロフィールの設定
        fishSpawnManager.SetSpawnProfile(profiles[2]);

        //ヒット率ランキングUIの表示
        RankingManager.instance.ShowHitRateRanking(2);
        //ハイスコアランキングUIの表示
        RankingManager.instance.ShowHighScoreRanking(2);
        //最高スコアUIの表示
        ScoreManager.instance.highScoreText.text = PlayfabManager.instance.HighScore_NyankasanOffshore;
        //最高ヒット率UIの表示
        ScoreManager.instance.highHitRateText.text = $"{PlayfabManager.instance.HitRate_NyankasanOffshore}%";
    }

    //ニャラスカ沖ステージ選択の処理をここに記述
    private void HandleNyalaskaOffshore()
    {
        //スポーンプロフィールの設定
        fishSpawnManager.SetSpawnProfile(profiles[3]);

        //ヒット率ランキングUIの表示
        RankingManager.instance.ShowHitRateRanking(3);
        //ハイスコアランキングUIの表示
        RankingManager.instance.ShowHighScoreRanking(3);
        //最高スコアUIの表示
        ScoreManager.instance.highScoreText.text = PlayfabManager.instance.HighScore_NyalaskaOffshore;
        //最高ヒット率UIの表示
        ScoreManager.instance.highHitRateText.text = $"{PlayfabManager.instance.HitRate_NyalaskaOffshore}%";
    }

    //ニャルー沖ステージ選択の処理をここに記述
    private void HandleNyaruOffshore()
    {
        //スポーンプロフィールの設定
        fishSpawnManager.SetSpawnProfile(profiles[4]);

        //ヒット率ランキングUIの表示
        RankingManager.instance.ShowHitRateRanking(4);
        //ハイスコアランキングUIの表示
        RankingManager.instance.ShowHighScoreRanking(4);
        //最高スコアUIの表示
        ScoreManager.instance.highScoreText.text = PlayfabManager.instance.HighScore_NyaruOffshore;
        //最高ヒット率UIの表示
        ScoreManager.instance.highHitRateText.text = $"{PlayfabManager.instance.HitRate_NyaruOffshore}%";
    }

    //ニャンキョクカイ沖ステージ選択の処理をここに記述
    private void HandleNyankyokukaiOffshore()
    {
        //スポーンプロフィールの設定
        fishSpawnManager.SetSpawnProfile(profiles[5]);

        //ヒット率ランキングUIの表示
        RankingManager.instance.ShowHitRateRanking(5);
        //ハイスコアランキングUIの表示
        RankingManager.instance.ShowHighScoreRanking(5);
        //最高スコアUIの表示
        ScoreManager.instance.highScoreText.text = PlayfabManager.instance.HighScore_NyankyokukaiOffshore;
        //最高ヒット率UIの表示
        ScoreManager.instance.highHitRateText.text = $"{PlayfabManager.instance.HitRate_NyankyokukaiOffshore}%";
    }

    //ニャーリングカイ沖ステージ選択の処理をここに記述
    private void HandleNyaringkaiOffshore()
    {
        //スポーンプロフィールの設定
        fishSpawnManager.SetSpawnProfile(profiles[6]);

        //ヒット率ランキングUIの表示
        RankingManager.instance.ShowHitRateRanking(6);
        //ハイスコアランキングUIの表示
        RankingManager.instance.ShowHighScoreRanking(6);
        //最高スコアUIの表示
        ScoreManager.instance.highScoreText.text = PlayfabManager.instance.HighScore_NyaringkaiOffshore;
        //最高ヒット率UIの表示
        ScoreManager.instance.highHitRateText.text = $"{PlayfabManager.instance.HitRate_NyaringkaiOffshore}%";
    }

    //ニャルトカイ沖ステージ選択の処理をここに記述
    private void HandleNyaltkaiOffshore()
    {
        //スポーンプロフィールの設定
        fishSpawnManager.SetSpawnProfile(profiles[7]);

        //ヒット率ランキングUIの表示
        RankingManager.instance.ShowHitRateRanking(7);
        //ハイスコアランキングUIの表示
        RankingManager.instance.ShowHighScoreRanking(7);
        //最高スコアUIの表示
        ScoreManager.instance.highScoreText.text = PlayfabManager.instance.HighScore_NyaltkaiOffshore;
        //最高ヒット率UIの表示
        ScoreManager.instance.highHitRateText.text = $"{PlayfabManager.instance.HitRate_NyaltkaiOffshore}%";
    }

    //ニャッカイ沖ステージ選択の処理をここに記述
    private void HandleNyakkaiOffshore()
    {
        //スポーンプロフィールの設定
        fishSpawnManager.SetSpawnProfile(profiles[8]);

        //ヒット率ランキングUIの表示
        RankingManager.instance.ShowHitRateRanking(8);
        //ハイスコアランキングUIの表示
        RankingManager.instance.ShowHighScoreRanking(8);
        //最高スコアUIの表示
        ScoreManager.instance.highScoreText.text = PlayfabManager.instance.HighScore_NyakkaiOffshore;
        //最高ヒット率UIの表示
        ScoreManager.instance.highHitRateText.text = $"{PlayfabManager.instance.HitRate_NyakkaiOffshore}%";
    }

    //ゲストモード選択の処理をここに記述
    private void HandleGuestMode()
    {
        //スポーンプロフィールの設定
        fishSpawnManager.SetSpawnProfile(profiles[9]);

        Debug.Log("ゲストモードが選択されました。");
    }


    //各漁場ステージのタイムオーバー処理を行うメソッド
    private void TimeOverByStages(StageState state)
    {
        //現在の漁場ステージの自分のハイスコアと順位を取得
        PlayfabManager.instance.GetAllLeaderboardValues();

        //ハイスコアの表示
        //ScoreManager.instance.highScoreText.text = ScoreManager.instance.highScore.ToString();
        RankingManager.instance.RequestAllHighScoreRanking();

        //ヒット率の送信
        ScoreManager.instance.HitRate();

        //ヒット率ランキングの取得と表示
        RankingManager.instance.RequestAllHitRateRanking();

        //現在の漁場ステージの自分のヒット率と順位を取得
        PlayfabManager.instance.GetAllHitRateLeaderboardValues();

        //DeckPanelにPlayfabIDを設定
        textPlayfabID.text = PlayfabManager.instance.masterPlayerAccountID;

        //漁場ステージ毎の処理をswitch文で実装
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

    //ニャルウェー沖のタイムオーバー時の処理をここに記述
    private void TimeOverNyarwayOffshore(StageState state)
    {
        Debug.Log($"{state}ステージが終了しました。");
    }

    //ニャナダ沖のタイムオーバー時の処理をここに記述
    private void TimeOverNyanadaOffshore(StageState state)
    {
        Debug.Log($"{state}ステージが終了しました。");
    }

    //ニャナダ沖のタイムオーバー時の処理をここに記述
    private void TimeOverNyankasanOffshore(StageState state)
    {
        Debug.Log($"{state}ステージが終了しました。");
    }

    //ニャナダ沖のタイムオーバー時の処理をここに記述
    private void TimeOverNyalaskaOffshore(StageState state)
    {
        Debug.Log($"{state}ステージが終了しました。");
    }

    //ニャナダ沖のタイムオーバー時の処理をここに記述
    private void TimeOverNyaruOffshore(StageState state)
    {
        Debug.Log($"{state}ステージが終了しました。");
    }

    //ニャナダ沖のタイムオーバー時の処理をここに記述
    private void TimeOverNyankyokukaiOffshore(StageState state)
    {
        Debug.Log($"{state}ステージが終了しました。");
    }

    //ニャナダ沖のタイムオーバー時の処理をここに記述
    private void TimeOverNyaringkaiOffshore(StageState state)
    {
        Debug.Log($"{state}ステージが終了しました。");
    }

    //ニャナダ沖のタイムオーバー時の処理をここに記述
    private void TimeOverNyaltkaiOffshore(StageState state)
    {
        Debug.Log($"{state}ステージが終了しました。");
    }

    //ニャナダ沖のタイムオーバー時の処理をここに記述
    private void TimeOverNyakkaiOffshore(StageState state)
    {
        Debug.Log($"{state}ステージが終了しました。");
    }
}
