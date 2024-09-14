using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckPanelManager : MonoBehaviour
{
    public static DeckPanelManager instance;

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

    public void SelectStageForDeckPanel(int stageIndex)
    {
        //デッキパネル上のシリンダーを点灯
        DeckPanelCylinderLightOn(stageIndex);
    }

    public void DeckPanelCylinderRenderer()
    {
        //配列のサイズを初期化
        deckPanelRenderers = new Renderer[deckPanelCylinder.Length];

        // 各シリンダーのレンダラーを取得して配列に格納
        for (int i = 0; i < deckPanelCylinder.Length; i++)
        {
            deckPanelRenderers[i] = deckPanelCylinder[i].GetComponent<Renderer>();
            if (deckPanelRenderers[i] != null)
            {
                // 初期状態を消灯マテリアルに設定
                deckPanelRenderers[i].material = defaultMaterial;
            }
        }
    }

    public void DeckPanelCylinderLightOn(int stageIndex)
    {
        //全てのシリンダーマテリアルをデフォルトに戻す
        for (int i = 0; i < deckPanelRenderers.Length; i++)
        {
            if (deckPanelRenderers[i] != null)
            {
                deckPanelRenderers[i].material = defaultMaterial;
            }
        }

        // 選択されたステージのデッキパネル上のシリンダーを点灯させる
        if (stageIndex < deckPanelRenderers.Length && deckPanelRenderers[stageIndex] != null)
        {
            deckPanelRenderers[stageIndex].material = lightOnMaterial;
        }
    }
}
