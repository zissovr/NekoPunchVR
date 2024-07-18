using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishPhysics : MonoBehaviour
{
    //���̈ړ��̗�
    public float force;

    //�^�[�Q�b�gEasy�̃X�R�A
    public int targetEasyScore = 8;

    private void Start()
    {
        GetComponent<Rigidbody>().AddForce(new Vector3(0, 0, force), ForceMode.Acceleration);
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
        }

        //�����L�n���h�ɓ��������L�p���`����ǉ�
        if (collision.gameObject.CompareTag("NekoHand"))
        {
            //����炵�p���`���ǉ�
            AudioManager.instance.punchSound.Play();
            NekopunchManager.instance.AddNekoPunch(1);

            //�p�[�e�B�N���\��
            //GetComponent<ParticleSystem>().Play();
        }
    }
}
