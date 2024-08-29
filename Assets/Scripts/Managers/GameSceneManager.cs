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
    //スコア表示切り替えのオブジェクト設定
    public GameObject currentScoreUI_Gameobject;
    public GameObject finalScoreUI_Gameobject;
    public GameObject rankingUI_Gameobject;
    public GameObject nekopunchUI_Gameobject;
    //鏡と操作パネルのオブジェクト
    public GameObject mirror_Gameobject;
    public GameObject mirrorButton_Gameobject;
    public GameObject deckPanel_Gameobject;
    public TextMeshProUGUI textDeckNamePanel;
    //スコアターゲットのオブジェクト
    public GameObject targets_Gameobject;

    [Header("Managers")]
    public GameObject fishSpawnManager;

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
        //漁場ステージの名前とインデックスをマッピング
        InitializeStageIndices();
    }

    private void Start()
    {
        //RayInteractorの取得と非表示
        RayInteractorDeactivate();

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
        restartButtonUI_Gameobject.SetActive(false);

        //鏡と操作パネルの非表示
        mirror_Gameobject.SetActive(false);
        mirrorButton_Gameobject.SetActive(false);
        deckPanel_Gameobject.SetActive(false);

        //ターゲットの表示
        targets_Gameobject.SetActive(true);

        //移動を制限
        moveLocomotion.SetActive(false);

        //漁場ステージをセット
        //SelectStage(currentState);
        SelectStage(StageState.NyarwayOffshore);
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
        fishSpawnManager.SetActive(false);

        //確定スコア・ランキング表示と現在スコア非表示
        finalScoreUI_Gameobject.SetActive(true);
        rankingUI_Gameobject.SetActive(true);
        nekopunchUI_Gameobject.SetActive(true);
        currentScoreUI_Gameobject.SetActive(false);
        restartButtonUI_Gameobject.SetActive(true);

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
        RankingManager.instance.RequestLeaderBoard();

        //ネコパンチ総数の送信
        PlayfabManager.instance.SubmitNumberOfNekopunch();

        //ネコパンチ総数の取得
        RankingManager.instance.RequestNekopunchLeaderBoard();

        //各漁場ステージのタイムオーバー処理
        TimeOverByStages(currentState);
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
    }

    //前の漁場ステージ変更ボタン
    public void OnClickPreviousChangeGyojoStage()
    {
        int previousIndex = ((int)currentState - 1 + stageIndices.Count - 1) % (stageIndices.Count - 1);
        currentState = (StageState)previousIndex;
        SelectStage(currentState);
    }

    //ゲストモードボタン
    public void OnClickGuestMode()
    {
        SelectStage(StageState.GuestMode);
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
        //ステージのターゲットを表示
        TargetsManager.instance.SelectStage(stageIndices[state]);

        //漁場ステージの名前をデッキパネルに表示
        textDeckNamePanel.text = state.ToString();

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
        Debug.Log("ニャルウェー沖ステージが選択されました。");
        //ランキングUIの表示
        //最高スコアUIの表示
        //最高ヒット率UIの表示
        //DeckPanelに取れる魚の表示
        //漁場ステージメッセージの表示
        //DeckPanelに取得した猫缶と猫用おやつを表示
    }

    //ニャナダ沖ステージ選択の処理をここに記述
    private void HandleNyanadaOffshore()
    {
        Debug.Log("ニャナダ沖ステージが選択されました。");
    }

    //ニャンカサン沖ステージ選択の処理をここに記述
    private void HandleNyankasanOffshore()
    {
        Debug.Log("ニャンカサン沖ステージが選択されました。");
    }

    //ニャラスカ沖ステージ選択の処理をここに記述
    private void HandleNyalaskaOffshore()
    {
        Debug.Log("ニャラスカ沖ステージが選択されました。");
    }

    //ニャルー沖ステージ選択の処理をここに記述
    private void HandleNyaruOffshore()
    {
        Debug.Log("ニャルー沖ステージが選択されました。");
    }

    //ニャンキョクカイ沖ステージ選択の処理をここに記述
    private void HandleNyankyokukaiOffshore()
    {
        Debug.Log("ニャンキョクカイ沖ステージが選択されました。");
    }

    //ニャーリングカイ沖ステージ選択の処理をここに記述
    private void HandleNyaringkaiOffshore()
    {
        Debug.Log("ニャーリングカイ沖ステージが選択されました。");
    }

    //ニャルトカイ沖ステージ選択の処理をここに記述
    private void HandleNyaltkaiOffshore()
    {
        Debug.Log("ニャルトカイ沖ステージが選択されました。");
    }

    //ニャッカイ沖ステージ選択の処理をここに記述
    private void HandleNyakkaiOffshore()
    {
        Debug.Log("ニャッカイ沖ステージが選択されました。");
    }

    //ゲストモード選択の処理をここに記述
    private void HandleGuestMode()
    {
        Debug.Log("ゲストモードが選択されました。");
    }


    //各漁場ステージのタイムオーバー処理を行うメソッド
    private void TimeOverByStages(StageState state)
    {
        //漁場ステージ毎の処理をswitch文で実装
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

    //ニャルウェー沖のタイムオーバー時の処理をここに記述
    private void TimeOverNyarwayOffshore(StageState state)
    {
        Debug.Log($"{state}ステージが終了しました。");

        //ヒット率の確認
        Debug.Log($"スポーンされた魚の数は{ScoreManager.instance.SpawnedFish}");
        Debug.Log($"{state}ステージのヒット率は{ScoreManager.instance.HitRate()}％です。");
    }
}
