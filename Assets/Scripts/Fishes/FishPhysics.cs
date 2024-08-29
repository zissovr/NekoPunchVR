using UnityEngine;

public class FishPhysics : MonoBehaviour
{
    //魚の移動の力
    public float force;

    //ターゲットEasyのスコア
    public int targetEasyScore = 8;

    //ターゲットEasyのスコア
    public int targetMediumScore = 10;

    //ターゲットEasyのスコア
    public int targetHardScore = 12;

    //ターゲットMoveのスコア
    public int targetMoveScore = 14;

    //ターゲットMoveMediumのスコア
    public int targetMoveMediumScore = 16;

    //ターゲットMoveMediumのスコア
    public int targetMoveHardScore = 18;

    //魚にヒットしたときのエフェクト
    public GameObject hitFishEffect;

    private void Start()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        //向いている方向に力を加える
        rb.AddForce(transform.forward * force, ForceMode.Acceleration);
        ScoreManager.instance.SpawnedFish++;

    }

    private void OnCollisionEnter(Collision collision)
    {
        //魚がターゲットEasyに当たったら
        if (collision.gameObject.CompareTag("TargetEasy"))
        {
            Destroy(this.gameObject);
            collision.gameObject.SetActive(false);

            //音を鳴らしスコア追加
            AudioManager.instance.hitSound.Play();
            ScoreManager.instance.AddScore(targetEasyScore);
        }

        //魚がターゲットMediumに当たったら
        if (collision.gameObject.CompareTag("TargetMedium"))
        {
            Destroy(this.gameObject);
            collision.gameObject.SetActive(false);

            //音を鳴らしスコア追加
            AudioManager.instance.hitSound.Play();
            ScoreManager.instance.AddScore(targetMediumScore);
        }

        //魚がターゲットHardに当たったら
        if (collision.gameObject.CompareTag("TargetHard"))
        {
            Destroy(this.gameObject);
            collision.gameObject.SetActive(false);

            //音を鳴らしスコア追加
            AudioManager.instance.hitSound.Play();
            ScoreManager.instance.AddScore(targetHardScore);
        }

        //魚がターゲットMoveに当たったら
        if (collision.gameObject.CompareTag("TargetMove"))
        {
            Destroy(this.gameObject);
            collision.gameObject.SetActive(false);

            //音を鳴らしスコア追加
            AudioManager.instance.hitSound.Play();
            ScoreManager.instance.AddScore(targetMoveScore);
        }

        //魚がターゲットMoveMediumに当たったら
        if (collision.gameObject.CompareTag("TargetMoveMedium"))
        {
            Destroy(this.gameObject);
            collision.gameObject.SetActive(false);

            //音を鳴らしスコア追加
            AudioManager.instance.hitSound.Play();
            ScoreManager.instance.AddScore(targetMoveMediumScore);
        }

        //魚がターゲットMoveHardに当たったら
        if (collision.gameObject.CompareTag("TargetMoveHard"))
        {
            Destroy(this.gameObject);
            collision.gameObject.SetActive(false);

            //音を鳴らしスコア追加
            AudioManager.instance.hitSound.Play();
            ScoreManager.instance.AddScore(targetMoveHardScore);
        }

        //魚が猫ハンドに当たった猫パンチ数を追加
        if (collision.gameObject.CompareTag("NekoHand"))
        {
            //音を鳴らしパンチ数追加
            AudioManager.instance.punchSound.Play();
            NekopunchManager.instance.AddNekoPunch(1);
            ScoreManager.instance.HitFish++;

            //パーティクル表示
            Instantiate(hitFishEffect, transform.position, Quaternion.identity);
        }
    }
}
