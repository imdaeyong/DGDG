using UnityEngine;
using UnityEngine.EventSystems;

public class DialogPanelClick : MonoBehaviour, IPointerClickHandler
{
    public DialogManager dialogManager;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (dialogManager != null)
        {
            dialogManager.OnDialogPanelClicked();
        }
    }
}