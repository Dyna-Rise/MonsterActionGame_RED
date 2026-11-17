using MonsterAction.Battle.Player;
using MonsterAction.Combat;
using MonsterAction.Data;
using System.Collections.Generic;
using UnityEngine;

//public class MagicBullet : PlayerTypeC_Magic
//public class MagicBullet : PlayerBattleController
public class MagicBullet : MonoBehaviour
{
    [Header("弾の消滅時間")]
    [SerializeField] private　float deleteTime = 2.0f; // 魔法が消えるまでの時間    初期設定は 5.0f

    [Header("弾速")]
    [SerializeField] private float bulletSpeed = 20.0f;


    [Header("プレイヤーTypeC")]
    GameObject playerTypeC;

    //仮置き・あとでPlayerTypeCから持ってくる
//    private float bulletDamage = 12.0f;
//    private float AttackMultiplier = 1.0f;

    private PlayerBattleController ownerPlayer;

    private PlayerTypeC_Magic player;

    private float damage;


    void Start()
    {
        Destroy(this.gameObject, deleteTime);    // 一定時間飛んだら自分自身を消す

    }

    private void OnTriggerEnter(Collider other) //other何か　にぶつかったら
    {
        if (other.gameObject.tag != "Player")   //プレイヤー以外にぶつかったら
        {
            Destroy(gameObject);    //この弾は即時消える

            //float dmg = MagicBulletDamage();
            //float dmg = playerTypeC.MagicBulletDamage();

            //↓ここから下、範囲攻撃のものをそのまま持ってきた
            Collider[] hits = Physics.OverlapSphere(
                transform.position + Vector3.up, //こいつの場所を中心に、
                1.0f,                     //範囲(1.0f)
                ~0,                              //←すべてのレイヤーという意味らしい
                QueryTriggerInteraction.Collide  //IsTriggerも含む
                );

            //   3. 同じボスに何回も当てないためのリスト
            List<IDamageable> alreadyHit = new List<IDamageable>();

            foreach (Collider col in hits)
            {
                if (col.transform.IsChildOf(transform)) continue;
                //自分(Player)と、その子(Playerのパーツ)の体なら、飛ばす（＝foreach終了する）

                //ダメージを受けられる相手か
                IDamageable target = col.GetComponentInParent<IDamageable>();
                if (target == null || !target.IsAlive) continue;
                //ターゲットがnullあるいは生きていないなら、foreach終了

                //ボスの部位（頭・しっぽなど）なら、持ち主のボスで「当てたか」を数える
                IDamageable boss = target;
                if (target is IDamagePart part) boss = part.Owner;
                if (alreadyHit.Contains(boss)) continue;
                alreadyHit.Add(boss);

                //敵にダメージを与える
                Vector3 dir = col.bounds.center - transform.position; dir.y = 0f;
                target.ApplyDamage(new DamageInfo(damage, col.bounds.center, dir.normalized, gameObject));

                Debug.Log($"魔法攻撃力x:{damage}");
            }
        }
    }

    //Shoot(プレイヤーの向き、ダメージ、オブジェクトのプレハブ）
    //internal void Shoot(Vector3 magicPos, float damage, GameObject bulletPrefab)
    internal void Shoot(Vector3 playerDirection, float dmg, GameObject bulletPrefab)
    {
        //Vector3 magicPos = transform.position + new Vector3(0.0f, 1.0f, 0.5f);  //追加。発射位置を調整用

        //攻撃開始Bulletプレハブ生成と前方(magicPosの持つ前方)へシュート

        damage = dmg;

        Rigidbody rbody = gameObject.GetComponent<Rigidbody>();  //こいつ自身（生成した魔法弾）のRigidbody

            rbody.AddForce(
            playerDirection * bulletSpeed,
            //transform.forward * bulletSpeed,
            ForceMode.Impulse
            );
        //前シュート！    前方(forward)に、*速度、モード弾く勢い（ジャンプと同じ）
        //打ち出すときにみにチェックしているので、Time.deltatimeはつけなくてもPCによる性能差は出ない

        //Debug.Log("魔法撃ったよ");
        //Debug.Log($"魔法攻撃力:{damage}");

    }

    private float MagicBulletDamage()
    {
        Debug.Log(player);
        //float dmg = GetMagicBulletDamage();
        float dmg = player.GetMagicBulletDamage();
        //float dmg = bulletDamage * AttackMultiplier;
        return dmg;
    }


}
