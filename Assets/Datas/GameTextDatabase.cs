using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameTextDatabase", menuName = "DGDG/Game Text Database")]
public class GameTextDatabase : ScriptableObject
{
    [Header("Menu Titles")]
    public string menuChat = "채팅";
    public string menuProfile = "프로필";
    public string menuGifts = "선물";
    public string menuSettings = "설정";

    [Header("Chat Samples (optional)")]
    [TextArea(2, 4)]
    public List<string> chatSampleLines = new List<string>()
    {
        "안녕하세요!",
        "오늘 뭐해요?",
        "같이 게임할래요?"
    };
}
