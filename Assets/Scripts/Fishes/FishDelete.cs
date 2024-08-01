using System.Collections;
using UnityEngine;

public class FishDelete : MonoBehaviour
{
    public float DeleteTime = 15f;

    private void Start()
    {
        StartCoroutine(DestroyTimer());

    }

    private void Update()
    {
        if (transform.position.z <= -10f)
        {
            Destroy(gameObject);
            ScoreManager.instance.MissedFish++;
        }

        if (transform.position.x > 17f || transform.position.x < -17f)
        {
            Destroy(gameObject);
            ScoreManager.instance.MissedFish++;
        }


    }

    private IEnumerator DestroyTimer()
    {
        yield return new WaitForSeconds(DeleteTime);
        if (gameObject != null)
        {
            Destroy(gameObject);
            ScoreManager.instance.MissedFish++;
        }

    }
}
