using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class TextureChanger : MonoBehaviour
{
    public static TextureChanger instance;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        instance = this;
    }

    //キャラクターマテリアル
    public Material characterMaterial;

    //表情テクスチャの配列
    public Texture[] textures;

    //現在のテクスチャインデックス
    private int currentTextureIndex = 0;

    void Start()
    {
        if (characterMaterial == null)
        {
            Debug.Log("Material not assigned");
        }

        //初期テクスチャを設定
        if (textures.Length > 0)
        {
            characterMaterial.mainTexture = textures[currentTextureIndex];
        }
    }

    //次のテクスチャに変更するメソッド
    public void ChangeNextTexture()
    {
        currentTextureIndex++;
        if (currentTextureIndex >= textures.Length)
        {
            //最初のテクスチャに戻る
            currentTextureIndex = 0;
        }

        characterMaterial.mainTexture = textures[currentTextureIndex];
    }

    //前のテクスチャに変更するメソッド
    public void ChangePreviousTexture()
    {
        currentTextureIndex--;
        if (currentTextureIndex < 0)
        {
            //最後のテクスチャに戻る
            currentTextureIndex = textures.Length - 1;
        }

        characterMaterial.mainTexture = textures[currentTextureIndex];
    }
}
