[System.Serializable]
public class DialogChoiceData
{
    public int id;
    public int groupId;
    public string unused;
    public string text;
    public int choiceDialogId;
    public int nextDialogId;
    public string memo;

    public DialogChoiceData(
        int choiceId,
        int dialogGroupId,
        string choiceUnused,
        string choiceText,
        int choiceDialogIdValue,
        int nextDialogIdValue,
        string choiceMemo)
    {
        id = choiceId;
        groupId = dialogGroupId;
        unused = choiceUnused;
        text = choiceText;
        choiceDialogId = choiceDialogIdValue;
        nextDialogId = nextDialogIdValue;
        memo = choiceMemo;
    }
}