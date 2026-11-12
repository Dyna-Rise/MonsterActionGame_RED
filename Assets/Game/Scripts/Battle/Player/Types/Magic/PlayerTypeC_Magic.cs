using System.Collections.Generic;
using MonsterAction.Combat;
using UnityEngine;

namespace MonsterAction.Battle.Player
{
   
    public class PlayerTypeC_Magic : PlayerBattleController
    {
        // ================================================================
        //  Inspector で設定するもの・覚えておく変数
        // ================================================================

        //変数の例）
        //弾の Prefab・ダメージ・撃つまでの時間
        //   ※ MagicBullet.cs を作ってから、// を外してください（先に外すとエラーになります）
        // [Header("魔法：弾")]
        // [SerializeField] private MagicBullet bulletPrefab;  // 弾の Prefab
        // [SerializeField] private float bulletDamage = 12f;  // 弾のダメージ
        // [SerializeField] private float castTime = 0.3f;     // □ を押してから弾が出るまで（秒）

        // TODO（ステップ6）：範囲攻撃の強さ・広さ・エフェクト
        // [Header("魔法：溜め（範囲攻撃）")]
        // [SerializeField] private float burstDamage = 30f;   // 範囲攻撃のダメージ
        // [SerializeField] private float burstRadius = 5f;    // 範囲攻撃の半径（m）
        // [SerializeField] private GameObject burstEffect;    // 範囲攻撃のエフェクト（無くてもよい）

        // 今やっていること（Cast：撃つ / Charge：溜める / Burst：範囲攻撃）… ステップ6で使う
        private enum Step { Cast, Charge, Burst }

        // TODO（ステップ4・6）：今の状態を覚えておく変数
        //   ※ 使いはじめたら // を外してください
        // private bool bulletFired;    // 弾をもう撃ったか（ステップ4）
        // private Step step;           // 今どの Step か（ステップ6）
        // private float stepStartTime; // その Step を始めたときの StateTime（ステップ6）

        protected override void Awake()
        {
            base.Awake();   // ← これが無いと Input などが null のまま（消さない）
        }

        // ================================================================
        //  攻撃を始める（普段の移動中に、毎フレーム呼ばれる）
        // ================================================================

        protected override bool TryStartAction()
        {
            // TODO：□ が押されたら、詠唱を始める
            //   if (Input.ConsumeAttack())
            //   {
            //       1. BeginAction();                                             … 攻撃中にする
            //       2. FaceDirection(GetAttackFacingDirection(), instant: true);  … 狙う方を向く
            //       3. CrossFade("Cast");                                         … 撃つモーション
            //       4. bulletFired = false;                                       … まだ撃っていない（ステップ4）
            //          ステップ6からは、4 のかわりに ChangeStep(Step.Cast);
            //       5. return true;                                               … 攻撃を始めた
            //   }
            //   ヒント：ステップ1では、BeginAction() と Debug.Log("魔法：□ が押された"); だけで試す
            return false; // 何もしなければ false（普段の移動のまま）
        }

        // ================================================================
        //  攻撃中（BeginAction のあと、毎フレーム呼ばれる）
        // ================================================================

        protected override void UpdateAction(float deltaTime)
        {
            // TODO：撃つ（または溜める）→ 少し待って戻る
            //   ★ 書きはじめたら、いちばん下の ReturnToLocomotion(); は消す
            //     （これがあると、すぐ普段の移動に戻ってしまう）
            //
            //   ステップ1：0.5 秒たったら戻る
            //     if (StateTime >= 0.5f) ReturnToLocomotion();
            //
            //   ステップ4：弾を撃つ
            //     1. if (TryCancelIntoDodge()) return;          … 回避できる
            //     2. FaceDirection(GetAttackFacingDirection()); … 狙う方を向き続ける
            //     3. StateTime が castTime 以上 かつ まだ撃っていない（!bulletFired）なら
            //          FireBullet();  bulletFired = true;      … 1回だけ撃つ
            //     4. StateTime が castTime + 0.4f 以上になったら ReturnToLocomotion();
            //
            //   ステップ6：Step で分ける（float t = StateTime - stepStartTime; が「今の Step の秒数」）
            //     if (step == Step.Cast)
            //       … castTime の時点で □ を押し続けていたら CrossFade("ChargeLoad"); ChangeStep(Step.Charge); return;
            //       … 離していたら、ステップ4と同じく弾を撃つ
            //     else if (step == Step.Charge)
            //       … 回避できる。□ を離したら CrossFade("ChargeCast"); Burst(); ChangeStep(Step.Burst);
            //     else if (step == Step.Burst)
            //       … t が 0.8 秒以上になったら ReturnToLocomotion();
            ReturnToLocomotion(); // ← 書きはじめたら消す
        }

        // ================================================================
        //  自分で作るメソッド
        // ================================================================

        /// <summary>Step を切り替える（ステップ6）</summary>
        private void ChangeStep(Step next)
        {
            // TODO：
            //   step = next;                 … 今の Step を変える
            //   stepStartTime = StateTime;   … いつ始めたかを覚える
            //   bulletFired = false;         … 新しい Step では、まだ撃っていない
        }

        /// <summary>弾を撃つ（ステップ4・5）</summary>
        private void FireBullet()
        {
            // TODO：
            //   1. 弾を出す位置：まずは胸の前
            //        Vector3 from = transform.position + Vector3.up * 1.3f + transform.forward * 0.5f;
            //      右手の位置が取れたら、そちらを使う
            //        if (Anim != null && Anim.isHuman)
            //        {
            //            Transform hand = Anim.GetBoneTransform(HumanBodyBones.RightHand);
            //            if (hand != null) from = hand.position + transform.forward * 0.3f;
            //        }
            //   2. 飛ばす向き：ふつうは正面 transform.forward
            //        ステップ5：IsFocusing なら GetAimDirection(from)
            //   3. 弾を作って飛ばす
            //        MagicBullet bullet = Instantiate(bulletPrefab, from, Quaternion.identity);
            //        bullet.Shoot(向き, bulletDamage * AttackMultiplier, gameObject);
        }

        /// <summary>自分の周りにまとめてダメージ（ステップ6）</summary>
        private void Burst()
        {
            // TODO：
            //   1. エフェクトがあれば出して、3 秒後に消す
            //        if (burstEffect != null) { GameObject fx = Instantiate(burstEffect, transform.position, Quaternion.identity); Destroy(fx, 3f); }
            //   2. 周りの Collider を全部集める
            //        Collider[] hits = Physics.OverlapSphere(transform.position + Vector3.up, burstRadius, ~0, QueryTriggerInteraction.Collide);
            //   3. 同じボスに何回も当てないためのリスト
            //        List<IDamageable> alreadyHit = new List<IDamageable>();
            //   4. foreach (Collider col in hits) で1つずつ
            //        ・自分の体なら飛ばす               if (col.transform.IsChildOf(transform)) continue;
            //        ・ダメージを受けられる相手か       IDamageable target = col.GetComponentInParent<IDamageable>();
            //                                           if (target == null || !target.IsAlive) continue;
            //        ・ボスの部位（頭・しっぽなど）なら、持ち主のボスで「当てたか」を数える
            //             IDamageable boss = target;
            //             if (target is IDamagePart part) boss = part.Owner;
            //             if (alreadyHit.Contains(boss)) continue;
            //             alreadyHit.Add(boss);
            //        ・ダメージを与える
            //             Vector3 dir = col.bounds.center - transform.position;  dir.y = 0f;
            //             target.ApplyDamage(new DamageInfo(burstDamage * AttackMultiplier, col.bounds.center, dir.normalized, gameObject));
        }

        // ================================================================
        //  回避・被弾で攻撃が中断されたとき
        // ================================================================

        protected override void OnActionInterrupted()
        {
            // 今は何もしなくてOK。
            // 余裕があれば：溜め中にオーラ（Human_SpellAura_Fire など）を出したら、ここで Destroy して消す
        }
    }
}
