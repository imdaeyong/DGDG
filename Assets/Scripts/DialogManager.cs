using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogManager : MonoBehaviour
{
    [Header("UI")]
    public TMP_Text nameText;
    public TMP_Text dialogText;
    public GameObject nextMark;
    public GameObject dialogPanel;

    [Header("Choice UI")]
    public GameObject choicePanel;
    public Button choiceButton1;
    public Button choiceButton2;
    public Button choiceButton3;
    public TMP_Text choiceButton1Text;
    public TMP_Text choiceButton2Text;
    public TMP_Text choiceButton3Text;

    [Header("Typing")]
    public float typingSpeed = 0.05f;

    [Header("Story Progress")]
    public bool useDebugStartGroup = true;
    public int debugStartGroupId = 10;

    private int currentStoryGroupId = 10;

    private List<DialogLine> allLines = new List<DialogLine>();
    private List<DialogLine> currentGroupLines = new List<DialogLine>();
    private List<DialogChoiceData> choiceDataList = new List<DialogChoiceData>();

    private int currentGroupId = -1;
    private int currentIndex = 0;

    private Coroutine typingCoroutine;
    private bool isTyping = false;
    private bool isDialogEnded = false;
    private bool isDialogStarted = false;
    private bool isChoiceWaiting = false;

    private bool isChoiceAnswerMode = false;
    private int pendingNextDialogId = -1;

    private List<DialogChoiceData> currentChoices = new List<DialogChoiceData>();


    // 초기 UI 상태 설정
    void Start()
    {
        if (nextMark != null)
        {
            nextMark.SetActive(false);
        }

        if (choicePanel != null)
        {
            choicePanel.SetActive(false);
        }

        if (dialogPanel != null)
        {
            dialogPanel.SetActive(false);
        }

        if (dialogText != null)
        {
            dialogText.text = "";
        }

        if (nameText != null)
        {
            nameText.text = "";
        }

        if (useDebugStartGroup)
        {
            currentStoryGroupId = debugStartGroupId;
        }
    }


    // 전체 데이터 설정 후 현재 스토리 그룹 또는 디버그 그룹으로 대화를 시작
    public void StartDialog(
        List<DialogLine> newLines,
        List<DialogChoiceData> newChoiceDataList,
        int startGroupId)
    {
        allLines = newLines != null ? newLines : new List<DialogLine>();
        choiceDataList = newChoiceDataList != null ? newChoiceDataList : new List<DialogChoiceData>();

        if (useDebugStartGroup)
        {
            currentStoryGroupId = debugStartGroupId;
        }
        else
        {
            currentStoryGroupId = startGroupId;
        }

        StartCurrentStoryGroupDialog();
    }


    // 현재 저장된 스토리 그룹으로 대화 시작
    public void StartCurrentStoryGroupDialog()
    {
        isDialogStarted = true;
        isDialogEnded = false;
        isChoiceWaiting = false;
        isChoiceAnswerMode = false;
        pendingNextDialogId = -1;
        currentChoices.Clear();

        currentGroupId = currentStoryGroupId;
        BuildCurrentGroupLines(currentGroupId);

        currentIndex = FindNextPlayableIndex(0);

        if (currentIndex >= currentGroupLines.Count)
        {
            Debug.LogWarning("현재 GroupID에 표시할 대사가 없습니다. GroupID=" + currentGroupId);
            EndDialog();
            return;
        }

        if (dialogPanel != null)
        {
            dialogPanel.SetActive(true);
        }

        ShowCurrentLine();
    }


    // 개발자가 원하는 GroupID를 직접 지정해서 시작
    public void StartSpecificGroupDialog(int groupId)
    {
        currentStoryGroupId = groupId;
        StartCurrentStoryGroupDialog();
    }


    // 현재 진행 중인 GroupID 반환
    public int GetCurrentStoryGroupId()
    {
        return currentStoryGroupId;
    }


    // 현재 GroupID에 해당하는 대사만 currentGroupLines에 구성
    void BuildCurrentGroupLines(int groupId)
    {
        currentGroupLines.Clear();

        int i;
        for (i = 0; i < allLines.Count; i++)
        {
            if (allLines[i].groupId == groupId)
            {
                currentGroupLines.Add(allLines[i]);
            }
        }
    }


    // 현재 줄 표시
    void ShowCurrentLine()
    {
        if (currentIndex < 0 || currentIndex >= currentGroupLines.Count)
        {
            EndDialog();
            return;
        }

        if (nextMark != null)
        {
            nextMark.SetActive(false);
        }

        if (choicePanel != null)
        {
            choicePanel.SetActive(false);
        }

        isChoiceWaiting = false;

        DialogLine line = currentGroupLines[currentIndex];

        ApplyOrdering(line);

        if (nameText != null)
        {
            if (line.type == "System")
            {
                nameText.text = "";
            }
            else
            {
                nameText.text = line.speaker;
            }
        }

        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }

        typingCoroutine = StartCoroutine(TypeText(line.text));
    }


    // 타이핑 출력
    IEnumerator TypeText(string fullText)
    {
        isTyping = true;

        if (dialogText != null)
        {
            dialogText.text = "";
        }

        int i;
        for (i = 0; i < fullText.Length; i++)
        {
            if (dialogText != null)
            {
                dialogText.text += fullText[i];
            }

            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;

        if (currentIndex < 0 || currentIndex >= currentGroupLines.Count)
        {
            yield break;
        }

        DialogLine line = currentGroupLines[currentIndex];

        if (isChoiceAnswerMode)
        {
            if (nextMark != null)
            {
                nextMark.SetActive(true);
            }

            yield break;
        }

        if (HasChoices(line.id))
        {
            PrepareChoices(line.id);
            ShowChoices();
        }
        else
        {
            if (nextMark != null)
            {
                nextMark.SetActive(true);
            }
        }
    }


    // 대화창 클릭 처리
    public void OnDialogPanelClicked()
    {
        if (!isDialogStarted || isDialogEnded)
        {
            return;
        }

        if (isChoiceWaiting)
        {
            return;
        }

        if (isTyping)
        {
            CompleteTyping();
            return;
        }

        if (isChoiceAnswerMode)
        {
            isChoiceAnswerMode = false;

            if (pendingNextDialogId > -1)
            {
                if (!MoveToDialogById(pendingNextDialogId))
                {
                    EndDialog();
                    return;
                }

                pendingNextDialogId = -1;
                ShowCurrentLine();
                return;
            }
        }

        currentIndex++;
        currentIndex = FindNextPlayableIndex(currentIndex);

        if (currentIndex >= currentGroupLines.Count)
        {
            EndDialog();
            return;
        }

        ShowCurrentLine();
    }


    // 타이핑 즉시 완료
    void CompleteTyping()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }

        if (currentIndex < 0 || currentIndex >= currentGroupLines.Count)
        {
            return;
        }

        if (dialogText != null)
        {
            dialogText.text = currentGroupLines[currentIndex].text;
        }

        isTyping = false;

        DialogLine line = currentGroupLines[currentIndex];

        if (isChoiceAnswerMode)
        {
            if (nextMark != null)
            {
                nextMark.SetActive(true);
            }

            return;
        }

        if (HasChoices(line.id))
        {
            PrepareChoices(line.id);
            ShowChoices();
        }
        else
        {
            if (nextMark != null)
            {
                nextMark.SetActive(true);
            }
        }
    }


    // 현재 Dialog ID에 연결된 선택지가 있는지 확인
    bool HasChoices(int dialogId)
    {
        int i;
        for (i = 0; i < choiceDataList.Count; i++)
        {
            if (choiceDataList[i].groupId == dialogId)
            {
                return true;
            }
        }

        return false;
    }


    // 현재 Dialog ID에 해당하는 선택지 목록 준비
    void PrepareChoices(int dialogId)
    {
        currentChoices.Clear();

        int i;
        for (i = 0; i < choiceDataList.Count; i++)
        {
            if (choiceDataList[i].groupId == dialogId)
            {
                currentChoices.Add(choiceDataList[i]);
            }
        }
    }


    // 선택지 표시
    void ShowChoices()
    {
        if (choicePanel == null)
        {
            Debug.LogWarning("choicePanel 연결이 안 되어 있습니다.");
            return;
        }

        if (currentChoices == null || currentChoices.Count == 0)
        {
            Debug.LogWarning("표시할 선택지가 없습니다.");
            return;
        }

        isChoiceWaiting = true;
        choicePanel.SetActive(true);

        if (choiceButton1 != null)
        {
            choiceButton1.gameObject.SetActive(false);
        }

        if (choiceButton2 != null)
        {
            choiceButton2.gameObject.SetActive(false);
        }

        if (choiceButton3 != null)
        {
            choiceButton3.gameObject.SetActive(false);
        }

        if (currentChoices.Count >= 1 && choiceButton1 != null)
        {
            choiceButton1.gameObject.SetActive(true);

            if (choiceButton1Text != null)
            {
                choiceButton1Text.text = currentChoices[0].text;
            }
        }

        if (currentChoices.Count >= 2 && choiceButton2 != null)
        {
            choiceButton2.gameObject.SetActive(true);

            if (choiceButton2Text != null)
            {
                choiceButton2Text.text = currentChoices[1].text;
            }
        }

        if (currentChoices.Count >= 3 && choiceButton3 != null)
        {
            choiceButton3.gameObject.SetActive(true);

            if (choiceButton3Text != null)
            {
                choiceButton3Text.text = currentChoices[2].text;
            }
        }
    }


    // 첫 번째 선택지 클릭 처리
    public void OnChoice1Clicked()
    {
        HandleChoice(0);
    }


    // 두 번째 선택지 클릭 처리
    public void OnChoice2Clicked()
    {
        HandleChoice(1);
    }


    // 세 번째 선택지 클릭 처리
    public void OnChoice3Clicked()
    {
        HandleChoice(2);
    }


    // 선택지 처리
    void HandleChoice(int index)
    {
        if (currentChoices == null || index < 0 || index >= currentChoices.Count)
        {
            return;
        }

        isChoiceWaiting = false;

        if (choicePanel != null)
        {
            choicePanel.SetActive(false);
        }

        DialogChoiceData selected = currentChoices[index];

        pendingNextDialogId = selected.nextDialogId;
        currentIndex = FindIndexInCurrentGroupByDialogId(selected.choiceDialogId);

        if (currentIndex >= currentGroupLines.Count)
        {
            Debug.LogWarning("ChoiceDialogId 를 현재 Group에서 찾지 못했습니다. ID=" + selected.choiceDialogId);
            EndDialog();
            return;
        }

        isChoiceAnswerMode = true;
        ShowCurrentLine();
    }


    // 일반 진행에서 표시 가능한 다음 인덱스를 찾음
    int FindNextPlayableIndex(int startIndex)
    {
        int i;

        for (i = startIndex; i < currentGroupLines.Count; i++)
        {
            if (currentGroupLines[i].type == "SelectOpt")
            {
                continue;
            }

            return i;
        }

        return currentGroupLines.Count;
    }


    // 현재 Group 대사 목록 안에서 특정 Dialog ID의 인덱스를 찾음
    int FindIndexInCurrentGroupByDialogId(int dialogId)
    {
        int i;

        for (i = 0; i < currentGroupLines.Count; i++)
        {
            if (currentGroupLines[i].id == dialogId)
            {
                return i;
            }
        }

        return currentGroupLines.Count;
    }


    // 전체 대사에서 특정 Dialog ID를 찾아 해당 Group으로 이동
    // 전체 대사에서 특정 Dialog ID를 찾아 해당 Group으로 이동
    bool MoveToDialogById(int dialogId)
    {
        int i;
        DialogLine targetLine = null;

        for (i = 0; i < allLines.Count; i++)
        {
            if (allLines[i].id == dialogId)
            {
                targetLine = allLines[i];
                break;
            }
        }

        if (targetLine == null)
        {
            Debug.LogWarning("NextDialogId에 해당하는 Dialog ID를 찾지 못했습니다. ID=" + dialogId);
            return false;
        }

        currentGroupId = targetLine.groupId;
        currentStoryGroupId = currentGroupId;

        BuildCurrentGroupLines(currentGroupId);

        currentIndex = FindIndexInCurrentGroupByDialogId(dialogId);

        // 이동한 첫 줄은 SelectOpt여도 그대로 출력
        if (currentIndex >= currentGroupLines.Count)
        {
            Debug.LogWarning("이동 후 대상 Dialog ID를 현재 Group에서 찾지 못했습니다. Dialog ID=" + dialogId);
            return false;
        }

        return true;
    }

    // Type 및 Ordering 기준으로 정렬 적용
    void ApplyOrdering(DialogLine line)
    {
        if (line == null)
        {
            return;
        }

        if (line.type == "System")
        {
            if (nameText != null)
            {
                nameText.alignment = TextAlignmentOptions.Center;
            }

            if (dialogText != null)
            {
                dialogText.alignment = TextAlignmentOptions.Top;
            }

            return;
        }

        string orderingValue = line.ordering;

        if (string.IsNullOrEmpty(orderingValue))
        {
            orderingValue = "Left";
        }

        orderingValue = orderingValue.Trim().ToLowerInvariant();

        if (orderingValue == "left")
        {
            if (nameText != null)
            {
                nameText.alignment = TextAlignmentOptions.Left;
            }

            if (dialogText != null)
            {
                dialogText.alignment = TextAlignmentOptions.TopLeft;
            }
        }
        else if (orderingValue == "center")
        {
            if (nameText != null)
            {
                nameText.alignment = TextAlignmentOptions.Center;
            }

            if (dialogText != null)
            {
                dialogText.alignment = TextAlignmentOptions.Top;
            }
        }
        else if (orderingValue == "right")
        {
            if (nameText != null)
            {
                nameText.alignment = TextAlignmentOptions.Right;
            }

            if (dialogText != null)
            {
                dialogText.alignment = TextAlignmentOptions.TopRight;
            }
        }
        else
        {
            if (nameText != null)
            {
                nameText.alignment = TextAlignmentOptions.Left;
            }

            if (dialogText != null)
            {
                dialogText.alignment = TextAlignmentOptions.TopLeft;
            }
        }
    }


    // 대화 종료 처리, 다음 스토리 GroupID를 10 증가시키고 대화창 숨김
    void EndDialog()
    {
        isDialogEnded = true;

        if (nextMark != null)
        {
            nextMark.SetActive(false);
        }

        if (choicePanel != null)
        {
            choicePanel.SetActive(false);
        }

        if (dialogPanel != null)
        {
            dialogPanel.SetActive(false);
        }

        currentStoryGroupId += 10;

        Debug.Log("대화 종료. 다음 GroupID = " + currentStoryGroupId);
    }
}