using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetsManager : MonoBehaviour
{
    public static TargetsManager instance;

    //9�̋���X�e�[�W���i�[����z��
    public GameObject[] targetGroups;

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
        DontDestroyOnLoad(this.gameObject);  // �V�[���̍ă��[�h���ɔj������Ȃ��悤�ɂ���
    }
    void Start()
    {
        //�ŏ��͂��ׂẴX�e�[�W���\���ɂ���
        DeactivateAllStages();

        // �����_���[���擾���郁�\�b�h���Ăяo��
        InitializeCylinderRenderers();

        //���������deckPanelCylinder���A�N�e�B�u��
        //SetDeckPanelCylinderActive(false);
    }

    // ���ׂẴX�e�[�W���A�N�e�B�u�ɂ���iUI�I��p�j
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

    // �V�[�����[�h��̂��ׂẴX�e�[�W���A�N�e�B�u���i�V�[�����[�h�p�j
    public void DeactivateAllStagesOnSceneLoad()
    {
        // �V�[�����[�h���ɔ�A�N�e�B�u�����������݂̂̂��A�N�e�B�u������
        DeactivateAllStages();
    }

    //�f�b�L�p�l���̊e�V�����_�[�̃����_���[���擾
    private void InitializeCylinderRenderers()
    {
        if (deckPanelCylinder == null || deckPanelCylinder.Length == 0)
        {
            Debug.LogError("deckPanelCylinder is null or empty.");
            return;
        }

        // �z��̃T�C�Y��������
        deckPanelRenderers = new Renderer[deckPanelCylinder.Length];

        // �e�V�����_�[�̃����_���[���擾���Ĕz��Ɋi�[
        for (int i = 0; i < deckPanelCylinder.Length; i++)
        {
            if (deckPanelCylinder[i] != null)
            {
                deckPanelRenderers[i] = deckPanelCylinder[i].GetComponent<Renderer>();
                if (deckPanelRenderers[i] != null)
                {
                    // ������Ԃ������}�e���A���ɐݒ�
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

    //�f�b�L�p�l�����A�N�e�B�u���E��A�N�e�B�u�����郁�\�b�h
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

    //UI����Ăяo���X�e�[�W�I�����\�b�h
    public void SelectStageForUI(int stageIndex)
    {
        Debug.Log($"SelectStageForUI called with stageIndex: {stageIndex}");

        // UI�I�����ɑ��̃X�e�[�W���\���ɂ���
        DeactivateAllStages();

        if (targetGroups == null || targetGroups.Length == 0)
        {
            Debug.LogError("targetGroups is null or empty.");
            return;
        }

        // �f�b�L�p�l���V�����_�[���A�N�e�B�u��
        //SetDeckPanelCylinderActive(false);

        // ���ׂẴV�����_�[��������Ԃɂ���
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

            // �I�����ꂽ�X�e�[�W�̃f�b�L�p�l����̃V�����_�[��_��������
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

    // �V�[�����[�h���ɌĂяo���X�e�[�W�I�����\�b�h
    public void SelectStageForSceneLoad(int stageIndex)
    {
        // �V�[�����[�h���͂��ׂẴX�e�[�W���\���ɂ���
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

    //�^�C���I�[�o�[���ɂ��ׂẴ^�[�Q�b�g���A�N�e�B�u�ɖ߂�
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