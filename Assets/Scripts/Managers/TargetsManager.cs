using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetsManager : MonoBehaviour
{
    public static TargetsManager instance;

    //9つの漁場ステージを格納する配列
    public GameObject[] targetGroups;

    //ターゲットのシリンダーを格納する配列
    [SerializeField] private GameObject[] targetCylinder;

    //デッキパネルの点灯シリンダーを格納する配列
    [SerializeField] private GameObject[] deckPanelCylinder;

    //シリンダーのレンダラー
    private Renderer[] deckPanelRenderers;

    //点灯させるシリンダーのマテリアル
    [SerializeField] private Material lightOnMaterial;

    //消灯状態のマテリアル(デフォルト)
    [SerializeField] private Material defaultMaterial;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(this.gameObject);  // シーンの再ロード時に破棄されないようにする
    }
    void Start()
    {
        //最初はすべてのステージを非表示にする
        DeactivateAllStages();

        // レンダラーを取得するメソッドを呼び出す
        InitializeCylinderRenderers();

        //初期化後にdeckPanelCylinderを非アクティブ化
        //SetDeckPanelCylinderActive(false);
    }

    // すべてのステージを非アクティブにする（UI選択用）
    private void DeactivateAllStages()
    {
        if (targetGroups == null || targetGroups.Length == 0)
        {
            Debug.LogError("targetGroups is not set or empty.");
            return;
        }

        foreach (GameObject group in targetGroups)
        {
            if (group != null)
            {
                group.SetActive(false);
            }
        }
    }

    // シーンロード後のすべてのステージを非アクティブ化（シーンロード用）
    public void DeactivateAllStagesOnSceneLoad()
    {
        // シーンロード時に非アクティブ化したいもののみを非アクティブ化する
        DeactivateAllStages();
    }

    //デッキパネルの各シリンダーのレンダラーを取得
    private void InitializeCylinderRenderers()
    {
        if (deckPanelCylinder == null || deckPanelCylinder.Length == 0)
        {
            Debug.LogError("deckPanelCylinder is null or empty.");
            return;
        }

        // 配列のサイズを初期化
        deckPanelRenderers = new Renderer[deckPanelCylinder.Length];

        // 各シリンダーのレンダラーを取得して配列に格納
        for (int i = 0; i < deckPanelCylinder.Length; i++)
        {
            if (deckPanelCylinder[i] != null)
            {
                deckPanelRenderers[i] = deckPanelCylinder[i].GetComponent<Renderer>();
                if (deckPanelRenderers[i] != null)
                {
                    // 初期状態を消灯マテリアルに設定
                    deckPanelRenderers[i].material = defaultMaterial;
                }
                else
                {
                    Debug.LogWarning($"deckPanelCylinder[{i}] does not have a Renderer component.");
                }
            }
            else
            {
                Debug.LogWarning($"deckPanelCylinder[{i}] is null.");
            }
        }

        if (deckPanelRenderers.Length == 0)
        {
            Debug.LogError("deckPanelRenderers is null or empty after initialization.");
        }
    } 

    //デッキパネルをアクティブ化・非アクティブ化するメソッド
    private void SetDeckPanelCylinderActive(bool isActive)
    {
        if (deckPanelCylinder == null)
        {
            Debug.LogError("deckPanelCylinder is null.");
            return;
        }

        foreach (GameObject cylinder in deckPanelCylinder)
        {
            if (cylinder != null)
            {
                cylinder.SetActive(isActive);
            }
            else
            {
                Debug.LogWarning("A cylinder in deckPanelCylinder is null.");
            }
        }
    }

    //UIから呼び出すステージ選択メソッド
    public void SelectStageForUI(int stageIndex)
    {
        Debug.Log($"SelectStageForUI called with stageIndex: {stageIndex}");

        // UI選択時に他のステージを非表示にする
        DeactivateAllStages();

        if (targetGroups == null || targetGroups.Length == 0)
        {
            Debug.LogError("targetGroups is null or empty.");
            return;
        }

        // デッキパネルシリンダーを非アクティブ化
        //SetDeckPanelCylinderActive(false);

        // すべてのシリンダーを消灯状態にする
        /*
        if (deckPanelRenderers != null)
        {
            foreach (var renderer in deckPanelRenderers)
            {
                if (renderer != null)
                {
                    renderer.material = defaultMaterial;
                }
                else
                {
                    Debug.LogWarning("A renderer in deckPanelRenderers is null.");
                }
            }
        }
        */

        if (stageIndex >= 0 && stageIndex < targetGroups.Length)
        {
            if (targetGroups[stageIndex] != null)
            {
                targetGroups[stageIndex].SetActive(true);
            }
            else
            {
                Debug.LogWarning($"Invalid stageIndex: {stageIndex} or targetGroups[{stageIndex}] is null.");
            }

            // 選択されたステージのデッキパネル上のシリンダーを点灯させる
            /*
            if (stageIndex < deckPanelRenderers.Length && deckPanelRenderers[stageIndex] != null)
            {
                deckPanelRenderers[stageIndex].material = lightOnMaterial;
                Debug.Log($"Set light on for renderer at index {stageIndex}.");
            }
            else
            {
                Debug.LogWarning($"deckPanelRenderers[{stageIndex}] is null or out of range.");
            }
            */
        }
        else
        {
            Debug.LogWarning($"Invalid stageIndex: {stageIndex} is out of range.");
        }
    }

    // シーンロード時に呼び出すステージ選択メソッド
    public void SelectStageForSceneLoad(int stageIndex)
    {
        // シーンロード時はすべてのステージを非表示にする
        DeactivateAllStages();

        if (targetGroups == null || targetGroups.Length == 0)
        {
            Debug.LogError("targetGroups is null or empty.");
            return;
        }

        if (stageIndex >= 0 && stageIndex < targetGroups.Length)
        {
            if (targetGroups[stageIndex] != null)
            {
                targetGroups[stageIndex].SetActive(true);
            }
            else
            {
                Debug.LogWarning($"Invalid stageIndex: {stageIndex} or targetGroups[{stageIndex}] is null.");
            }
        }
        else
        {
            Debug.LogWarning($"Invalid stageIndex: {stageIndex} is out of range.");
        }
    }

    //タイムオーバー時にすべてのターゲットをアクティブに戻す
    public void ActivateAllTargets()
    {
        if (targetCylinder == null || targetCylinder.Length == 0)
        {
            Debug.LogError("targetCylinder is null or empty.");
            return;
        }

        foreach (GameObject target in targetCylinder)
        {
            if (target != null)
            {
                target.SetActive(true);
            }
            else
            {
                Debug.LogWarning("targetCylinder contains a null element.");
            }
        }
    }
}