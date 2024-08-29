using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetsManager : MonoBehaviour
{
    public static TargetsManager instance;

    //9�̋���X�e�[�W���i�[����z��
    [SerializeField] private GameObject[] targetGroups;

    //�^�[�Q�b�g�̃V�����_�[���i�[����z��
    [SerializeField] private GameObject[] targetCylinder;

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
    void Start()
    {
        //�ŏ��͂��ׂẴX�e�[�W���\���ɂ���
        DeactivateAllStages();

        // �����_���[���擾���郁�\�b�h���Ăяo��
        CylinderRenderer();
    }

    //���ׂẴX�e�[�W���A�N�e�B�u�ɂ��郁�\�b�h
    void DeactivateAllStages()
    {
        foreach (GameObject group in targetGroups)
        {
            group.SetActive(false);
        }
    }

    //�f�b�L�p�l���̊e�V�����_�[�̃����_���[���擾
    private void CylinderRenderer()
    {
        //�z��̃T�C�Y��������
        deckPanelRenderers = new Renderer[deckPanelCylinder.Length];

        //�e�V�����_�[�̃����_���[���擾���Ĕz��Ɋi�[
        for (int i = 0; i < deckPanelCylinder.Length; i++)
        {
            deckPanelRenderers[i] = deckPanelCylinder[i].GetComponent<Renderer>();
            if (deckPanelRenderers[i] != null)
            {
                //������Ԃ������}�e���A���ɐݒ�
                deckPanelRenderers[i].material = defaultMaterial;
            }
        }
    } 

    //UI����Ăяo���X�e�[�W�I�����\�b�h
    public void SelectStage(int stageIndex)
    {
        //���̃X�e�[�W���ꊇ��\���ɂ���
        DeactivateAllStages();

        //���ׂẴV�����_�[��������Ԃɂ���
        for (int i = 0; i < deckPanelRenderers.Length; i++)
        {
            if (deckPanelRenderers[i] != null)
            {
                deckPanelRenderers[i].material = defaultMaterial;
            }
        }

        if (stageIndex >= 0 && stageIndex < targetGroups.Length)
        {
            //�I�����ꂽ�X�e�[�W�݂̂�\������
            targetGroups[stageIndex].SetActive(true);

            //�I�����ꂽ�X�e�[�W�̃f�b�L�p�l����̃V�����_�[��_��������
            if(stageIndex < deckPanelRenderers.Length && deckPanelRenderers[stageIndex] != null)
            {
                deckPanelRenderers[stageIndex].material = lightOnMaterial;
            }
        }
    }

    //�^�C���I�[�o�[���ɂ��ׂẴ^�[�Q�b�g���A�N�e�B�u�ɖ߂�
    public void ActivateAllTargets()
    {
        foreach(GameObject target in targetCylinder)
        {
            target.SetActive(true);
        }
    }
}