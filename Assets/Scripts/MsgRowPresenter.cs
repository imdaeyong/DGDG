using UnityEngine;
using TMPro;

public class MsgRowPresenter : MonoBehaviour
{
    public RectTransform bubble;
    public RectTransform spacer;
    public TextMeshProUGUI text;

    public void Set(string message, bool isRight)
    {
        text.text = message;

        if (isRight)
        {
            spacer.SetAsFirstSibling();
            bubble.SetAsLastSibling();
            text.alignment = TextAlignmentOptions.TopRight;

            // 坷弗率 富浅急 ℃ 哭率 部府 见扁扁
            bubble.anchoredPosition = new Vector2(-30f, 0f);
        }
        else
        {
            bubble.SetAsFirstSibling();
            spacer.SetAsLastSibling();
            text.alignment = TextAlignmentOptions.TopLeft;

            // 哭率 富浅急 ℃ 坷弗率 部府 见扁扁
            bubble.anchoredPosition = new Vector2(30f, 0f);
        }
    }
}