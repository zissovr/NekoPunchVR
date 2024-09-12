using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NekopunchManager : MonoBehaviour
{
    public static NekopunchManager instance;

    [Header("UI Fields")]
    public TextMeshProUGUI nekoPunchText;
    public TextMeshProUGUI dailyNekoPunchText;

    public int nekoPunch;
    public int dailyNekoPunchCount = 0;

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
        //���܂ł̑��L�p���`�����擾���X�V
        nekoPunch = PlayerPrefs.GetInt("TotalNekoPunch", 0);
        nekoPunchText.text = nekoPunch.ToString();
    }

    public void AddNekoPunch(int nekoPunchPoint)
    {
        nekoPunch = nekoPunch + nekoPunchPoint;
        PlayerPrefs.SetInt("TotalNekoPunch", nekoPunch);

        //�l�R�p���`�����A�b�v�f�[�g
        nekoPunchText.text = nekoPunch.ToString();

        //�f�C���[�L�p���`�������Z
        dailyNekoPunchCount += nekoPunchPoint;
    }
    
}
