using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishDelete : MonoBehaviour
{
    private void Update()
    {
        if (transform.position.z <= -10f)
        {
            Destroy(gameObject);
        }

        if (transform.position.x > 17f || transform.position.x < -17f)
        {
            Destroy(gameObject);
        }
    }
}
