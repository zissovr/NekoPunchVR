using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioSource[] musicThemes;
    public AudioSource hitSound;
    public AudioSource punchSound;

    //�I�[�f�B�I�̒����̕ϐ�
    public float audioClipLength;

    public static AudioManager instance;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    public void PlayWithMusicTheme(int stageIndex)
    {
        //�Ȃ̒������擾
        audioClipLength = musicThemes[stageIndex].clip.length;

        //�C���f�b�N�X�͈̔͂��`�F�b�N
        if (stageIndex < 0 || stageIndex >= musicThemes.Length)
        {
            Debug.LogError("Invalid stage index: " + stageIndex);
            return;
        }

        //�S�Ẳ��y�e�[�}���~����A�N�e�B�u��
        for (int i = 0; i < musicThemes.Length; i++)
        {
            if (musicThemes[i] != null)
            {
                musicThemes[i].gameObject.SetActive(false);
            }
        }

        //�w�肳�ꂽ�X�e�[�W�̃I�[�f�B�I���A�N�e�B�u��
        if (musicThemes[stageIndex] != null)
        {
            Debug.Log($"StagaIndex: {stageIndex}");
            musicThemes[stageIndex].gameObject.SetActive(true);
            musicThemes[stageIndex].Play();
        }
    }
}
