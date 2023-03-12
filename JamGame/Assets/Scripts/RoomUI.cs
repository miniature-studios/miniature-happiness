using Common;
using UnityEngine;
using UnityEngine.EventSystems;

public class RoomUI : MonoBehaviour, IPointerDownHandler
{
    LocationBuilder locationBuilder;

    public RoomType roomType;
    private void Awake()
    {
        locationBuilder = FindObjectOfType<LocationBuilder>();
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        if (locationBuilder.IsEnoughPlace()) {
            locationBuilder.AddRoomToScene(roomType);
            if (roomType != RoomType.Corridor) { Destroy(gameObject); }
        }
        else
        {
            Debug.LogWarning("Not enough place");
        }
    }
}
