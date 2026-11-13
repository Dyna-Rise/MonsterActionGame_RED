using UnityEngine;

namespace MonsterAction.Battle.Player
{

    public class PlayerTypeB_Bow : PlayerBattleController
    {
        // ================================================================
        //  Inspector で設定するもの・覚えておく変数
        // ================================================================

        //変数の例）
        // 矢の Prefab とダメージを Inspector で設定できるようにする
        //   ※ BowArrow.cs を作ってから、// を外してください（先に外すとエラーになります）
        [Header("弓")]
        [SerializeField] private BowArrow arrowPrefab;      // 矢の Prefab
        [SerializeField] private float damage = 10f;        // ふつうの矢のダメージ
        [SerializeField] private float chargedDamage = 25f; // 溜めた矢のダメージ（ステップ7）
        [SerializeField] private float chargeTime = 1f;     // この秒数以上 押していたら「溜めた矢」（ステップ7）

        // 今の状態を覚えておく変数
        private bool isHolding;        // 弓を引いている途中か
        private float fireTime;        // 撃ったときの StateTime（撃ったあと何秒たったか調べるため）
        private bool holdAnimStarted;  // 「引いたまま」のモーションに切り替えたか

        // ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
        private float holdTime;

        protected override void Awake()
        {
            base.Awake();   // ← これが無いと Input などが null のまま（消さない）
        }

        // ================================================================
        //  攻撃を始める（普段の移動中に、毎フレーム呼ばれる）
        // ================================================================

        protected override bool TryStartAction()
        {
            // TODO：□ が押されたら、弓を引きはじめる
            //   if (Input.ConsumeAttack())
            //   {
            //       1. BeginAction();                                             … 攻撃中にする
            //       2. FaceDirection(GetAttackFacingDirection(), instant: true);  … 狙う方を向く
            //       3. CrossFade("BowLoad");                                      … 弓を引くモーション
            //       4. isHolding = true;  holdAnimStarted = false;                … 「引いている」と覚える
            //       5. return true;                                               … 攻撃を始めた
            //   }
            //   ヒント：ステップ1では、BeginAction() と Debug.Log("弓：□ が押された"); だけで試す

            if (Input.ConsumeAttack())
            {
                BeginAction();
                Debug.Log("弓：□ が押された");
                FaceDirection(GetAttackFacingDirection(), instant: true);
                CrossFade("BowLoad");
                isHolding = true;
                holdAnimStarted = false;
                return true;
            }
            return false; // 何もしなければ false（普段の移動のまま）
        }

        // ================================================================
        //  攻撃中（BeginAction のあと、毎フレーム呼ばれる）
        // ================================================================

        protected override void UpdateAction(float deltaTime)
        {
            // TODO：引く → 離して放つ → 少し待って戻る
            //   ★ 書きはじめたら、いちばん下の ReturnToLocomotion(); は消す
            //     （これがあると、すぐ普段の移動に戻ってしまう）
            //
            //   ステップ1：0.5 秒たったら戻る
            //     if (StateTime >= 0.5f) ReturnToLocomotion();
            //
            //   ステップ3 から：
            //   if (isHolding)   … 弓を引いている間
            //   {
            //       1. if (TryCancelIntoDodge()) return;          … 回避できる
            //       2. FaceDirection(GetAttackFacingDirection()); … 狙う方を向き続ける
            //       3. 0.4 秒たったら、1回だけ CrossFade("BowHold");（holdAnimStarted で1回だけにする）
            //       4. □ を離したら（!Input.AttackHeld）、0.3 秒以上引いていれば Fire();
            // ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
            //          ステップ7：StateTime >= chargeTime なら強い矢、そうでなければふつうの矢
            // ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
            //       5. return;
            //   }
            //   撃ったあと：StateTime - fireTime が 0.5 秒以上になったら ReturnToLocomotion();

            if (isHolding)
            {
                if (TryCancelIntoDodge()) return;
                FaceDirection(GetAttackFacingDirection());
                if (StateTime >= 0.4f && !holdAnimStarted)
                {
                    holdAnimStarted = true;
                    CrossFade("BowHold");
                }
                if (!Input.AttackHeld)
                {
                    if (StateTime >= 0.3) Fire();
                }
            }
            if (StateTime - fireTime >= 0.5f) ReturnToLocomotion();
        }

        // ================================================================
        //  矢を放つ（自分で作るメソッド）
        // ================================================================

        /// <summary>矢を放つ。ステップ7では Fire(float power, bool flinch) のように引数を足す</summary>
        private void Fire()
        {
            // TODO：
            //   1. isHolding = false;  fireTime = StateTime;   … もう引いていない・撃った時刻を覚える
            //   2. CrossFade("BowRelease");                    … 放つモーション
            //   3. ステップ3では Debug.Log("発射！"); だけで試す
            //
            // ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
            //   ステップ5 から：矢を出す
            //   4. 矢を出す位置（胸の高さの少し前）
            //        Vector3 from = transform.position + Vector3.up * 1.4f + transform.forward * 0.5f;
            //   5. 飛ばす向き：ふつうは正面 transform.forward
            //        ステップ6：IsFocusing なら GetAimDirection(from)
            //   6. 矢を作って飛ばす
            //        BowArrow arrow = Instantiate(arrowPrefab, from, Quaternion.identity);
            //        arrow.Shoot(向き, damage * AttackMultiplier, gameObject);
            //        （ステップ7：arrow.Shoot(向き, power * AttackMultiplier, gameObject, flinch);）

            isHolding = false;
            fireTime = StateTime;
            CrossFade("BowRelease");
            Debug.Log("発射！");
        }

        // ================================================================
        //  回避・被弾で攻撃が中断されたとき
        // ================================================================

        protected override void OnActionInterrupted()
        {
            // TODO：引いている途中で中断されたら、引くのをやめる
            //   isHolding = false;

            isHolding = false;
        }
    }
}
