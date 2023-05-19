using Common;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class CustomButton : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private UnityEvent buttonClicked;
    public UIHider UIHider;
    public void OnPointerClick(PointerEventData event_data)
    {
        if (UIHider == null)
        {
            buttonClicked?.Invoke();
            return;
        }
        if (UIHider.UIElementState == UIElementState.Shown)
        {
            buttonClicked?.Invoke();
            return;
        }
    }
}

