using ShabuStudio;
using UnityEngine;
using UnityEngine.EventSystems;

public class RogueliteButton : MonoBehaviour , IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        GameManager.Instance.StartRogueliteMode();
    }
}
