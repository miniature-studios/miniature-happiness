using Common;
using UnityEngine;

public class ScrollViewHider : MonoBehaviour
{
    [SerializeField] private float openYPosition = 50;
    [SerializeField] private float closeYPosition = -60;
    [SerializeField] private float hideSpeed = 10;
    [SerializeField] private Canvas canvas;
    [SerializeField] private RectTransform rectTransform;
    public UIElementState UIElementState { get; set; } = UIElementState.Hidden;
    public void Update()
    {
        Vector2 mousePosition = Input.mousePosition / canvas.scaleFactor;
        Vector3 position = Vector3.Lerp(
            rectTransform.anchoredPosition3D,
            new Vector3(
                rectTransform.anchoredPosition3D.x,
                IsInOverZone(mousePosition) ? openYPosition : closeYPosition,
                rectTransform.anchoredPosition3D.z
                ),
            Time.deltaTime * hideSpeed
            );
        rectTransform.anchoredPosition3D = position;
    }
    public bool IsInOverZone(Vector2 point)
    {
        return point.x > rectTransform.offsetMin.x && point.x < canvas.pixelRect.width + rectTransform.offsetMax.x
           && point.y < rectTransform.rect.height && (UIElementState == UIElementState.Shown);
    }
}
