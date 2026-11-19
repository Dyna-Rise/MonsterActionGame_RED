using System.IO.IsolatedStorage;
using MonsterAction.Combat; //IDamageable使うのに必要
using UnityEngine;

public class BowArrow : MonoBehaviour
{
    public float arrowSpeed=10.0f; //矢のスピード
    public float deleteTime = 3.0f; //削除時間

    private float damage;
    private GameObject owner;
    private bool flinch;

    void Start()
    {
        Destroy(gameObject, deleteTime); // 一定時間で消す
    }

    //PlayerTypeB_Bow.csから引数を受け取る
    public void Shoot(Vector3 dir, float dmg, GameObject own, bool flin)
    {
        //変数にTypeBから受け取った情報を入れる
        damage = dmg;
        owner = own;
        flinch = flin;

        //矢のRigidbody使って矢を飛ばす
        Rigidbody rbody = GetComponent<Rigidbody>();
        rbody.AddForce(dir * arrowSpeed, ForceMode.Impulse);
        //transform.forward(オブジェクト(ここでは矢)の正面)をdir(プレイヤーの正面)に合わせる
        transform.forward = dir;
    }

    private void OnTriggerEnter(Collider other)
    {
        Destroy(gameObject);
        //IDamageableを持っているかどうかで敵かそれ以外かを判定
        // IDamageable:ダメージを受け取れる機能(今回はBossBase.csが所持)

        //GetComponentInParent:衝突した自身からスタートし、親オブジェクト(上階層)に遡って探す
        //                     スクリプトが親に付いてて、Colliderが部位ごとに付いてるときでも対処可能
        //                     ⇔GetComponentsInChildren

        //仮にEnemyタグ使う場合、コライダー同士がぶつかった時はFindGameObjectsWithTag使わず、
        // 相手コライダー(other)をそのまま使う if (other.CompareTag("Enemy"))
        IDamageable target = other.GetComponentInParent<IDamageable>();

        if (target != null)
        {
            //flinch有無でknockback条件付け
            float knockback = flinch ? 15f : 0f;
            //BossBase.csのFlinch()はprotectedで直接呼び出せないため、Damage.Infoを参照
            target.ApplyDamage(new DamageInfo(damage, transform.position, transform.forward, owner, flinch, knockback));

            //相手がEnemyの時だけ刺さり他は弾く仕様
            transform.SetParent(other.transform); //矢くっつける
            //▼ColliderとRigidBodyを無効化しSetParent後も矢が進み続けるのを防止
            //GetComponentsは複数形で配列になる
            foreach (Collider col in GetComponents<Collider>())
            {
                col.enabled = false;
            }

            //  2DならsimulatedをOffに、3DならisKinematicをOnに
            GetComponent<Rigidbody>().isKinematic = true;
        }
    }
}
