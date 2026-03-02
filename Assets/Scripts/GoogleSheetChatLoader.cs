using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class GoogleSheetChatLoader : MonoBehaviour
{
    [Header("Google Sheet CSV URL")]
    public string csvUrl;

    [Header("Target ChatScript (ScriptableObject)")]
    public ChatScript chatScript;

    [Header("UI Presenter (optional, but recommended)")]
    public ChatListPresenter chatListPresenter;

    [Header("Options")]
    public bool clearExistingBeforeApply = true;
    public bool refreshOnStart = true;
    public bool scrollToBottomAfterRender = true;

    void Start()
    {
        if (refreshOnStart)
        {
            StartCoroutine(LoadAndApply());
        }
    }

    [ContextMenu("Reload CSV Now")]
    public void ReloadNow()
    {
        StartCoroutine(LoadAndApply());
    }

    IEnumerator LoadAndApply()
    {
        if (string.IsNullOrEmpty(csvUrl))
        {
            Debug.LogError("[GoogleSheetChatLoader] csvUrl is empty.");
            yield break;
        }
        if (chatScript == null)
        {
            Debug.LogError("[GoogleSheetChatLoader] chatScript is null.");
            yield break;
        }

        Debug.Log("[GoogleSheetChatLoader] CSV load start: " + csvUrl);

        UnityWebRequest req = UnityWebRequest.Get(csvUrl);
        yield return req.SendWebRequest();

        if (req.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("[GoogleSheetChatLoader] CSV download failed: " + req.error);
            yield break;
        }

        string csv = req.downloadHandler.text;
        if (string.IsNullOrEmpty(csv))
        {
            Debug.LogError("[GoogleSheetChatLoader] CSV content is empty.");
            yield break;
        }

        List<ChatLine> parsedLines = ParseCsvToChatLines(csv);
        Debug.Log("[GoogleSheetChatLoader] CSV parsed lines: " + parsedLines.Count);

        // parsedLines가 0이면 덮어쓰기하지 않게(원래 데이터 유지하고 싶으면) 여기서 return 가능
        // 지금은 "구글이 진짜로 적용됐는지" 확인 위해 0이어도 clear 옵션에 따라 반영함.

        ApplyToChatScript(parsedLines);

        // UI 갱신
        if (chatListPresenter != null)
        {
            chatListPresenter.chatScript = chatScript;
            chatListPresenter.Render();

            if (scrollToBottomAfterRender && chatListPresenter.scrollRect != null)
            {
                // 한 프레임 기다렸다가 (레이아웃 계산 후) 맨 아래로
                yield return null;
                Canvas.ForceUpdateCanvases();
                chatListPresenter.scrollRect.verticalNormalizedPosition = 0f;
            }
        }
        else
        {
            Debug.LogWarning("[GoogleSheetChatLoader] chatListPresenter is null. Data applied but UI not refreshed.");
        }
    }

    void ApplyToChatScript(List<ChatLine> newLines)
    {
        if (clearExistingBeforeApply)
        {
            chatScript.lines.Clear();
        }

        for (int i = 0; i < newLines.Count; i++)
        {
            chatScript.lines.Add(newLines[i]);
        }
    }

    List<ChatLine> ParseCsvToChatLines(string csv)
    {
        List<ChatLine> result = new List<ChatLine>();

        string[] rows = csv.Split('\n');
        if (rows.Length <= 1) return result;

        // 헤더 파싱
        List<string> header = ParseCsvLine(rows[0].TrimEnd('\r'));

        int sideIndex = header.IndexOf("side");
        int textIndex = header.IndexOf("text");

        // 헤더가 없으면 0,1로 가정
        if (sideIndex < 0) sideIndex = 0;
        if (textIndex < 0) textIndex = 1;

        for (int i = 1; i < rows.Length; i++)
        {
            string line = rows[i].TrimEnd('\r').Trim();
            if (string.IsNullOrEmpty(line)) continue;

            List<string> cols = ParseCsvLine(line);
            if (cols.Count <= Mathf.Max(sideIndex, textIndex)) continue;

            string side = cols[sideIndex].Trim();
            string text = cols[textIndex].Trim();

            if (string.IsNullOrEmpty(text)) continue;

            ChatSide chatSide = (side == "R" || side == "Right" || side == "right") ? ChatSide.Right : ChatSide.Left;

            ChatLine chatLine = new ChatLine();
            chatLine.side = chatSide;
            chatLine.text = text;

            result.Add(chatLine);
        }

        return result;
    }

    // 쉼표/따옴표 처리되는 간단 CSV 파서
    List<string> ParseCsvLine(string line)
    {
        List<string> result = new List<string>();
        bool inQuotes = false;
        System.Text.StringBuilder sb = new System.Text.StringBuilder();

        for (int i = 0; i < line.Length; i++)
        {
            char c = line[i];

            if (c == '\"')
            {
                // "" 이스케이프 처리
                if (inQuotes && i + 1 < line.Length && line[i + 1] == '\"')
                {
                    sb.Append('\"');
                    i++;
                }
                else
                {
                    inQuotes = !inQuotes;
                }
            }
            else if (c == ',' && !inQuotes)
            {
                result.Add(sb.ToString());
                sb.Length = 0;
            }
            else
            {
                sb.Append(c);
            }
        }

        result.Add(sb.ToString());
        return result;
    }
}