using Common;
using UnityEngine;

public class UIHider : MonoBehaviour
{
    [SerializeField] private Vector2 shownPosition;
    [SerializeField] private Vector2 hiddenPosition;
    [SerializeField] private float hideSpeed = 10;

    public UIElementState UIElementState;

    private RectTransform rectTransform;
    private Vector2 currentPosition;

    public void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        SetState(UIElementState);
    }

    public void Update()
    {
        rectTransform.anchoredPosition3D = Vector3.Lerp(
            rectTransform.anchoredPosition3D,
            new Vector3(
                currentPosition.x,
                currentPosition.y,
                rectTransform.anchoredPosition3D.z
                ),
            Time.deltaTime * hideSpeed
            );
    }

    public void SetState(UIElementState button_state)
    {
        UIElementState = button_state;
        currentPosition = button_state switch
        {
            UIElementState.Hidden => hiddenPosition,
            _ => shownPosition,
        };
    }
}

