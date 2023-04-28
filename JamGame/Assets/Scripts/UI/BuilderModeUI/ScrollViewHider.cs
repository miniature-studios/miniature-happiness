using UnityEngine;
using UnityEngine.EventSystems;

public class ScrollViewHider : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] float OpenYPosition = 50;
    [SerializeField] float CloseYPosition = -60;
    [SerializeField] float HideSpeed = 10;
    [SerializeField] Canvas canvas;
    [SerializeField] TileBuilderController controller;
    RectTransform rectTransform;
    public void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }
    public void Update()
    {
        Vector2 mousePosition = Input.mousePosition / canvas.scaleFactor;
        Vector3 position = Vector3.Lerp(
            rectTransform.anchoredPosition3D,
            new Vector3(
                rectTransform.anchoredPosition3D.x,
                IsInRectangle(mousePosition) ? OpenYPosition : CloseYPosition,
                rectTransform.anchoredPosition3D.z
                ),
            Time.deltaTime * HideSpeed
            );
        rectTransform.anchoredPosition3D = position;
    }
    public bool IsInRectangle(Vector2 point)
    {
        if(point.x > rectTransform.offsetMin.x && point.x < canvas.pixelRect.width + rectTransform.offsetMax.x
           && point.y > 0 && point.y < rectTransform.rect.height)
            return true;
        return false;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        controller.MouseUIEnter();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        controller.MouseUIExit();
    }
}
