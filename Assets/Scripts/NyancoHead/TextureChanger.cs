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

    //�L�����N�^�[�}�e���A��
    public Material characterMaterial;

    //�\��e�N�X�`���̔z��
    public Texture[] textures;

    //���݂̃e�N�X�`���C���f�b�N�X
    private int currentTextureIndex = 0;

    void Start()
    {
        if (characterMaterial == null)
        {
            Debug.Log("Material not assigned");
        }

        //�����e�N�X�`����ݒ�
        if (textures.Length > 0)
        {
            characterMaterial.mainTexture = textures[currentTextureIndex];
        }
    }

    //���̃e�N�X�`���ɕύX���郁�\�b�h
    public void ChangeNextTexture()
    {
        currentTextureIndex++;
        if (currentTextureIndex >= textures.Length)
        {
            //�ŏ��̃e�N�X�`���ɖ߂�
            currentTextureIndex = 0;
        }

        characterMaterial.mainTexture = textures[currentTextureIndex];
    }

    //�O�̃e�N�X�`���ɕύX���郁�\�b�h
    public void ChangePreviousTexture()
    {
        currentTextureIndex--;
        if (currentTextureIndex < 0)
        {
            //�Ō�̃e�N�X�`���ɖ߂�
            currentTextureIndex = textures.Length - 1;
        }

        characterMaterial.mainTexture = textures[currentTextureIndex];
    }
}
