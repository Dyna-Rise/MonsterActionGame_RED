using MonsterAction.Data;
using UnityEditor;
using UnityEngine;

namespace MonsterAction.EditorTools
{
    /// <summary>
    /// テスト用：Play 中に GameData（所持金・スキル）を直接いじるエディタウィンドウ。
    /// メニュー「Tools > GameData Debug」から開く。
    /// Editor フォルダなのでビルドには入らず、ゲーム側のスクリプトやシーンにも一切手を加えない。
    ///
    /// 例：ショップが未実装でも、スキルを★1〜3にして装備画面の動きを確認できる。
    /// ※ 装備画面を開いたままだと表示が更新されないので、一度閉じて開き直す。
    /// </summary>
    public class GameDataDebugWindow : EditorWindow
    {
        private int addAmount = 1000;

        [MenuItem("Tools/GameData Debug")]
        private static void Open() => GetWindow<GameDataDebugWindow>("GameData Debug");

        private void OnInspectorUpdate() => Repaint();

        private void OnGUI()
        {
            if (!Application.isPlaying)
            {
                EditorGUILayout.HelpBox("Play 中のみ使えます。", MessageType.Info);
                return;
            }

            EditorGUILayout.LabelField("所持金", $"{GameData.Money} G", EditorStyles.boldLabel);
            using (new EditorGUILayout.HorizontalScope())
            {
                addAmount = EditorGUILayout.IntField(addAmount);
                if (GUILayout.Button("お金を追加", GUILayout.Width(100))) GameData.AddMoney(addAmount);
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField($"スキル（装備 {GameData.EquippedSkillCount} / {GameData.MaxEquippedSkills}）", EditorStyles.boldLabel);
            foreach (var type in SkillData.All)
            {
                int level = GameData.GetSkillLevel(type);
                using (new EditorGUILayout.HorizontalScope())
                {
                    string equipped = GameData.IsSkillEquipped(type) ? "  [装備中]" : "";
                    EditorGUILayout.LabelField($"{SkillData.GetName(type)}  {SkillData.GetStars(level)}{equipped}");

                    using (new EditorGUI.DisabledScope(GameData.IsSkillMaxed(type)))
                    {
                        // 本番と同じ TryBuySkill を通す（お金が足りなければ失敗する）
                        if (GUILayout.Button("購入", GUILayout.Width(60)) && !GameData.TryBuySkill(type))
                            Debug.Log($"[GameData Debug] {SkillData.GetName(type)} を購入できません（所持金 {GameData.Money} / 必要 {GameData.GetNextSkillPrice(type)}）");

                        // 値段分のお金を足してから買う（お金を気にせず★を上げたいとき）
                        if (GUILayout.Button("無料で★+1", GUILayout.Width(80)))
                        {
                            GameData.AddMoney(GameData.GetNextSkillPrice(type));
                            GameData.TryBuySkill(type);
                        }
                    }
                }
            }

            EditorGUILayout.Space();
            if (GUILayout.Button("初期状態に戻す")) GameData.ResetToDefault();
        }
    }
}
