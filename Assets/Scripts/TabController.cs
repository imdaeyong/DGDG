using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TabController : MonoBehaviour
{
    public GameObject panelChat;
    public GameObject panelProfile;
    public GameObject panelGifts;
    public GameObject panelSettings;

    public TextMeshProUGUI titleText;

    public Button tabChat;
    public Button tabProfile;
    public Button tabGifts;
    public Button tabSettings;

    //  탭 버튼 라벨(자식 TMP) 연결
    public TextMeshProUGUI tabChatLabel;
    public TextMeshProUGUI tabProfileLabel;
    public TextMeshProUGUI tabGiftsLabel;
    public TextMeshProUGUI tabSettingsLabel;

    public GameTextDatabase textDb;

    Color normalColor = new Color(0.9f, 0.9f, 0.9f);
    Color selectedColor = new Color(1f, 0.75f, 0.8f); // 핑크톤

    void Start()
    {
        RefreshTabLabels(); //  하단 탭 라벨 반영
        ShowChat();
    }

    void RefreshTabLabels()
    {
        if (textDb == null) return;

        if (tabChatLabel != null) tabChatLabel.text = textDb.menuChat;
        if (tabProfileLabel != null) tabProfileLabel.text = textDb.menuProfile;
        if (tabGiftsLabel != null) tabGiftsLabel.text = textDb.menuGifts;
        if (tabSettingsLabel != null) tabSettingsLabel.text = textDb.menuSettings;
    }

    void SetTabColor(Button selected)
    {
        if (tabChat != null && tabChat.image != null) tabChat.image.color = normalColor;
        if (tabProfile != null && tabProfile.image != null) tabProfile.image.color = normalColor;
        if (tabGifts != null && tabGifts.image != null) tabGifts.image.color = normalColor;
        if (tabSettings != null && tabSettings.image != null) tabSettings.image.color = normalColor;

        if (selected != null && selected.image != null) selected.image.color = selectedColor;
    }

    public void ShowChat()
    {
        SetActive(panelChat, panelProfile, panelGifts, panelSettings);

        if (titleText != null)
        {
            titleText.text = (textDb != null) ? textDb.menuChat : "채팅";
        }

        SetTabColor(tabChat);
    }

    public void ShowProfile()
    {
        SetActive(panelProfile, panelChat, panelGifts, panelSettings);

        if (titleText != null)
        {
            titleText.text = (textDb != null) ? textDb.menuProfile : "프로필";
        }

        SetTabColor(tabProfile);
    }

    public void ShowGifts()
    {
        SetActive(panelGifts, panelChat, panelProfile, panelSettings);

        if (titleText != null)
        {
            titleText.text = (textDb != null) ? textDb.menuGifts : "선물";
        }

        SetTabColor(tabGifts);
    }

    public void ShowSettings()
    {
        SetActive(panelSettings, panelChat, panelProfile, panelGifts);

        if (titleText != null)
        {
            titleText.text = (textDb != null) ? textDb.menuSettings : "설정";
        }

        SetTabColor(tabSettings);
    }

    void SetActive(GameObject on, GameObject off1, GameObject off2, GameObject off3)
    {
        if (on != null) on.SetActive(true);
        if (off1 != null) off1.SetActive(false);
        if (off2 != null) off2.SetActive(false);
        if (off3 != null) off3.SetActive(false);
    }
}
