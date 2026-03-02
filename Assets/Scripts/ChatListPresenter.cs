using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ChatListPresenter : MonoBehaviour
{
    public ChatScript chatScript;

    public Transform content;
    public GameObject msgRowPrefab; // MsgRow.prefab

    public ScrollRect scrollRect; // ✅ 추가

    void Start()
    {
        
    }

    public void Render()
    {
        Debug.Log("[Chat] Render() called");
        Debug.Log("[Chat] content=" + content + " chatScript=" + chatScript + " prefab=" + msgRowPrefab);

        if (content == null || chatScript == null || msgRowPrefab == null) return;

        Debug.Log("[Chat] lines count=" + chatScript.lines.Count);

        // clear
        for (int i = content.childCount - 1; i >= 0; i--)
            Destroy(content.GetChild(i).gameObject);

        // create
        for (int i = 0; i < chatScript.lines.Count; i++)
        {
            var line = chatScript.lines[i];
            GameObject go = Instantiate(msgRowPrefab, content);

            var row = go.GetComponent<MsgRowPresenter>();
            if (row != null)
            {
                bool isRight = (line.side == ChatSide.Right);
                row.Set(line.text, isRight);
            }
        }

        // ✅ 추가: 렌더 끝나고 맨 아래로 이동
        if (scrollRect != null)
            StartCoroutine(ScrollToBottomNextFrame());
    }

    IEnumerator ScrollToBottomNextFrame()
    {
        // ✅ 다음 프레임까지 기다려서 LayoutGroup/SizeFitter 계산이 끝나게 함
        yield return null;

        // ✅ 레이아웃 강제 갱신(안정성)
        Canvas.ForceUpdateCanvases();

        // ✅ 1=위, 0=아래
        scrollRect.verticalNormalizedPosition = 0f;

        Canvas.ForceUpdateCanvases();
    }
}