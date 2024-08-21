using UnityEngine;

public class FishPhysics : MonoBehaviour
{
    //���̈ړ��̗�
    public float force;

    //�^�[�Q�b�gEasy�̃X�R�A
    public int targetEasyScore = 8;

    //�^�[�Q�b�gMove�̃X�R�A
    public int targetMoveScore = 12;

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
            Destroy(collision.gameObject);

            //����炵�X�R�A�ǉ�
            AudioManager.instance.hitSound.Play();
            ScoreManager.instance.AddScore(targetEasyScore);
            ScoreManager.instance.HitFish++;
        }

        //�����^�[�Q�b�gMove�ɓ���������
        if (collision.gameObject.CompareTag("TargetMove"))
        {
            Destroy(this.gameObject);
            Destroy(collision.gameObject);

            //����炵�X�R�A�ǉ�
            AudioManager.instance.hitSound.Play();
            ScoreManager.instance.AddScore(targetMoveScore);
            ScoreManager.instance.HitFish++;
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
