using MonsterAction.Combat;
using MonsterAction.Data;
using System;
using System.Collections.Generic;
using UnityEngine;

public class MagicBullet : MonoBehaviour
{
    [Header("弾の消滅時間")]
    public float deleteTime = 2.0f; // 魔法が消えるまでの時間    初期設定は 5.0f

    [Header("弾速")]
    public float bulletSpeed = 20.0f;

    void Start()
    {
        Destroy(gameObject, deleteTime);    // 一定時間飛んだら自分自身を消す
    }

    private void OnTriggerEnter(Collider other) //other何か　にぶつかったら
    {
        if (other.gameObject.tag != "Player")   //プレイヤー以外にぶつかったら
        {
            Destroy(gameObject);    //この弾は即時消える
        }
    }

    //Shoot(プレイヤーの向き、ダメージ、オブジェクトのプレハブ）
    //internal void Shoot(Vector3 magicPos, float damage, GameObject bulletPrefab)
    internal void Shoot(Vector3 playerDirection, float damage)
    {
        //Vector3 magicPos = transform.position + new Vector3(0.0f, 1.0f, 0.5f);  //追加。発射位置を調整用

        //攻撃開始Bulletプレハブ生成と前方(magicPosの持つ前方)へシュート


        Rigidbody rbody = gameObject.GetComponent<Rigidbody>();  //こいつ自身（生成した魔法弾）のRigidbody

            rbody.AddForce(
            playerDirection * bulletSpeed,
            //transform.forward * bulletSpeed,
            ForceMode.Impulse
            );
        //前シュート！    前方(forward)に、*速度、モード弾く勢い（ジャンプと同じ）
        //打ち出すときにみにチェックしているので、Time.deltatimeはつけなくてもPCによる性能差は出ない

        //Debug.Log("魔法撃ったよ");

    }
}
