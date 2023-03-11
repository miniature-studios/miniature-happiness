using Common;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RoomUI : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private bool IsClicked;
    private bool IsMouseOverScrollView;
    OverDetector scrollViewOverDetector;
    RectTransform contentRectTransform;
    RectTransform canvasRectTransform;
    LocationBuilder locationBuilder;
    Image prefab_image;

    [SerializeField]
    public RoomType roomType;

    RectTransform rectTransform;
    private Vector2 local_position;
    private void Awake()
    {
        locationBuilder = FindObjectOfType<LocationBuilder>();
        contentRectTransform = FindObjectOfType<HorizontalLayoutGroup>().GetComponent<RectTransform>();
        canvasRectTransform = FindObjectOfType<Canvas>().GetComponent<RectTransform>();
        scrollViewOverDetector = FindObjectOfType<OverDetector>();
        prefab_image = GetComponent<Image>();
        rectTransform = GetComponent<RectTransform>();
    }

    private void Update()
    {
        if (IsClicked)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRectTransform, Input.mousePosition, Camera.main, out local_position);
            rectTransform.anchoredPosition = new Vector2(local_position.x + Screen.width / 2, local_position.y - Screen.height / 2);

            if (scrollViewOverDetector.IsPointerOverUIObject())
            {
                IsMouseOverScrollView = true;
                // Show All Elements
                prefab_image.enabled = true;
            }
            else
            {
                IsMouseOverScrollView = false;
                // Hide All Elements
                prefab_image.enabled = false;
            }
        }
    }
    public void LoadImage()
    {
        // Set image from prefab
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        IsClicked = true;
        transform.SetParent(canvasRectTransform, true);
        locationBuilder.RenderSelectedRoom(roomType);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (IsMouseOverScrollView)
        {
            transform.SetParent(contentRectTransform);
            locationBuilder.DeleteSelectedRoom();
        }
        else if (IsClicked)
        {
            locationBuilder.BuildChoosedRoom();
            Destroy(gameObject);
        }
        IsClicked = false;
    }
}
