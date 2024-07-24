using UnityEngine;

public class FishDelete : MonoBehaviour
{
    public float DeleteTime = 15f;

    private void Start()
    {
        Destroy(gameObject, DeleteTime);
    }

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
