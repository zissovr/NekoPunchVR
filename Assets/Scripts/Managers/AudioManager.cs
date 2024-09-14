using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioSource[] musicThemes;
    public AudioSource hitSound;
    public AudioSource punchSound;

    //オーディオの長さの変数
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
        //曲の長さを取得
        audioClipLength = musicThemes[stageIndex].clip.length;

        //インデックスの範囲をチェック
        if (stageIndex < 0 || stageIndex >= musicThemes.Length)
        {
            Debug.LogError("Invalid stage index: " + stageIndex);
            return;
        }

        //全ての音楽テーマを停止し非アクティブ化
        for (int i = 0; i < musicThemes.Length; i++)
        {
            if (musicThemes[i] != null)
            {
                musicThemes[i].gameObject.SetActive(false);
            }
        }

        //指定されたステージのオーディオをアクティブ化
        if (musicThemes[stageIndex] != null)
        {
            Debug.Log($"StagaIndex: {stageIndex}");
            musicThemes[stageIndex].gameObject.SetActive(true);
            musicThemes[stageIndex].Play();
        }
    }
}
