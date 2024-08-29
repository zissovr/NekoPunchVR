using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetsManager : MonoBehaviour
{
    public static TargetsManager instance;

    //9つの漁場ステージを格納する配列
    [SerializeField] private GameObject[] targetGroups;

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
    }
    void Start()
    {
        //最初はすべてのステージを非表示にする
        DeactivateAllStages();

        // レンダラーを取得するメソッドを呼び出す
        CylinderRenderer();
    }

    //すべてのステージを非アクティブにするメソッド
    void DeactivateAllStages()
    {
        foreach (GameObject group in targetGroups)
        {
            group.SetActive(false);
        }
    }

    //デッキパネルの各シリンダーのレンダラーを取得
    private void CylinderRenderer()
    {
        //配列のサイズを初期化
        deckPanelRenderers = new Renderer[deckPanelCylinder.Length];

        //各シリンダーのレンダラーを取得して配列に格納
        for (int i = 0; i < deckPanelCylinder.Length; i++)
        {
            deckPanelRenderers[i] = deckPanelCylinder[i].GetComponent<Renderer>();
            if (deckPanelRenderers[i] != null)
            {
                //初期状態を消灯マテリアルに設定
                deckPanelRenderers[i].material = defaultMaterial;
            }
        }
    } 

    //UIから呼び出すステージ選択メソッド
    public void SelectStage(int stageIndex)
    {
        //他のステージを一括非表示にする
        DeactivateAllStages();

        //すべてのシリンダーを消灯状態にする
        for (int i = 0; i < deckPanelRenderers.Length; i++)
        {
            if (deckPanelRenderers[i] != null)
            {
                deckPanelRenderers[i].material = defaultMaterial;
            }
        }

        if (stageIndex >= 0 && stageIndex < targetGroups.Length)
        {
            //選択されたステージのみを表示する
            targetGroups[stageIndex].SetActive(true);

            //選択されたステージのデッキパネル上のシリンダーを点灯させる
            if(stageIndex < deckPanelRenderers.Length && deckPanelRenderers[stageIndex] != null)
            {
                deckPanelRenderers[stageIndex].material = lightOnMaterial;
            }
        }
    }

    //タイムオーバー時にすべてのターゲットをアクティブに戻す
    public void ActivateAllTargets()
    {
        foreach(GameObject target in targetCylinder)
        {
            target.SetActive(true);
        }
    }
}