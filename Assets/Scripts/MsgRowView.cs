using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MsgRowView : MonoBehaviour
{
    public LayoutElement leftSpacer;
    public LayoutElement rightSpacer;
    public Image bubble;
    public TextMeshProUGUI bodyText;

    [Header("Layout")]
    public float maxBubbleWidth = 650f; // 폰 폭에 맞게 조절

    public void Bind(string message, bool isRight)
    {
        bodyText.text = message;

        if (isRight)
        {
            leftSpacer.flexibleWidth = 1;
            rightSpacer.flexibleWidth = 0;
            bodyText.alignment = TextAlignmentOptions.MidlineRight;
        }
        else
        {
            leftSpacer.flexibleWidth = 0;
            rightSpacer.flexibleWidth = 1;
            bodyText.alignment = TextAlignmentOptions.MidlineLeft;
        }
    }
}