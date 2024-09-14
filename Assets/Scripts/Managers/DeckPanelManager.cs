using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckPanelManager : MonoBehaviour
{
    public static DeckPanelManager instance;

    //�f�b�L�p�l���̓_���V�����_�[���i�[����z��
    [SerializeField] private GameObject[] deckPanelCylinder;

    //�V�����_�[�̃����_���[
    private Renderer[] deckPanelRenderers;

    //�_��������V�����_�[�̃}�e���A��
    [SerializeField] private Material lightOnMaterial;

    //������Ԃ̃}�e���A��(�f�t�H���g)
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
        //�f�b�L�p�l����̃V�����_�[��_��
        DeckPanelCylinderLightOn(stageIndex);
    }

    public void DeckPanelCylinderRenderer()
    {
        //�z��̃T�C�Y��������
        deckPanelRenderers = new Renderer[deckPanelCylinder.Length];

        // �e�V�����_�[�̃����_���[���擾���Ĕz��Ɋi�[
        for (int i = 0; i < deckPanelCylinder.Length; i++)
        {
            deckPanelRenderers[i] = deckPanelCylinder[i].GetComponent<Renderer>();
            if (deckPanelRenderers[i] != null)
            {
                // ������Ԃ������}�e���A���ɐݒ�
                deckPanelRenderers[i].material = defaultMaterial;
            }
        }
    }

    public void DeckPanelCylinderLightOn(int stageIndex)
    {
        //�S�ẴV�����_�[�}�e���A�����f�t�H���g�ɖ߂�
        for (int i = 0; i < deckPanelRenderers.Length; i++)
        {
            if (deckPanelRenderers[i] != null)
            {
                deckPanelRenderers[i].material = defaultMaterial;
            }
        }

        // �I�����ꂽ�X�e�[�W�̃f�b�L�p�l����̃V�����_�[��_��������
        if (stageIndex < deckPanelRenderers.Length && deckPanelRenderers[stageIndex] != null)
        {
            deckPanelRenderers[stageIndex].material = lightOnMaterial;
        }
    }
}
