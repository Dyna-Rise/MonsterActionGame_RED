using UnityEngine;

public class MagicBullet : MonoBehaviour
{
    public float deleteTime = 5; // 魔法が消えるまでの時間    初期設定は 5
    //public int attackPower = 1;  // 攻撃力

    void Start()
    {
        Destroy(gameObject, deleteTime);    // 一定時間で自分自身を消す
    }

    // ゲームオブジェクトに接触
    void OnCollisionEnter2D(Collision2D collision)
    {
        Destroy(gameObject);    //とりあえず何かに当たったら魔法弾の方は消しておく
    }
}
