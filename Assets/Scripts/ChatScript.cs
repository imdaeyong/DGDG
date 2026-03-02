using System;
using System.Collections.Generic;
using UnityEngine;

public enum ChatSide
{
    Left,
    Right
}

[Serializable]
public class ChatLine
{
    public ChatSide side;
    [TextArea(2, 6)]
    public string text;
}

[CreateAssetMenu(fileName = "ChatScript", menuName = "DGDG/Chat Script", order = 0)]
public class ChatScript : ScriptableObject
{
    public List<ChatLine> lines = new List<ChatLine>();
}