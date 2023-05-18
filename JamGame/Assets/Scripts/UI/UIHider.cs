using Common;
using UnityEngine;


public class UIHider : MonoBehaviour
{
    [SerializeField] private Vector2 unhidedPosition;
    [SerializeField] private Vector2 hidedPosition;
    [SerializeField] private float hideSpeed = 10;
    private RectTransform rectTransform;
    public UIElementState UIElementState;
    private Vector3 position;
    public void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        SetState(UIElementState = UIElementState.Hided);
    }
    public void Update()
    {
        rectTransform.anchoredPosition3D = position;
    }
    public void SetState(UIElementState buttonState)
    {
        UIElementState = buttonState;
        Vector2 position = buttonState switch
        {
            UIElementState.Hided => hidedPosition,
            _ => unhidedPosition,
        };
        this.position = Vector3.Lerp(
            rectTransform.anchoredPosition3D,
            new Vector3(
                position.x,
                position.y,
                rectTransform.anchoredPosition3D.z
                ),
            Time.deltaTime * hideSpeed
            );
    }
}

