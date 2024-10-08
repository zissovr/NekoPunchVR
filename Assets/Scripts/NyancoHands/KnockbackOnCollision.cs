using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnockbackOnCollision : MonoBehaviour
{
    //吹っ飛ぶ力の大きさ
    public float knockbackForce = 10f;

    private void OnCollisionEnter(Collision collision)
    {
        //魚のRigidbodyを取得
        Rigidbody rb = collision.collider.GetComponent<Rigidbody>();

        if (rb != null)
        {
            //衝突点の法線方向に力を加える
            Vector3 knockbackDirection = collision.contacts[0].normal;
            rb.AddForce(-knockbackDirection * knockbackForce, ForceMode.Impulse);
        }
    }
}
