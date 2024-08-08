using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnockbackOnCollision : MonoBehaviour
{
    //������ԗ͂̑傫��
    public float knockbackForce = 10f;

    private void OnCollisionEnter(Collision collision)
    {
        //����Rigidbody���擾
        Rigidbody rb = collision.collider.GetComponent<Rigidbody>();

        if (rb != null)
        {
            //�Փ˓_�̖@�������ɗ͂�������
            Vector3 knockbackDirection = collision.contacts[0].normal;
            rb.AddForce(-knockbackDirection * knockbackForce, ForceMode.Impulse);
        }
    }
}
