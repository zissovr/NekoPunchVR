using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishPhysics : MonoBehaviour
{
    //魚の移動の力
    public float force;

    //ターゲットEasyのスコア
    public int targetEasyScore = 8;

    private void Start()
    {
        GetComponent<Rigidbody>().AddForce(new Vector3(0, 0, force), ForceMode.Acceleration);
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
        }

        //魚が猫ハンドに当たった猫パンチ数を追加
        if (collision.gameObject.CompareTag("NekoHand"))
        {
            //音を鳴らしパンチ数追加
            AudioManager.instance.punchSound.Play();
            NekopunchManager.instance.AddNekoPunch(1);

            //パーティクル表示
            //GetComponent<ParticleSystem>().Play();
        }
    }
}
