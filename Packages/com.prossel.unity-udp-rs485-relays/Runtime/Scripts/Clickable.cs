using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class Clickable : MonoBehaviour, IPointerClickHandler
{
    public UnityEvent onClick = new UnityEvent();

    public void OnPointerClick(PointerEventData eventData)
    {
        onClick.Invoke();
    }
}
