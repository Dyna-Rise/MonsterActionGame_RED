using TMPro;
using UnityEngine;

namespace MonsterAction.Battle.UI
{
    /// <summary>
    /// TextMeshProUGUI に縁取りを付ける（旧 uGUI の Outline コンポーネントの代わり）。
    /// uGUI の Outline / Shadow は TextMeshPro には効かないので、TMP 自身のアウトライン機能を使う。
    /// 実行時にマテリアルのインスタンスを作って太さ・色を設定する。
    /// </summary>
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class TMPOutline : MonoBehaviour
    {
        [SerializeField] private Color color = new Color(0f, 0f, 0f, 0.8f);
        [Tooltip("縁取りの太さ（0〜1）")]
        [Range(0f, 1f)]
        [SerializeField] private float width = 0.15f;

        private void Start()
        {
            var text = GetComponent<TextMeshProUGUI>();
            text.outlineWidth = width;
            text.outlineColor = color;
        }
    }
}
