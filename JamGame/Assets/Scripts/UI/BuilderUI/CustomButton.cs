using Common;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class CustomButton : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private UnityEvent buttonClicked;
    public UIHider UIHider;
    public void OnPointerClick(PointerEventData eventData)
    {
        if (UIHider.UIElementState == UIElementState.Unhided)
        {
            buttonClicked?.Invoke();
        }
    }
}

