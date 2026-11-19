//指示外に🔥→消去で復元可
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

        protected override void Awake()
        {
            base.Awake();   // ← これが無いと Input などが null のまま（消さない）
        }

        //━━━━━━━━━━━━━━━━━━━
        //🔥追加変数
        [SerializeField] private BowArrow superArrowPrefab; //強い矢→オブジェクト変更
        [SerializeField] private GameObject effectObj; //強い矢チャージ中のエフェクト追加
        private GameObject currentEffect; //エフェクトの実体収納(Destroy用)
        private bool effectStarted; //エフェクト表示フラグ


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
                effectStarted = false; //🔥フラグリセット
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
                if (TryCancelIntoDodge())
                {
                    DestroyEffect(); //🔥回避時エフェクト消去
                    return; //回避キャンセル
                }

                FaceDirection(GetAttackFacingDirection()); //ターゲットに向き続ける
                if (StateTime >= 0.4f && !holdAnimStarted) //0.4秒経過かつholdAnimStartedじゃなければ
                {
                    holdAnimStarted = true; //フラグリセット
                    CrossFade("BowHold"); //Holdアニメ再生
                }

                if (StateTime >= chargeTime && !effectStarted) //🔥chargeTime以上なら1回だけエフェクト開始
                {
                    effectStarted = true;
                    //第4引数のtransform→Playerが移動しても追尾
                    currentEffect = Instantiate(effectObj, transform.position + Vector3.up * 0.1f, Quaternion.identity, transform);
                }

            if (!Input.AttackHeld) //□ボタン離したら
            {
                if (StateTime >= chargeTime)
                {
                    Fire(chargedDamage, true, superArrowPrefab); //chargeTime以上なら強い矢！(🔥第3引数追加)
                }
                else if (StateTime >= 0.3)
                {
                    Fire(damage, false, arrowPrefab); //🔥第3引数追加
                }
                else ReturnToLocomotion(); //0.3秒未満なら通常へ戻る
            }
            return; //Hold中に▼ifなるの防止
        }
            if (StateTime - fireTime >= 0.5f) ReturnToLocomotion(); //撃った後0.5秒以上経過で通常に戻る
        }

        void DestroyEffect() //🔥エフェクト消去メソッド
        {
            if(currentEffect!=null)Destroy(currentEffect);
        }

        // ================================================================
        //  矢を放つ（自分で作るメソッド）
        // ================================================================

        /// <summary>矢を放つ。ステップ7では Fire(float power, bool flinch) のように引数を足す</summary>
        private void Fire(float power, bool flinch, BowArrow prefab) //🔥第3引数追加
        {
            // TODO：
            //   1. isHolding = false;  fireTime = StateTime;   … もう引いていない・撃った時刻を覚える
            //   2. CrossFade("BowRelease");                    … 放つモーション
            //   3. ステップ3では Debug.Log("発射！"); だけで試す
            //
            //   ステップ5 から：矢を出す
            //   4. 矢を出す位置（胸の高さの少し前）
            //        Vector3 from = transform.position + Vector3.up * 1.4f + transform.forward * 0.5f;
            //   5. 飛ばす向き：ふつうは正面 transform.forward
            //        ステップ6：IsFocusing なら GetAimDirection(from)
            //   6. 矢を作って飛ばす
            //        BowArrow arrow = Instantiate(arrowPrefab, from, Quaternion.identity);
            //        arrow.Shoot(向き, damage * AttackMultiplier, gameObject);
            // ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
            //        （ステップ7：arrow.Shoot(向き, power * AttackMultiplier, gameObject, flinch);）

            //矢の生成位置決定
            Vector3 from = transform.position + Vector3.up * 1.4f + transform.forward * 0.5f;
            //決めた位置に矢を生成
            BowArrow arrow = Instantiate(prefab, from, Quaternion.identity);

            //飛んで行く向きの条件付け(プレイヤー自身を回転させるのではなく矢の向きを変える)
            //transform.forward:正面(オブジェクト(ここではプレイヤー)のZ軸(青矢印)の向き)
            //▼2行のifでの言い換え
            //if (IsFocusing) { arrow.Shoot(GetAimDirection(from), damage * AttackMultiplier, gameObject, flinch); }
            //else { arrow.Shoot(transform.forward, damage * AttackMultiplier, gameObject, flinch); }
            Vector3 dir = IsFocusing ? GetAimDirection(from) : transform.forward;

            //矢飛ばす:BowArrow.csのShootメソッド発動
            //引数に(方向,ダメージ量(*攻撃力ｱｯﾌﾟｽｷﾙの倍率),発射元(自分自身),ひるみ有無)の情報を入れて渡す
            arrow.Shoot(dir, power * AttackMultiplier, gameObject, flinch);

            DestroyEffect(); //🔥エフェクト消去

            isHolding = false; //フラグリセット
            //撃った後の時間計測用(isHolding後にReturnToLocomotionするため)
            fireTime = StateTime;
            CrossFade("BowRelease"); //Releaseアニメ再生
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
            DestroyEffect(); //🔥被弾時もエフェクト消去
        }
    }
}
