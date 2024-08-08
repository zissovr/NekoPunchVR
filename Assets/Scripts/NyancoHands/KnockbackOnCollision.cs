using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnockbackOnCollision : MonoBehaviour
{
    //‚Á”ò‚Ô—Í‚Ì‘å‚«‚³
    public float knockbackForce = 10f;

    private void OnCollisionEnter(Collision collision)
    {
        //‹›‚ÌRigidbody‚ğæ“¾
        Rigidbody rb = collision.collider.GetComponent<Rigidbody>();

        if (rb != null)
        {
            //Õ“Ë“_‚Ì–@ü•ûŒü‚É—Í‚ğ‰Á‚¦‚é
            Vector3 knockbackDirection = collision.contacts[0].normal;
            rb.AddForce(-knockbackDirection * knockbackForce, ForceMode.Impulse);
        }
    }
}
