using System.Collections;
using UnityEngine;

namespace MonsterAction.Battle.Boss
{    
    public class DragonNightmareBoss : BossBase
    {
        [Header("距離")]
        [SerializeField] private float meleeRange = 7f;
        [SerializeField] private float farRange = 18f;

        [Header("移動")]
        [SerializeField] private float walkSpeed = 3f;

        [Header("攻撃判定（ボーンの子に置いた BossAttackHitbox）")]
        [SerializeField] private BossAttackHitbox biteHitbox;     // 口（ブレスの発射位置にも使う）
        [SerializeField] private BossAttackHitbox[] clawHitboxes; // 両翼の爪
        [SerializeField] private BossAttackHitbox bodyHitbox;     // 胴（突進用）

        [Header("小攻撃")]
        [SerializeField] private float biteDamage = 20f;
        [SerializeField] private float clawDamage = 15f;

        [Header("突進★")]
        [SerializeField] private float chargeDamage = 40f;
        [SerializeField] private float chargeSpeed = 13f;
        [Tooltip("突進中のホーミングの強さ（旋回速度の倍率）。小さいほど避けやすい")]
        [SerializeField] private float chargeHoming = 0.35f;
        [Tooltip("1回の突進の最大時間（秒）")]
        [SerializeField] private float chargeMaxTime = 2.5f;
        [Tooltip("突進の回数（ドリフトをはさんで最大何回走るか）")]
        [SerializeField] private int maxCharges = 3;
        [Tooltip("ドリフト（滑りながら向き直る）の時間")]
        [SerializeField] private float driftTime = 0.7f;
        [Tooltip("ドリフト中の旋回速度の倍率")]
        [SerializeField] private float driftTurn = 4f;

        [Header("ブレス★（正面へ扇状に、地面すれすれをまっすぐ飛ぶ）")]
        [SerializeField] private BossProjectile breathPrefab; // 空なら球を自動で作る
        [SerializeField] private float breathDamage = 35f;
        [SerializeField] private float breathSpeed = 20f;
        [SerializeField] private float breathRadius = 1.0f;
        [Tooltip("一度に出す火球の数")]
        [SerializeField] private int breathCount = 3;
        [Tooltip("火球どうしの角度（度）")]
        [SerializeField] private float breathSpreadAngle = 12f;
        [Tooltip("飛ぶ高さ（m）。プレイヤーの胸あたり")]
        [SerializeField] private float breathHeight = 1.2f;
        [Tooltip("ボスの中心からどれだけ前で出すか（m）")]
        [SerializeField] private float breathStartForward = 4f;
        [SerializeField] private Color breathColor = new Color(1f, 0.45f, 0.1f);

        [Header("投擲★（正面へ扇状に、低い弾道で決まった距離まで飛ぶ）")]
        [SerializeField] private BossProjectile rockPrefab; // 空なら球を自動で作る
        [SerializeField] private float rockDamage = 30f;
        [SerializeField] private float rockRadius = 0.8f;
        [Tooltip("一度に投げる岩の数")]
        [SerializeField] private int rockCount = 5;
        [Tooltip("岩どうしの角度（度）")]
        [SerializeField] private float rockSpreadAngle = 12f;
        [Tooltip("岩が地面に落ちるまでの距離（m）")]
        [SerializeField] private float rockDistance = 16f;
        [Tooltip("岩が落ちるまでの時間（秒）")]
        [SerializeField] private float rockFlightTime = 1.0f;
        [Tooltip("岩の重力。小さいほど低く平らに飛ぶ（プレイヤーの高さを通り続ける）")]
        [SerializeField] private float rockGravity = -6f;
        [SerializeField] private float rockStartHeight = 1.5f;
        [SerializeField] private float rockStartForward = 3f;
        [SerializeField] private Color rockColor = new Color(0.45f, 0.4f, 0.35f);

        [Header("咆哮")]
        [SerializeField] private float roarRadius = 12f;

        [Header("怒り")]
        [Tooltip("怒り状態のモーション速度")]
        [SerializeField] private float enragedAnimSpeed = 1.2f;
        [Tooltip("怒り状態の休み時間の倍率")]
        [SerializeField] private float enragedRestRate = 0.6f;

        // Animator のステート名（DragonNightmare 用の Animator Controller と同じ名前）
        private const string Walk = "Walk";
        private const string Run = "Run";
        private const string BasicAttack = "Basic Attack";
        private const string ClawAttack = "Claw Attack";
        private const string HornAttack = "Horn Attack";
        private const string Scream = "Scream";

        private bool hasRoared;
        private bool enraged;

        private float RestTime(float seconds) => enraged ? seconds * enragedRestRate : seconds;

        // ================================================================
        //  どの行動をするか決める
        // ================================================================

        /// <summary>次の行動を決めて返す（行動が終わるたびに呼ばれる）</summary>
        protected override IEnumerator ThinkNextAction()
        {
            // TODO：次にやる行動を return で返す
            //   ★ 書きはじめたら、下の「yield return null;」は必ず消す
            //     （yield return があるメソッドでは return Roar(); と書けない＝エラーになる）
            //
            //   まずはこれだけ（ずっと吠え続ける）：
            //     return Roar();
            //
            //   できるようになったら：
            //   1. 最初に1回だけ吠える
            //        if (!hasRoared) { hasRoared = true; return Roar(); }
            //   2. HP が半分を切ったら怒る（1回だけ）
            //        if (!enraged && HpRate <= 0.5f) { enraged = true; Anim.speed = enragedAnimSpeed; return Roar(); }
            //   3. 距離とサイコロで選ぶ
            //        float distance = DistanceToPlayer;
            //        int r = Random.Range(0, 100);          // 0〜99。数字の幅が出やすさ
            //        if (distance <= meleeRange) { if (r < 40) return Bite(); ... }
            //        if (distance <= farRange)   { if (r < 55) return ChargeCombo(); ... }
            //        （遠いとき）...
            //   4. 最後は必ず何かを return する（どの if にも入らなかったとき用）
            yield return null; //エラー回避用
        }

        // ================================================================
        //  咆哮
        // ================================================================

        /// <summary>咆哮：プレイヤーの方を向いて吠える。近くにいるとのけぞらせる（ダメージなし）</summary>
        private IEnumerator Roar()
        {
            // TODO：
            //   1. プレイヤーの方を向く          yield return TurnToPlayer(1f, Walk);
            //   2. 吠えるモーション              Play(Scream);
            //   3. モーションの 25% まで待つ      yield return WaitStateTime(Scream, 0.25f);
            //   4. 周りをのけぞらせる            AreaAttack(transform.position, roarRadius, 0f, knockback: 1.5f);
            //                                    （3つ目の 0f がダメージ。0 なのでのけぞるだけ）
            //   5. モーションの最後まで待つ      yield return WaitStateTime(Scream, 0.95f);
            //   6. 少し休む                      yield return Rest(RestTime(0.5f));
            yield return null; //エラー回避用
        }

        // ================================================================
        //  小攻撃
        // ================================================================

        /// <summary>噛みつき（小）：軸合わせしながら踏み込んで噛む</summary>
        private IEnumerator Bite()
        {
            // TODO：攻撃は「向く → 振りかぶり → 判定 ON → 判定 OFF → 休む」の順
            //   1. プレイヤーの方を向く（0.5 秒）   yield return TurnToPlayer(0.5f, Walk);
            //   2. 噛みつきモーション              Play(BasicAttack);
            //   3. 振りかぶり：モーションが 30% になるまで、向きを合わせながら少し前へ
            //        while (GetStateTime(BasicAttack) < 0.3f)
            //        {
            //            FaceTowards(DirectionToPlayer, 1.5f);
            //            Move(transform.forward * 2.5f);
            //            yield return null;
            //        }
            //   4. 口の攻撃判定 ON                 biteHitbox.Begin(biteDamage, knockback: 1f);
            //   5. 55% まで待って OFF              WaitStateTime(BasicAttack, 0.55f) → biteHitbox.End();
            //   6. 最後まで待って休む              WaitStateTime(BasicAttack, 0.95f) → Rest(RestTime(1.0f))
            yield return null; //エラー回避用
        }

        /// <summary>ひっかき（小）：軸合わせしてから翼の爪で軽く払う</summary>
        private IEnumerator Scratch()
        {
            // TODO：Bite とほぼ同じ形。違うところ：
            //   ・モーションは ClawAttack
            //   ・振りかぶりは WaitStateTimeTracking(ClawAttack, 0.3f, 1.5f) で「向き直りながら待つ」だけ（前には進まない）
            //   ・爪は左右に2つあるので、foreach で両方 ON / OFF する
            //        foreach (var h in clawHitboxes) h.Begin(clawDamage, knockback: 0.8f);
            //        ...
            //        foreach (var h in clawHitboxes) h.End();
            //   ・判定は 30% 〜 55%、最後に Rest(RestTime(0.9f))
            yield return null; //エラー回避用
        }

        // ================================================================
        //  遠距離攻撃（大）
        // ================================================================

        /// <summary>
        /// ブレス★：軸合わせしてから、正面へ扇状に火球を撃つ。
        /// 狙いはプレイヤーの位置ではなく「ボスの正面」。地面すれすれをまっすぐ飛ぶので、正面にいればほぼ当たる
        /// </summary>
        private IEnumerator Breath()
        {
            // TODO：（先に FanDirections を作っておく）
            //   1. 向く（0.8 秒）→ Play(Scream) → WaitStateTimeTracking(Scream, 0.35f, 1.2f)（予兆の間、軸合わせ）
            //   2. 火球を出す位置：口の前あたり
            //        Vector3 start = transform.position + transform.forward * breathStartForward + Vector3.up * breathHeight;
            //   3. FanDirections(breathCount, breathSpreadAngle) の向きごとに、火球を作って飛ばす
            //        foreach (Vector3 dir in FanDirections(breathCount, breathSpreadAngle))
            //        {
            //            var ball = BossProjectile.Spawn(breathPrefab, start, breathRadius, breathColor);
            //            ball.Launch(dir * breathSpeed, MakeDamage(breathDamage, knockback: 4f, knockDown: true), transform);
            //        }
            //   4. WaitStateTime(Scream, 0.95f) → Rest(RestTime(1.4f))
            yield return null; //エラー回避用
        }

        /// <summary>
        /// 投擲★：軸合わせしてから、正面へ扇状に岩を投げる。
        /// いつも同じ角度・同じ距離（rockDistance）に落ちる。低い弾道なので、途中にいても当たる
        /// </summary>
        private IEnumerator Throw()
        {
            // TODO：Breath とほぼ同じ形。違うところ：
            //   ・モーションは HornAttack、予兆は 40%（WaitStateTimeTracking(HornAttack, 0.4f, 1.2f)）
            //   ・出す位置は rockStartForward / rockStartHeight、数と角度は rockCount / rockSpreadAngle
            //   ・まっすぐではなく、放物線で「決まった場所」に落とす
            //        Vector3 landing = transform.position + dir * rockDistance;   // 落ちる場所
            //        landing.y = transform.position.y;
            //        var rock = BossProjectile.Spawn(rockPrefab, start, rockRadius, rockColor);
            //        rock.Launch(BossProjectile.BallisticVelocity(start, landing, rockFlightTime, rockGravity),
            //                    MakeDamage(rockDamage, knockback: 3f, knockDown: true), transform, rockGravity);
            //   ・最後は WaitStateTime(HornAttack, 0.95f) → Rest(RestTime(1.4f))
            yield return null; //エラー回避用
        }

        /// <summary>正面を中心に、count 本の方向を angle 度ずつ左右に広げて返す（水平）</summary>
        private System.Collections.Generic.IEnumerable<Vector3> FanDirections(int count, float angle)
        {
            // TODO：例）count = 3, angle = 12 なら「左 12°・正面・右 12°」の3つの向きを返す
            //   1. 正面の向き（水平）     Vector3 forward = transform.forward; forward.y = 0f; forward.Normalize();
            //   2. for (int i = 0; i < count; i++) で1本ずつ
            //        float offset = (i - (count - 1) * 0.5f) * angle;   // 3本なら -12, 0, +12
            //        yield return Quaternion.AngleAxis(offset, Vector3.up) * forward;   // 正面を offset 度だけ回した向き
            //   ★ 書きはじめたら、下の「yield break;」は消してOK
            yield break; // 何も返さない（エラー回避用）
        }

        // ================================================================
        //  突進（大）
        // ================================================================

        /// <summary>
        /// 突進コンボ★：ゆるくホーミングして走り抜ける → ドリフトで向き直る → また突進 …（最大 maxCharges 回）
        /// </summary>
        private IEnumerator ChargeCombo()
        {
            // TODO：突進全体の流れ（実際に走るのは ChargeRun、止まるのは Brake）
            //   1. 向く（0.8 秒）
            //   2. 予兆：短く吠える        Play(Scream, 0.1f); → WaitStateTimeTracking(Scream, 0.25f, 1.5f)
            //   3. 走る                    yield return ChargeRun();   （コルーチンの中から別のコルーチンを呼んで、終わるまで待てる）
            //      ★ 慣れたら：回数を決めて for でくり返し、突進と突進の間に Drift() をはさむ
            //        int charges = Random.Range(1, maxCharges + 1);
            //        for (int i = 0; i < charges; i++) { yield return ChargeRun(); if (i < charges - 1) yield return Drift(); }
            //   4. 止まる                  yield return Brake(0.6f);
            //   5. 休む                    yield return Rest(RestTime(1.6f));
            yield return null; //エラー回避用
        }

        /// <summary>1回分の突進：ゆるく追いかけながら走り、プレイヤーを通り過ぎたら終わる</summary>
        private IEnumerator ChargeRun()
        {
            // TODO：
            //   1. Play(Run, 0.1f);
            //   2. 体の攻撃判定 ON（大攻撃）   bodyHitbox.Begin(chargeDamage, knockback: 6f, knockDown: true);
            //   3. chargeMaxTime 秒まで、毎フレーム
            //        ・FaceTowards(DirectionToPlayer, chargeHoming);   // ゆるく追いかける（小さいほど曲がりにくい）
            //        ・Move(transform.forward * chargeSpeed);
            //        ・プレイヤーが後ろ側（Mathf.Abs(AngleToPlayer) > 100f）にいる時間を数えて、0.35 秒を超えたら break
            //        ・time += Time.deltaTime;  yield return null;
            //   4. 判定 OFF                    bodyHitbox.End();
            yield return null; //エラー回避用
        }

        /// <summary>ドリフト：勢いで滑りながら、すばやくプレイヤーの方へ向き直る（この間も当たる）</summary>
        private IEnumerator Drift()
        {
            // TODO：
            //   1. 今の勢いを覚える            Vector3 slide = transform.forward * chargeSpeed;
            //   2. プレイヤーがいる側へ滑るモーション
            //        Play(AngleToPlayer > 0f ? "Walk Right" : "Walk Left", 0.1f);
            //   3. 判定 ON（突進より少し弱く） bodyHitbox.Begin(chargeDamage * 0.7f, knockback: 4f, knockDown: true);
            //   4. driftTime 秒の間、毎フレーム
            //        float rate = 1f - time / driftTime;     // 1 → 0 にだんだん減る
            //        Move(slide * rate);                      // 勢いがだんだん落ちる
            //        FaceTowards(DirectionToPlayer, driftTurn);  // すばやく向き直る
            //   5. 判定 OFF
            yield return null; //エラー回避用
        }

        /// <summary>減速して止まる</summary>
        private IEnumerator Brake(float seconds)
        {
            // TODO：
            //   1. Play(Walk, 0.2f);
            //   2. seconds 秒の間、毎フレーム少しずつ遅く進む
            //        Move(transform.forward * chargeSpeed * (1f - time / seconds) * 0.5f);
            //        time += Time.deltaTime;  yield return null;
            yield return null; //エラー回避用
        }

        // ================================================================
        //  移動
        // ================================================================

        /// <summary>歩いて近づく</summary>
        private IEnumerator Approach(float maxSeconds)
        {
            // TODO：
            //   1. Play(Walk, 0.25f);
            //   2. 終わる時刻を決める          float end = Time.time + maxSeconds;
            //   3. 時間切れになるか、十分近づく（DistanceToPlayer > meleeRange * 0.8f でなくなる）まで、毎フレーム
            //        FaceTowards(DirectionToPlayer);          // プレイヤーの方へ向く
            //        Move(transform.forward * walkSpeed);     // 前に進む
            //        yield return null;                       // 次のフレームまで待つ
            yield return null; //エラー回避用
        }

#if UNITY_EDITOR
        /// <summary>セットアップツール用（消さないこと）</summary>
        public void EditorSetup(BossAttackHitbox bite, BossAttackHitbox[] claws, BossAttackHitbox body)
        {
            biteHitbox = bite;
            clawHitboxes = claws;
            bodyHitbox = body;
        }
#endif
    }
}
