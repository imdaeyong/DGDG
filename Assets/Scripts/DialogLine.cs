[System.Serializable]
public class DialogLine
{
    public int id;
    public int groupId;
    public string unused;
    public string type;
    public string speaker;
    public string ordering;
    public string text;
    public string memo;
    public string typeSound;
    public string effectSound;

    public DialogLine(
        int dialogId,
        int dialogGroupId,
        string dialogUnused,
        string dialogType,
        string speakerName,
        string dialogOrdering,
        string lineText,
        string dialogMemo,
        string dialogTypeSound,
        string dialogEffectSound)
    {
        id = dialogId;
        groupId = dialogGroupId;
        unused = dialogUnused;
        type = dialogType;
        speaker = speakerName;
        ordering = dialogOrdering;
        text = lineText;
        memo = dialogMemo;
        typeSound = dialogTypeSound;
        effectSound = dialogEffectSound;
    }
}