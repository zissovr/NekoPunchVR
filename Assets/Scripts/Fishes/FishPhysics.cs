using UnityEngine;

public class FishPhysics : MonoBehaviour
{
    //���̈ړ��̗�
    public float force;

    //�^�[�Q�b�gEasy�̃X�R�A
    public int targetEasyScore = 8;

    //�^�[�Q�b�gEasy�̃X�R�A
    public int targetMediumScore = 10;

    //�^�[�Q�b�gEasy�̃X�R�A
    public int targetHardScore = 12;

    //�^�[�Q�b�gMove�̃X�R�A
    public int targetMoveScore = 14;

    //�^�[�Q�b�gMoveMedium�̃X�R�A
    public int targetMoveMediumScore = 16;

    //�^�[�Q�b�gMoveMedium�̃X�R�A
    public int targetMoveHardScore = 18;

    //���Ƀq�b�g�����Ƃ��̃G�t�F�N�g
    public GameObject hitFishEffect;

    private void Start()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        //�����Ă�������ɗ͂�������
        rb.AddForce(transform.forward * force, ForceMode.Acceleration);
        ScoreManager.instance.SpawnedFish++;

    }

    private void OnCollisionEnter(Collision collision)
    {
        //�����^�[�Q�b�gEasy�ɓ���������
        if (collision.gameObject.CompareTag("TargetEasy"))
        {
            Destroy(this.gameObject);
            collision.gameObject.SetActive(false);

            //����炵�X�R�A�ǉ�
            AudioManager.instance.hitSound.Play();
            ScoreManager.instance.AddScore(targetEasyScore);
        }

        //�����^�[�Q�b�gMedium�ɓ���������
        if (collision.gameObject.CompareTag("TargetMedium"))
        {
            Destroy(this.gameObject);
            collision.gameObject.SetActive(false);

            //����炵�X�R�A�ǉ�
            AudioManager.instance.hitSound.Play();
            ScoreManager.instance.AddScore(targetMediumScore);
        }

        //�����^�[�Q�b�gHard�ɓ���������
        if (collision.gameObject.CompareTag("TargetHard"))
        {
            Destroy(this.gameObject);
            collision.gameObject.SetActive(false);

            //����炵�X�R�A�ǉ�
            AudioManager.instance.hitSound.Play();
            ScoreManager.instance.AddScore(targetHardScore);
        }

        //�����^�[�Q�b�gMove�ɓ���������
        if (collision.gameObject.CompareTag("TargetMove"))
        {
            Destroy(this.gameObject);
            collision.gameObject.SetActive(false);

            //����炵�X�R�A�ǉ�
            AudioManager.instance.hitSound.Play();
            ScoreManager.instance.AddScore(targetMoveScore);
        }

        //�����^�[�Q�b�gMoveMedium�ɓ���������
        if (collision.gameObject.CompareTag("TargetMoveMedium"))
        {
            Destroy(this.gameObject);
            collision.gameObject.SetActive(false);

            //����炵�X�R�A�ǉ�
            AudioManager.instance.hitSound.Play();
            ScoreManager.instance.AddScore(targetMoveMediumScore);
        }

        //�����^�[�Q�b�gMoveHard�ɓ���������
        if (collision.gameObject.CompareTag("TargetMoveHard"))
        {
            Destroy(this.gameObject);
            collision.gameObject.SetActive(false);

            //����炵�X�R�A�ǉ�
            AudioManager.instance.hitSound.Play();
            ScoreManager.instance.AddScore(targetMoveHardScore);
        }

        //�����L�n���h�ɓ��������L�p���`����ǉ�
        if (collision.gameObject.CompareTag("NekoHand"))
        {
            //����炵�p���`���ǉ�
            AudioManager.instance.punchSound.Play();
            NekopunchManager.instance.AddNekoPunch(1);
            ScoreManager.instance.HitFish++;

            //�p�[�e�B�N���\��
            Instantiate(hitFishEffect, transform.position, Quaternion.identity);
        }
    }
}
