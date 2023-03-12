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
        locationBuilder.AddRoomToScene(roomType);
        if (roomType != RoomType.Corridor) { Destroy(gameObject); }
    }
}
