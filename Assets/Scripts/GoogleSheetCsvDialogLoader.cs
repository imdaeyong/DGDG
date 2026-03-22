using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class GoogleSheetCsvDialogLoader : MonoBehaviour
{
    public string dialogCsvUrl;
    public string dialogChoiceCsvUrl;

    public int startGroupId = 10;
    public DialogManager dialogManager;

    void Start()
    {
        StartCoroutine(LoadAllCsv());
    }

    IEnumerator LoadAllCsv()
    {
        if (string.IsNullOrEmpty(dialogCsvUrl))
        {
            Debug.LogError("dialogCsvUrl 이 비어 있습니다.");
            yield break;
        }

        if (string.IsNullOrEmpty(dialogChoiceCsvUrl))
        {
            Debug.LogError("dialogChoiceCsvUrl 이 비어 있습니다.");
            yield break;
        }

        UnityWebRequest dialogRequest = UnityWebRequest.Get(dialogCsvUrl);
        yield return dialogRequest.SendWebRequest();

        if (dialogRequest.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Dialog CSV 로드 실패: " + dialogRequest.error);
            yield break;
        }

        UnityWebRequest choiceRequest = UnityWebRequest.Get(dialogChoiceCsvUrl);
        yield return choiceRequest.SendWebRequest();

        if (choiceRequest.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("DialogChoice CSV 로드 실패: " + choiceRequest.error);
            yield break;
        }

        string dialogCsvText = dialogRequest.downloadHandler.text;
        string choiceCsvText = choiceRequest.downloadHandler.text;

        List<DialogLine> dialogLines = ParseDialogCsv(dialogCsvText);
        List<DialogChoiceData> choiceList = ParseDialogChoiceCsv(choiceCsvText);

        Debug.Log("Dialog 개수: " + dialogLines.Count);
        Debug.Log("DialogChoice 개수: " + choiceList.Count);

        if (dialogManager == null)
        {
            Debug.LogError("DialogManager 연결이 안 되어 있습니다.");
            yield break;
        }

        dialogManager.StartDialog(dialogLines, choiceList, startGroupId);
    }

    List<DialogLine> ParseDialogCsv(string csvText)
    {
        List<DialogLine> list = new List<DialogLine>();

        if (string.IsNullOrEmpty(csvText))
        {
            return list;
        }

        string[] rows = csvText.Split('\n');

        int i;
        for (i = 1; i < rows.Length; i++)
        {
            string row = rows[i].Trim();

            if (string.IsNullOrEmpty(row))
            {
                continue;
            }

            string[] cols = SplitCsvRow(row);

            if (cols.Length < 8)
            {
                continue;
            }

            int id;
            int groupId;

            if (!int.TryParse(CleanCell(cols[0]), out id))
            {
                continue;
            }

            if (!int.TryParse(CleanCell(cols[1]), out groupId))
            {
                continue;
            }

            string unused = CleanCell(cols[2]);
            string type = CleanCell(cols[3]);
            string speaker = CleanCell(cols[4]);
            string ordering = CleanCell(cols[5]);
            string text = CleanCell(cols[6]);
            string memo = CleanCell(cols[7]);

            if (unused == "1")
            {
                continue;
            }

            if (string.IsNullOrEmpty(text))
            {
                continue;
            }

            list.Add(
                new DialogLine(
                    id,
                    groupId,
                    unused,
                    type,
                    speaker,
                    ordering,
                    text,
                    memo
                )
            );
        }

        return list;
    }

    List<DialogChoiceData> ParseDialogChoiceCsv(string csvText)
    {
        List<DialogChoiceData> list = new List<DialogChoiceData>();

        if (string.IsNullOrEmpty(csvText))
        {
            return list;
        }

        string[] rows = csvText.Split('\n');

        int i;
        for (i = 1; i < rows.Length; i++)
        {
            string row = rows[i].Trim();

            if (string.IsNullOrEmpty(row))
            {
                continue;
            }

            string[] cols = SplitCsvRow(row);

            if (cols.Length < 7)
            {
                continue;
            }

            int id;
            int groupId;
            int choiceDialogId;
            int nextDialogId;

            if (!int.TryParse(CleanCell(cols[0]), out id))
            {
                continue;
            }

            if (!int.TryParse(CleanCell(cols[1]), out groupId))
            {
                continue;
            }

            if (!int.TryParse(CleanCell(cols[4]), out choiceDialogId))
            {
                continue;
            }

            if (!int.TryParse(CleanCell(cols[5]), out nextDialogId))
            {
                continue;
            }

            string unused = CleanCell(cols[2]);
            string text = CleanCell(cols[3]);
            string memo = CleanCell(cols[6]);

            if (unused == "1")
            {
                continue;
            }

            if (string.IsNullOrEmpty(text))
            {
                continue;
            }

            list.Add(
                new DialogChoiceData(
                    id,
                    groupId,
                    unused,
                    text,
                    choiceDialogId,
                    nextDialogId,
                    memo
                )
            );
        }

        return list;
    }

    string CleanCell(string value)
    {
        if (value == null)
        {
            return "";
        }

        string result = value.Trim();

        if (result.Length >= 2 && result[0] == '"' && result[result.Length - 1] == '"')
        {
            result = result.Substring(1, result.Length - 2);
        }

        result = result.Replace("\"\"", "\"");
        result = result.Replace("\r", "");

        return result;
    }

    string[] SplitCsvRow(string row)
    {
        List<string> result = new List<string>();
        bool inQuotes = false;
        string current = "";

        int i;
        for (i = 0; i < row.Length; i++)
        {
            char c = row[i];

            if (c == '"')
            {
                if (inQuotes && i + 1 < row.Length && row[i + 1] == '"')
                {
                    current += '"';
                    i++;
                }
                else
                {
                    inQuotes = !inQuotes;
                }
            }
            else if (c == ',' && !inQuotes)
            {
                result.Add(current);
                current = "";
            }
            else
            {
                current += c;
            }
        }

        result.Add(current);

        return result.ToArray();
    }
}