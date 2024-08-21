using UnityEngine;

public class FishPhysics : MonoBehaviour
{
    //魚の移動の力
    public float force;

    //ターゲットEasyのスコア
    public int targetEasyScore = 8;

    //ターゲットMoveのスコア
    public int targetMoveScore = 12;

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
            Destroy(collision.gameObject);

            //音を鳴らしスコア追加
            AudioManager.instance.hitSound.Play();
            ScoreManager.instance.AddScore(targetEasyScore);
            ScoreManager.instance.HitFish++;
        }

        //魚がターゲットMoveに当たったら
        if (collision.gameObject.CompareTag("TargetMove"))
        {
            Destroy(this.gameObject);
            Destroy(collision.gameObject);

            //音を鳴らしスコア追加
            AudioManager.instance.hitSound.Play();
            ScoreManager.instance.AddScore(targetMoveScore);
            ScoreManager.instance.HitFish++;
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
