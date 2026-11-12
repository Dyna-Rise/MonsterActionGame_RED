using System.Collections.Generic;
using MonsterAction.Data;
using UnityEngine;

namespace MonsterAction.Home.UI
{
    
    public class ShopMenu : HomeMenuBase
    {
        private static readonly Color PriceColor = new Color(1f, 0.85f, 0.3f);
        private static readonly Color DisabledColor = new Color(0.6f, 0.6f, 0.6f);

        // 行の中身（アイテムかスキルのどちらか）
        private struct Entry
        {
            public ItemDefinition item;
            public SkillType skill;
            public bool IsItem => item != null; // item が入っていればアイテムの行、空ならスキルの行
        }

        // rows[0] の中身は entries[0]、rows[1] の中身は entries[1] … と、同じ番号で対応している
        private readonly List<Entry> entries = new List<Entry>();

        // ================================================================
        //  一覧を作る（メニューを開いたときに1回呼ばれる）
        // ================================================================

        protected override void BuildRows()
        {
            entries.Clear();

            CreateHeader("アイテム");
            foreach (var item in ItemDefinition.All)
            {
                if (item == null) continue;
                CreateRow();                             // 行を1つ作る
                entries.Add(new Entry { item = item });  // その行の中身を覚えておく
            }

            // TODO ②：スキルも並べる
            //   ・CreateHeader でスキルの見出しを作る
            //   ・SkillData.All を foreach して、CreateRow() と entries.Add(new Entry { skill = skill })
            //   （上のアイテムとほとんど同じ形です）
        }

        // ================================================================
        //  表示を最新にする（開いたとき・買ったあとに呼ばれる）
        // ================================================================

        protected override void Refresh()
        {
            if (moneyText != null) moneyText.text = "所持金  " + FormatMoney(GameData.Money);

            for (int i = 0; i < rows.Count && i < entries.Count; i++)
            {
                var e = entries[i];
                if (e.IsItem)
                {
                    // アイテムの行：「名前」「所持 3 / 10」「100 G」
                    int count = GameData.GetCount(e.item);
                    bool full = count >= e.item.MaxCount;
                    rows[i].Set(e.item.DisplayName,
                                $"所持 {count} / {e.item.MaxCount}",
                                full ? "所持上限" : FormatMoney(e.item.Price),
                                full ? DisabledColor : PriceColor);
                }
                else
                {
                    // TODO ③：スキルの行の表示
                    //   ・int level = GameData.GetSkillLevel(e.skill);  で今の★の数
                    //   ・名前    … SkillData.GetName(e.skill)
                    //   ・情報    … SkillData.GetStars(level)  （★★☆ のような文字）
                    //   ・右の文字 … 売り切れ（GameData.IsSkillMaxed）なら "売切れ"、
                    //                そうでなければ FormatMoney(GameData.GetNextSkillPrice(e.skill))
                    //   ・右の色  … 売り切れなら DisabledColor、そうでなければ PriceColor
                    //   rows[i].Set(名前, 情報, 右の文字, 右の色); で表示します（上のアイテムが参考になります）
                }
            }
        }

        // ================================================================
        //  行にカーソルが乗った → 説明を出す
        // ================================================================

        protected override void OnRowSelected(int index)
        {
            if (index < 0 || index >= entries.Count) return;
            var e = entries[index];

            if (e.IsItem)
            {
                SetDescription(e.item.Description);
            }
            else
            {
                // TODO ④：スキルの説明を出す
                //   SkillData.GetDescription(e.skill) を SetDescription に渡す
                //   （「装備画面（ドア）で付けると効果が出ます。」と付け足すと親切）
            }
        }

        // ================================================================
        //  行が決定された → 買う
        // ================================================================

        protected override void OnRowClicked(int index)
        {
            if (index < 0 || index >= entries.Count) return;
            var e = entries[index];

            if (e.IsItem)
            {
                // TODO ①：アイテムを買う
                //   1. もう最大数（e.item.MaxCount）まで持っていたら … ShowMessage("これ以上持てません")
                //   2. GameData.TryBuy(e.item) が true なら         … ShowMessage($"{e.item.DisplayName} を買った！")
                //   3. false なら（お金が足りない）                 … ShowMessage("お金が足りません")
                //   ヒント：if / else if / else を使います
            }
            else
            {
                // TODO ⑤：スキルを買う（アイテムとほぼ同じ形）
                //   1. 売り切れ（GameData.IsSkillMaxed）なら       … "売り切れです"
                //   2. GameData.TryBuySkill(e.skill) が true なら … "〇〇 が ★2 になった！" のように出す
                //   3. false なら                                 … "お金が足りません"
            }

            Refresh(); // 買ったあと、所持金や所持数の表示を更新する
        }
    }
}
