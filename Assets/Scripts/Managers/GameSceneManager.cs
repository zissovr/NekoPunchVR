using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;

public class GameSceneManager : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI timeText;
    public Image progressBarImage;
    public GameObject timerUI_Gameobject;
    public GameObject restartButtonUI_Gameobject;
    //�X�R�A�\���؂�ւ��̃I�u�W�F�N�g�ݒ�
    public GameObject currentScoreUI_Gameobject;
    public GameObject finalScoreUI_Gameobject;
    public GameObject rankingUI_Gameobject;
    public GameObject nekopunchUI_Gameobject;
    //���̃I�u�W�F�N�g
    public GameObject mirror_Gameobject;

    [Header("Managers")]
    public GameObject fishSpawnManager;

    [Header("XRInteraction")]
    //RayInteractor�̎擾
    public GameObject rightRayInteractor;
    public GameObject leftRayInteractor;
    XRRayInteractor rightxrRay;
    XRRayInteractor leftxrRay;
    //Move�̎擾
    public GameObject moveLocomotion;

    //�I�[�f�B�I�̒����̕ϐ�
    float audioClipLength;

    private void Start()
    {
        //RayInteractor�̎擾�Ɣ�\��
        RayInteractorDeactivate();

        //�Ȃ̒������擾
        audioClipLength = AudioManager.instance.musicTheme.clip.length;

        //�Ȃ̃J�E���g�_�E�����X�^�[�g������
        StartCoroutine(StartCountdown(audioClipLength));

        //�v���O���X�o�[��������
        progressBarImage.fillAmount = Mathf.Clamp(0, 0, 1);

        //�m��X�R�A�E�����L���O��\���ƌ��݃X�R�A�\��
        finalScoreUI_Gameobject.SetActive(false);
        rankingUI_Gameobject.SetActive(false);
        nekopunchUI_Gameobject.SetActive(false);
        currentScoreUI_Gameobject.SetActive(true);
        restartButtonUI_Gameobject.SetActive(false);

        //���̔�\��
        mirror_Gameobject.SetActive(false);

        //�ړ��𐧌�
        moveLocomotion.SetActive(false);
    }

    //RayInteractor�̎擾�Ɣ�\��
    public void RayInteractorDeactivate()
    {
        rightxrRay = rightRayInteractor.GetComponent<XRRayInteractor>();
        leftxrRay = leftRayInteractor.GetComponent<XRRayInteractor>();
        rightxrRay.enabled = false;
        leftxrRay.enabled = false;
    }

    public IEnumerator StartCountdown(float countdownValue)
    {
        while (countdownValue > 0)
        {
            yield return new WaitForSeconds(1.0f);
            countdownValue -= 1.0f;

            timeText.text = ConvertToMinAndSeconds(countdownValue);

            progressBarImage.fillAmount = (AudioManager.instance.musicTheme.time / audioClipLength);
        }
        TimeOver();
    }

    //�^�C���I�[�o�[����
    public void TimeOver()
    {
        Debug.Log("Time Over");
        timeText.text = ConvertToMinAndSeconds(0);

        //�X�|�[���I��
        fishSpawnManager.SetActive(false);

        //�m��X�R�A�E�����L���O�\���ƌ��݃X�R�A��\��
        finalScoreUI_Gameobject.SetActive(true);
        rankingUI_Gameobject.SetActive(true);
        nekopunchUI_Gameobject.SetActive(true);
        currentScoreUI_Gameobject.SetActive(false);
        restartButtonUI_Gameobject.SetActive(true);

        //���̔�\��
        mirror_Gameobject.SetActive(true);

        //���C�̕\��
        rightxrRay.enabled = true;
        leftxrRay.enabled = true;

        //�ړ����J��
        moveLocomotion.SetActive(true);

        //�����L���O�̎擾
        RankingManager.instance.RequestLeaderBoard();

        //�l�R�p���`�����̑��M
        PlayfabManager.instance.SubmitNumberOfNekopunch();

        //�l�R�p���`�����̎擾
        RankingManager.instance.RequestNekopunchLeaderBoard();
    }

    //���ƕb�ɕϊ�����
    public string ConvertToMinAndSeconds(float totalTimeInSeconds)
    {
        string timeText = Mathf.Floor(totalTimeInSeconds / 60).ToString("00") + ":" + Mathf.FloorToInt(totalTimeInSeconds % 60).ToString("00");
        return timeText;
    }

    //���X�^�[�g�{�^��
    public void OnClickBackToScene()
    {
        SceneManager.LoadScene("SampleScene");
    }
}
